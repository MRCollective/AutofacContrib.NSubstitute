using Autofac;
using Autofac.Builder;
using NSubstitute.Core;
using System;
using System.ComponentModel;

namespace AutofacContrib.NSubstitute
{
    /// <summary>
    /// A class to configure substituted services.
    /// </summary>
    /// <typeparam name="TService">The type of the substituted service.</typeparam>
    public class SubstituteForBuilder<TService> : AutoSubstituteBuilder
        where TService : class
    {
        private readonly IRegistrationBuilder<TService, SimpleActivatorData, SingleRegistrationStyle> _registration;

        internal SubstituteForBuilder(
            AutoSubstituteBuilder builder,
            IRegistrationBuilder<TService, SimpleActivatorData, SingleRegistrationStyle> registration,
            bool isSubstituteFor)
            : base(builder)
        {
            _registration = registration;
            IsSubstituteFor = isSubstituteFor;
        }

        /// <summary>
        /// Used to identify if a builder was created by <see cref="AutoSubstituteBuilder.SubstituteFor{TService}(object[])"/> or <see cref="AutoSubstituteBuilder.SubstituteForPartsOf{TService}(object[])"/>.
        /// </summary>
        internal bool IsSubstituteFor { get; }

        internal ISubstitutionContext Context => SubstitutionContext.Current;

        /// <summary>
        /// Allows for configuration of the service.
        /// </summary>
        /// <param name="action">The delegate to configure the service.</param>
        /// <returns>The original <see cref="AutoSubstituteBuilder"/>.</returns>
        [Obsolete("Use ConfigureSubstitute instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public AutoSubstituteBuilder Configure(Action<TService> action)
            => ConfigureSubstitute((s, _) => action(s));

        /// <summary>
        /// Allows for configuration of the service.
        /// </summary>
        /// <param name="action">The delegate to configure the service.</param>
        /// <returns>The original <see cref="AutoSubstituteBuilder"/>.</returns>
        public SubstituteForBuilder<TService> ConfigureSubstitute(Action<TService> action)
            => ConfigureSubstitute((s, _) => action(s));

        /// <summary>
        /// Allows for configuration of the service with access to the resolved components.
        /// </summary>
        /// <param name="action">The delegate to configure the service.</param>
        /// <returns>The original <see cref="AutoSubstituteBuilder"/>.</returns>
        public SubstituteForBuilder<TService> ConfigureSubstitute(Action<TService, IComponentContext> action)
        {
            _registration.OnActivated(args =>
            {
                action(args.Instance, args.Context.Resolve<IComponentContext>());
            });

            return this;
        }
    }
}