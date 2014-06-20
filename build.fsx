#I "packages/FAKE/tools"
#r "FakeLib.dll"
#load "packages/SourceLink.Fake/tools/SourceLink.fsx"

open System
open System.IO
open Fake
open Fake.AssemblyInfoFile
open SourceLink

let dt = DateTime.UtcNow
let cfg = getBuildConfig __SOURCE_DIRECTORY__
let repo = new GitRepo(__SOURCE_DIRECTORY__)

let versionFile = getBuildParamOrDefault "versionFile" "3.3.3.0"
let versionAssembly = getBuildParamOrDefault "versionAssembly" versionFile
let rawUrl = getBuildParamOrDefault "rawUrl" "https://raw.githubusercontent.com/ctaggart/AutofacContrib.NSubstitute/{0}/%var2%"

let buildVersion =
    getBuildParamOrDefault "buildVersion"
        (   let vf = Version versionFile
            let pr = getBuildParamOrDefault "prerelease" (sprintf "-ci%s" (dt.ToString "yyMMddHHmm")) // 20 char limit
            sprintf "%d.%d.%d%s" vf.Major vf.Minor vf.Build pr )
let versionInfo =
    let vi = sprintf """{"buildVersion":"%s","buildDate":"%s","gitCommit":"%s"}""" buildVersion dt.IsoDateTime repo.Revision // json
    vi.Replace("\"","\\\"") // escape quotes

Target "Clean" (fun _ -> !! "**/bin/" ++ "**/obj/" |> CleanDirs)

Target "BuildVersion" (fun _ ->
    let args = sprintf "UpdateBuild -Version \"%s\"" buildVersion
    Shell.Exec("appveyor", args) |> ignore
)

Target "AssemblyInfo" (fun _ ->
    let common = [
        Attribute.Title "AutofacContrib.NSubstitute"
        Attribute.Product "AutofacContrib.NSubstitute"
        Attribute.Copyright "Copyright ©  2012"
        Attribute.ComVisible false
        Attribute.Version versionAssembly
        Attribute.FileVersion versionFile
        Attribute.InformationalVersion versionInfo]

    [   Attribute.Guid "c52f5919-ce5d-456d-b81d-838da19a89bc"
    ] @ common |> CreateCSharpAssemblyInfo @"AutofacContrib.NSubstitute\Properties\AssemblyInfo.cs"
)

Target "Build" (fun _ ->
    !! @"AutofacContrib.NSubstitute.sln" |> MSBuildRelease "" "Rebuild" |> ignore
)

// NUnit-Console Command Line Options
// http://www.nunit.org/index.php?p=consoleCommandLine&r=2.6.3
Target "Test" (fun _ ->
    !! @"AutofacContrib.NSubstitute.Tests\bin\Release\AutofacContrib.NSubstitute.Tests.dll"
    |> NUnit (fun p -> { p with ErrorLevel = Error; Framework="net-4.0" })
)

Target "SourceLink" (fun _ ->
    !! @"AutofacContrib.NSubstitute\AutofacContrib.NSubstitute.csproj"
    |> Seq.iter (fun f ->
        let proj = VsProj.LoadRelease f
        logfn "source linking %s" proj.OutputFilePdb
        let files = proj.Compiles -- "**/AssemblyInfo.cs"
        repo.VerifyChecksums files
        proj.VerifyPdbChecksums files
        proj.CreateSrcSrv rawUrl repo.Revision (repo.Paths files)
        Pdbstr.exec proj.OutputFilePdb proj.OutputFilePdbSrcSrv
    )
)

Target "NuGet" (fun _ ->
    CreateDir "bin"
    let common (p:NuGetParams) =
        { p with
            Version = buildVersion
            OutputPath = "bin"
        }

    NuGet (fun p ->
        { p with
            WorkingDir = @"AutofacContrib.NSubstitute\bin\Release"
            Dependencies =
                [   "NSubstitute", GetPackageVersion "packages" "NSubstitute"
                    "Autofac", GetPackageVersion "packages" "Autofac" ]
        } |> common) @"AutofacContrib.NSubstitute\AutofacContrib.NSubstitute.nuspec"
)

Target "Start" DoNothing

"Start"
    =?> ("BuildVersion", buildServer = BuildServer.AppVeyor)
    ==> "AssemblyInfo"
    ==> "Build"
    ==> "Test"
    =?> ("SourceLink", buildServer = BuildServer.AppVeyor || hasBuildParam "link")
    ==> "NuGet"

RunTargetOrDefault "NuGet"