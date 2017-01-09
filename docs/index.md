---
layout: default
---
# What is Autofac.TypedFactories ?

Autofac.TypedFactories aims at solving a problem typically encountered when practicing TDD.
When unit-testing a method, whenever a new object is instantiated in its body, the unit test has no control over what the ``new`` statement will return.

```csharp
// myFoo cannot be mocked
var myFoo = new Foo(2);
Assert.AreEqual(2, myFoo.Number);
```

In typical scenarios, we will want our test to assert that the created object has been used properly. For instance, that a given method has been called on it a number of times, with a certain parameter, or that it has been passed as an argument to another method.

```csharp
// myFoo can be mocked
var myFoo = fooFactory.Create(2);
Assert.AreEqual(2, myFoo.Number);
```

Unfortunately, there is no easy and cheap way to control what the ``new`` statement returns. The usual way to work around it is to replace every ``new`` statement by a call to a ``Create`` method on a factory injected as a dependency. The factory can then be mocked and injected in the arrange part of the test, and configured so that the returned object is also a mock. This mock can ultimately be monitored by a mocking framework, such as [Moq](http://www.moqthis.com/).

The problem, however, is that it is a tedious task because every single factory needs to be implemented manually. For each type instantiated in a tested method, there would be:

**An implementation of the instantiated class.**

```csharp
public class FoobarProvider : IFoobarProvider
{
    public int Number { get; set; }

    public IDependencyService Dependency { get; set; }

    public FoobarProvider(int number, IDependencyService dependency)
    {
        this.Number = number;
        this.Dependency = dependency;
    }
}
```
**An interface the class will need to implement (in order to mock the returned object)**

```csharp
public interface IFoobarProvider
{
    int Number { get; set; }

    IDependencyService Dependency { get; set; }
}
```
**An interface describing the contract of the factory**

```csharp
public interface IFoobarProviderFactory
{
    IFoobarProvider Create(int number);
}
```
**An implementation of the factory.**

```csharp
public class FoobarProviderFactory : IFoobarProviderFactory
{
    private readonly IDependencyService dependencyService;

    public FoobarProviderFactory(IDependencyService dependencyService)
    {
        this.dependencyService = dependencyService;
    }
    public IFoobarProvider Create(int number)
    {
        return new FoobarProvider(number, dependencyService);
    }
}
```

To some, the two interfaces could be perceived as an overkill for a scenario as simple as instantiating a class, but this is the price of testability. Tools like [ReSharper](https://www.jetbrains.com/resharper/) can mitigate the burden by allowing to easily extract the interface of a class, based on its implementation.

I am not too concerned about the three first types, but the last one doesn't bring any value and it should be automatically generated.

All we expect the factory to do is to take the arguments that were passed to the factory's ``Create`` method, and forward them to the class' constructor. All the other arguments that weren't passed to the ``Create``  method should be received by the factory through dependeny injection.

Boilerplate code, nothing else. This can certainly be improved, and it is exactly what Autofac.TypedFactories aims to do.

## How ?
There would basically be two different approaches, each with its own pros and cons.

### Generated Code
Using Roslyn, it would be technically possible to analyze the signatures of a ``Create`` method, and the constructor of its returned type. A class implementing the factory's interface could therefore be generated on file and added to the csproj. This would yield extremely good performances, because the generated code would ultimately be compiled to vanilla .net code. All performance impacts would be paid upfront when compiling the consuming project. The process, however, is a bit quirky, as it would involve having lots of generated files that would give a cluttered feel to the solution, and these factories would still need to be registered in the IoC container.

### Dynamic proxies
The other approach is to generate dynamic proxy objects at runtime for the factory interfaces. These dynamic proxies would instantiate the objects to return, not by calling the new statement, but by leveraging the power of an IoC container.
On the plus side, this means no files would be generated at compilation, hence less files in the solution, and both the implementation and the usage of this package are much simpler.
The downside is that the performances aren't quite as good as if it were standard C# code. Autofac claims that resolving a type through its IoC container is ten times more expensive than newing it up with the ``new`` statement. Meaning that if this approach is great for general purposes, you probably will want to hand roll your own factories using standard C# classes if your factory needs to instantiate hundreds of objects per seconds. Luckily, you will probably never need to.

## Additional benefits
The following is not exactly related to Autofac.TypedFactories, but is more of a behavioral trait linked to the price of writing code without factories. One problem that usually starts showing up when instantiating objects without the use of a factory is how dependencies are expressed.

Quite often, the created objects will need to receive a dependency, such as a service. This service would typically be a singleton whose state will not depend on the context in which the object was instantiated. Therefore, the dependency could be directly resolved by an IoC, instead of explicitly passing an instance to the constructor of the class.

Typically, when no factories are used to instantiate a class, the containing class will receive a dependency in its constructor and keep a reference to it as an instance field. Sometimes, this field is not even used by the class for anything else than passing it to the constructor of a class to instantiate.

**Given an object to be instantiated:**

```csharp
public class CustomerViewModel
{
    private readonly int id;
    private readonly IGraphicsProvider graphicsProvider;

    public CustomerViewModel(int id, IGraphicsProvider graphicsProvider)
    {
        this.id = id;
        this.graphicsProvider = graphicsProvider;
    }
}
```
**Without factories:**

```csharp
public class CustomersController : Controller
{
    // This field will not even be used by CustomersController
    private readonly IGraphicsProvider graphicsProvider;

    // graphicsProvider is injected in this constructor, even if the
    // class doesn't depend on it, solely because one of the methods
    // will instantiate another class that depends on it. Codesmell!
    public CustomersController(IGraphicsProvider graphicsProvider)
    {
        this.graphicsProvider = graphicsProvider;
    }

    public ActionResult Get(int id)
    {
        // Here, we have an overly complicated call.
        // if only CustomerViewModel could resolve its own dependencies,
        // we could just pass the valuable information: id.
        var viewModel = new CustomerViewModel(id, this.graphicsProvider);
        return View(viewModel);
    }
}
```

When such a thing happens, the principle of dependency injection is broken, because there will be a class, which will receive a service injected in its constructor, but on which the class itself doesn't depend directly. What it will depend on, though, is the ability to instantiate another object, and this dependency should be expressed by injecting a factory.

**With factories**

```csharp
public class CustomersController : Controller
{
    private readonly ICustomerViewModelFactory customerViewModelFactory;

    // We replaced the services CustomerViewModel depends on by a factory.
    // This allows us to express that CustomersController depends on
    // the need to create CustomerViewModel objects.
    // In real-life scenarios, the factory will probably replace more than one
    // injection, making the constructor easier to digest.
    public CustomersController(ICustomerViewModelFactory customerViewModelFactory)
    {
        this.customerViewModelFactory = customerViewModelFactory;
    }

    public ActionResult Get(int id)
    {
        var viewModel = this.customerViewModelFactory.Create(id);
        return View(viewModel);
    }
}

internal interface ICustomerViewModelFactory
{
    CustomerViewModel Create(int id);
}
```
In this example, we replaced one service by a factory. This is because the factory will abstract away the creation of the ``CustomerViewModel``, and since the ``CustomersController`` doesn't need ``IGraphicsProvider`` firsthand, it can be removed from its injections.

In real-life scenarios, the factory will quite often replace more than one argument, making the constructor easier to digest. Even more elegantly, they will not show up in the factory implementation either, because the implementation doesn't exist, since it is generated on the fly by Autofac.TypedFactories.

The way the magic works behind the scene is quite simple: since the factory instantiates objects by using the autofac IoC container, any parameter that isn't specified in the ``Create`` method of the factory will be resolved by the IoC using a service locator approach, so as long as this service was registered in autofac, the instantiated object will have its dependencies satisfied. Otherwise, an exception will be thrown when the factory will be resolved.

## A few words about the service locator approach.
One of my favorite articles ever written about IoC containers and dependency injection is Mark Seemann's [Service Locator is an Anti-Pattern](http://blog.ploeh.dk/2010/02/03/ServiceLocatorisanAnti-Pattern/).

The title of the article pretty much sums it all up, and therefore, the fact that Autofac.TypedFactories relies on a service locator approach could be worrying. However, in this particular case, we are dealing with an internal mechanism, wrapped with explicit exceptions, and the consumer of the package will never have to use a service locator pattern itself. SOLID principles are preserved, encapsulation is preserved, everything is fine. You can knock yourself out using typed factories without feeling guilty about resorting to anti-patterns, as it doesn't apply to this particular scenario.
