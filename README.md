# Fano's ASCII Table Utility
This is a utility used to format a generic `List<T>` into a pretty ASCII table, useful when formatted data needs to be shared on applications that only allow for plain text input.

## Installation
This package is available on NuGet:
* Via CLI: `dotnet add package Fano.ASCIITableUtil --version 1.1.0`
* Package Reference: `<PackageReference Include="Fano.ASCIITableUtil" Version="1.1.0" />`
* Package Manager: `PM> Install-Package Fano.ASCIITableUtil -Version 1.1.0`

## Examples
```c#
public class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
}

...

var people = new List<Person>
{
    new Person { FirstName = "John", LastName = "Smith", Address = "1234 Fake St." },
    new Person { FirstName = "Jane", LastName = "Doe", Address = "4321 Phony Ave."  },
    new Person { FirstName = "Billy", LastName = "Walters", Address = "5678 Counterfeit Rd."  }
};

var table = AsciiTable.Create(people);
Console.WriteLine(table);
```

Result:
```
FirstName | LastName | Address
========= | ======== | ====================
John      | Smith    | 1234 Fake St.
Jane      | Doe      | 4321 Phony Ave.
Billy     | Walters  | 5678 Counterfeit Rd.
```

### Attributes
You can use custom attributes in your class definitions to exclude columns, change the header display name, or set a custom order when formatting them into a table.


#### Excluding columns
Use the `AsciiIgnoreColumn` attribute to exclude that property from the result:
```c#
public class Person
{
    
    public string FirstName { get; set; }
    public string LastName { get; set; }

    [AsciiIgnoreColumn]
    public string Address { get; set; }
}

...

var table = AsciiTable.Create(people);
Console.WriteLine(table);
```

Result:
```
FirstName  | LastName
========== | =========
John       | Smith
Jane       | Doe
Billy      | Walters
```

#### Changing the header's display name
Use the `AsciiHeaderName` attribute to change the header display name of a property:

```c#
public class Person
{
    [AsciiHeaderName("First Name")]
    public string FirstName { get; set; }

    [AsciiHeaderName("Last Name")]
    public string LastName { get; set; }

    public string Address { get; set; }
}
```

Result:
```
First Name | Last Name | Address
========== | ========= | ====================
John       | Smith     | 1234 Fake St.
Jane       | Doe       | 4321 Phony Ave.
Billy      | Walters   | 5678 Counterfeit Rd.
```

#### Reordering columns
Use the `AsciiColumnIndex` attribute to set an ordering for your columns without shuffling your properties around:

```c#
public class Person
{
    [AsciiColumnIndex(1)]
    public string FirstName { get; set; }

    [AsciiColumnIndex(0)]
    public string LastName { get; set; }

    [AsciiIgnoreColumn]
    public string Address { get; set; }
}

...

var table = AsciiTable.Create(people);
Console.WriteLine(table);
```

Result:
```
LastName | FirstName
======== | =========
Smith    | John
Doe      | Jane
Walters  | Billy
```

### Custom Headers
You can pass in a custom list of header definitions if custom attributes are not enough or if you do not want to pollute your classes with custom attributes:

```c#
var headers = new List<TableHeader<Person>>
{
    new("First Name", p => p.FirstName),
    new("Last Name", p => p.LastName)
};
var table = AsciiTable.Create(people, headers);
Console.WriteLine(table);
```

Result:
```
First Name | Last Name
========== | =========
John       | Smith
Jane       | Doe
Billy      | Walters
```

This approach is also useful if you need custom string formatting for a particular property:
```c#
public class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    
    public DateTime Birthday { get; set; }
}

...

var headers = new List<TableHeader<Person>>
{
    new("First Name", p => p.FirstName),
    new("Last Name", p => p.LastName),
    new("Birthday", p => p.Birthday.ToShortDateString())
};
var table = AsciiTable.Create(people, headers);
Console.WriteLine(table);
```

Result:
```
First Name | Last Name | Birthday
========== | ========= | =========
John       | Smith     | 5/16/1992
Jane       | Doe       | 7/21/1995
Billy      | Walters   | 3/13/1979
```
