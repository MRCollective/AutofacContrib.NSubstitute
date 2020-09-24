using Autofac;
using Autofac.Builder;
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
        private readonly IRegistrationBuilder<TService, SimpleActivatorData, SingleRegistrationStyle> _registration;

        internal SubstituteForBuilder(AutoSubstituteBuilder builder, IRegistrationBuilder<TService, SimpleActivatorData, SingleRegistrationStyle> registration)
        {
            _builder = builder;
            _registration = registration;
        }

        /// <summary>
        /// Allows for configuration of the service.
        /// </summary>
        /// <param name="action">The delegate to configure the service.</param>
        /// <returns>The original <see cref="AutoSubstituteBuilder"/>.</returns>
        public AutoSubstituteBuilder Configure(Action<TService> action)
            => Configure((s, _) => action(s));

        /// <summary>
        /// Allows for configuration of the service with access to the resolved components.
        /// </summary>
        /// <param name="action">The delegate to configure the service.</param>
        /// <returns>The original <see cref="AutoSubstituteBuilder"/>.</returns>
        public AutoSubstituteBuilder Configure(Action<TService, IComponentContext> action)
        {
            _registration.OnActivated(args =>
            {
                action(args.Instance, args.Context);
            });

            return _builder;
        }

        /// <summary>
        /// Completes the configuration of the substitute.
        /// </summary>
        /// <returns>The original <see cref="AutoSubstituteBuilder"/>.</returns>
        public AutoSubstituteBuilder Configured()
        {
            return _builder;
        }

        /// <summary>
        /// Allows a way to access the services being configured that the container will provide.
        /// </summary>
        /// <param name="service">Parameter to obtain the substituted value.</param>
        /// <returns></returns>
        public SubstituteForBuilder<TService> Provide(out IProvidedValue<TService> service)
        {
            service = _builder.CreateProvidedValue(c => c.Resolve<TService>());
            return this;
        }
    }
}