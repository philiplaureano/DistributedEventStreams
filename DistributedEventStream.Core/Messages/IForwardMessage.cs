using System;
using Akka.Actor;

namespace DistributedEventStream.Core.Messages
{
    public interface IForwardMessage
    {
        object Message { get; }
        string Channel { get; }
        string OriginAddress { get; }
        Type MessageType { get; }
    }
}