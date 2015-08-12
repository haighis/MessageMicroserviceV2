using System;
using System.Collections.Generic;
using Akka.Actor;
using DataModel;

namespace Actors.MessageTopic.Producer
{
    public class MessageProducerCoordinatorActor : ReceiveActor
    {
        /// <summary>
        /// Restart any children who throw an <see cref="Exception"/> message.
        /// </summary>
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(10,10,Decider.From(Directive.Restart,
                new KeyValuePair<Type, Directive>(typeof(Exception), Directive.Restart)));
        }

        public MessageProducerCoordinatorActor()
        {
            Receive<Message>(msg =>
            {
                var todoChildActor = LookupOrCreateTodoChildActor(Guid.NewGuid().ToString());
                Console.WriteLine("In Producer actor path " + Self.Path);
                todoChildActor.Forward(msg);
            });
        }

        private IActorRef LookupOrCreateTodoChildActor(string name)
        {
            return LookupOrCreateTodoChildActor(Context, name);
        }

        private IActorRef LookupOrCreateTodoChildActor(IActorContext context, string name)
        {
            var child = context.Child(name);
            if (child.Equals(ActorRefs.Nobody)) //child doesn't exist
            {
                return Context.ActorOf(Props.Create(() => new MessageProducerChildActor()),
                    name);
            }
            return child;
        }
    }
}
