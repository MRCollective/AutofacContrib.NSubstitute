using NSubstitute;
using NSubstitute.Core;
using NSubstitute.Exceptions;
using Xunit;

namespace AutofacContrib.NSubstitute.Tests
{
    public class AutoSubstituteFixture
    {
        #region stubs
        public interface IServiceA
        {
            void RunA();
        }

        public interface IServiceB
        {
            void RunB();
        }

        public class ServiceA : IServiceA
        {
            public void RunA() { }
        }

        public sealed class TestComponent
        {
            private readonly IServiceA _serviceA;
            private readonly IServiceB _serviceB;

            public TestComponent(IServiceA serviceA, IServiceB serviceB)
            {
                this._serviceA = serviceA;
                this._serviceB = serviceB;
            }

            public void RunAll()
            {
                this._serviceA.RunA();
                this._serviceB.RunB();
            }
        }
        #endregion

        /// <summary>
        /// Defaults the constructor is loose.
        /// </summary>
        [Fact]
        public void DefaultConstructorIsLoose()
        {
            using (var mock = new AutoSubstitute())
            {
                RunWithSingleSetupationTest(mock);
            }
        }

        [Fact]
        public void ProvideMock()
        {
            using (var autoSubstitute = new AutoSubstitute())
            {
                var mockA = Substitute.For<IServiceA>();
                autoSubstitute.Provide(mockA);

                var component = autoSubstitute.Resolve<TestComponent>();
                component.RunAll();

                mockA.Received().RunA();
            }
        }

        [Fact]
        public void ProvideImplementation()
        {
            using (var mock = new AutoSubstitute())
            {
                var serviceA = mock.Provide<IServiceA, ServiceA>();

                Assert.NotNull(serviceA);
                Assert.False(serviceA is ICallRouter);
            }
        }

        [Fact]
        public void DefaultConstructorWorksWithAllTests()
        {
            using (var mock = new AutoSubstitute())
            {
                RunTest(mock);
            }
        }

        [Fact]
        public void WorksWithUnmetSetupations()
        {
            using (var loose = new AutoSubstitute())
            {
                RunWithSingleSetupationTest(loose);
            }
        }

        [Fact]
        public void NormalSetupationsAreVerified()
        {
            using (var mock = new AutoSubstitute())
            {
                Assert.Throws<ReceivedCallsException>(() => SetUpSetupations(mock));
            }
        }

        [Fact]
        public void ProperInitializationIsPerformed()
        {
            var autoSubstitute = new AutoSubstitute();
            Assert.NotNull(autoSubstitute.Container);
        }

        private static void RunTest(AutoSubstitute mock)
        {
            var component = mock.Resolve<TestComponent>();
            component.RunAll();

            SetUpSetupations(mock);
        }

        private static void SetUpSetupations(AutoSubstitute autoSubstitute)
        {
            autoSubstitute.Resolve<IServiceB>().Received().RunB();
            autoSubstitute.Resolve<IServiceA>().Received().RunA();
        }

        private static void RunWithSingleSetupationTest(AutoSubstitute autoSubstitute)
        {
            var component = autoSubstitute.Resolve<TestComponent>();
            component.RunAll();

            autoSubstitute.Resolve<IServiceB>().Received().RunB();
        }
    }
}