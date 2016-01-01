namespace DistributedEventStream.Routing.Messages
{
    public class AssociatedWithChannel
    {
        public AssociatedWithChannel(string actorAddress, string channelName)
        {
            ActorAddress = actorAddress;
            ChannelName = channelName;
        }

        public string ActorAddress { get; }
        public string ChannelName { get; }
    }
}