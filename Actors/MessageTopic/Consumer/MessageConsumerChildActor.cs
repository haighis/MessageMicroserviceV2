using System;
using System.Collections.Generic;
using Actors.BusinessLogic;
using Akka.Actor;
using DataModel;
using KafkaMessageService;

namespace Actors.MessageTopic.Consumer
{
    /// <summary>
    /// Child Actor for taking on dangerous task of Consuming a Message from kafka Topic "message"
    /// </summary>
    public class MessageConsumerChildActor : ReceiveActor
    {
        private KafkaConsumerService<MessageDTO> consumerService;

        public MessageConsumerChildActor()
        {
            Receive<int>(msg =>
            {
                Console.WriteLine("in consumer child before");
                consumerService = new KafkaConsumerService<MessageDTO>("keltonjohntest", "messagegroup", new Uri[] { new Uri("http://logsvrdev:9092") });
                Console.WriteLine("in consumer child after");
                var list = consumerService.Consume(); //new List<MessageDTO>();
                
                //consumerService.Consume().ContinueWith(items =>
                //{
                //    Console.WriteLine("in consumer count " + list.Count);
                //    list = items.Result;
                //    Console.WriteLine("after" + list.Count);
                //})
                //.PipeTo(Self);

                // Consume the topic "message"
                var actor = Context.ActorOf(Props.Create(() => new MessageCoordinatorActor()),
                    Guid.NewGuid().ToString());

                foreach (var item in list)
                {
                    actor.Forward(new Message(item.Message, Guid.NewGuid()));
                }

                 Console.WriteLine("IN consumer after send to database ");
                // Console.WriteLine(msg);
                // INSERT TO A DATABASE OR SEND AN EMAIL
            });  
        }

        /// <summary>
        /// PreRestart method that is called when Actor is restarted. 
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="message"></param>
        protected override void PreRestart(Exception reason, object message)
        {
            Self.Tell(message);
        }

        /// <summary>
        /// Dispose of any resources (I.e. dispose of database connection)
        /// </summary>
        protected override void PostStop()
        {
            base.PostStop();
        }
    }
}
