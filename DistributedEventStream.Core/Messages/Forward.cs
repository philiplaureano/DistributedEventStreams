using System;
using Akka.Actor;

namespace DistributedEventStream.Core.Messages
{
    public class Forward<TMessage> : IForwardMessage
    {
        public Forward(TMessage message, string channel, IActorRef sender, Type messageType)
        {
            Message = message;
            Channel = channel;
            Sender = sender;
            MessageType = messageType;
        }

        public TMessage Message { get; }
        object IForwardMessage.Message => Message;

        public string Channel { get; }
        public IActorRef Sender { get; }
        public Type MessageType { get; }
    }
}
