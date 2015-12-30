using System;
using Akka.Actor;

namespace DistributedEventStream.Core.Messages
{
    public interface IForwardMessage
    {
        object Message { get; }
        string Channel { get; }
        IActorRef Sender { get; }
        Type MessageType { get; }
    }
}