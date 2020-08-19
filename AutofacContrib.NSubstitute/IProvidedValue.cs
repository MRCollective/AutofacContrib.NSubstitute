namespace AutofacContrib.NSubstitute
{
    public interface IProvidedValue<T>
    {
        T Value { get; }
    }
}