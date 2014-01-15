AutofacContrib.NSubstitute (AutoSubstitute)
===========================================

An auto-mocking `Autofac` container that resolves unknown dependencies from `NSubstitute`. Useful for unit testing classes with lots of dependencies.

Example Usage
-------------

Given the following code:

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

Then consider the following test which outlines how to use the `AutoSubstitute` class:

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

You can also provide concrete classes to `AutoSubstitute` either explicitly or by type, consider the following code to add the code above:

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

And then the following tests:

    [Test]
    public void Example_test_with_concrete_type_provided()
    {
        const int val = 3;
        var autoSubstitute = new AutoSubstitute();
        autoSubstitute.Resolve<IDependency2>().SomeOtherMethod().Returns(val); // This shouldn't do anything because of the next line
        autoSubstitute.Provide<IDependency2, Dependency2>();
        autoSubstitute.Resolve<IDependency1>().SomeMethod(Arg.Any<int>()).Returns(c => c.Arg<int>());

        var result = autoSubstitute.Resolve<MyClass>().AMethod();

        Assert.That(result, Is.EqualTo(Dependency2.Value));
    }

    [Test]
    public void Example_test_with_concrete_object_provide()
    {
        const int val1 = 3;
        const int val2 = 2;
        var autoSubstitute = new AutoSubstitute();
        autoSubstitute.Resolve<IDependency2>().SomeOtherMethod().Returns(val1);
        autoSubstitute.Provide(new ConcreteClass(val2));

        var result = autoSubstitute.Resolve<MyClassWithConcreteDependency>().AMethod();

        Assert.That(result, Is.EqualTo(val1 + val2));
    }

There is also a convenient syntax for registering and resolving a `Substitute.For<T>()` with the underlying container for concrete classes:

    [Test]
    public void Example_test_with_substitute_for_concrete()
    {
        const int val1 = 3;
        const int val2 = 2;
        const int val3 = 10;
        var autoSubstitute = new AutoSubstitute();
        autoSubstitute.Resolve<IDependency2>().SomeOtherMethod().Returns(val1);
        autoSubstitute.SubstituteFor<ConcreteClass>(val2).Add(Arg.Any<int>()).Returns(val3);

        var result = autoSubstitute.Resolve<MyClassWithConcreteDependency>().AMethod();

        Assert.That(result, Is.EqualTo(val3));
    }

Similarly, you can resolve a concrete type from the autosubstitute container and register that with the underlying container using the `ResolveAndSubstituteFor` method:

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
        var AutoSubstitute = new AutoSubstitute();
        // Much better / more maintainable than:
        //AutoSubstitute.SubstituteFor<ConcreteClassWithDependency>(AutoSubstitute.Resolve<IDependency1>(), val1);
        AutoSubstitute.ResolveAndSubstituteFor<ConcreteClassWithDependency>(new TypedParameter(typeof(int), val1));
        AutoSubstitute.Resolve<IDependency2>().SomeOtherMethod().Returns(val2);
        AutoSubstitute.Resolve<IDependency1>().SomeMethod(val1).Returns(val3);

        var result = AutoSubstitute.Resolve<MyClassWithConcreteDependencyThatHasDependencies>().AMethod();

        Assert.That(result, Is.EqualTo(val2*val3*2));
    }

If you need to access the underlying Autofac container for some reason then you use the `Container` property on the `AutoSubstitute` object.

If you want to make modifications to the container builder before the container is build from it there is a second constructor parameter you can use, for example:

    var autoSubstitute = new AutoSubstitute(cb => cb.RegisterModule<SomeModule>());
