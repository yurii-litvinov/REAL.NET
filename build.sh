#!/usr/bin/env bash

set -eu
set -o pipefail

export BUILD_PACKAGES=.fake
export FAKE_CLI="$BUILD_PACKAGES/fake.exe"

if [ ! -f "$%FAKE_CLI" ]; then
  dotnet tool install fake-cli --tool-path ./$BUILD_PACKAGES
fi

dotnet restore build.proj
dotnet fake build-core $@

