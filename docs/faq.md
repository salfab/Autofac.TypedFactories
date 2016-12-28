---
layout: default
---
# FAQ

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
