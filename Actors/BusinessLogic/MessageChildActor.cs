using System;
using Akka.Actor;
using BusinessLogic;
using DataModel;

namespace Actors.BusinessLogic
{
    /// <summary>
    /// Child Actor for taking on dangerous task of inserting to a database.
    /// </summary>
    public class MessageChildActor : ReceiveActor
    {
        private IMessageBusinessLogic messageBusinessLogic;

        public MessageChildActor()
        {
            Receive<Message>(msg =>
            {
                Console.WriteLine("this is being inserted to the database beofre " + Self.Path);
                messageBusinessLogic = new MessageBusinessLogic();

                messageBusinessLogic.AddTodo(msg.Data);

                Console.WriteLine("this is being inserted to the database " + Self.Path);
                Console.WriteLine(msg.Data);
            });  
        }

        /// <summary>
        /// PreRestart method that is called when Actor is restarted. Tell ourself the message so that message
        /// is added to the Mailbox on restart. This will allow the message to be resent when the actor 
        /// restarts.
        /// 
        /// Note this actor message will be lost in the case the actor system is restarted (unplanned restart. I.e. service fails)
        /// 
        /// See for more information: https://petabridge.com/blog/how-actors-recover-from-failure-hierarchy-and-supervision/
        /// See API for more information: http://api.getakka.net/docs/unstable/html/28D639D8.htm
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
