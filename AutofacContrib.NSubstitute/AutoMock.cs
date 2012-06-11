using System;
using System.ComponentModel;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Features.ResolveAnything;
using IContainer = Autofac.IContainer;
using SubstituteAlias = NSubstitute.Substitute;

namespace AutofacContrib.NSubstitute
{

    /// <summary>
    /// Auto mocking container using <see cref="Autofac"/> and <see cref="NSubstitute"/>.
    /// </summary>
    public class AutoMock : IDisposable
    {
        /// <summary>
        /// <see cref="IContainer"/> that handles the component resolution.
        /// </summary>
        public IContainer Container { get; private set; }

        /// <summary>
        /// Create an AutoMock.
        /// </summary>
        public AutoMock()
        {
            var builder = new ContainerBuilder();

            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            builder.RegisterSource(new NSubstituteRegistrationHandler());
            Container = builder.Build();
        }


        /// <summary>
        /// Verifies mocks and disposes internal container.
        /// </summary>
        public void Dispose()
        {
            Container.Dispose();
        }

        /// <summary>
        /// Resolve the specified type in the container (register it if needed)
        /// </summary>
        /// <typeparam name="T">Service</typeparam>
        /// <param name="parameters">Optional parameters</param>
        /// <returns>The service.</returns>
        public T Resolve<T>(params Parameter[] parameters)
        {
            return Container.Resolve<T>(parameters);
        }

        /// <summary>
        /// Resolve the specified type in the container (register it if needed)
        /// </summary>
        /// <typeparam name="TService">Service</typeparam>
        /// <typeparam name="TImplementation">The implementation of the service.</typeparam>
        /// <param name="parameters">Optional parameters</param>
        /// <returns>The service.</returns>
        public TService Provide<TService, TImplementation>(params Parameter[] parameters)
        {
            Container.ComponentRegistry.Register(
                RegistrationBuilder.ForType<TImplementation>().As<TService>().InstancePerLifetimeScope().CreateRegistration());

            return Container.Resolve<TService>(parameters);
        }

        /// <summary>
        /// Resolve the specified type in the container (register specified instance if needed)
        /// </summary>
        /// <typeparam name="TService">Service</typeparam>
        /// <returns>The instance resolved from container.</returns>
        public TService Provide<TService>(TService instance)
            where TService : class
        {
            Container.ComponentRegistry.Register(
                RegistrationBuilder.ForDelegate((c, p) => instance).InstancePerLifetimeScope().CreateRegistration());

            return Container.Resolve<TService>();
        }

        /// <summary>
        /// Crates and returns a substitute. This is useful mainly for concrete classes where NSubstitutes won't be created by default
        /// For advanced uses consider using directly Substitute.For and then calling <see cref="Provide{TService}"/> so that type is used on dependencies for other Resolved types.
        /// </summary>
        /// <typeparam name="TService">Service</typeparam>
        /// <returns>The instance resolved from container.</returns>
        public TService SubstituteFor<TService>() where TService : class
        {
            // SubstituteAlias=Substitute, but I have to use an Alias since the class name conflics with this member and can't use FQN since it conflics with this namesace
            var substitute = SubstituteAlias.For<TService>();
            return Provide(substitute);
        }
    }
}