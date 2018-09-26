<h1>
<img src="https://raw.githubusercontent.com/divicent/clamper/develop/media/logo/clamper_64.png" alt="Icon" width="64px"/>
Clamper
</h1>

[![Build Status](https://travis-ci.org/Divicent/Clamper.svg?branch=develop)](https://travis-ci.org/Divicent/Clamper)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/328b02683cf54fe1ba675f8cd56cb5f5)](https://app.codacy.com/app/rusith/Clamper?utm_source=github.com&utm_medium=referral&utm_content=Divicent/Clamper&utm_campaign=Badge_Grade_Dashboard)

Clamper is a tool which can generate a library that can be used to access a database. the generated library has an optimized, rich API that can be used to do all kind of operations on a relational database without writing any SQL.

Clamper uses the Dapper Micro ORM to map the result sets. this makes the mapping high efficient. Clamper can generate libraries for .Net Framework and .Net core. the generated library has the API to access the database and it references the Clamper.Core library from Nuget. the Core library has all infrastructure that is necessary the generated library to work.

Clamper is being developed keeping the best practices in mind. you can integrate the generated code easily as it exposes a layer of abstraction to the upper layers.

# Getting Started

## Downloading

You can download the latest version from the [Github releases page](https://github.com/Divicent/Clamper/releases).
This page contains the 3 versions of the release which are for Windows, Linux and Mac.


## Installing

The release is a self contained package which means it has no external dependencies.
You can extract the downloaded package to anywhere in and add a the path to the folder to the PATH environment variable. then you will be able to run ClamperCLI by just typing ClamperCLI in the terminal. otherwise you will have to run the ClamperCLI by providing the full path to executable.

## Updating

Download the new version and extract to the existing installation and replace existing files.


# Basics

## How it works

Basically it reads the configuration file in a folder, retrieves the metadata from the specified database in the configuration and creates a .Net library.

## Basic setup

Below is a very basic example configuration file that will direct ClamperCLI to generate a library

```
{
    "connectionString": "Server=myServerAddress;Database=myDataBase;User Id=myUsername;
Password=myPassword;",
    "projectPath": "D://Projects/MyProjects",
    "baseNamespace": "MyProject",
    "ProjectFile": "MyProject.DAL.csproj",
}
```

to start generating, you must create something like this with the filename `clamper.config.json` and run ClamperCLI from that location.
