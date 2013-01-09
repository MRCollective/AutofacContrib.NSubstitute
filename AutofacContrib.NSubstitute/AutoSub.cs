namespace AutofacContrib.NSubstitute
{
    public class AutoSub<TSut> where TSut : class
    {
        private TSut _sut;
        private AutoSubstitute _autoSubstitute = new AutoSubstitute();

        public AutoSub()
        {
            _sut = _autoSubstitute.ResolveAndSubstituteFor<TSut>();
        }

        public TSut SUT()
        {
            return _sut;
        }

        public T Dependency<T>() where T : class
        {
            return _autoSubstitute.Resolve<T>();
        }

        public T SubFor<T>() where T : class
        {
            return _autoSubstitute.ResolveAndSubstituteFor<T>();
        }
    }
}
