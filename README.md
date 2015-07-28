# Message Microservice Sample

- Akka.NET Cluster
- REST Interface - Where a message is created
- Kafka - The Log for processing all messages when a Message sent via REST interface
- Azure Table Storage - Backend data store - Where Messages end up when they are saved.
- Azure Redis - Frontend Web Cache - Where Messages are read from they are requested.