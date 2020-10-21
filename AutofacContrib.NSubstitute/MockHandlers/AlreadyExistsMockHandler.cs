namespace AutofacContrib.NSubstitute.MockHandlers
{
    internal class AlreadyExistsMockHandler : MockHandler
    {
        public static MockHandler Instance { get; } = new AlreadyExistsMockHandler();

        private AlreadyExistsMockHandler()
        {
        }

        protected internal override void OnMockCreating(MockCreatingContext context)
        {
            if (context.HasRegistrations)
            {
                context.DoNotCreate();
            }
        }
    }
}