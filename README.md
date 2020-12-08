# Entity Management

EF Core database context abstraction.

## Build Status

![.NET Core](https://github.com/TheMagnificent11/entity-management/workflows/.NET%20Core/badge.svg)

## Dependencies

- .Net Standard 2.1
- Entity Framework Core 5
- Autofac 6.x.x

## How To Use

1. All entity types that the you wish to access should inherit `EntityManagement.Core.BaseEntity`.
   - See [Team.cs](SampleApiWebApp/Domain/Team.cs)
2. Create fluent configurations for each of these data models by inheriting `EntityManagement.BaseEntityConfiguration<TEntity, TId>` for each entity.
   - See [TeamConfiguration.cs](SampleApiWebApp/Data/Configuration/TeamConfiguration.cs)
3. Implement database context by inheriting `Microsoft.EntityFrameworkCore.DbContext` (ensure the configurations created in step 2 are added in the `OnModelCreating` method).
   - See [DatabaseContext.cs](SampleApiWebApp/Data/DatabaseContext.cs)
4. Create database context design-time factory.
   - See [DatabaseContextFactory.cs](SampleApiWebApp/Data/DatabaseContextFactory.cs)
5. Call `serices.ConfigureDatabaseContextAndFactory<YouDbContext>(databaseConnectionString)` in the `ConfigureServices` method of your `Startup` class.
   - See [Startup.cs](SampleApiWebApp/Startup.cs)

