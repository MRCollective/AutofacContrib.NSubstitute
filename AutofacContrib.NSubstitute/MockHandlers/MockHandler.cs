using AutofacContrib.NSubstitute.MockHandlers;

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
        /// <param name="context">Created context.</param>
        protected internal virtual void OnMockCreated(MockCreatedContext context)
        {
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
