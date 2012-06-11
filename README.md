AutofacContrib.NSubstitute
==========================

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

Then consider the following test which outlines how to use the `AutoMock` class:

    [Test]
    public void Example_test()
    {
        const int val = 3;
        var autoMock = new AutoMock();
        autoMock.Resolve<IDependency2>().SomeOtherMethod().Returns(val);
        autoMock.Resolve<IDependency1>().SomeMethod(val).Returns(c => c.Arg<int>());

        var result = autoMock.Resolve<MyClass>().AMethod();

        Assert.That(result, Is.EqualTo(val));
    }

You can also provide classes to `AutoMock`, consider the following code in addition to the above:

    public class ConcreteClass
    {
        private readonly int _i;

        public ConcreteClass(int i)
        {
            _i = i;
        }

        public int Add(int val)
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

And then the following test:

    [Test]
    public void Example_test_with_provide_and_resolve()
    {
        const int val1 = 3;
        const int val2 = 2;
        var autoMock = new AutoMock();
        autoMock.Resolve<IDependency2>().SomeOtherMethod().Returns(val1);
        autoMock.Provide(new ConcreteClass(val2));

        var result = autoMock.Resolve<MyClassWithConcreteDependency>().AMethod();

        Assert.That(result, Is.EqualTo(val1 + val2));
    }

If you need to access the underlying Autofac container for some reason then you use the `Container` property on the `AutoMock` object.