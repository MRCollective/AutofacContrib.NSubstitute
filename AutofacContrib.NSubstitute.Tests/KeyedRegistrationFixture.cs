using Autofac.Features.Indexed;
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
        public static void ShouldResolveIndexedDependency()
        {
            var autoSubstitute = new AutoSubstitute();

            var target = autoSubstitute.Resolve<ClassWithKeyedDependencies>();

            Assert.NotNull(target.OnDependency);
            Assert.NotNull(target.OffDependency);
        }
    }
}
