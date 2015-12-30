using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using DistributedEventStream.Core.Messages;

namespace DistributedEventStream.Core.Actors
{
    public abstract class ForwardingActorBase : ReceiveActor
    {
        protected ForwardingActorBase()
        {
            Receive<IForwardMessage>(message =>
            {
                // Determine the list of remote actor systems and target actors
                var actors = GetForwardingActors(message).ToArray();
                var logger = Context.GetLogger();
                logger.Warning($"Forwarding messages to {actors.Length} actors");
                // Fire and forget
                foreach (var actor in actors)
                {
                    actor?.Tell(message, Self);
                }
            });
        }

        protected abstract IEnumerable<IActorRef> GetForwardingActors(IForwardMessage currentMessage);
    }
}