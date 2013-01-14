using System;
using System.Collections.Generic;
using Autofac.Core;
using System.Linq;

namespace AutofacContrib.NSubstitute
{
    /// <summary>
    /// Auto-mocking container around a single System Under Test (SUT) class.
    /// </summary>
    /// <typeparam name="TSut">The class being tested</typeparam>
    public class AutoSubstitute<TSut> where TSut : class
    {
        private Lazy<TSut> _sut;
        private AutoSubstitute _autoSubstitute;
        private IList<Parameter> _parameters;

        /// <summary>
        /// Create an AutoSubstitute SUT container.
        /// </summary>
        public AutoSubstitute()
        {
            Reset();
        }

        /// <summary>
        /// Get the System Under Test instance.
        /// </summary>
        public TSut SUT
        {
            get { return _sut.Value; }
        }

        /// <summary>
        /// Reset the container for a new set of tests against the SUT.
        /// </summary>
        public virtual void Reset()
        {
            if (_autoSubstitute != null)
                _autoSubstitute.Dispose();
            _autoSubstitute = new AutoSubstitute();
            _parameters = new List<Parameter>();
            _sut = new Lazy<TSut>(() => _autoSubstitute.ResolveAndSubstituteFor<TSut>(_parameters.ToArray()));
        }

        /// <summary>
        /// Specify a constructor parameter.
        /// </summary>
        /// <param name="parameter">The constructor parameter</param>
        public void WithConstructorParameter(Parameter parameter)
        {
            _parameters.Add(parameter);
        }

        /// <summary>
        /// Return a dependency of the SUT.
        /// </summary>
        /// <typeparam name="T">The dependency type to return</typeparam>
        /// <returns>The dependency</returns>
        public T Dependency<T>()
        {
            return _autoSubstitute.Resolve<T>();
        }

        /// <summary>
        /// Provide a dependency to be registered in the auto mocking container.
        /// </summary>
        /// <typeparam name="T">The type to register the dependency as</typeparam>
        /// <param name="dependency">The dependency being provided</param>
        public void ProvideDependency<T>(T dependency) where T : class
        {
            _autoSubstitute.Provide(dependency);
        }

        /// <summary>
        /// Resolve, register and return a substitute for the given type.
        /// </summary>
        /// <typeparam name="T">The type to return a substitute for</typeparam>
        /// <returns>The substitute for the type</returns>
        public T SubstituteFor<T>() where T : class
        {
            return _autoSubstitute.ResolveAndSubstituteFor<T>();
        }
    }
}
