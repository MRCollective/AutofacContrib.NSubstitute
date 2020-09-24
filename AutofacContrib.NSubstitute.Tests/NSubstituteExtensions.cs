using NSubstitute.Extensions;

namespace AutofacContrib.NSubstitute.Tests
{
    internal static class NSubstituteExtensions
    {
        public static void AssertIsNSubstituteMock(this object obj)
        {
            // This throws an exception if it is not an NSubstitute mock
            obj.Configure();
        }
    }
}
