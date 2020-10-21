using Autofac.Core.Registration;
using AutofacContrib.NSubstitute.MockHandlers;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace AutofacContrib.NSubstitute.Tests
{
    [TestFixture]
    public class TypesToSkipForMockingFixture
    {
        [Test]
        public void WithoutOption()
        {
            var mock = AutoSubstitute.Configure()
                .Provide<IDependency, Impl1>(out var impl1)
                .Provide<IDependency, Impl2>(out var impl2)
                .Build();

            var items = mock.Resolve<IDependency[]>();

            Assert.AreEqual(2, items.Length);
            Assert.AreEqual(impl1.Value, items[0]);
            Assert.AreEqual(impl2.Value, items[1]);
        }

        [Test]
        [Obsolete]
        public void ManuallyAddTypeToSkip()
        {
            var mock = AutoSubstitute.Configure()
                .ConfigureOptions(options =>
                {
                    options.TypesToSkipForMocking.Add(typeof(IDependency));
                })
                .Build();

            Assert.Throws<ComponentNotRegisteredException>(() => mock.Resolve<IDependency>());
        }

        [Test]
        public void ManuallyCheckTypeToSkip()
        {
            var mock = AutoSubstitute.Configure()
                .ConfigureOptions(options =>
                {
                    options.MockHandlers.Add(SkipTypeMockHandler.Create<IDependency>());
                })
                .Build();

            Assert.Throws<ComponentNotRegisteredException>(() => mock.Resolve<IDependency>());
        }

        [Test]
        public void RegisteredTypesAreNotMocked()
        {
            var mock = AutoSubstitute.Configure()
                .Provide<IDependency, Impl1>(out var impl1)
                .Provide<IDependency, Impl2>(out var impl2)
                .Build();

            var items = mock.Resolve<IEnumerable<IDependency>>();

            CollectionAssert.AreEqual(items, new[] { impl1.Value, impl2.Value });
        }

        public interface IDependency
        {
        }

        public class Impl1 : IDependency
        {
        }

        public class Impl2 : IDependency
        {
        }
    }
}
