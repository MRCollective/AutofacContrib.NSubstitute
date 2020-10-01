using Autofac.Builder;
using System;
using System.Collections.Generic;

namespace AutofacContrib.NSubstitute
{
    public class AutoSubstituteOptions
    {
        /// <summary>
        /// Gets a collection of handlers that can be used to modify mocks after they are created.
        /// </summary>
        public ICollection<MockHandler> MockHandlers { get; } = new List<MockHandler>();

        /// <summary>
        /// Gets a collection of delegates that can augment the registrations of objects created but not registered.
        /// </summary>
        public ICollection<Action<IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>>> ConfigureAnyConcreteTypeRegistration { get; } = new List<Action<IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>>>(); 
    }
}