using System;
using System.ComponentModel;

namespace AutofacContrib.NSubstitute
{
    /// <summary>
    /// Deprecated auto mocking container. Use <see cref="AutoSubstitute"/> instead.
    /// </summary>
    [Obsolete("AutoMock has been deprecated in favour of AutoSubstitute.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class AutoMock : AutoSubstitute
    {
    }
}