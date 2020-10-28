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
        private readonly Dictionary<Type, object> _customDataManager;
        private readonly Dictionary<Type, object> _substituteForRegistrations;
        private readonly List<Action<IComponentContext>> _afterBuildActions;
        private readonly ContainerBuilder _builder;
        private readonly AutoSubstituteOptions _options;

        /// <summary>
        /// Create a new instance of the builder.
        /// </summary>
        public AutoSubstituteBuilder()
        {
            _substituteForRegistrations = new Dictionary<Type, object>();
            _customDataManager = new Dictionary<Type, object>();
            _afterBuildActions = new List<Action<IComponentContext>>();
            _builder = new ContainerBuilder();
            _options = new AutoSubstituteOptions();
        }

        /// <summary>
        /// Creates a new instance that allows linking to the previous instance for derived builders.
        /// </summary>
        /// <param name="other">A <see cref="AutoSubstituteBuilder"/> that should be connected to this instance</param>
        protected AutoSubstituteBuilder(AutoSubstituteBuilder other)
        {
            _substituteForRegistrations = other._substituteForRegistrations;
            _customDataManager = other._customDataManager;
            _afterBuildActions = other._afterBuildActions;
            _builder = other._builder;
            _options = other._options;
        }

        /// <summary>
        /// Builds an <see cref="AutoSubstitute"/> from the current builder.
        /// </summary>
        /// <returns>A new <see cref="AutoSubstitute"/> instance.</returns>
        public AutoSubstitute Build()
            => new AutoSubstitute(InternalBuild());

        internal IContainer InternalBuild()
        {
            _builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource
            {
                RegistrationConfiguration = c =>
                {
                    foreach (var configure in _options.ConfigureAnyConcreteTypeRegistration)
                    {
                        configure(c);
                    }
                }
            });
            _builder.RegisterSource(new NSubstituteRegistrationHandler(_options));

            var container = _options.BuildContainerFactory(_builder);

            foreach (var action in _afterBuildActions)
            {
                action(container);
            }

            return container;
        }

        /// <summary>
        /// Configures the option associated with this substitue builder.
        /// </summary>
        /// <param name="action">A delegate that configures the <see cref="AutoSubstituteOptions"/>.</param>
        /// <returns>The <see cref="AutoSubstituteBuilder"/>.</returns>
        public AutoSubstituteBuilder ConfigureOptions(Action<AutoSubstituteOptions> action)
        {
            action(_options);
            return this;
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
        /// Register the specified implementation type to the container as itself with the given parameterst.
        /// </summary>
        /// <typeparam name="TService">The type to register the implementation as</typeparam>
        /// <typeparam name="TImplementation">The implementation type</typeparam>
        /// <param name="parameters">Optional constructor parameters</param>
        /// <returns>The current <see cref="AutoSubstituteBuilder"/>.</returns>
        public AutoSubstituteBuilder Provide<TService>(params Parameter[] parameters)
            => Provide<TService, TService>(out _, parameters);

        /// <summary>
        /// Register the type with the given factory to the container. 
        /// </summary>
        /// <typeparam name="TService">The type to register the implementation as</typeparam>
        /// <param name="providedValue">Parameter to obtain a provided value.</param>
        /// <param name="factory">The factory method to produce the service.</param>
        /// <returns>The current <see cref="AutoSubstituteBuilder"/>.</returns>
        public AutoSubstituteBuilder Provide<TService>(out IProvidedValue<TService> providedValue, Func<IComponentContext, TService> factory)
        {
            var key = new object();

            _builder.Register(factory)
                .Keyed<TService>(key)
                .As<TService>()
                .InstancePerLifetimeScope();

            providedValue = CreateProvidedValue<TService>(c => c.ResolveKeyed<TService>(key));

            return this;
        }

        /// <summary>
        /// Register the type with the given factory to the container. 
        /// </summary>
        /// <typeparam name="TService">The type to register the implementation as</typeparam>
        /// <param name="factory">The factory method to produce the service.</param>
        /// <returns>The current <see cref="AutoSubstituteBuilder"/>.</returns>
        public AutoSubstituteBuilder Provide<TService>(Func<IComponentContext, TService> factory)
            => Provide(out _, factory);

        /// <summary>
        /// Register the specified implementation type to the container as itself with the given parameters.
        /// </summary>
        /// <typeparam name="TService">The type to register the implementation as</typeparam>
        /// <typeparam name="TImplementation">The implementation type</typeparam>
        /// <param name="providedValue">Parameter to obtain a provided value.</param>
        /// <param name="parameters">Optional constructor parameters</param>
        /// <returns>The current <see cref="AutoSubstituteBuilder"/>.</returns>
        public AutoSubstituteBuilder Provide<TService>(out IProvidedValue<TService> providedValue, params Parameter[] parameters)
            => Provide<TService, TService>(out providedValue, parameters);

        /// <summary>
        /// Register the specified implementation type to the container as the specified service type with the given parameterst.
        /// </summary>
        /// <typeparam name="TService">The type to register the implementation as</typeparam>
        /// <typeparam name="TImplementation">The implementation type</typeparam>
        /// <param name="parameters">Optional constructor parameters</param>
        /// <returns>The current <see cref="AutoSubstituteBuilder"/>.</returns>
        public AutoSubstituteBuilder Provide<TService, TImplementation>(params Parameter[] parameters)
            => Provide<TService, TImplementation>(out _, parameters);

        /// <summary>
        /// Register the specified implementation type to the container as the specified service type with the given parameterst.
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
        [Obsolete("Use a Provide method instead")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
                .AutoActivate()
                .InstancePerLifetimeScope();
            var builder = new SubstituteForBuilder<TService>(this, registration, isSubstituteFor);

            _substituteForRegistrations.Add(typeof(TService), builder);

            if (_options.AutoInjectProperties)
            {
                builder.InjectProperties();
            }

            return builder;
        }

        /// <summary>
        /// Registers a callback for about <see cref="Build"/> is called.
        /// </summary>
        /// <param name="callback">Callback to call.</param>
        protected void RegisterBuildCallback(Action<IComponentContext> callback)
            => _afterBuildActions.Add(callback);

        /// <summary>
        /// Creates a delayed provided value with the given factory method.
        /// </summary>
        /// <typeparam name="TService">Service to expose for the provided value.</typeparam>
        /// <param name="factory">Factory to create value.</param>
        /// <returns>A provided value.</returns>
        protected IProvidedValue<TService> CreateProvidedValue<TService>(Func<IComponentContext, TService> factory)
        {
            var value = new ProvidedValue<TService>(factory);

            RegisterBuildCallback(c => value.SetComponentContext(c));

            return value;
        }

        /// <summary>
        /// Retrieves storage of item that needs to be persisted across multiple calls to configuration methods.
        /// Some methods will wrap the builder into a new object, so this ensures that data used in one builder
        /// can be accessed later on.
        /// 
        /// Since the data is keyed by type, it is recommended to use a custom data type to hold the data if it
        /// is a well-known type.
        /// </summary>
        /// <typeparam name="T">Type of custom data.</typeparam>
        /// <param name="factory">Factory to create data.</param>
        /// <returns>A cached instance of the data, or a new instance if not already available.</returns>
        protected T GetCustomData<T>(Func<T> factory)
        {
            if (_customDataManager.TryGetValue(typeof(T), out var result))
            {
                return (T)result;
            }

            var created = factory();

            _customDataManager.Add(typeof(T), created);

            return created;
        }

        private class ProvidedValue<T> : IProvidedValue<T>
        {
            private readonly Func<IComponentContext, T> _factory;

            private IComponentContext _componentContext;

            public ProvidedValue(Func<IComponentContext, T> factory)
            {
                _factory = factory;
            }

            public T Value
            {
                get
                {
                    if (_componentContext is null)
                    {
                        throw new InvalidOperationException("Build must be called before using a provided value.");
                    }

                    return _factory(_componentContext);
                }
            }

            public void SetComponentContext(IComponentContext c) => _componentContext = c;
        }
    }
}