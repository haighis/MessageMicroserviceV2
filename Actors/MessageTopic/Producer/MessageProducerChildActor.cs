using System;
using Akka.Actor;
using DataModel;
using KafkaMessageService;

namespace Actors.MessageTopic.Producer
{
    /// <summary>
    /// Child Actor for taking on dangerous task of inserting to a database.
    /// </summary>
    public class MessageProducerChildActor : ReceiveActor
    {
        private KakfaProducerService<MessageDTO> producerService;

        public MessageProducerChildActor()
        {
            Receive<Message>(msg =>
            {
                Console.WriteLine("before Producer ");
                producerService = new KakfaProducerService<MessageDTO>("keltonjohntest", new Uri[] { new Uri("http://logsvrdev:9092"), });
                producerService.Send(new MessageDTO(msg.Data));
                Console.WriteLine("Producer Send Child Actr path " + Self.Path + " message from frontend sent to kafka" + msg.Data);
                Console.WriteLine(msg.Data);
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
