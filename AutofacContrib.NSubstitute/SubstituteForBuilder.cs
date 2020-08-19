using System;

namespace AutofacContrib.NSubstitute
{
    /// <summary>
    /// A class to configure substituted services.
    /// </summary>
    /// <typeparam name="TService">The type of the substituted service.</typeparam>
    public class SubstituteForBuilder<TService>
        where TService : class
    {
        private readonly AutoSubstituteBuilder _builder;
        private readonly TService _service;

        internal SubstituteForBuilder(AutoSubstituteBuilder builder, TService service)
        {
            _builder = builder;
            _service = service;
        }

        /// <summary>
        /// Allows for configuration of the service.
        /// </summary>
        /// <param name="action">The delegate to configure the service.</param>
        /// <returns>The original <see cref="AutoSubstituteBuilder"/>.</returns>
        public AutoSubstituteBuilder Configure(Action<TService> action)
        {
            action(_service);
            return _builder;
        }
    }

}