using NSubstitute;
using NSubstitute.Core;
using NSubstitute.Exceptions;
using NUnit.Framework;

namespace AutofacContrib.NSubstitute.Tests
{
    [TestFixture]
    public sealed class AutoSubstituteFixture
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
        [Test]
        public void DefaultConstructorIsLoose()
        {
            using (var mock = new AutoSubstitute())
            {
                RunWithSingleSetupationTest(mock);
            }
        }

        [Test]
        public void ProvideMock()
        {
            var mockA = Substitute.For<IServiceA>();

            using (var autoSubstitute = new AutoSubstitute(b =>
            {
                b.Provide(mockA);
            }))
            {
                var component = autoSubstitute.Resolve<TestComponent>();
                component.RunAll();

                mockA.Received().RunA();
            }
        }

        [Test]
        public void ProvideImplementation()
        {
            using (var mock = new AutoSubstitute(b =>
            {
                b.Provide<IServiceA, ServiceA>();
            }))
            {
                var serviceA = mock.Resolve<ServiceA>();

                Assert.IsNotNull(serviceA);
                Assert.IsFalse(serviceA is ICallRouter);
            }
        }

        [Test]
        public void DefaultConstructorWorksWithAllTests()
        {
            using (var mock = new AutoSubstitute())
            {
                RunTest(mock);
            }
        }

        [Test]
        public void WorksWithUnmetSetupations()
        {
            using (var loose = new AutoSubstitute())
            {
                RunWithSingleSetupationTest(loose);
            }
        }

        [Test]
        public void NormalSetupationsAreVerified()
        {
            using (var mock = new AutoSubstitute())
            {
                Assert.That(() => SetUpSetupations(mock), Throws.TypeOf<ReceivedCallsException>());
            }
        }

        [Test]
        public void ProperInitializationIsPerformed()
        {
            var autoSubstitute = new AutoSubstitute();
            Assert.IsNotNull(autoSubstitute.Container);
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