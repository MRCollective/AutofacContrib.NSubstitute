using NSubstitute;
using NSubstitute.Core;
using NSubstitute.Exceptions;
using NUnit.Framework;

namespace AutofacContrib.NSubstitute.Tests
{
    [TestFixture]
    public sealed class AutoMockFixture
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
            using (var mock = new AutoMock())
            {
                RunWithSingleSetupationTest(mock);
            }
        }

        [Test]
        public void ProvideMock()
        {
            using (var autoMock = new AutoMock())
            {
                var mockA = Substitute.For<IServiceA>();
                autoMock.Provide(mockA);

                var component = autoMock.Resolve<TestComponent>();
                component.RunAll();

                mockA.Received().RunA();
            }
        }

        [Test]
        public void ProvideImplementation()
        {
            using (var mock = new AutoMock())
            {
                var serviceA = mock.Provide<IServiceA, ServiceA>();

                Assert.IsNotNull(serviceA);
                Assert.IsFalse(serviceA is ICallRouter);
            }
        }

        [Test]
        public void DefaultConstructorWorksWithAllTests()
        {
            using (var mock = new AutoMock())
            {
                RunTest(mock);
            }
        }

        [Test]
        public void WorksWithUnmetSetupations()
        {
            using (var loose = new AutoMock())
            {
                RunWithSingleSetupationTest(loose);
            }
        }

        [Test]
        [ExpectedException(typeof(ReceivedCallsException))]
        public void NormalSetupationsAreVerified()
        {
            using (var mock = new AutoMock())
            {
                SetUpSetupations(mock);
            }
        }

        [Test]
        public void ProperInitializationIsPerformed()
        {
            var autoMock = new AutoMock();
            Assert.IsNotNull(autoMock.Container);
        }

        private static void RunTest(AutoMock mock)
        {
            var component = mock.Resolve<TestComponent>();
            component.RunAll();

            SetUpSetupations(mock);
        }

        private static void SetUpSetupations(AutoMock autoMock)
        {
            autoMock.Resolve<IServiceB>().Received().RunB();
            autoMock.Resolve<IServiceA>().Received().RunA();
        }

        private static void RunWithSingleSetupationTest(AutoMock autoMock)
        {
            var component = autoMock.Resolve<TestComponent>();
            component.RunAll();

            autoMock.Resolve<IServiceB>().Received().RunB();
        }
    }
}