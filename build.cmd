.nuget\nuget.exe install FAKE -OutputDirectory packages -ExcludeVersion
.nuget\nuget.exe install SourceLink.Fake -OutputDirectory packages -ExcludeVersion
.nuget\nuget.exe install NUnit.Runners -OutputDirectory packages
.nuget\nuget.exe restore
packages\FAKE\tools\FAKE.exe build.fsx %*