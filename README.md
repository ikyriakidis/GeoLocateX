# Natech Dev

## Generate migrations scripts

```bash
dotnet ef migrations add InitialCreate --project ../GeoLocateX.Data --startup-project .
```

## Generate database

```bash
dotnet ef database update --project ../GeoLocateX.Data --startup-project .
```