using Autofac;
using NSubstitute.Core;
using System;

namespace AutofacContrib.NSubstitute.MockHandlers
{
    /// <summary>
    /// Context for a mock that has been created.
    /// </summary>
    public class MockCreatedContext
    {
        internal MockCreatedContext(object instance, Type type, IComponentContext context, ISubstitutionContext substitutionContext)
        {
            Instance = instance;
            Type = type;
            Context = context;
            SubstitutionContext = substitutionContext;
        }

        /// <summary>
        /// Gets the created mock instance.
        /// </summary>
        public object Instance { get; }

        /// <summary>
        /// Gets the type that the mock is being used for.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the current resolve context.
        /// </summary>
        public IComponentContext Context { get; }

        /// <summary>
        /// Gets the NSubstitute substitution context.
        /// </summary>
        public ISubstitutionContext SubstitutionContext { get; }
    }
}