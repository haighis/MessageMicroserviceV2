# Message MicroserviceSample

Includes: 

-- Console Application (SystemConsole console application)

-- Console Application 2 (SystemConsole2 for testing)

-- Data Model - common messages (DataModel class libary)

-- Actors - common actors shared between Console Applications 

-- Data Access using Entity Framework

-- Business Logic Layer

-- Web Application that uses Group Router to send messages to backend cluster called GroupRouterSystem

Actors
- CoordinatorActor and ChildActor - Supervisor Strategry Pattern that write to the console. In a real system
we would write to a database or send an email.

Features

- Clustering
- Hocon configuration from app.config


About this Sample

In this sample there are few aspects that handle different concerns to form this Microservice that will interface with a RESTful interface. 

Requirements:

- RESTful interface for saving entities via POST
- Highly concurrent
- Resilient to database connectivity issues such as the case when a database is down or temporarily unavailable (Supervisor Strategy Pattern)
- Resilient to database connectivity issues such as the case when a database is offline and allow for storing message in alternate backend system and then retreiving when db is available (db up/db down Pattern) 
- Resilient to business logic exceptions in event business logic is violated (BusinessLogicException with supervisor strategy)
- A single entity called Message needs to be saved to a MS SQL Server database
- Extensible to allow for easy addition of requirements in the future - 
- Handle high load of messages with back pressure. When the heat is turned up we want to stay cool under pressure. - future
- Be alerted when actor system is experiencing issues/exceptions. - Real time error alerting to AlertSystem. Separate alert system project that
uses group router to pass message other AlertSystem. AlertSystem will have a WPF GUI that receives errors real time.

Problems include:

- How will messages flow from a RESTful interface to the backend Cluster System? 
- How will we handle 

1. Create a Cluster system through the name "GroupRouterSystem". This name can be whatever you like but to form a cluster you all 
instances of the Cluster system must have the same value.

2. 