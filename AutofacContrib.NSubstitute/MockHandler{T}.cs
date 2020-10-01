using Autofac;
using NSubstitute.Core;
using System;

namespace AutofacContrib.NSubstitute
{
    /// <summary>
    /// A handler that is used to modify any of the auto-generated NSubstitute mocks when the type is <typeparamref name="T"/>.
    /// </summary>
    public abstract class MockHandler<T> : MockHandler
        where T : class
    {
        /// <inheritdoc />
        protected internal sealed override void OnMockCreated(object instance, Type type, IComponentContext context, ISubstitutionContext substitutionContext)
        {
            if (typeof(T) == type && instance is T t)
            {
                OnMockCreated(t, context, substitutionContext);
            }
        }

        /// <summary>
        /// Gets the type associated with this handler.
        /// </summary>
        protected Type Type => typeof(T);

        /// <summary>
        /// Provides a way to manage mocks after creation but before returned from the container registry.
        /// </summary>
        /// <param name="instance">The mock instance.</param>
        /// <param name="context">The current component context.</param>
        /// <param name="substitutionContext">The current substitution context.</param>
        protected abstract void OnMockCreated(T instance, IComponentContext context, ISubstitutionContext substitutionContext);
    }
}