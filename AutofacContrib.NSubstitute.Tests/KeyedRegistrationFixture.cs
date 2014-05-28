using Autofac.Features.Indexed;
using NSubstitute;
using NUnit.Framework;

namespace AutofacContrib.NSubstitute.Tests
{
    public enum Switch
    {
        Off,
        On
    }

    public class ClassWithKeyedDependencies
    {
        public ClassWithKeyedDependencies(IIndex<Switch, IDependency2> dependencies)
        {
            OnDependency = dependencies[Switch.On];
            OffDependency = dependencies[Switch.Off];
        }

        public IDependency2 OnDependency { get; private set; }
        public IDependency2 OffDependency { get; private set; }
    }

    public static class KeyedRegistrationFixture
    {
        [Test]
        public static void ShouldResolveIndexedDependencies()
        {
            var autoSubstitute = new AutoSubstitute();

            var target = autoSubstitute.Resolve<ClassWithKeyedDependencies>();

            Assert.NotNull(target.OnDependency);
            Assert.NotNull(target.OffDependency);
        }

        [Test]
        public static void ShouldResolveASubstituteForIndexedDependency()
        {
            var autoSubstitute = new AutoSubstitute();
            var index = autoSubstitute.Resolve<IIndex<Switch, IDependency2>>();
            index[Switch.On].SomeOtherMethod().Returns(5);

            var target = autoSubstitute.Resolve<ClassWithKeyedDependencies>();

            Assert.That(target.OnDependency.SomeOtherMethod(), Is.EqualTo(5));
        }

        [Test]
        public static void ShouldAcceptProvidedIndexedDependency()
        {
            var autoSubstitute = new AutoSubstitute();
            var substitute = Substitute.For<IDependency2>();
            substitute.SomeOtherMethod().Returns(5);
            autoSubstitute.Provide(substitute, Switch.On);
            
            var target = autoSubstitute.Resolve<ClassWithKeyedDependencies>();

            Assert.That(target.OnDependency.SomeOtherMethod(), Is.EqualTo(5));
        }
    }
}
