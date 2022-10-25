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
3. Set credentials for Iperbooking:
```
dotnet user-secrets set "Iperbooking:IdHotel" "{YourIdHotel}"
dotnet user-secrets set "Iperbooking:Username" "{YourIperbookingUsername}"
dotnet user-secrets set "Iperbooking:Password" "{YourIperbookingPassword}"
```
4. Set Portale Alloggiati credentials (you would need to generate WsKey for this first on the Portale Alloggiati website):
```
dotnet user-secrets set "AlloggiatiService:Utente" "{YourPortaleAlloggiatiUsername}"
dotnet user-secrets set "AlloggiatiService:Password" "{YourPortaleAlloggiatiPassword}"
dotnet user-secrets set "AlloggiatiService:WsKey" "{YourGeneratedWsKey}"
```
5. Set STU credentials:
```
dotnet user-secrets set "STU:Username" "{YourSTUUsername}"
dotnet user-secrets set "STU:Password" "{YourSTUPassword}"
```
6. Run it:
```
dotnet run
```
