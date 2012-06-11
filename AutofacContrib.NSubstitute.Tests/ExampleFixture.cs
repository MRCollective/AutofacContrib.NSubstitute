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

    [TestFixture]
    class ExampleFixture
    {
        [Test]
        public void Example_test_with_resolve()
        {
            const int val = 3;
            var autoMock = new AutoMock();
            autoMock.Resolve<IDependency2>().SomeOtherMethod().Returns(val);
            autoMock.Resolve<IDependency1>().SomeMethod(val).Returns(c => c.Arg<int>());

            var result = autoMock.Resolve<MyClass>().AMethod();

            Assert.That(result, Is.EqualTo(val));
        }

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
    }
}
