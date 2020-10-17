# Ndb Library

_NOTE: Library has been developed back in Apr 17th, 2009. It was developed in Test First Development in mind and implements code-centric approach to work with Database. I don’t work on it anymore since Entity Framework team shipped EF Code First as a part of the Entity Framework 4 release. Ndb Library was designed to work with different database engines (MySQL, PostgreSQL, SqLite, MSSQL Server are supported). In the same time it’s pretty easy to add other engines. Moved it from Code Plex just in case_

Ndb is an Unit Tests & Continues Integration oriented database access library.
It allows easily and more productive for programmers and architectors to work with databases.
If you like Domain-Driven Design probably you'll like Ndb.

**Ndb provides the following features:**
* Allows you to easily map your .NET classes to a database tables
* Using _DbGateway_ - you can store, update, remove and load instances of the mapped classes
* Using _DbStructureGateway_ - you can create, update, delete tables associated with your .NET classes

**Ndb Library has the following advantages:**
* It's easy to use and intuitive
* You don't need to code standart actions like storing object to database. So you can easily generate any data for your Unit tests. Also all Ndb functionality supports several database engines.
* You can create database structure (tables, primary and foreign keys, indexes) for you project just from your assemblies, therefore Ndb simplifies team development - as synchronized the source code, you already have synchronized database structure.
* You can easily setup Continuous Integration for the projects you create using Ndb. You just need to create one Unit tests assembly which will syncronize your updated project assemblies with database on you build server. After that you can run all tests from your solution since your database is syncronized.
* Ndb implements several patterns (approaches) to working with database.

**Supported databases:**

* MySQL
	* Ndb works with InnoDB tables engine (since it has foreign keys constraints)

* PostgreSQL
	* Database and column names in database are lowercased
	* There are no unsigned types in PostgreSQL therefore system use bigger type. For example Ndb uses int8 for UInt32 values storing

* SqLite
	* SqLite doesn't have foreign keys functionality therefore Ndb emulate them by generating triggers
	* Besides poor ALTER TABLE support by SqLite, Ndb can't update it's tables now

* MS SQL
	* Added in 1.0.611.0
	* There are no unsigned types in MS SQL so you will have an exception if you use one of them

* Oracle
	* Sorry, but only in plans...



Ndb allows you to easily **load objects from relational database**
```
User user = DbGateway.Instance.Load<User>(userId);
User user = DbGateway.Instance.LoadParent<User>(TestData.WorkLog);
WorkLog []() events = DbGateway.Instance.LoadChilds<WorkLog>(TestData.TestUser);
Task []() tasks = DbGateway.Instance.LoadAssociated<Task, TasksAssignment>(TestData.TestUser);

[DbRecord](DbRecord)
public class User
{
    [DbPrimaryKey](DbPrimaryKey) public ulong Id;
    [DbField](DbField) public RolesManager.Ids RoleId;
    [DbField](DbField) public string Email;
    [DbField](DbField) public string Password;
    [DbField](DbField) public string FirstName;
    [DbField](DbField) public string LastName;

    public string FullName { get { return FirstName + " " + LastName; } }
}

[DbRecord](DbRecord)
public class WorkLog
{
   [DbPrimaryKeyField](DbPrimaryKeyField)
   public ulong Id;

   [DbForeignKeyField(typeof(User))](DbForeignKeyField(typeof(User))) 
   public ulong UserId;

   ...
}

[DbRecord](DbRecord)
public class TasksAssignment
{
   [DbForeignKeyField(typeof(Task))](DbForeignKeyField(typeof(Task)))
   public ulong TaskId;

   [DbForeignKeyField(typeof(User))](DbForeignKeyField(typeof(User)))
   public ulong UserId;

   ...
}

[DbRecord](DbRecord)
public class Task
{
    [DbPrimaryKeyField](DbPrimaryKeyField)
    public ulong Id;

    ...
}
```

**Database Structure Access**

Ndb allows you to create database structure based on your assemblies and types. Also it can check is database structure and your types are syncronized, ie what all required tables and fields present in database
**DbStructureGateway** class provides all functionality needed to work this database structure **CreateTable() AlterTable() DropTable() IsValid()**

```
DbStructureGateway.Instance.CreateTable(typeof(User));
DbStructureGateway.Instance.AlterTable(typeof(User));
DbStructureGateway.Instance.DropTable(typeof(User));
DbStructureGateway.Instance.IsValid(typeof(User))
```
**FAQ**
* [Why do not use stored procedures?](http://www.codeproject.com/KB/architecture/DudeWheresMyBusinessLogic.aspx) ([ru version](http://habrahabr.ru/blogs/refactoring/65432/))
