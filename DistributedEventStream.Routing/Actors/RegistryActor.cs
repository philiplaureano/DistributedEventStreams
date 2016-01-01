using System.Collections.Generic;
using Akka.Actor;
using DistributedEventStream.Routing.Messages;

namespace DistributedEventStream.Routing.Actors
{
    public class RegistryActor : ReceiveActor
    {
        private readonly Dictionary<string, IList<string>>
            _addresses = new Dictionary<string, IList<string>>();

        public RegistryActor()
        {
            Receive<AssociateActorWithChannel>(message =>
            {
                var channelName = message.ChannelName;
                if (!_addresses.ContainsKey(channelName))
                    _addresses[channelName] = new List<string>();

                var actorAddress = message.ActorAddress;
                var currentAddresses = _addresses[channelName];

                if (!currentAddresses.Contains(actorAddress))
                {
                    currentAddresses.Add(actorAddress);
                    var result = new AssociatedWithChannel(actorAddress, channelName);
                    Sender.Tell(result, Self);

                    // Publish the result
                    Context?.System?.EventStream?.Publish(result);
                }
            });

            Receive<DisassociateActorFromChannel>(message =>
            {
                var channelName = message.ChannelName;
                var address = message.ActorAddress;
                if (!_addresses.ContainsKey(channelName) || !_addresses[channelName].Contains(address))
                    return;

                _addresses[channelName].Remove(address);
                var result = new DisassociatedFromChannel(address, channelName);

                Sender.Tell(result, Self);

                // Publish the result
                Context?.System?.EventStream?.Publish(result);
            });           
        }
    }
}