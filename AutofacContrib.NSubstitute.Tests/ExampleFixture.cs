using Autofac;
using NSubstitute;
using NUnit.Framework;

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
            return _dependency.SomeMethod(_i)*2;
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
            var AutoSubstitute = new AutoSubstitute();
            AutoSubstitute.Resolve<IDependency2>().SomeOtherMethod().Returns(val); // This shouldn't do anything because of the next line
            AutoSubstitute.Provide<IDependency2, Dependency2>();
            AutoSubstitute.Resolve<IDependency1>().SomeMethod(Arg.Any<int>()).Returns(c => c.Arg<int>());

            var result = AutoSubstitute.Resolve<MyClass>().AMethod();

            Assert.That(result, Is.EqualTo(Dependency2.Value));
        }

        [Test]
        public void Example_test_with_concrete_object_provided()
        {
            const int val1 = 3;
            const int val2 = 2;
            var AutoSubstitute = new AutoSubstitute();
            AutoSubstitute.Resolve<IDependency2>().SomeOtherMethod().Returns(val1);
            AutoSubstitute.Provide(new ConcreteClass(val2));

            var result = AutoSubstitute.Resolve<MyClassWithConcreteDependency>().AMethod();

            Assert.That(result, Is.EqualTo(val1 + val2));
        }

        [Test]
        public void Example_test_with_substitute_for_concrete()
        {
            const int val1 = 3;
            const int val2 = 2;
            const int val3 = 10;
            var AutoSubstitute = new AutoSubstitute();
            AutoSubstitute.Resolve<IDependency2>().SomeOtherMethod().Returns(val1);
            AutoSubstitute.SubstituteFor<ConcreteClass>(val2).Add(Arg.Any<int>()).Returns(val3);

            var result = AutoSubstitute.Resolve<MyClassWithConcreteDependency>().AMethod();

            Assert.That(result, Is.EqualTo(val3));
        }

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
    }
}
