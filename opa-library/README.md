# Local Library Utilization Instruction
## Enviroment
This library works on linux using .NET Core SDK v3.1.419
If you want to make it run in other enviroment, consider updating the opa.zip layer

## Creating a library/package and adding it to your project
In order to add the local library package to your project using only dotnet cli, it's possible to use the following procedure:

**Creating the library/package**
1. In this directory use the command ``` dotnet build ``` cli command to generate the package that will be located at bin\Debug\\{packageName}.{packageVersion}.nupkg

**Suggestion of who to use the library/package in your project**
1. In your project, use the ``` dotnet new nugetconfig ``` to create a new nuget enviroment where you can configure it's sources of packages.
2. Into your project's terminal, add the local package source by entering the ``` dotnet nuget add source {sourcePath} -n {sourceName} ``` command where the {sourcePath} can be the bin\Debug\ folder of your library project
3. You can check your sources by doing the ``` dotnet nuget list source ``` command
4. Add the package to your project by doing ``` dotnet add package {packageName} -s {sourceName}```, check if the package has been successfully installed with ``` dotnet list package ```
5. If you couldn't install the right package or had some mistakes while executing the command, disable all of the others sources (check by doing ``` dotnet nuget list source ```) with ``` dotnet nuget disable source {sourceName} ```
6. With only the local source enabled, do ``` dotnet add package {packageName} ```
7. To code with your library/package, import it's namespace into your code.