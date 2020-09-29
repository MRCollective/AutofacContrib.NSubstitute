using Autofac;
using Autofac.Core;
using NUnit.Framework;

namespace AutofacContrib.NSubstitute.Tests
{
    [TestFixture]
    public class AutoSubstituteOptionsFixture
    {
        [Test]
        public void NoInjectPropertiesInterface()
        {
            using var mock = AutoSubstitute.Configure()
                .Build()
                .Container;

            var testInterface1 = mock.Resolve<ITestInterface1>();
            var class1 = mock.Resolve<Class1>();

            Assert.AreNotSame(class1, testInterface1.Instance);
        }

        [Test]
        public void InjectPropertiesInterface()
        {
            using var mock = AutoSubstitute.Configure()
                .InjectProperties()
                .Build()
                .Container;

            var testInterface1 = mock.Resolve<ITestInterface1>();

            Assert.That(testInterface1.Instance, Is.TypeOf<Class1>());
        }

        [Test]
        public void InjectPropertiesInterfaceWithUnregisteredPerLifetime()
        {
            using var mock = AutoSubstitute.Configure()
                .InjectProperties()
                .MakeUnregisteredTypesPerLifetime()
                .Build()
                .Container;

            var class1 = mock.Resolve<Class1>();
            var testInterface1 = mock.Resolve<ITestInterface1>();

            Assert.AreSame(class1, testInterface1.Instance);
        }

        [Test]
        public void NoInjectPropertiesClass()
        {
            using var mock = AutoSubstitute.Configure()
                .Build()
                .Container;

            var obj = mock.Resolve<WithProperties>();

            Assert.IsNull(obj.Service);
        }

        [Test]
        public void InjectPropertiesClass()
        {
            using var mock = AutoSubstitute.Configure()
                .InjectProperties()
                .Build()
                .Container;

            var obj = mock.Resolve<WithProperties>();

            Assert.IsNotNull(obj.Service);
        }

        [Test]
        public void InjectPropertiesInterfaceRecursive()
        {
            using var mock = AutoSubstitute.Configure()
                .InjectProperties()
                .MakeUnregisteredTypesPerLifetime()
                .Build()
                .Container;

            var obj = mock.Resolve<ITestInterfaceRecursive>();

            Assert.AreSame(obj, obj.Instance.Recursive);
        }

        [Test]
        public void InternalConstructorFails()
        {
            using var mock = AutoSubstitute.Configure()
                .Build()
                .Container;

            Assert.Throws<DependencyResolutionException>(() => mock.Resolve<ClassWithInternalConstructor>());
        }

        [Test]
        public void InternalConstructorSucceeds()
        {
            using var mock = AutoSubstitute.Configure()
                .UnregisteredTypesUseInternalConstructor()
                .Build()
                .Container;

            Assert.NotNull(mock.Resolve<ClassWithInternalConstructor>());
        }

        public interface ITestInterface1
        {
            Class1 Instance { get; }
        }

        public class WithProperties
        {
            public ITestInterface1 Service { get; set; }
        }

        public class Class1
        {
        }

        public interface ITestInterfaceRecursive
        {
            ClassRecursive Instance { get; }
        }

        public class ClassRecursive
        {
            public ITestInterfaceRecursive Recursive { get; set; }
        }

        public class ClassWithInternalConstructor
        {
            internal ClassWithInternalConstructor()
            {
            }
        }
    }
}
