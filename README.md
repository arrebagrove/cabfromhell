  _____      _        __                       _          _ _ 
 / ____|    | |      / _|                     | |        | | |
| |     __ _| |__   | |_ _ __ ___  _ __ ___   | |__   ___| | |
| |    / _` | '_ \  |  _| '__/ _ \| '_ ` _ \  | '_ \ / _ \ | |
| |___| (_| | |_) | | | | | | (_) | | | | | | | | | |  __/ | |
 \_____\__,_|_.__/  |_| |_|  \___/|_| |_| |_| |_| |_|\___|_|_|

This project was conceived during the IoT Hackathon in Prague on December 16th.

CabFromHell is the working name of an application that monitors taxi drivers by using accellerometer sensors and an Azure services to ingest and analyse driving behaviour. 

## Clients

# Cab Driver - car device

Raspberry PI 2 based device running Windows 10 IOT Core. It publishes data directly related to the cars movement. The cab company can then monitor their cars and easily gamify internally with "Driver of the Month" awards and similar.

# Mobile

A Xamarin client running on iOS and Android lets the end user report cab rides. It will run throughout the ride and publish the accellerometer results.

# Power BI

Inside Power BI we have a dashboard with all the details you could ask for and this would be a natural place to do the driver of the month.


## Cloud Services

# Web App - API Proxy

Some clients connect through a Web API that has been exposed. 

# IOT Hub

We digest all events into the IOT Hub.

# Service Fabric

All handling of events are done in a service fabric cluster.

# Event Hub

Output of handled events are put into an EventHub for further handling

# Stream Analytics

We deal with the events here for doing real time analysis and be able to push to visualization tools like Power BI

## Team Chat

we use Slack (http://cabfromhell.slack.com) for internal Team communications and possibly hooking up DevOps features such as controlling the build and deployment. GitHub and VSTS can then be integrated here to give us a true agile and continuous experience.

## Development Environment

# GitHub for holding the source

We're doing this as an Open Source contribution under the MIT license.

# Raspberry client

Built using Windows UWP in C#

# Mobile

Built using Xamarin and C#

# API

Build using Visual Studio Code and DotNetCore with C#. AspNet 5 WebAPI sitting at the core.


## Message Contracts

### IoT Hub

For each reading we do a delta from the previous reading of the accellerometer vector.
We accumulate the length of the delta vector for every reading. 

Every N seconds (roughly) we publish an event, the accumulated delta lengths along side with
the name of the cab driver, time period in seconds the event represent and the number of
readings for the event.

CabDriverName|sum(sqrt(deltaX^2 + deltaY^2 + deltaZ^2))|TimePeriodInSeconds|NumberOfReadings

Thomas|3.1415|5.234|7

Ola|213.343|5.0243|5





