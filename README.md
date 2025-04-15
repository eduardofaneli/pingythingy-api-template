# PingyThingy Secure API Template Source Code (NarebaCoder)

This repository contains the source code for the `dotnet new` template **NarebaCoder.PingyThingy.Api.Template**.

The template aims to provide a solid and secure foundation for creating .NET 9 APIs, incorporating security and observability best practices.

## NuGet Package

The compiled and ready-to-use template is available as a NuGet package:

[![NuGet version](https://img.shields.io/nuget/v/NarebaCoder.PingyThingy.Api.Template.svg)](https://www.nuget.org/packages/NarebaCoder.PingyThingy.Api.Template)

**Link:** [https://www.nuget.org/packages/NarebaCoder.PingyThingy.Api.Template](https://www.nuget.org/packages/NarebaCoder.PingyThingy.Api.Template)

## How to Use the Template

1.  **Install the Template:**
    ```powershell
    dotnet new install NarebaCoder.PingyThingy.Api.Template
    ```

2.  **Create a New Project:**
    ```powershell
    dotnet new narebacoder-pingyapi -n YourNewProjectName
    ```
    (Replace `YourNewProjectName` with the desired name for your API).

3.  **Follow Instructions:** Refer to the `README.md` generated inside your new project for additional configurations (like User Secrets).

## Repository Structure

*   `./PingyThingy.Template/`: Contains the `.csproj` project to package the template as a NuGet package.
    *   `./PingyThingy.Template/README.md`: The specific README displayed on the NuGet package page.
    *   `./PingyThingy.Template/content/`: Contains all the actual source code that will be included in the template (the .NET solution, Dockerfiles, etc.).
*   `./README.md`: This file, explaining the purpose of the repository.
*   `.gitignore`: Standard file to ignore unnecessary Git files.
*   `.editorconfig`: Defines coding styles and line endings (LF).

## Building the Template from Source

If you cloned this repository and want to build and install the `.nupkg` package locally:

1.  Navigate to the `PingyThingy.Template` folder:
    ```powershell
    cd PingyThingy.Template
    ```
2.  Run the `dotnet pack` command:
    ```powershell
    dotnet pack --configuration Release
    ```
    This will generate the `.nupkg` file inside `bin/Release`.
3.  Install the package locally:
    ```powershell
    dotnet new install .\bin\Release\NarebaCoder.PingyThingy.Api.Template.<version>.nupkg
    ```
    (Replace `<version>` with the current version defined in the `.csproj`).

## Contributions

Contributions are welcome! Feel free to open Issues or Pull Requests.
