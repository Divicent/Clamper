#!/usr/bin/env bash
cd CLI
dotnet restore
dotnet build

if [ $? -ne 0 ]; then
    exit 3
fi

cd ..
if [ ! -z "$CLAMPER_VERSION" -a "$CLAMPER_VERSION" != " " ]; then
  cd CLI
  dotnet restore
  printf "$CLAMPER_VERSION" > .version
  cd ..
  dotnet publish -c release -r win10-x64
  dotnet publish -c release -r ubuntu.16.10-x64
  dotnet publish -c release -r osx-x64
fi