using Autofac;
using Autofac.Core;
using Autofac.Features.ResolveAnything;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace AutofacContrib.NSubstitute
{
    public class AutoSubstituteBuilder
    {
        private readonly Dictionary<Type, object> _substituteForRegistrations = new Dictionary<Type, object>();
        private readonly List<IProvidedValue> _providedValues;
        private readonly ContainerBuilder _builder;

        public AutoSubstituteBuilder()
        {
            _builder = new ContainerBuilder();
            _providedValues = new List<IProvidedValue>();
        }

        public AutoSubstitute Build()
            => new AutoSubstitute(InternalBuild());

        internal IContainer InternalBuild()
        {
            _builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            _builder.RegisterSource(new NSubstituteRegistrationHandler());

            var container = _builder.Build();

            foreach (var provided in _providedValues)
            {
                provided.SetContainer(container);
            }

            return container;
        }

        /// <summary>
        /// Provides direct access to the <see cref="ContainerBuilder"/> to manipulate how needed.
        /// </summary>
        /// <param name="action">A delegate to run to configure the builder.</param>
        /// <returns>The current <see cref="AutoSubstituteBuilder"/>.</returns>
        public AutoSubstituteBuilder ConfigureBuilder(Action<ContainerBuilder> action)
        {
            action(_builder);

            return this;
        }

        /// <summary>
        /// Register the specified implementation type to the container as the specified service type and resolve it using the given parameters.
        /// </summary>
        /// <typeparam name="TService">The type to register the implementation as</typeparam>
        /// <typeparam name="TImplementation">The implementation type</typeparam>
        /// <param name="providedValue">Parameter to obtain a provided value.</param>
        /// <param name="parameters">Optional constructor parameters</param>
        /// <returns>The current <see cref="AutoSubstituteBuilder"/>.</returns>
        public AutoSubstituteBuilder Provide<TService, TImplementation>(out IProvidedValue<TService> providedValue, params Parameter[] parameters)
        {
            var key = new object();

            _builder.RegisterType<TImplementation>()
                .Keyed<TService>(key)
                .As<TService>()
                .WithParameters(parameters)
                .InstancePerLifetimeScope();

            providedValue = CreateProvidedValue<TService>(c => c.ResolveKeyed<TService>(key));

            return this;
        }

        /// <summary>
        /// Register the specified object to the container as the specified service type and resolve it.
        /// </summary>
        /// <typeparam name="TService">The type to register the object as</typeparam>
        /// <param name="instance">The object to register into the container</param>
        /// <returns>The current <see cref="AutoSubstituteBuilder"/>.</returns>
        public AutoSubstituteBuilder Provide<TService>(TService instance)
            where TService : class
        {
            _builder.RegisterInstance(instance);

            return this;
        }

        /// <summary>
        /// Register the specified object to the container as the specified keyed service type and resolve it.
        /// </summary>
        /// <typeparam name="TService">The type to register the object as</typeparam>
        /// <param name="instance">The object to register into the container</param>
        /// <param name="serviceKey">The key to register the service with</param>
        /// <returns>The current <see cref="AutoSubstituteBuilder"/>.</returns>
        public AutoSubstituteBuilder Provide<TService>(TService instance, object serviceKey)
            where TService : class
        {
            _builder.Register(_ => instance)
                .Keyed<TService>(serviceKey)
                .InstancePerLifetimeScope();

            return this;
        }

        /// <summary>
        /// Registers a substitute to the container a given concrete class given the explicit constructor parameters using <see cref="Substitute.For{TService}(object[])"/>.
        /// This is used for concrete classes where NSubstitutes won't be created by default by the container when using Resolve.
        /// </summary>
        /// <typeparam name="TService">The type to register and return a substitute for</typeparam>
        /// <param name="parameters">Optional constructor parameters</param>
        /// <returns>An instance to help configure the substitution.</returns>
        public SubstituteForBuilder<TService> SubstituteFor<TService>(params object[] parameters)
            where TService : class
            => CreateSubstituteForBuilder<TService>(() => Substitute.For<TService>(parameters), true);

        /// <summary>
        /// Registers a substitute to the container a given concrete class given the explicit constructor parameters using <see cref="Substitute.ForPartsOf{TService}(object[])"/>.
        /// This is used for concrete classes where NSubstitutes won't be created by default by the container when using Resolve.
        /// </summary>
        /// <typeparam name="TService">The type to register and return a substitute for</typeparam>
        /// <param name="parameters">Optional constructor parameters</param>
        /// <returns>An instance to help configure the substitution.</returns>
        public SubstituteForBuilder<TService> SubstituteForPartsOf<TService>(params object[] parameters)
            where TService : class
            => CreateSubstituteForBuilder(() => Substitute.ForPartsOf<TService>(parameters), false);

        /// <summary>
        /// Registers to the container and returns a substitute for a given concrete class using autofac to resolve the constructor parameters.
        /// This is used for concrete classes where NSubstitutes won't be created by default by the container when using Resolve.
        /// For advanced uses consider using directly <see cref="Substitute.For{TService}"/> and then calling <see cref="Provide{TService}(TService)"/> so that type is used on dependencies for other Resolved types.
        /// </summary>
        /// <typeparam name="TService">The type to register and return a substitute for</typeparam>
        /// <param name="parameters">Any constructor parameters that Autofac can't resolve automatically</param>
        /// <returns>The current <see cref="AutoSubstituteBuilder"/>.</returns>
        public AutoSubstituteBuilder ResolveAndSubstituteFor<TService>(params Parameter[] parameters) where TService : class
        {
            _builder.RegisterType<TService>()
                .WithParameters(parameters)
                .InstancePerLifetimeScope();

            return this;
        }

        private SubstituteForBuilder<TService> CreateSubstituteForBuilder<TService>(Func<TService> factory, bool isSubstituteFor)
            where TService : class
        {
            if (_substituteForRegistrations.TryGetValue(typeof(TService), out var result))
            {
                var previousBuilder = (SubstituteForBuilder<TService>)result;

                if (previousBuilder.IsSubstituteFor != isSubstituteFor)
                {
                    throw new InvalidOperationException("Cannot change a service registration between SubstituteFor and SubstituteForPartsOf");
                }

                return previousBuilder;
            }

            var registration = _builder.Register(_ => factory())
                .As<TService>()
                .InstancePerLifetimeScope();
            var builder = new SubstituteForBuilder<TService>(this, registration, isSubstituteFor);

            _substituteForRegistrations.Add(typeof(TService), builder);

            return builder;
        }

        internal IProvidedValue<TService> CreateProvidedValue<TService>(Func<IContainer, TService> factory)
        {
            var value = new ProvidedValue<TService>(factory);

            _providedValues.Add(value);

            return value;
        }

        private interface IProvidedValue
        {
            void SetContainer(IContainer container);
        }

        private class ProvidedValue<T> : IProvidedValue<T>, IProvidedValue
        {
            private readonly Func<IContainer, T> _factory;

            private IContainer _container;

            public ProvidedValue(Func<IContainer, T> factory)
            {
                _factory = factory;
            }

            public T Value
            {
                get
                {
                    if (_container is null)
                    {
                        throw new InvalidOperationException("Build must be called before using a provided value.");
                    }

                    return _factory(_container);
                }
            }

            public void SetContainer(IContainer container) => _container = container;
        }
    }
}