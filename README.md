# Entity Management
Repository pattern that uses generics for entity types that can be used for Entity Framework Core data access.

## Build Status
[![Build status](https://saji.visualstudio.com/Open%20Source/_apis/build/status/EntityManagement-import)](https://saji.visualstudio.com/Open%20Source/_build/latest?definitionId=25)

## Dependencies
- .Net Standard 2.0
- Entity Framework Core 2.2

## How To Use
1. All data models that the you wish to access should implement `EntityManagement.IEntity`
2. Implement `EntityManagement.IDatabaseContext` by inheriting `Microsoft.EntityFrameworkCore.DbContext`
3. Register your database context in your dependency injection container
   - For example, when using `Autofac`, this can be done using the following code
     `builder.RegisterType<DatabaseContext>().As<IDatabaseContext>().InstancePerLifetimeScope();`
4. Register your entity repositories in your dependency injection container
   - For example, when using `Autofac`, access for all data models that implement `EntityManagement.IEntity` can be done with the following code
     `builder.RegisterGeneric(typeof(EntityRepository<,>)).As(typeof(IEntityRepository<,>));`
5. Inject `IEntityRepository<TEntity, TId>` into the appropriate classes that perform data access