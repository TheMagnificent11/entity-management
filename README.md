# Entity Management
Repository pattern that uses generics for entity types that can be used for Entity Framework Core data access.

## Build Status
[![Build Status](https://saji.visualstudio.com/Open%20Source/_apis/build/status/EntityManagement?branchName=master)](https://saji.visualstudio.com/Open%20Source/_build/latest?definitionId=40&branchName=master)

## Dependencies
- .Net Standard 2.0
- Entity Framework Core 3.1
- Autofac 4.9.4

## How To Use
1. All data models that the you wish to access should implement `EntityManagement.IEntity`
2. Implement `EntityManagement.IDatabaseContext` by inheriting `Microsoft.EntityFrameworkCore.DbContext`
3. Use `EntityManagement.EntityManagementModule{T}` (Autofac module) to register your implementation of `EntityManagement.IDatabaseContext` and related repositories (registration is keyed on the name of the database context so that you can register multiple database contexts in a solution)
