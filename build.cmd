@echo off

set BUILD_PACKAGES=.fake
set FAKE_CLI="%BUILD_PACKAGES%/fake.exe"

if not exist %FAKE_CLI% (
  dotnet tool install fake-cli --tool-path ./%BUILD_PACKAGES%
)

rem comments following lines once you are done with your script, the idea is to be sure paket install regenerate the lock file if we add new nuget in the fsx
rem if exist ".fake"          (rmdir /Q /S ".fake"         )
rem if exist "build.fsx.lock" (del         "build.fsx.lock")

dotnet restore build.proj

%FAKE_CLI% run build.fsx --target "All"
