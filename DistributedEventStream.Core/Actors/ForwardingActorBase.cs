using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using DistributedEventStream.Core.Messages;

namespace DistributedEventStream.Core.Actors
{
    public abstract class ForwardingActorBase : ReceiveActor, IWithUnboundedStash
    {
        protected ForwardingActorBase()
        {
            Receive<IForwardMessage>(message =>
            {
                // Determine the list of remote actor systems and target actors
                var actors = GetForwardingActors(message).ToArray();
                var logger = Context.GetLogger();
                
                if (actors.Length == 0)
                {
                    logger.Warning("Unable to forward messages -- no actors found.");
                    return;
                }

                logger.Debug($"Forwarding messages to {actors.Length} actors");

                // Fire and forget
                foreach (var actor in actors)
                {
                    actor?.Tell(message, Self);
                }
            });
        }

        protected abstract IEnumerable<IActorRef> GetForwardingActors(IForwardMessage currentMessage);
        public IStash Stash { get; set; }
    }
}