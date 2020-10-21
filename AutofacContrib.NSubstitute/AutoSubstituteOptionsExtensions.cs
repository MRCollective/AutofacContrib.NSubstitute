using Autofac;
using Autofac.Core.Activators.Reflection;
using AutofacContrib.NSubstitute.MockHandlers;
using System.Reflection;

namespace AutofacContrib.NSubstitute
{
    public static class AutoSubstituteOptionsExtensions
    {
        /// <summary>
        /// Configures the auto-generated types Autofac creates to be the same throughout a lifetime scope.
        /// </summary>
        /// <param name="builder">A <see cref="AutoSubstituteBuilder"/></param>
        /// <returns>The supplied <see cref="AutoSubstituteBuilder"/></returns>
        public static AutoSubstituteBuilder MakeUnregisteredTypesPerLifetime(this AutoSubstituteBuilder builder)
            => builder.ConfigureOptions(options =>
            {
                options.ConfigureAnyConcreteTypeRegistration.Add(registration => registration.InstancePerLifetimeScope());
            });

        /// <summary>
        /// Configures the auto-generated types Autofac creates to search through internal constructors as well. If a type is explicitly registered, it will not search for internal constructors automatically.
        /// </summary>
        /// <param name="builder">A <see cref="AutoSubstituteBuilder"/></param>
        /// <returns>The supplied <see cref="AutoSubstituteBuilder"/></returns>
        public static AutoSubstituteBuilder UnregisteredTypesUseInternalConstructor(this AutoSubstituteBuilder builder)
            => builder.ConfigureOptions(options =>
            {
                options.ConfigureAnyConcreteTypeRegistration.Add(registration => registration.FindConstructorsWith(NonPublicConstructorFinder.Finder));
            });

        /// <summary>
        /// Configures the auto-generated types to populate the properties based on the configured container. If a type is explicitly registered, then the properties will not be automatically satisfied.
        /// </summary>
        /// <param name="builder">A <see cref="AutoSubstituteBuilder"/></param>
        /// <returns>The supplied <see cref="AutoSubstituteBuilder"/></returns>
        public static AutoSubstituteBuilder InjectProperties(this AutoSubstituteBuilder builder)
            => builder.ConfigureOptions(options =>
            {
                options.AutoInjectProperties = true;
                options.MockHandlers.Add(AutoPropertyInjectorMockHandler.Instance);
                options.ConfigureAnyConcreteTypeRegistration.Add(r => r.PropertiesAutowired());
            });

        private class NonPublicConstructorFinder : DefaultConstructorFinder
        {
            public static IConstructorFinder Finder { get; } = new NonPublicConstructorFinder();

            private NonPublicConstructorFinder()
                : base(type => type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
            }
        }
    }
}