﻿using Actors.MessageTopic.Consumer;
using Actors.MessageTopic.Producer;
using Akka.Actor;
using Akka.Configuration;
using Akka.Configuration.Hocon;
using Akka.Routing;
using DataModel;
using System;
using System.Configuration;

namespace GroupRouterSystem
{
    internal class Program
    {
        private static Config _clusterConfig;
        private static IActorRef _testCoordinator;

        private static void Main(string[] args)
        {
            var section = (AkkaConfigurationSection)ConfigurationManager.GetSection("akka");
            _clusterConfig = section.AkkaConfig;
            LaunchBackend(new[] { "2551" });
            LaunchBackend(new[] { "2552" });
            LaunchBackend(new string[0]);
            TestConsumer();
            string input;

            Console.WriteLine("Enter send to send the message bar or quit to exit.");

            while ((input = Console.ReadLine()) != null)
            {
                var cmd = input;
                switch (cmd)
                {
                    case "quit":
                        return; // Stop the run thread
                    //case "consumer":
                    //    // Send to backend which will write to the console
                    //    TestConsumer();
                        break;
                    case "send":
                        // Send to backend which will write to the console
                        SendToBackend();
                        break;
                }
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }


        /// <summary>
        /// Send to Backend. - Sample only. You wouldn't do this in production. Here an actor system is created each time
        /// this method is called which is an expensive operation.
        /// </summary>
        private static void SendToBackend()
        {
            if (_testCoordinator != null)
            {
                _testCoordinator.Tell(new Message("this is a message from send to backend", Guid.NewGuid()));
                Console.WriteLine("path " + _testCoordinator.Path);
            }
        }

        private static ActorSystem consumerActorSystem; 

        public static void TestConsumer()
        {
            //Console.WriteLine("In test consumer ");
            consumerActorSystem = ActorSystem.Create("GroupRouterSystem");
            var consumerActorCoordinator = consumerActorSystem.ActorOf<MessageConsumerCoordinatorActor>("MessageConsumer");

            consumerActorSystem.Scheduler.Schedule(TimeSpan.FromSeconds(0),
                         TimeSpan.FromSeconds(60),
                         consumerActorCoordinator, 1);
        }

        private static void LaunchBackend(string[] args)
        {
            var port = args.Length > 0 ? args[0] : "0";

            var config = ConfigurationFactory.ParseString("akka.remote.helios.tcp.port=" + port).WithFallback(_clusterConfig);
            
            var system = ActorSystem.Create("GroupRouterSystem", config);

            // Create Coordinator Actor that will supervise risky child (Character Actor) actor's
            var actor = system.ActorOf(Props.Create(() => new MessageProducerCoordinatorActor()), "testcoordinator");

            //// Send some messages to get gossip going
            actor.Tell(new Message("test", Guid.NewGuid()));
            actor.Tell(new Message("test", Guid.NewGuid()));
            actor.Tell(new Message("test", Guid.NewGuid()));
            actor.Tell(new Message("test", Guid.NewGuid()));

            if (_testCoordinator == null)
            {
                // hocon config
                _testCoordinator = system.ActorOf(Props.Empty.WithRouter(FromConfig.Instance), "test-group");
                
                Console.WriteLine("path " + _testCoordinator.Path);
            }

            actor.Tell(new Message("warmup the system and get some gossip going 1", Guid.NewGuid()));
            actor.Tell(new Message("warmup the system and get some gossip going 1", Guid.NewGuid()));
            actor.Tell(new Message("warmup the system and get some gossip going 1", Guid.NewGuid()));
            actor.Tell(new Message("warmup the system and get some gossip going 1", Guid.NewGuid()));
        }
    }
}