using Autofac;
using System;

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
        public abstract void OnMockCreated(object instance, Type type, IComponentContext context);
    }
}