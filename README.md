![Chameleon Forms logo](https://raw.githubusercontent.com/MRCollective/AutofacContrib.NSubstitute/master/logo.png)

AutofacContrib.NSubstitute (AutoSubstitute)
===========================================

[![Build status](https://ci.appveyor.com/api/projects/status/wwmq5opfp47ff937?svg=true)](https://ci.appveyor.com/project/MRCollective/autofaccontrib-nsubstitute)
[![NuGet downloads](https://img.shields.io/nuget/dt/AutofacContrib.NSubstitute.svg)](https://www.nuget.org/packages/AutofacContrib.NSubstitute) 
[![NuGet version](https://img.shields.io/nuget/vpre/AutofacContrib.NSubstitute.svg)](https://www.nuget.org/packages/AutofacContrib.NSubstitute)

An auto-mocking `Autofac` container that resolves unknown dependencies from `NSubstitute`. Useful for unit testing classes with lots of dependencies.

Installation
------------

Install via [NuGet](http://www.nuget.org/packages/AutofacContrib.NSubstitute/), either in Visual Studio (right-click project, Manage NuGet Packages, search for `AutofacContrib.NSubstitute`) or via the package manager console using `Install-Package AutofacContrib.NSubstitute`.

Example Usage
-------------

Given the following code:
```c#
public interface IDependency1
{
    int SomeMethod(int i);
}

public interface IDependency2
{
    int SomeOtherMethod();
}

public class MyClass
{
    private readonly IDependency1 _d1;
    private readonly IDependency2 _d2;

    public MyClass(IDependency1 d1, IDependency2 d2)
    {
        _d1 = d1;
        _d2 = d2;
    }

    public int AMethod()
    {
        return _d1.SomeMethod(_d2.SomeOtherMethod());
    }
}
```

Then consider the following test which outlines how to use the `AutoSubstitute` class:

```c#
[Test]
public void Example_test_with_standard_resolve()
{
    const int val = 3;
    var autoSubstitute = new AutoSubstitute();
    autoSubstitute.Resolve<IDependency2>().SomeOtherMethod().Returns(val);
    autoSubstitute.Resolve<IDependency1>().SomeMethod(val).Returns(c => c.Arg<int>());

    var result = autoSubstitute.Resolve<MyClass>().AMethod();

    Assert.That(result, Is.EqualTo(val));
}
```

You can also provide concrete classes to `AutoSubstitute` either explicitly or by type, consider the following code to add the code above:

```c#
public class Dependency2 : IDependency2
{
    public const int Value = 10;

    public int SomeOtherMethod()
    {
        return Value;
    }
}

public class ConcreteClass
{
    private readonly int _i;

    public ConcreteClass(int i)
    {
        _i = i;
    }

    public virtual int Add(int val)
    {
        return val + _i;
    }
}

public class MyClassWithConcreteDependency
{
    private readonly ConcreteClass _c;
    private readonly IDependency2 _d2;

    public MyClassWithConcreteDependency(ConcreteClass c, IDependency2 d2)
    {
        _c = c;
        _d2 = d2;
    }

    public int AMethod()
    {
        return _c.Add(_d2.SomeOtherMethod());
    }
}
```

And then the following tests:

```c#
[Test]
public void Example_test_with_concrete_type_provided()
{
    const int val = 3;
    using var autoSubstitute = AutoSubstitute.Configure()
        .Provide<IDependency2, Dependency2>()
        .Build();
    autoSubstitute.Resolve<IDependency2>().SomeOtherMethod().Returns(val); // This shouldn't do anything because of the next line
    autoSubstitute.Resolve<IDependency1>().SomeMethod(Arg.Any<int>()).Returns(c => c.Arg<int>());

    var result = autoSubstitute.Resolve<MyClass>().AMethod();

    Assert.That(result, Is.EqualTo(Dependency2.Value));
}

[Test]
public void Example_test_with_concrete_object_provide_by_parameter()
{
    using var mock = AutoSubstitute.Configure()
        .Provide<ConcreteClassWithDependency>(new TypedParameter(typeof(int), 5))
        .Build();
}

[Test]
public void Example_test_with_concrete_object_provide()
{
    const int val1 = 3;
    const int val2 = 2;
    using var autoSubstitute = AutoSubstitute.Configure()
        .Provide(new ConcreteClass(val2))
        .Build();
    autoSubstitute.Resolve<IDependency2>().SomeOtherMethod().Returns(val1);

    var result = autoSubstitute.Resolve<MyClassWithConcreteDependency>().AMethod();

    Assert.That(result, Is.EqualTo(val1 + val2));
}
```

### NSubstitute Helpers

There is also a convenient syntax for registering and resolving a `Substitute.For<T>()` with the underlying container for concrete classes:

```c#
[Test]
public void Example_test_with_substitute_for_concrete()
{
    const int val1 = 3;
    const int val2 = 2;
    const int val3 = 10;
    using var autoSubstitute = AutoSubstitute.Configure()
        .SubstituteFor<ConcreteClass>(val2).Configure(c => c.Add(Arg.Any<int>()).Returns(val3))
        .Build();
    autoSubstitute.Resolve<IDependency2>().SomeOtherMethod().Returns(val1);

    var result = autoSubstitute.Resolve<MyClassWithConcreteDependency>().AMethod();

    Assert.That(result, Is.EqualTo(val3));
}
```

If you want to configure it with a service from the container, you can use a separate overload:

```c#
[Test]
public void SubstituteForConfigureWithContext()
{
	const int val = 2;

	using var utoSubstitute = AutoSubstitute.Configure()
		.SubstituteFor<ConcreteClass>(val).Configured()
		.SubstituteFor<ConcreteClassWithObject>().Configure((s, ctx) =>
		{
			s.Configure().GetResult().Returns(ctx.Resolve<ConcreteClass>());
		})
		.Build()
		.Container;

	var result = utoSubstitute.Resolve<ConcreteClassWithObject>().GetResult();

	Assert.AreSame(result, utoSubstitute.Resolve<ConcreteClass>());
}
```

If you would rather register it using NSubstitute's `SubstituteForPartsOf` method, you may use the following:

```c#
public abstract class Test1
{
	public virtual object Throws() => throw new InvalidOperationException();
}

[Test]
public void BaseCalledOnSubstituteForPartsOf()
{
    using var mock = AutoSubstitute.Configure()
        .SubstituteForPartsOf<Test1>().Configured()
        .Build();

    var test1 = mock.Resolve<Test1>();

    Assert.Throws<InvalidOperationException>(() => test1.Throws());
}
```

There is a helper method to ensure base calls are not routed by default for any method on partial substitutes if desired:

```c#
public abstract class Test1
{
	public virtual object Throws() => throw new InvalidOperationException();
}

[Test]
public void BaseCalledOnSubstituteForPartsOf()
{
    using var mock = AutoSubstitute.Configure()
        .SubstituteForPartsOf<Test1>().DoNotCallBase().Configured()
        .Build();

    var test1 = mock.Resolve<Test1>();

    Assert.IsNull(test1.Throws());
}
```

You can also enforce all properties to be automatically injected on this substitute:

```c#
[Test]
public void PropertiesSetIfRequested()
{
	using var mock = AutoSubstitute.Configure()
		.Provide<IProperty, CustomProperty>(out var property)
		.SubstituteFor<TestWithProperty>()
			.InjectProperties()
			.Configured()
		.Build();

	Assert.AreEqual(property.Value, mock.Resolve<TestWithProperty>().PropertySetter);
	Assert.AreEqual(property.Value, mock.Resolve<TestWithProperty>().VirtualProperty);
}

[Test]
public void PropertiesSetIfGloballyRequested()
{
	using var mock = AutoSubstitute.Configure()
		.InjectProperties()
		.Provide<IProperty, CustomProperty>(out var property)
		.SubstituteFor<TestWithProperty>().Configured()
		.Build();

	Assert.AreEqual(property.Value, mock.Resolve<TestWithProperty>().PropertySetter);
	Assert.AreEqual(property.Value, mock.Resolve<TestWithProperty>().VirtualProperty);
}

```

Similarly, you can resolve a concrete type from the AutoSubstitute container and register that with the underlying container using the `ResolveAndSubstituteFor` method:

```c#
public class ConcreteClassWithDependency
{
    private readonly IDependency1 _dependency;
    private readonly int _i;

    public ConcreteClassWithDependency(IDependency1 dependency, int i)
    {
        _dependency = dependency;
        _i = i;
    }

    public int Double()
    {
        return _dependency.SomeMethod(_i)*2;
    }
}

public class MyClassWithConcreteDependencyThatHasDependencies
{
    private readonly ConcreteClassWithDependency _c;
    private readonly IDependency2 _d2;

    public MyClassWithConcreteDependencyThatHasDependencies(ConcreteClassWithDependency c, IDependency2 d2)
    {
        _c = c;
        _d2 = d2;
    }

    public int AMethod()
    {
        return _d2.SomeOtherMethod() * _c.Double();
    }
}

...

[Test]
public void Example_test_with_substitute_for_concrete_resolved_from_autofac()
{
    const int val1 = 2;
    const int val2 = 3;
    const int val3 = 4;
    using var AutoSubstitute = AutoSubstitute.Configure()
        .ResolveAndSubstituteFor<ConcreteClassWithDependency>(new TypedParameter(typeof(int), val1))
        .Build();
    AutoSubstitute.Resolve<IDependency2>().SomeOtherMethod().Returns(val2);
    AutoSubstitute.Resolve<IDependency1>().SomeMethod(val1).Returns(val3);

    var result = AutoSubstitute.Resolve<MyClassWithConcreteDependencyThatHasDependencies>().AMethod();

    Assert.That(result, Is.EqualTo(val2*val3*2));
}
```

### Direct configuration with ContainerBuilder

If you need to access the underlying Autofac container for some reason then you use the `Container` property on the `AutoSubstitute` object.

If you want to make modifications to the container builder before the container is build from it there is a second constructor parameter you can use, for example:

```c#
var autoSubstitute = new AutoSubstitute(cb => cb.RegisterModule<SomeModule>());
```

Options Configuration
---------------------

There are various options that can be used to set up the container and NSubstitute in helpful ways for some scenarios.  These are exposed via the `AutoSubstituteBuilder.ConfigureOptions()` method. The options currently available:

- Add custom `MockHandler` instances that can intercept the creation of the NSubstitute mocks
- Add custom handlers for the registration of implicit service creation via `AnyConcreteTypeNotAlreadyRegisteredSource`
- Add types that will be skipped during the mock generation via `TypesToSkipForMocking` option
- Automatically skip mock generation of types that are specified in Provide methods via `AutomaticallySkipMocksForProvidedValues`

Some convenience methods build upon this to enable a few common scenarios:

- `AutoSubstituteBuilder.InjectProperties()` - This enables auto injection of properties on the NSubstitute mock from the configured container
- `AutoSubstituteBuilder.MakeUnregisteredTypesPerLifetime()` - This configures the `AnyConcreteTypeNotAlreadyRegisteredSource` to be per lifetime scope
- `AutoSubstituteBuilder.UnregisteredTypesUseInternalConstructor()` - This configures the `AnyConcreteTypeNotAlreadyRegisteredSource` to search for non-public constructors as well.
