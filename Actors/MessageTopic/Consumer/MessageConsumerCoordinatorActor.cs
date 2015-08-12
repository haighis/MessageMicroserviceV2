using System;
using System.Collections.Generic;
using Akka.Actor;

namespace Actors.MessageTopic.Consumer
{
    public class MessageConsumerCoordinatorActor : ReceiveActor
    {
        /// <summary>
        /// Restart any children who throw an <see cref="Exception"/> message.
        /// </summary>
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(10,10,Decider.From(Directive.Restart,
                new KeyValuePair<Type, Directive>(typeof(Exception), Directive.Restart)));
        }

        public MessageConsumerCoordinatorActor()
        {
            Receive<int>(msg =>
            {
                var todoChildActor = LookupOrCreateTodoChildActor(Guid.NewGuid().ToString());
                Console.WriteLine("IN consumer running to CoordinatorActor path " + Self.Path);
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
                return Context.ActorOf(Props.Create(() => new MessageConsumerChildActor()),
                    name);
            }
            return child;
        }
    }
}
