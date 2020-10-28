using Autofac;
using NSubstitute;
using NSubstitute.Extensions;
using NUnit.Framework;
using System;

namespace AutofacContrib.NSubstitute.Tests
{
    [TestFixture]
    public class SubstituteForFixture
    {
        [Test]
        public void ConfigureSubstitueFor()
        {
            using var mock = AutoSubstitute.Configure()
                .SubstituteFor<Concrete1>()
                    .ConfigureSubstitute((c, ctx) =>
                    {
                        var concrete2 = ctx.Resolve<Concrete2>();
                        c.Configure().Get().Returns(concrete2);
                    })
                .SubstituteFor<Concrete2>()
                    .ConfigureSubstitute((c, ctx) =>
                    {
                        var concrete3 = ctx.Resolve<Concrete3>();
                        c.Configure().Get().Returns(concrete3);
                    })
                 .SubstituteFor<Concrete3>()
                .Build();

            var result = mock.Resolve<ConcreteTest>().Get();
            var expected = mock.Resolve<Concrete3>();

            Assert.AreSame(expected, result);
        }

        public abstract class Concrete1
        {
            public abstract Concrete2 Get();
        }

        public abstract class Concrete2
        {
            public abstract Concrete3 Get();
        }

        public abstract class Concrete3
        {
        }

        public class ConcreteTest
        {
            private readonly Concrete2 _c2;

            public ConcreteTest(Concrete1 c1)
            {
                _c2 = c1.Get();
            }

            public Concrete3 Get() => _c2.Get();
        }

        [Test]
        [Obsolete]
        public void Example_test_with_substitute_for_concrete_obsolete()
        {
            const int val1 = 3;
            const int val2 = 2;
            const int val3 = 10;

            using var utoSubstitute = AutoSubstitute.Configure()
                .SubstituteFor<ConcreteClass>(val2).Configure(c => c.Add(Arg.Any<int>()).Returns(val3))
                .Build();

            utoSubstitute.Resolve<IDependency2>().SomeOtherMethod().Returns(val1);

            var result = utoSubstitute.Resolve<MyClassWithConcreteDependency>().AMethod();

            Assert.That(result, Is.EqualTo(val3));
        }

        [Test]
        public void Example_test_with_substitute_for_concrete()
        {
            const int val1 = 3;
            const int val2 = 2;
            const int val3 = 10;

            using var utoSubstitute = AutoSubstitute.Configure()
                .SubstituteFor<ConcreteClass>(val2).ConfigureSubstitute(c => c.Add(Arg.Any<int>()).Returns(val3))
                .Build();

            utoSubstitute.Resolve<IDependency2>().SomeOtherMethod().Returns(val1);

            var result = utoSubstitute.Resolve<MyClassWithConcreteDependency>().AMethod();

            Assert.That(result, Is.EqualTo(val3));
        }

        [Test]
        public void SubstituteForConfigureWithContext()
        {
            const int val = 2;

            using var utoSubstitute = AutoSubstitute.Configure()
                .SubstituteFor<ConcreteClass>(val)
                .SubstituteFor<ConcreteClassWithObject>().ConfigureSubstitute((s, ctx) =>
                {
                    s.Configure().GetResult().Returns(ctx.Resolve<ConcreteClass>());
                })
                .Build()
                .Container;

            var result = utoSubstitute.Resolve<ConcreteClassWithObject>().GetResult();

            Assert.AreSame(result, utoSubstitute.Resolve<ConcreteClass>());
        }

        [Test]
        public void BaseCalledOnSubstituteForPartsOf()
        {
            using var mock = AutoSubstitute.Configure()
                .SubstituteForPartsOf<Test1>()
                .Build();

            var test1 = mock.Resolve<Test1>();

            Assert.That(test1, Is.NSubstituteMock);
            Assert.Throws<InvalidOperationException>(() => test1.Throws());
        }

        [Test]
        public void BaseNotCalledOnSubstituteFor()
        {
            using var mock = AutoSubstitute.Configure()
                .SubstituteFor<Test1>()
                .Build();

            var test1 = mock.Resolve<Test1>();

            Assert.That(test1, Is.NSubstituteMock);
            Assert.Null(test1.Throws());
        }

        [Test]
        public void FailsIfSubstituteTypeIsChanged()
        {
            var builder = AutoSubstitute.Configure()
                .SubstituteFor<Test1>();

            Assert.Throws<InvalidOperationException>(() => builder.SubstituteForPartsOf<Test1>());
        }

        [Test]
        public void FailsIfSubstituteTypeIsChanged2()
        {
            var builder = AutoSubstitute.Configure()
                .SubstituteForPartsOf<Test1>();

            Assert.Throws<InvalidOperationException>(() => builder.SubstituteFor<Test1>());
        }

        [Test]
        public void PropertiesNotSetByDefault()
        {
            using var mock = AutoSubstitute.Configure()
                .Provide<IProperty, CustomProperty>(out _)
                .SubstituteFor<TestWithProperty>()
                .Build();

            Assert.IsNull(mock.Resolve<TestWithProperty>().PropertySetter);
            Assert.That(mock.Resolve<TestWithProperty>().VirtualProperty, Is.NSubstituteMock);
        }

        [Test]
        public void PropertiesSetIfRequested()
        {
            using var mock = AutoSubstitute.Configure()
                .Provide<IProperty, CustomProperty>(out var property)
                .SubstituteFor<TestWithProperty>()
                    .InjectProperties()
                .Build();

            Assert.AreEqual(property.Value, mock.Resolve<TestWithProperty>().PropertySetter);
            Assert.AreEqual(property.Value, mock.Resolve<TestWithProperty>().VirtualProperty);
        }

        [Test]
        public void PropertiesSetIfGloballyRequested()
        {
            using var mock = AutoSubstitute.Configure()
                .InjectProperties()
                .Provide<IProperty, CustomProperty>(out var property)
                .SubstituteFor<TestWithProperty>()
                .Build();

            Assert.AreEqual(property.Value, mock.Resolve<TestWithProperty>().PropertySetter);
            Assert.AreEqual(property.Value, mock.Resolve<TestWithProperty>().VirtualProperty);
        }

        public abstract class Test1
        {
            public virtual object Throws() => throw new InvalidOperationException();
        }

        public abstract class TestWithProperty
        {
            public IProperty PropertySetter { get; set; }

            public virtual IProperty VirtualProperty { get; }
        }

        public interface IProperty
        {
        }

        public class CustomProperty : IProperty { }
    }
}
