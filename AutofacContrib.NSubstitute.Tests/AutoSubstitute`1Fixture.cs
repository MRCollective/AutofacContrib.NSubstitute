using NSubstitute;
using NUnit.Framework;

namespace AutofacContrib.NSubstitute.Tests
{
    [TestFixture]
    public class AutoSubstituteSUTFixture
    {
        [Test]
        public void ShouldCreateTheSut()
        {
            var autoSub = new AutoSubstitute<ConcreteClassWithOneDependency>();

            var sut = autoSub.SUT;

            Assert.That(sut, Is.InstanceOf<ConcreteClassWithOneDependency>());
        }

        [Test]
        public void SutShouldNotBeNull()
        {
            var autoSub = new AutoSubstitute<ConcreteClassWithOneDependency>();

            var sut = autoSub.SUT;

            Assert.That(sut, Is.Not.Null);
        }

        [Test]
        public void ShouldCreateSutConstructorDependency()
        {
            var autoSub = new AutoSubstitute<ConcreteClassWithOneDependency>();

            var sut = autoSub.SUT;

            Assert.That(autoSub.Dependency<IDependency>(), Is.Not.Null);
        }

        [Test]
        public void ShouldReturnSameDependencyAsSutDependency()
        {
            var autoSub = new AutoSubstitute<ConcreteClassWithOneDependency>();

            var sut = autoSub.SUT;

            Assert.That(autoSub.Dependency<IDependency>(), Is.EqualTo(sut.DependencyAsProperty));
        }

        [Test]
        public void ShouldThrowIfNonDependencyRequested()
        {
            var autoSub = new AutoSubstitute<ConcreteClassWithOneDependency>();

            var sut = autoSub.SUT;

            Assert.That(autoSub.Dependency<IUnrelatedDependency>(), Is.Null);
        }

        [Test]
        public void DependencyShouldReturnValueWhenSet()
        {
            var autoSub = new AutoSubstitute<ConcreteClassWithOneDependency>();
            var sut = autoSub.SUT;
            var itest = autoSub.Dependency<IDependency>();

            itest.SomeInt().Returns(3);

            Assert.AreEqual(3, sut.ReturnInt());
        }
    }

    public class ConcreteClassWithOneDependency
    {
        private readonly IDependency _dependency;

        public IDependency DependencyAsProperty
        {
            get { return _dependency; }
        }

        public ConcreteClassWithOneDependency(IDependency dependency)
        {
            _dependency = dependency;
        }

        public int ReturnInt()
        {
            return _dependency.SomeInt();
        }
    }

    public interface IDependency
    {
        int SomeInt();
    }

    public interface IUnrelatedDependency {}
}
