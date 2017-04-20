using Autofac.Features.Indexed;
using NSubstitute;
using Xunit;

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

    public class KeyedRegistrationFixture
    {
        [Fact]
        public static void ShouldResolveIndexedDependencies()
        {
            var autoSubstitute = new AutoSubstitute();

            var target = autoSubstitute.Resolve<ClassWithKeyedDependencies>();

            Assert.NotNull(target.OnDependency);
            Assert.NotNull(target.OffDependency);
        }

        [Fact]
        public static void ShouldResolveASubstituteForIndexedDependency()
        {
            var autoSubstitute = new AutoSubstitute();
            var index = autoSubstitute.Resolve<IIndex<Switch, IDependency2>>();
            index[Switch.On].SomeOtherMethod().Returns(5);

            var target = autoSubstitute.Resolve<ClassWithKeyedDependencies>();

            Assert.Equal(target.OnDependency.SomeOtherMethod(), 5);
        }

        [Fact]
        public static void ShouldAcceptProvidedIndexedDependency()
        {
            var autoSubstitute = new AutoSubstitute();
            var substitute = Substitute.For<IDependency2>();
            substitute.SomeOtherMethod().Returns(5);
            autoSubstitute.Provide(substitute, Switch.On);
            
            var target = autoSubstitute.Resolve<ClassWithKeyedDependencies>();

            Assert.Equal(target.OnDependency.SomeOtherMethod(), 5);
        }
    }
}
