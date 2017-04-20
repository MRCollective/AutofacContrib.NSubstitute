using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace AutofacContrib.NSubstitute.Tests
{
    public class AutoSubstituteCollectionFixture
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

        [Fact]
        public void TestIEnumerableCorrectlyResolves()
        {
            using(var autosub = new AutoSubstitute())
            {
                var mockA = autosub.Provide<IServiceItem, ServiceItemA>();
                var mockB = autosub.Provide<IServiceItem, ServiceItemB>();
                var component = autosub.Resolve<TestIEnumerableComponent>();

                Assert.NotEmpty(component.ServiceItems);
                Assert.True(component.ServiceItems.Contains(mockA));
                Assert.True(component.ServiceItems.Contains(mockB));
            }
        }

        [Fact]
        public void TestIListCorrectlyResolves()
        {
            using(var autosub = new AutoSubstitute())
            {
                var mockA = autosub.Provide<IServiceItem, ServiceItemA>();
                var mockB = autosub.Provide<IServiceItem, ServiceItemB>();
                var component = autosub.Resolve<TestIListComponent>();

                Assert.NotEmpty(component.ServiceItems);
                Assert.True(component.ServiceItems.Contains(mockA));
                Assert.True(component.ServiceItems.Contains(mockB));
            }
        }

        [Fact]
        public void TestIReadOnlyCollectionCorrectlyResolves()
        {
            using(var autosub = new AutoSubstitute())
            {
                var mockA = autosub.Provide<IServiceItem, ServiceItemA>();
                var mockB = autosub.Provide<IServiceItem, ServiceItemB>();
                var component = autosub.Resolve<TestIReadOnlyCollectionComponent>();

                Assert.NotEmpty(component.ServiceItems);
                Assert.True(component.ServiceItems.Contains(mockA));
                Assert.True(component.ServiceItems.Contains(mockB));
            }
        }

        [Fact]
        public void TestICollectionCorrectlyResolves()
        {
            using(var autosub = new AutoSubstitute())
            {
                var mockA = autosub.Provide<IServiceItem, ServiceItemA>();
                var mockB = autosub.Provide<IServiceItem, ServiceItemB>();
                var component = autosub.Resolve<TestICollectionComponent>();

                Assert.NotEmpty(component.ServiceItems);
                Assert.True(component.ServiceItems.Contains(mockA));
                Assert.True(component.ServiceItems.Contains(mockB));
            }
        }

        [Fact]
        public void TestIReadOnlyListCorrectlyResolves()
        {
            using(var autosub = new AutoSubstitute())
            {
                var mockA = autosub.Provide<IServiceItem, ServiceItemA>();
                var mockB = autosub.Provide<IServiceItem, ServiceItemB>();
                var component = autosub.Resolve<TestIReadOnlyListComponent>();

                Assert.NotEmpty(component.ServiceItems);
                Assert.True(component.ServiceItems.Contains(mockA));
                Assert.True(component.ServiceItems.Contains(mockB));
            }
        }
    }
}