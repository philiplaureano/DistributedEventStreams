namespace DistributedEventStream.Routing.Messages
{
    public class DisassociateActorFromChannel
    {
        public DisassociateActorFromChannel(string actorAddress, string channelName)
        {
            ActorAddress = actorAddress;
            ChannelName = channelName;
        }

        public string ActorAddress { get; }
        public string ChannelName { get; }
    }
}