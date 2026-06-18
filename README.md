# The Meeple System
## Overview
The Meeple System is a C# .NET MVC Razor Page web-app that allows users to interact and manage a database of board games! This is the culmination of months of work by Laura Kirkpatrick, Gabriel Stokes, and Rylan Wade. 
This app is a colleciton management system that was built to help a local game store manage it's vast board game collection of over 1300 games. Before immplemention, they were only able to cull their collection once a year, but with the rate of board game releases increasing, their collection has been growing faster than they can manage!
## Features
The Meeple System is a custom web-app to help one of our local game stores, Village Meeple Boardgame Cafe, located in Springfield Missouri. They gave Laura a list of their needs and she worked with the team to design an app that would fulfill those needs.
### Collection Management
The collection can easily be managed in our app via different pages such as the add game, delete game, check-in game, search, and report pages. These allow Village Meeple to ensure that they always have popular games on display as well as remove less-played games from their collection.
### Report Page
The report page is the shining star of the Meeple System. This allows the users to use one button with filters to find the most/least popular games in a certain timeframe!
### Barcode Scanning
The barcode scanner is built into the system so that the app can easily be used from a mobile device. With this, it allows users to scan barcodes on their phone for easy check-ins and deletions from the collection system.
## Credit
Laura built most of the API including the endpoints, gameDAL, the database context, and the game controller. She also Designed and implemented the SQL Server database where the games are stored.
Gabe built most of the Client including the barcode scanner, webpages, formats, connecting the API and the Client, and layouts.
Rylan helped with specific endpoints, the main one being the import by CSV. He also built the web page for the delete game endpoint.
