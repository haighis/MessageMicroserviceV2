using System;
using System.Collections.Generic;
using Akka.Actor;
using DataModel;

namespace Actors.BusinessLogic
{
    public class MessageCoordinatorActor : ReceiveActor
    {
        /// <summary>
        /// Restart any children who throw an <see cref="Exception"/> message.
        /// </summary>
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(50,10,Decider.From(Directive.Restart,
                new KeyValuePair<Type, Directive>(typeof(Exception), Directive.Restart)));
        }

        public MessageCoordinatorActor()
        {
            Receive<Message>(msg =>
            {
                var todoChildActor = LookupOrCreateMessageChildActor(Guid.NewGuid().ToString());
                Console.WriteLine("CoordinatorActor path " + Self.Path);
                todoChildActor.Forward(msg);
            });
        }

        private IActorRef LookupOrCreateMessageChildActor(string name)
        {
            return LookupOrCreateMessageChildActor(Context, name);
        }

        private IActorRef LookupOrCreateMessageChildActor(IActorContext context, string name)
        {
            var child = context.Child(name);
            if (child.Equals(ActorRefs.Nobody)) //child doesn't exist
            {
                return Context.ActorOf(Props.Create(() => new MessageChildActor()),
                    name);
            }
            return child;
        }
    }
}
