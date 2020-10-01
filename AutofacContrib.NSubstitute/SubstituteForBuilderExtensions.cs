using NSubstitute.Core;

namespace AutofacContrib.NSubstitute
{
    public static class SubstituteForBuilderExtensions
    {
        /// <summary>
        /// Configures the auto-generated instance of <typeparamref name="T"/> to not call the base method for any of the methods.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static SubstituteForBuilder<T> DoNotCallBase<T>(this SubstituteForBuilder<T> builder)
            where T : class
        {
            builder.Configure(t =>
            {
                var router = SubstitutionContext.Current.GetCallRouterFor(t);

                router.CallBaseByDefault = false;
            });

            return builder;
        }
    }
}