using System;
using Akka.Actor;
using DistributedEventStream.Core.Messages;

namespace DistributedEventStream.Core.Actors
{
    public class EventForwarder<TMessage> : ReceiveActor
    {
        private readonly IActorRef _forwardingActor;

        public EventForwarder(IActorRef forwardingActor,
            Func<TMessage, Forward<TMessage>> eventPackager,
            Func<TMessage, bool> eventFilter = null)
        {
            _forwardingActor = forwardingActor;
            Receive<TMessage>(message =>
            {
                if (eventFilter != null && !eventFilter(message))
                    return;

                var packagedMessage = eventPackager(message);
                _forwardingActor.Tell(packagedMessage);
            });
        }
    }
}