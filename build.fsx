#r "paket:
nuget Fake.IO.FileSystem
nuget Fake.Dotnet.MSBuild
nuget Fake.Sql.DacPac
nuget Fake.BuildServer.TeamCity
nuget Fake.Core.Target //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.DotNet
open Fake.BuildServer

BuildServer.install [
    TeamCity.Installer
]

// If you additionally want output in the console, even on the build-server (otherwise remove this line).
CoreTracing.ensureConsoleListener ()

// Properties
let buildDir = "./build/"

// Targets
Target.create "Clean" (fun _ ->
    Shell.cleanDir buildDir
)

Target.create "BuildDb" (fun _ -> 
    !! "*.sqlproj"
    |> MSBuild.runRelease id buildDir "Build"
    |> Trace.logItems "DbBuild-Output: "
)

/// the database for local development + compile
Target.create "DeployLocalDb" (fun _ ->
    let connectionString = "Data Source=.;Initial Catalog=Notitia;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False"
    let dacPacPath = "./build/Notitia.dacpac"
    Fake.Sql.DacPac.deployDb (fun args -> { args with Source = dacPacPath; Destination = connectionString }) |> ignore)

Target.create "Default" (fun _ ->
  Trace.trace "Hello World from FAKE"
)

// Dependencies
open Fake.Core.TargetOperators

"Clean"
    ==> "BuildDb"
    ==> "DeployLocalDb"
    ==> "Default"

// start build
Target.runOrDefault "Default"