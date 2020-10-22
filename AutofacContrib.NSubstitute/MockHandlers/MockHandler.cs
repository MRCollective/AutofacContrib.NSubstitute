using Autofac;
using AutofacContrib.NSubstitute.MockHandlers;
using NSubstitute.Core;
using System;
using System.ComponentModel;

namespace AutofacContrib.NSubstitute
{
    /// <summary>
    /// A handler that is used to modify any of the auto-generated NSubstitute mocks.
    /// </summary>
    public abstract class MockHandler
    {
        protected MockHandler()
        {
        }

        /// <summary>
        /// Provides a way to manage mocks after creation but before returned from the container registry.
        /// </summary>
        /// <param name="instance">The mock instance.</param>
        /// <param name="type">The type the mock was created for.</param>
        /// <param name="context">The current component context.</param>
        /// <param name="substitutionContext">The current substitution context.</param>
        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void OnMockCreated(object instance, Type type, IComponentContext context, ISubstitutionContext substitutionContext)
        {
        }

        /// <summary>
        /// Provides a way to manage mocks after creation but before returned from the container registry.
        /// </summary>
        /// <param name="context">Created context.</param>
        protected internal virtual void OnMockCreated(MockCreatedContext context)
        {
#pragma warning disable CS0612 // Type or member is obsolete
            OnMockCreated(context.Instance, context.Type, context.Context, context.SubstitutionContext);
#pragma warning restore CS0612 // Type or member is obsolete
        }

        /// <summary>
        /// Provides a way to manage the initial creation of mocks. Defaults to creating for all types.
        /// </summary>
        /// <param name="context">Context of currently being created mock.</param>
        protected internal virtual void OnMockCreating(MockCreatingContext context)
        {
        }
    }
}
