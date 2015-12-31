using System;
using Akka.Actor;

namespace DistributedEventStream.Core.Messages
{
    public class Forward<TMessage> : IForwardMessage
    {
        public Forward(TMessage message, string channel, string originAddress, Type messageType)
        {
            Message = message;
            Channel = channel;
            OriginAddress = originAddress;
            MessageType = messageType;
        }

        public TMessage Message { get; }
        object IForwardMessage.Message => Message;

        public string Channel { get; }
        public string OriginAddress { get; }
        public Type MessageType { get; }
    }
}
