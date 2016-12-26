|master branch|latest commit|NuGet|
|:---:|:---:|:---:|
|[![Build status](https://ci.appveyor.com/api/projects/status/ge22hqh4xken8rgv/branch/master?svg=true)](https://ci.appveyor.com/project/salfab/autofac-typedfactories/branch/master)|[![Build status](https://ci.appveyor.com/api/projects/status/ge22hqh4xken8rgv?svg=true)](https://ci.appveyor.com/project/salfab/autofac-typedfactories)| [![NuGet version](https://img.shields.io/nuget/v/Autofac.TypedFactories.svg)](https://badge.fury.io/nu/Autofac.TypedFactories)|

# TL;DR - Autofac.TypedFactories
This package provides automatic Automatic Factory functionality similar to Castle.Windsor Typed Factories, for the Autofac IoC container.

# What is Autofac.TypedFactories ?

Autofac.TypedFactories aims at solving a problem typically encountered when practicing TDD.
When unit-testing a method, whenever a new object is instantiated in the body of the method, the unit test has no control over what the new statement will return.

In typical scenarios, we will want our test to assert that the created object has been used properly. For instance, that a given method has been called on it a number of times, with a certain parameter, or that it has been passed as an argument to another method.

Unfortunately, there are no easy and cheap way to control what the new statement returns. The usual way to get around to it is to replace every new statement by a call to a Create method on a factory injected as a dependency. The factory can then be mocked and injected in the arrange part of the test, and configured so that the returned object is also a mock, which can ultimately be monitored by a mocking framework, such as moq.

The problem however, is that this is a tedious task, because every single factory needs to be implemented manually. For each type instantiated in a tested method, there must be: 

- An implementation of the instantiated class.
- An interface the class will need to implement (in order to mock the returned object)
- An interface describing the contract of the factory
- An implementation of the factory.
	
I am not too fussed about the interfaces, the one for the instantiated class can be extracted from the implementation in no time, thanks to tools such as ReSharper. The interface for the factory needs to be described, there is no magic if we need our code to be strongly typed. This leaves us with the implementation of the factory itself. This usually brings no value, since all we expect it to do is to take the arguments that were passed to the factory's method, and pass them along to the call to the class' constructor. Boilerplate code, nothing else. This can certainly be improved, and it is exactly what Autofac.TypedFactories aims to do.

## How ?
There would basically be two different approaches, each with their pros and cons.

### Generated Code
Using Roslyn, it would be technically possible to analyze the signatures of both the Create method, and the constructor of the object it should return. A file could therefore be generated and implement the factory. This would yield extremely good performances, because the code generated would be standard compiled .net code. All performance impact would be paid upfront when compiling the project using the factory. The process, however is a bit quirky, as it would involves having lots of generated files that would give a cluttered feel to the solution and that would still need to be registered in the IoC container. 

### Dynamic proxies
The other approach is to generate dynamic proxy objects at runtime, and instantiate the objects, not by calling the new statement, but by leveraging the power of an IoC container.
On the plus side, this means no files generated at compilation, less files in the solution, and a simpler implementation of the package.
The downside is that the performances aren't quite as good as if it were standard C# code. Autofac claims that resolving a type through its IoC container is ten times more expensive than  newing it up with the new statement. This means that if this approach is great for general purposes, you probably will want to hand roll your own factories using standard C# classes if your factory needs to instantiate hundreds of objects per seconds. Luckily, you'll probably never need to.

## Additional benefits
One problem that usually starts showing up when instantiating objects without the use of a factory is how dependencies are expressed. 
Quite often, the created objects will need to receive a dependency such as a service. These service would typically be singleton, and not depend on the context in which the object was instantiated. Therefore, these dependencies could be directly resolved by an IoC, instead of passing an instance to the constructor of the class.

What would typically happen when a method instantiates a class without the use of a factory is that the containing class will receive a dependency in its constructor and keep a reference to it as an instance field. Sometimes, this field is not even used by the class for anything else than passing it to the constructor of a class to instantiate. 

When such a thing happens, the principle of dependency injection is broken, because there will be a class, who will get a service injected in its constructor, but on which the class itself doesn't depend directly. What it will depend on, though, is the ability to instantiate an other object, and this dependency should be expressed by injection.

The mechanism responsible for the instantiation of that other object, however, will depend on a number of services that will be passed to its constructor. The beauty of it is that these services will not be present in the first class, because they will be abstracted by the injected factory. Even more elegantly, they will not show up in the factory implementation either, because the implementation doesn't exist, since it is generated on the fly by Autofac.TypedFactories.

The magic behind it is quite simple: since the factory instantiates objects by using the autofac IoC, any parameter that isn't specified in the Create method of the factory will be resolved by the IoC using the a service locator approach, so as long as this service was registered in autofac, the instantiated object will have its dependencies satisfied. Otherwise, an exception will be thrown.

## A few words about the service locator approach.
One of my favorite article ever written about IoC containers and dependency injection is ploeh's Service Locator is an Anti-Pattern. http://blog.ploeh.dk/2010/02/03/ServiceLocatorisanAnti-Pattern/.

The title of the article pretty much sums it all, and therefore, the fact thattyped factories rely on a service locator could be worrying, however,  in this particular case we are dealing with an internal mechanism, wrapped with explicit exceptions and the consumer of the package will never have to use a service locator pattern itself. SOLID principles are preserved, encapsulation is preserved, everything is fine. You can knock yourself out using typed factories without feeling guilty about resorting to anti-patterns, it doesn't apply to this particular scenario.

# Q&A

## Doesn't autofac already provide something for that ?
Yes, autofac provides what they call delegate factories. (http://autofac.readthedocs.io/en/latest/advanced/delegate-factories.html). Delegate Factories are C# delegates that need to be declared in the class to be instantiated, and whose signature should match the constructor. Such delegates can be injected as a dependency, and invoked when a new instance of that class needs to be created. The convenient thing with this approach is that autofac will register the delegate along with the type to be built. The downside is that since the so-called factory isn't represented as an interface, it is not easy to mock its usage in the assert of a unit-test. Instead of a proper mock, a new instance of a delegate must be created for the unit tests, from scratch. Since it will not be possible to use a mocking framework such as mock, some common testing scenarios will prove quite challenging. For instance, counting the number of time a factory has been called will need to be implemented manually. If we had used a mocking framework, this would have been supported out of the box with a dedicated mocking API. Same for configuring it to return different values depending on the parameters.

Long story short, autofac's delegate factories can be stubbed, not mocked.

## Isn't there already other TypedFactories packages ?
There are many IoC containers for .net out there, and pretty much all of them have their TypedFactories package.
- Ninject has an official Factory extension, https://github.com/ninject/Ninject.Extensions.Factory.
- Castle.Windsor has an official Typed factory facility interface-based factories. https://github.com/castleproject/Windsor/blob/master/docs/typed-factory-facility-interface-based.md. 
- Unity has a community-maintained package, https://github.com/PombeirP/Unity.TypedFactories. 
	
Unfortunately, no such package existed for autofac. It looked like a mission for me :)

Unity.TypedFactories was actually developed by a former colleague of mine, and I have used it in the past, when I was working with Unity. I am therefore quite partial to its approach and the codebase was simple enough that I could base an autofac implementation on it.
