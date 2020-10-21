using System;

namespace AutofacContrib.NSubstitute.MockHandlers
{
    /// <summary>
    /// Context for the mock currently being created.
    /// </summary>
    public class MockCreatingContext
    {
        internal MockCreatingContext(Type type, bool hasRegistrations)
        {
            Type = type;
            ShouldCreate = true;
            HasRegistrations = hasRegistrations;
        }

        /// <summary>
        /// Gets whether the type is already registered.
        /// </summary>
        public bool HasRegistrations { get; }

        internal bool ShouldCreate { get; private set; }

        /// <summary>
        /// Gets the type of the mock being created.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Marks the current mark to be created.  A call to <see cref="DoNotCreate"/> may change this state if called later.
        /// </summary>
        public void Create()
        {
            ShouldCreate = true;
        }

        /// <summary>
        /// Marks the current mark to not be created. A call to <see cref="Create"/> may change this state if called later.
        /// </summary>
        public void DoNotCreate()
        {
            ShouldCreate = false;
        }
    }
}