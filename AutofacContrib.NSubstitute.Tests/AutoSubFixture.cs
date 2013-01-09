using NSubstitute;
using NUnit.Framework;

namespace AutofacContrib.NSubstitute.Tests
{
    [TestFixture]
    public class AutoSubTests
    {
        [Test]
        public void ShouldCreateTheSut()
        {
            var autoSub = new AutoSub<ConcreteClassWithOneDependency>();
            var sut = autoSub.SUT();
            Assert.IsInstanceOf<ConcreteClassWithOneDependency>(sut);
        }

        [Test]
        public void SutShouldNotBeNull()
        {
            var autoSub = new AutoSub<ConcreteClassWithOneDependency>();
            var sut = autoSub.SUT();
            Assert.IsNotNull(sut);
        }

        [Test]
        public void ShouldCreateSutConstructorDependency()
        {
            var autoSub = new AutoSub<ConcreteClassWithOneDependency>();
            var sut = autoSub.SUT();
            Assert.IsNotNull(autoSub.Dependency<IDependency>());
        }

        [Test]
        public void ShouldReturnSameDependencyAsSutDependency()
        {
            var autoSub = new AutoSub<ConcreteClassWithOneDependency>();
            var sut = autoSub.SUT();
            Assert.AreSame(autoSub.Dependency<IDependency>(), sut.DependencyAsProperty);
        }

        [Test]
        public void ShouldThrowIfNonDependencyRequested()
        {
            var autoSub = new AutoSub<ConcreteClassWithOneDependency>();
            var sut = autoSub.SUT();

            Assert.IsNull(autoSub.Dependency<IUnrelatedDependency>());
        }

        [Test]
        public void DependencyShouldReturnValueWhenSet()
        {
            var autoSub = new AutoSub<ConcreteClassWithOneDependency>();
            var sut = autoSub.SUT();
            var itest = autoSub.Dependency<IDependency>();
            itest.SomeInt().Returns(3);
            Assert.AreEqual(3, sut.returnInt());
        }

    }

    public class ConcreteClassWithOneDependency
    {
        private IDependency _dependency;

        public IDependency DependencyAsProperty
        {
            get { return _dependency; }
        }

        public ConcreteClassWithOneDependency(IDependency dependency)
        {
            _dependency = dependency;
        }

        public int returnInt()
        {
            return _dependency.SomeInt();
        }
    }

    public interface IDependency
    {
        int SomeInt();
    }

    public interface IUnrelatedDependency
    {
        
    }
}
