# MassTransit State Machines

_[MassTransit](https://masstransit-project.com/) is a free, open-source distributed application framework for .NET. MassTransit makes it easy to create applications and services that leverage message-based, loosely-coupled asynchronous communication for higher availability, reliability, and scalability._

## Introduction

I have been using MassTransit for quite sometime now. A few weeks ago, I add a requirement in which I had to integrate a state machine with a MySql database. Although the unit tests worked perfectly, the integration with MySql seemed to not be working. Therefore, I made this small project in order to test out integration with several databases, using Entity Framework Core, and conclude about the problem in hands.

## Conclusion

The database locking statement is not correctly formulated, causing the query to return empty, thus not locking any row. This results in records being constantly added to the table.

Currently used on MassTransit:
```Sql
SELECT * FROM {1} WHERE {2} = \"@p0\" FOR UPDATE
```

Correctly formulated:
```Sql
SELECT * FROM `{1}` WHERE `{2}` = @p0 FOR UPDATE
```

## Testing
In order to validate this behavior run the integration tests. To do so, you should:

1. Build the infrastructure and run the necessary migrations through `run-migrations.sh`

2. Run the tests by using `run-tests.sh`. If you wish to have the result in HTML format, use `run-tests.sh html`

Check that tests that integrate with MySQL and are using the default lock statement will **fail** (at least 50%).