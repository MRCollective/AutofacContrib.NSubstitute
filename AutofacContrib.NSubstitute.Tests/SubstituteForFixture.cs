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
        public void Example_test_with_substitute_for_concrete()
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
        public void SubstituteForConfigureWithContext()
        {
            const int val = 2;

            using var utoSubstitute = AutoSubstitute.Configure()
                .SubstituteFor<ConcreteClass>(val).Configured()
                .SubstituteFor<ConcreteClassWithObject>().Configure((s, ctx) =>
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
                .SubstituteForPartsOf<Test1>().Configured()
                .Build();

            var test1 = mock.Resolve<Test1>();

            Assert.That(test1, Is.NSubstituteMock);
            Assert.Throws<InvalidOperationException>(() => test1.Throws());
        }

        [Test]
        public void BaseNotCalledOnSubstituteFor()
        {
            using var mock = AutoSubstitute.Configure()
                .SubstituteFor<Test1>().Configured()
                .Build();

            var test1 = mock.Resolve<Test1>();

            Assert.That(test1, Is.NSubstituteMock);
            Assert.Null(test1.Throws());
        }

        [Test]
        public void FailsIfSubstituteTypeIsChanged()
        {
            var builder = AutoSubstitute.Configure()
                .SubstituteFor<Test1>().Configured();

            Assert.Throws<InvalidOperationException>(() => builder.SubstituteForPartsOf<Test1>());
        }

        [Test]
        public void FailsIfSubstituteTypeIsChanged2()
        {
            var builder = AutoSubstitute.Configure()
                .SubstituteForPartsOf<Test1>().Configured();

            Assert.Throws<InvalidOperationException>(() => builder.SubstituteFor<Test1>());
        }

        public abstract class Test1
        {
            public virtual object Throws() => throw new InvalidOperationException();
        }
    }
}
