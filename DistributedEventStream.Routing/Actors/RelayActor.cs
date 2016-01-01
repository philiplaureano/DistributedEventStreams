using System;
using System.Collections.Generic;
using Akka.Actor;
using DistributedEventStream.Core.Messages;
using DistributedEventStream.Routing.Messages;

namespace DistributedEventStream.Routing.Actors
{
    public class RelayActor : ReceiveActor
    {
        private readonly Dictionary<string, IList<string>>
            _addresses = new Dictionary<string, IList<string>>();

        public RelayActor() : this((context, message, address) => 
            context.ActorSelection(address).Tell(message))
        {
        }

        public RelayActor(Action<IUntypedActorContext, IForwardMessage, string> forwardMessageAction)
        {
            var forwardMessage = forwardMessageAction;
            Receive<IForwardMessage>(message =>
            {
                var channel = message.Channel;
                if (!_addresses.ContainsKey(channel))
                    return;

                var targetAddresses = _addresses[channel];
                foreach (var address in targetAddresses)
                {
                    forwardMessage(Context, message, address);
                }
            });

            Receive<AssociatedWithChannel>(message =>
            {
                var channel = message.ChannelName;
                if (!_addresses.ContainsKey(channel))
                    _addresses[channel] = new List<string>();

                if (!_addresses[channel].Contains(message.ActorAddress))
                    _addresses[channel].Add(message.ActorAddress);
            });

            Receive<DisassociatedFromChannel>(message =>
            {
                var channel = message.ChannelName;
                if (!_addresses.ContainsKey(channel))
                    return;

                if (_addresses[channel].Contains(message.ActorAddress) && _addresses[channel].Contains(message.ActorAddress))
                    _addresses[channel].Remove(message.ActorAddress);
            });
        }
    }
}