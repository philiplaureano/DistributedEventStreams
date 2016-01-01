namespace DistributedEventStream.Routing.Messages
{
    public class DisassociatedFromChannel
    {
        public DisassociatedFromChannel(string actorAddress, string channelName)
        {
            ActorAddress = actorAddress;
            ChannelName = channelName;
        }

        public string ActorAddress { get; }
        public string ChannelName { get; }
    }
}