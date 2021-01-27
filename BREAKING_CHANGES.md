AutofacContrib.NSubstitute (AutoSubstitute) Breaking Changes
============================================================

Version 7.0.0
-------------

Removed obsolete methods that were in the project.

### Reason
They built up some unnecessary cruft and had easy replacements

### Workaround
Update to the latest 6.x package and review obsolete messages.

Version 6.0.0
-------------

Removed `AutoSubstitute.Provide(...)` methods and added `AutoSubstitute.Configure()` with a builder pattern.

### Reason
Autofac now enforces immutability and containers cannot be changed after being built.

### Workaround
Update usage of `.Provide(...)` to use the builder pattern instead.

Version 4.0.0
-------------

Removed support for .NET 4.0; now targetting .NET 4.5+.

### Reason
In order to keep up to date with latest Autofac dependency we need to drop .NET 4.0 support.

### Workaround
Use version 3.3.7 if you need .NET 4.0 support.


Version 3.2.0
-------------

Signing of the assembly has been removed.

### Reason
We're fundamentally against it; it causes more problems than it's worth.

### Workaround
Use version 3.1.X or below or feel free to argue your case at https://github.com/MRCollective/AutofacContrib.NSubstitute/issues.
