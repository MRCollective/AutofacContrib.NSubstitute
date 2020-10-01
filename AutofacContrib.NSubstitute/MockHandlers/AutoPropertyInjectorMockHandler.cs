using Autofac;
using NSubstitute.Core;
using System;

namespace AutofacContrib.NSubstitute
{
    internal class AutoPropertyInjectorMockHandler : MockHandler
    {
        public static AutoPropertyInjectorMockHandler Instance { get; } = new AutoPropertyInjectorMockHandler();

        private AutoPropertyInjectorMockHandler()
        {
        }

        protected internal override void OnMockCreated(object instance, Type type, IComponentContext context, ISubstitutionContext substitutionContext)
        {
            var router = substitutionContext.GetCallRouterFor(instance);

            router.RegisterCustomCallHandlerFactory(_ => new AutoPropertyInjectorCallHandler(context));
        }

        private class AutoPropertyInjectorCallHandler : ICallHandler
        {
            private readonly IComponentContext _context;

            public AutoPropertyInjectorCallHandler(IComponentContext context)
            {
                _context = context;
            }

            public RouteAction Handle(ICall call)
            {
                var property = call.GetMethodInfo().GetPropertyFromGetterCallOrNull();

                if (property is null)
                {
                    return RouteAction.Continue();
                }

                return RouteAction.Return(_context.Resolve(call.GetReturnType()));
            }
        }
    }
}