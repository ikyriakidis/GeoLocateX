# Natech Dev

## Prerequisites

* SQL Server
* .NET SDK 8
* Entity Framework Core tools

## Install Entity Framework Core tools

```bash
dotnet tool install --global dotnet-ef
```

## Generate migrations scripts

```bash
dotnet ef migrations add InitialCreate --project ../GeoLocateX.Data --startup-project .
```

## Generate database

```bash
dotnet ef database update --project ../GeoLocateX.Data --startup-project .
```