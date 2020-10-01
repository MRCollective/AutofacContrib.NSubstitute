using NSubstitute.Core;
using NSubstitute.Exceptions;
using NUnit.Framework.Constraints;

namespace AutofacContrib.NSubstitute.Tests
{
    internal class NSubstituteConstraint : Constraint
    {
        public NSubstituteConstraint()
        {
            Description = "Object expected to be NSubstitute mock";
        }

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            try
            {
                var router = SubstitutionContext.Current.GetCallRouterFor(actual);

                return new ConstraintResult(this, actual, router != null);
            }
            catch (NotASubstituteException)
            {
                return new ConstraintResult(this, actual, false);
            }
        }
    }

    internal class Is : NUnit.Framework.Is
    {
        public static NSubstituteConstraint NSubstituteMock { get; } = new NSubstituteConstraint();
    }
}
