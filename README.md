# C-Client-Server-Service-Hi-Lo
A project demonstrating C# .NET frramework/WPF and Windows Services with a client-server system, 
that plays a simple Hi-Lo game between multiple uniquely identified clients and a server running as a service

## Installation
For Client: build in release mode and run the .exe program
For Server: build in release mode, then install as a service using installutil.exe. 

## Usage
For Client: run the .exe generated by building the release version. Enter a valid username, make sure the IP and port match up with the server's config and start game.
For Server: open Windows Services, select HiLo_ServerService and start as a service. 