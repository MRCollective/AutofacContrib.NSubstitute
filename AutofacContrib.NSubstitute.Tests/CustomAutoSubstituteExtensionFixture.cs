using NUnit.Framework;

namespace AutofacContrib.NSubstitute.Tests
{
    [TestFixture]
    public sealed class CustomAutoSubstituteExtensionFixture
    {
        [Test]
        public void CusomExtensionCanAccessData()
        {
            var builder = AutoSubstitute.Configure();
            var custom1 = new CustomExtensionBuilder(builder);
            var substituted = builder.SubstituteFor<ITest>();
            var custom2 = new CustomExtensionBuilder(substituted);

            Assert.AreSame(custom1.Data, custom2.Data);
        }

        private class CustomExtensionBuilder : AutoSubstituteBuilder
        {
            public CustomExtensionBuilder(AutoSubstituteBuilder other)
                : base(other)
            {
            }

            public CustomData Data => GetCustomData(() => new CustomData());
        }

        private class CustomData
        {
        }

        public interface ITest
        {
        }
    }
}