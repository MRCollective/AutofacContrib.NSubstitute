using Autofac;
using Autofac.Core;
using Autofac.Features.ResolveAnything;
using System;

namespace AutofacContrib.NSubstitute
{
    /// <summary>
    /// Deprecated auto mocking container. Use <see cref="AutoSubstitute"/> instead.
    /// </summary>
    [Obsolete("AutoMock has been deprecated in favour of AutoSubstitute.")]
    public class AutoMock : AutoSubstitute { }

    /// <summary>
    /// Auto mocking container using <see cref="Autofac"/> and <see cref="NSubstitute"/>.
    /// </summary>
    public class AutoSubstitute : IDisposable
    {
        /// <summary>
        /// Creates a builder to configure the mocks and dependencies.
        /// </summary>
        /// <returns>An instance of <see cref="AutoSubstituteBuilder"/>.</returns>
        public static AutoSubstituteBuilder Configure() => new AutoSubstituteBuilder();

        /// <summary>
        /// <see cref="IContainer"/> that handles the component resolution.
        /// </summary>
        public IContainer Container { get; }

        /// <summary>
        /// Create an AutoSubstitute.
        /// </summary>
        public AutoSubstitute()
        {
            Container = Configure().InternalBuild();
        }

        /// <summary>
        /// Create an AutoSubstitute, but modify the <see cref="Autofac.ContainerBuilder"/> before building a container.
        /// </summary>
        /// <param name="builderModifier">Action to modify the <see cref="Autofac.ContainerBuilder"/></param>
        public AutoSubstitute(Action<ContainerBuilder> builderModifier)
        {
            Container = Configure()
                .ConfigureBuilder(builderModifier)
                .InternalBuild();
        }

        internal AutoSubstitute(IContainer container)
        {
            Container = container;
        }

        /// <summary>
        /// Cleans up the <see cref="Autofac.Core.Container"/>.
        /// </summary>
        public void Dispose()
        {
            Container.Dispose();
        }

        /// <summary>
        /// Resolve the specified type from the container.
        /// </summary>
        /// <typeparam name="T">The type to resolve</typeparam>
        /// <param name="parameters">Optional constructor parameters</param>
        /// <returns>The resolved object</returns>
        public T Resolve<T>(params Parameter[] parameters)
        {
            return Container.Resolve<T>(parameters);
        }
    }
}