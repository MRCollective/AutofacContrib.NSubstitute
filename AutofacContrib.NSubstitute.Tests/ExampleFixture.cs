using Autofac;
using NSubstitute;
using NUnit.Framework;
using System;

namespace AutofacContrib.NSubstitute.Tests
{
    public interface IDependency1
    {
        int SomeMethod(int i);
    }

    public interface IDependency2
    {
        int SomeOtherMethod();
    }

    public class Dependency2 : IDependency2
    {
        public const int Value = 10;

        public int SomeOtherMethod()
        {
            return Value;
        }
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

    public class ConcreteClassWithObject
    {
        public virtual object GetResult()
        {
            return new object();
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
            return _dependency.SomeMethod(_i) * 2;
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

    [TestFixture]
    class ExampleFixture
    {
        [Test]
        public void Example_test_with_standard_resolve()
        {
            const int val = 3;
            var AutoSubstitute = new AutoSubstitute();
            AutoSubstitute.Resolve<IDependency2>().SomeOtherMethod().Returns(val);
            AutoSubstitute.Resolve<IDependency1>().SomeMethod(val).Returns(c => c.Arg<int>());

            var result = AutoSubstitute.Resolve<MyClass>().AMethod();

            Assert.That(result, Is.EqualTo(val));
        }

        [Test]
        public void Example_test_with_concrete_type_provided()
        {
            const int val = 3;

            using var mock = AutoSubstitute.Configure()
                .Provide<IDependency2, Dependency2>()
                .Build();

            mock.Resolve<IDependency2>().SomeOtherMethod().Returns(val); // This shouldn't do anything because of the next line
            mock.Resolve<IDependency1>().SomeMethod(Arg.Any<int>()).Returns(c => c.Arg<int>());

            var result = mock.Resolve<MyClass>().AMethod();

            Assert.That(result, Is.EqualTo(Dependency2.Value));
        }

        [Test]
        public void Example_test_with_concrete_object_provided()
        {
            const int val1 = 3;
            const int val2 = 2;

            var mock = AutoSubstitute.Configure()
                .Provide(new ConcreteClass(val2))
                .Build();

            mock.Resolve<IDependency2>().SomeOtherMethod().Returns(val1);

            var result = mock.Resolve<MyClassWithConcreteDependency>().AMethod();

            Assert.That(result, Is.EqualTo(val1 + val2));
        }

        [Test]
        [Obsolete]
        public void Example_test_with_substitute_for_concrete_resolved_from_autofac()
        {
            const int val1 = 2;
            const int val2 = 3;
            const int val3 = 4;

            using var mock = AutoSubstitute.Configure()
                .ResolveAndSubstituteFor<ConcreteClassWithDependency>(new TypedParameter(typeof(int), val1))
                .Build();

            mock.Resolve<IDependency2>().SomeOtherMethod().Returns(val2);
            mock.Resolve<IDependency1>().SomeMethod(val1).Returns(val3);

            var result = mock.Resolve<MyClassWithConcreteDependencyThatHasDependencies>().AMethod();

            Assert.That(result, Is.EqualTo(val2 * val3 * 2));
        }

        [Test]
        public void Example_provide_service()
        {
            const int val1 = 2;
            const int val2 = 3;
            const int val3 = 4;

            using var mock = AutoSubstitute.Configure()
                .Provide<ConcreteClassWithDependency>(new TypedParameter(typeof(int), val1))
                .Build();

            mock.Resolve<IDependency2>().SomeOtherMethod().Returns(val2);
            mock.Resolve<IDependency1>().SomeMethod(val1).Returns(val3);

            var result = mock.Resolve<MyClassWithConcreteDependencyThatHasDependencies>().AMethod();

            Assert.That(result, Is.EqualTo(val2 * val3 * 2));
        }

        [Test]
        public void Example_provide_service_with_out_param()
        {
            const int val1 = 2;

            using var mock = AutoSubstitute.Configure()
                .Provide<ConcreteClassWithDependency>(out var c, new TypedParameter(typeof(int), val1))
                .Build();

            var result = mock.Resolve<ConcreteClassWithDependency>();

            Assert.AreSame(result, c.Value);
        }

        [Test]
        public void CustomContainerBuilder()
        {
            var container = Substitute.For<IContainer>();

            using var mock = AutoSubstitute.Configure()
                .ConfigureOptions(options =>
                {
                    options.BuildContainerFactory = _ => container;
                })
                .Build();

            Assert.AreSame(container, mock.Container);
        }
    }
}
