# Clamper

<h1>
<img src="https://raw.githubusercontent.com/divicent/clamper/develop/media/logo/clamper_64.png" alt="Icon" width="64px"/>
Clamper
</h1>

[![Build Status](https://travis-ci.org/Divicent/Clamper.svg?branch=develop)](https://travis-ci.org/Divicent/Clamper)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/328b02683cf54fe1ba675f8cd56cb5f5)](https://app.codacy.com/app/rusith/Clamper?utm_source=github.com&utm_medium=referral&utm_content=Divicent/Clamper&utm_campaign=Badge_Grade_Dashboard)

Clamper is a tool which can generate a library that can be used to access a database. the generated library has an optimized, rich API that can be used to do all kind of operations on a relational database without writing any SQL.

Clamper uses the Dapper Micro ORM to map the result sets. this makes the mapping high efficient. Clamper can generate libraries for .Net Framework and .Net core. the generated library has the API to access the database and it references the Clamper.Core library from Nuget. the Core library has all infrastructure that is necessary the generated library to work.

Clamper is being developed keeping the best practices in mind. you can integrate the generated code easily as it exposes a layer of abstraction without exposing the implementation details.