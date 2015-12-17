# cabfromhell

This project was conceived during the IoT Hackathon in Prague on December 16th.

CabFromHell is the working name of an application that monitors taxi drivers by using accellerometer sensors and an Azure services to ingest and analyse driving behaviour. 

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





