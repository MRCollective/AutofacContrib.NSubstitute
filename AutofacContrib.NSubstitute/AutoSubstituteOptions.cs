using Autofac;
using Autofac.Builder;
using AutofacContrib.NSubstitute.MockHandlers;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AutofacContrib.NSubstitute
{
    public class AutoSubstituteOptions
    {
        private readonly static Func<ContainerBuilder, Autofac.IContainer> _defaultContainerBuilder = b => b.Build();

        private readonly HashSet<Type> _typesToSkipForMocking = new HashSet<Type>();

        public AutoSubstituteOptions()
        {
            MockHandlers = new List<MockHandler>
            {
                AlreadyExistsMockHandler.Instance,
                SkipTypeMockHandler.Create(_typesToSkipForMocking)
            };
        }

        internal bool AutoInjectProperties { get; set; }

        /// <summary>
        /// Gets a collection of handlers that can be used to modify mocks after they are created.
        /// </summary>
        public ICollection<MockHandler> MockHandlers { get; }

        /// <summary>
        /// Gets a collection of delegates that can augment the registrations of objects created but not registered.
        /// </summary>
        public ICollection<Action<IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>>> ConfigureAnyConcreteTypeRegistration { get; } = new List<Action<IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>>>();

        /// <summary>
        /// Gets a collection of types that will be skipped during generation of NSubstitute mocks.
        /// </summary>
        [Obsolete("Use a custom MockHandler instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ICollection<Type> TypesToSkipForMocking => _typesToSkipForMocking;

        /// <summary>
        /// Gets or sets a flag indicating whether mocks should be excluded for provided values. This will automatically add values given to Provide methods to <see cref="TypesToSkipForMocking"/>.
        /// </summary>
        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool AutomaticallySkipMocksForProvidedValues { get; set; }

        /// <summary>
        /// Gets or sets a factory to create an <see cref="IContainer"/> given a <see cref="ContainerBuilder"/>. This defaults to simply calling <see cref="ContainerBuilder.Build()"/>.
        /// </summary>
        public Func<ContainerBuilder, Autofac.IContainer> BuildContainerFactory { get; set; } = _defaultContainerBuilder;
    }
}