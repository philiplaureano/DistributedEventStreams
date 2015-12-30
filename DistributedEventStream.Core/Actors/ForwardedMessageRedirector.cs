using System;
using Akka.Actor;
using DistributedEventStream.Core.Messages;

namespace DistributedEventStream.Core.Actors
{
    public class ForwardedMessageRedirector<TMessage> : ReceiveActor
    {
        private readonly IActorRef _targetActor;

        public ForwardedMessageRedirector(IActorRef targetActor) : this(targetActor, null)
        {
        }
        public ForwardedMessageRedirector(IActorRef targetActor, Func<Forward<TMessage>, bool> messageFilter)
        {
            _targetActor = targetActor;
            Receive<Forward<TMessage>>(message =>
            {
                if (messageFilter != null && !messageFilter.Invoke(message))
                    return;

                _targetActor.Tell(message.Message);
            });
        }
    }
}