using Autofac;
using Autofac.Core;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NSubstitute;

namespace AutofacContrib.NSubstitute
{
    public static class AutoSubstituteExtensions
    {
        /// <summary>
        /// Register the specified implementation type to the container as the specified service type and resolve it using the given parameters.
        /// </summary>
        /// <typeparam name="TService">The type to register the implementation as</typeparam>
        /// <typeparam name="TImplementation">The implementation type</typeparam>
        /// <param name="parameters">Optional constructor parameters</param>
        /// <returns>The resolved service instance</returns>
        public static void Provide<TService, TImplementation>(this ContainerBuilder builder, params Parameter[] parameters)
        {
            builder.RegisterType<TImplementation>()
                .AsSelf() // TODO: Provide way of retrieving a provide
                .As<TService>()
                .WithParameters(parameters)
                .InstancePerLifetimeScope();
        }

        /// <summary>
        /// Register the specified object to the container as the specified service type and resolve it.
        /// </summary>
        /// <typeparam name="TService">The type to register the object as</typeparam>
        /// <param name="instance">The object to register into the container</param>
        /// <returns>The instance resolved from container</returns>
        public static void Provide<TService>(this ContainerBuilder builder, TService instance)
            where TService : class
        {
            builder.RegisterInstance(instance);
        }

        /// <summary>
        /// Register the specified object to the container as the specified keyed service type and resolve it.
        /// </summary>
        /// <typeparam name="TService">The type to register the object as</typeparam>
        /// <param name="instance">The object to register into the container</param>
        /// <param name="serviceKey">The key to register the service with</param>
        /// <returns>The instance resolved from container</returns>
        public static void Provide<TService>(this ContainerBuilder b, TService instance, object serviceKey)
            where TService : class
        {
            b.Register(_ => instance)
                .Keyed<TService>(serviceKey)
                .InstancePerLifetimeScope();
        }

        /// <summary>
        /// Registers to the container and returns a substitute for a given concrete class given the explicit constructor parameters.
        /// This is used for concrete classes where NSubstitutes won't be created by default by the container when using Resolve.
        /// For advanced uses consider using directly <see cref="Substitute.For{TService}"/> and then calling <see cref="Provide{TService}(TService)"/> so that type is used on dependencies for other Resolved types.
        /// </summary>
        /// <typeparam name="TService">The type to register and return a substitute for</typeparam>
        /// <param name="parameters">Optional constructor parameters</param>
        /// <returns>The instance resolved from the container</returns>
        public static TService SubstituteFor<TService>(this ContainerBuilder builder, params object[] parameters)
            where TService : class
        {
            var substitute = Substitute.For<TService>(parameters);

            builder.Provide(substitute);

            return substitute;
        }

        /// <summary>
        /// Registers to the container and returns a substitute for a given concrete class using autofac to resolve the constructor parameters.
        /// This is used for concrete classes where NSubstitutes won't be created by default by the container when using Resolve.
        /// For advanced uses consider using directly <see cref="Substitute.For{TService}"/> and then calling <see cref="Provide{TService}(TService)"/> so that type is used on dependencies for other Resolved types.
        /// </summary>
        /// <typeparam name="TService">The type to register and return a substitute for</typeparam>
        /// <param name="parameters">Any constructor parameters that Autofac can't resolve automatically</param>
        /// <returns>The instance resolved from the container</returns>
        public static void ResolveAndSubstituteFor<TService>(this ContainerBuilder b, params Parameter[] parameters) where TService : class
        {
            b.RegisterType<TService>()
                .WithParameters(parameters)
                .InstancePerLifetimeScope();
        }
    }
}