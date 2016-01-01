namespace DistributedEventStream.Routing.Messages
{
    public class AssociateActorWithChannel
    {
        public AssociateActorWithChannel(string actorAddress, string channelName)
        {
            ActorAddress = actorAddress;
            ChannelName = channelName;
        }

        public string ActorAddress { get; }
        public string ChannelName { get; }         
    }
}