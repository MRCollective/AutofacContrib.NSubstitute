using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Autofac;
using NSubstitute;
using NUnit.Framework;

namespace AutofacContrib.NSubstitute.Tests
{
    [TestFixture]
    public sealed class AutoSubstituteCollectionFixture
    {
        #region stubs

        public interface IServiceItem
        {
        }

        public class ServiceItemA : IServiceItem
        {
        }

        public class ServiceItemB : IServiceItem
        {
        }

        public sealed class TestIEnumerableComponent
        {
            public readonly IEnumerable<IServiceItem> ServiceItems;

            public TestIEnumerableComponent(IEnumerable<IServiceItem> serviceItems)
            {
                ServiceItems = serviceItems;
            }
        }

        public sealed class TestIListComponent
        {
            public readonly IList<IServiceItem> ServiceItems;

            public TestIListComponent(IList<IServiceItem> serviceItems)
            {
                ServiceItems = serviceItems;
            }
        }

        public sealed class TestIReadOnlyCollectionComponent
        {
            public readonly IReadOnlyCollection<IServiceItem> ServiceItems;

            public TestIReadOnlyCollectionComponent(IReadOnlyCollection<IServiceItem> serviceItems)
            {
                ServiceItems = serviceItems;
            }
        }

        public sealed class TestICollectionComponent
        {
            public readonly ICollection<IServiceItem> ServiceItems;

            public TestICollectionComponent(ICollection<IServiceItem> serviceItems)
            {
                ServiceItems = serviceItems;
            }
        }

        public sealed class TestIReadOnlyListComponent
        {
            public readonly IReadOnlyList<IServiceItem> ServiceItems;

            public TestIReadOnlyListComponent(IReadOnlyList<IServiceItem> serviceItems)
            {
                ServiceItems = serviceItems;
            }
        }

        #endregion

        [Test]
        public void TestIEnumerableCorrectlyResolves()
        {
            using (var autosub = new AutoSubstitute(b =>
             {
                 b.Provide<IServiceItem, ServiceItemA>();
                 b.Provide<IServiceItem, ServiceItemB>();
             }))
            {
                var mockA = autosub.Resolve<ServiceItemA>();
                var mockB = autosub.Resolve<ServiceItemB>();
                var component = autosub.Resolve<TestIEnumerableComponent>();

                Assert.That(component.ServiceItems, Is.Not.Empty);
                Assert.That(component.ServiceItems.Contains(mockA), Is.True);
                Assert.That(component.ServiceItems.Contains(mockB), Is.True);
            }
        }

        [Test]
        public void TestIListCorrectlyResolves()
        {
            using (var autosub = new AutoSubstitute(b =>
            {
                b.Provide<IServiceItem, ServiceItemA>();
                b.Provide<IServiceItem, ServiceItemB>();
            }))
            {
                var mockA = autosub.Resolve<ServiceItemA>();
                var mockB = autosub.Resolve<ServiceItemB>();
                var component = autosub.Resolve<TestIListComponent>();

                Assert.That(component.ServiceItems, Is.Not.Empty);
                Assert.That(component.ServiceItems.Contains(mockA), Is.True);
                Assert.That(component.ServiceItems.Contains(mockB), Is.True);
            }
        }

        [Test]
        public void TestIReadOnlyCollectionCorrectlyResolves()
        {
            using (var autosub = new AutoSubstitute(b =>
            {
                b.Provide<IServiceItem, ServiceItemA>();
                b.Provide<IServiceItem, ServiceItemB>();
            }))
            {
                var mockA = autosub.Resolve<ServiceItemA>();
                var mockB = autosub.Resolve<ServiceItemB>();
                var component = autosub.Resolve<TestIReadOnlyCollectionComponent>();

                Assert.That(component.ServiceItems, Is.Not.Empty);
                Assert.That(component.ServiceItems.Contains(mockA), Is.True);
                Assert.That(component.ServiceItems.Contains(mockB), Is.True);
            }
        }

        [Test]
        public void TestICollectionCorrectlyResolves()
        {
            using (var autosub = new AutoSubstitute(b =>
            {
                b.Provide<IServiceItem, ServiceItemA>();
                b.Provide<IServiceItem, ServiceItemB>();
            }))
            {
                var mockA = autosub.Resolve<ServiceItemA>();
                var mockB = autosub.Resolve<ServiceItemB>();
                var component = autosub.Resolve<TestICollectionComponent>();

                Assert.That(component.ServiceItems, Is.Not.Empty);
                Assert.That(component.ServiceItems.Contains(mockA), Is.True);
                Assert.That(component.ServiceItems.Contains(mockB), Is.True);
            }
        }

        [Test]
        public void TestIReadOnlyListCorrectlyResolves()
        {
            using (var autosub = new AutoSubstitute(b =>
            {
                b.Provide<IServiceItem, ServiceItemA>();
                b.Provide<IServiceItem, ServiceItemB>();
            }))
            {
                var mockA = autosub.Resolve<ServiceItemA>();
                var mockB = autosub.Resolve<ServiceItemB>();
                var component = autosub.Resolve<TestIReadOnlyListComponent>();

                Assert.That(component.ServiceItems, Is.Not.Empty);
                Assert.That(component.ServiceItems.Contains(mockA), Is.True);
                Assert.That(component.ServiceItems.Contains(mockB), Is.True);
            }
        }
    }
}