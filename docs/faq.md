---
layout: default
---
# FAQ

## Doesn't autofac already provide automatic factories?
Yes, autofac provides what they call [delegate factories](http://autofac.readthedocs.io/en/latest/advanced/delegate-factories.html).

Delegate Factories are C# delegates that need to be declared in the class to be instantiated, and whose signature should match the constructor. Such delegates can be injected as a dependency, and invoked when a new instance of that class needs to be created.

example:
```csharp
public class Shareholding
{
  public delegate Shareholding Factory(string symbol, uint holding);

  public Shareholding(string symbol, uint holding)
  {
    Symbol = symbol;
    Holding = holding;
  }

  public string Symbol { get; private set; }

  public uint Holding { get; set; }
}
```
and the factory can be injected and used in a constructor as follows:

```csharp
public class MyClass
{
	public MyClass(Shareholding.Factory shareholdingFactory)
	{
		var shareholding = shareholdingFactory.Invoke("ABC", 1234);
	}
}
```
### Delegate Factories support stubs, not mocks
The convenient thing with this approach is that autofac will register the ``Shareholding.Factory`` delegate along with the ``Shareholding`` type. The downside is that since the so-called factory is not represented as an interface, it is not easy to mock its usage in a unit-test. Instead of a mocking the factory with a mocking framework, a new instance of the delegate must be created from scratch for the unit tests. For this reason, some common testing scenarios will prove quite challenging. For instance, counting the number of time a factory has been called will need to be implemented manually. Had we used a mocking framework, this would have been supported out of the box with a dedicated mocking API. Same for configuring it to return different values depending on the parameters.

Long story short, autofac's delegate factories can be _stubbed_, not _mocked_.

## Aren't there already other dynamic factory packages?
There are many IoC containers for .net out there, and pretty much all of them have their TypedFactories package.
- Ninject [has an official Factory extension](https://github.com/ninject/Ninject.Extensions.Factory).
- Castle.Windsor has an official [Typed Factory Facility](https://github.com/castleproject/Windsor/blob/master/docs/typed-factory-facility-interface-based.md), supporting interface-based factories.
- Unity has a [community-maintained package](https://github.com/PombeirP/Unity.TypedFactories).

Unfortunately, no such package existed for autofac. It looked like a mission for me :)

Unity.TypedFactories was actually developed by a former colleague of mine, and since I have used it in the past when I was working with Unity, I was familiar enough with its approach and its codebase to base an autofac implementation on it.
