# Entity Management
EF Core database context abstraction.

## Build Status
[![Build Status](https://saji.visualstudio.com/Open%20Source/_apis/build/status/EntityManagement?branchName=master)](https://saji.visualstudio.com/Open%20Source/_build/latest?definitionId=40&branchName=master)

## Dependencies
- .Net Standard 2.1
- Entity Framework Core 3.1
- Autofac 6.x.x

## How To Use
1. All data models that the you wish to access should implement `EntityManagement.IEntity`
2. Implement `EntityManagement.IDatabaseContext` by inheriting `Microsoft.EntityFrameworkCore.DbContext`
3. Use `EntityManagement.EntityManagementModule{T}` (Autofac module) to register your implementation of `EntityManagement.IDatabaseContext`.
