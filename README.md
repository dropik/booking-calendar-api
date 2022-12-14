# Booking Calendar API
RESTful web service for booking calendar app.

## What for?
It serves for connecting together all of the endpoints involved in this app:
the web app itself on the client side, Iperbooking for retrieving booking and guests data,
MySQL database with room and color assignments and session storage,
Police web service for publishing arrived people data, ISTAT web service for similar data publishing purpose.

## How to run?
For fully utilizing this app the credentials for all of the endpoints involved are needed. Considering all of these credentials are available, do the following steps:
1. Set credentials for the database using user-secrets:
```
dotnet user-secrets set "DB:ConnectionString" "Server={mysql-server-address}; Database={database-name}; Uid={username}; Pwd={password}"
```
2. Update database using PM console:
```
Update-Database
```
Or via .NET CLI:
```
dotnet database update
```
3. Use **merge-iso-statipolizia.csv** file to import Police nation codes into your newly created **policenations** table in your MySQL database.
4. Set credentials for Iperbooking:
```
dotnet user-secrets set "Iperbooking:IdHotel" "{YourIdHotel}"
dotnet user-secrets set "Iperbooking:Username" "{YourIperbookingUsername}"
dotnet user-secrets set "Iperbooking:Password" "{YourIperbookingPassword}"
```
5. Set Portale Alloggiati credentials (you would need to generate WsKey for this first on the Portale Alloggiati website):
```
dotnet user-secrets set "AlloggiatiService:Utente" "{YourPortaleAlloggiatiUsername}"
dotnet user-secrets set "AlloggiatiService:Password" "{YourPortaleAlloggiatiPassword}"
dotnet user-secrets set "AlloggiatiService:WsKey" "{YourGeneratedWsKey}"
```
6. Set STU credentials:
```
dotnet user-secrets set "C59Service:Username" "{YourSTUUsername}"
dotnet user-secrets set "C59Service:Password" "{YourSTUPassword}"
dotnet user-secrets set "C59Service:Struttura" "{YourSTUStructureId}"
```
7. Run it:
```
dotnet run
```

## Changelog
### v1.2.0
- Added base board information to tiles.
- Fixed minor bugs.
### v1.1.2
- Fixed correct tracked records order by record type.
### v1.1.1
- Fixed missing date specified field in ISTAT publication.
### v1.1.0
- Added ISTAT endpoint to send new ISTAT data and to get last published date.
- Fixed wrong police data row serialization.
- Added nation descriptions to nations table.
- Better police place of birth search by description.
- Fixed simultaneous context bound stuff on police data generation.
- Fixed taking bookings from following date when publishing to police.
