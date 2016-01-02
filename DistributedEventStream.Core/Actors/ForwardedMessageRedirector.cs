using System;
using Akka.Actor;
using DistributedEventStream.Core.Messages;

namespace DistributedEventStream.Core.Actors
{
    public class ForwardedMessageRedirector : ReceiveActor
    {
        private readonly IActorRef _targetActor;

        public ForwardedMessageRedirector(IActorRef targetActor) : this(targetActor, null)
        {
        }
        public ForwardedMessageRedirector(IActorRef targetActor, Func<IForwardMessage, bool> messageFilter)
        {
            _targetActor = targetActor;
            Receive<IForwardMessage>(message =>
            {
                if (messageFilter != null && !messageFilter.Invoke(message))
                    return;

                _targetActor.Tell(message.Message);
            });
        }
    }
}