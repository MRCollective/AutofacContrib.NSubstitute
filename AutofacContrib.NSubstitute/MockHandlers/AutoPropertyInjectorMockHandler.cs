using Autofac;
using NSubstitute.Core;
using System;

namespace AutofacContrib.NSubstitute.MockHandlers
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

                var service = _context.ResolveOptional(call.GetReturnType());

                if (service is null)
                {
                    return RouteAction.Continue();
                }

                return RouteAction.Return(service);
            }
        }
    }
}