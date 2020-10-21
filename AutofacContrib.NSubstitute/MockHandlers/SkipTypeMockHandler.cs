using Autofac;
using NSubstitute.Core;
using System;
using System.Collections.Generic;

namespace AutofacContrib.NSubstitute.MockHandlers
{
    /// <summary>
    /// An implementation of a <see cref="MockHandler"/> that will skip creation of mocks for given types.
    /// </summary>
    public sealed class SkipTypeMockHandler : MockHandler
    {
        private readonly IEnumerable<Type> _types;

        /// <summary>
        /// Create a <see cref="MockHandler"/> that skips <typeparamref name="T"/>.
        /// </summary>
        /// <returns>A mock handler to skip supplied type.</returns>
        public static MockHandler Create<T>() => Create(typeof(T));

        /// <summary>
        /// Create a <see cref="MockHandler"/> that skips all types contained in <paramref name="types"/>.
        /// </summary>
        /// <param name="types">List of types to return.</param>
        /// <returns>A mock handler to skip supplied types.</returns>
        public static MockHandler Create(params Type[] types) => new SkipTypeMockHandler(types);

        /// <summary>
        /// Create a <see cref="MockHandler"/> that skips all types contained in <paramref name="types"/>.
        /// </summary>
        /// <param name="types">List of types to return.</param>
        /// <returns>A mock handler to skip supplied types.</returns>
        public static MockHandler Create(IEnumerable<Type> types) => new SkipTypeMockHandler(types);

        private SkipTypeMockHandler(IEnumerable<Type> types)
        {
            _types = types ?? throw new ArgumentNullException(nameof(types));
        }

        protected internal override void OnMockCreating(MockCreatingContext context)
        {
            foreach (var type in _types)
            {
                if (type.IsGenericType && type == type.GetGenericTypeDefinition())
                {
                    context.DoNotCreate();
                }
                else if (type == context.Type)
                {
                    context.DoNotCreate();
                }
            }
        }
    }
}