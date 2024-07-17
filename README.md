# IP Address Geographic Identification Application

The application (API) will enable the geographical identification of one or more IP addresses using the Free IP
Geolocation API (FreeGeoIP) service https://freegeoip.app.

1. Create an endpoint (GET) that will accept as input an IP address and return the fields: IP, country
code, country name, time zone, latitude, and longitude. You will call the FreeGeoIP API for IP
geolocation.
2. Create an endpoint (POST) that will accept as input a list of IP addresses. You will call the FreeGeoIP
API to identify each IP geographically. The FreeGeoIP API call must be made asynchronously for the
process to run in the background. The endpoint should return a URL (of the 3rd endpoint), along
with an ID for the batch process, which, if called, will display the progress of the geographically
identified IPs.
3. Create an endpoint (GET) that will accept as input the ID code of a batch process and return the
progress of the IPs that have been identified geographically (e.g., 20/100) and the estimated time of
completion of the process. The answer of the 2nd endpoint will contain the URL of this endpoint.


## Prerequisites

* SQL Server
* .NET SDK 8
* Entity Framework Core tools
* SQL Server Management Studio (SSMS)

## Install Entity Framework Core tools

```bash
dotnet tool install --global dotnet-ef
```

## Create the GeoLocateX database

Open SQL Server Management Studio (SSMS) and execute the following SQL script

```bash
CREATE DATABASE GeoLocateX;
```

## Generate migrations scripts

```bash
dotnet ef migrations add InitialCreate --project ../GeoLocateX.Data --startup-project .
```

## Generate database

```bash
dotnet ef database update --project ../GeoLocateX.Data --startup-project .
```