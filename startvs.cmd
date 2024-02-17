@ECHO OFF
SETLOCAL

:: This command launches a Visual Studio solution with environment variables required to use a local version of the .NET Core SDK.

SET sln=Hyperion.sln

start "" "%sln%"
