namespace DistributedEventStream.Core.Messages
{
    public class ActorHeartbeatMessage
    {
        public ActorHeartbeatMessage(string actorAddress)
        {
            ActorAddress = actorAddress;
        }

        public string ActorAddress { get; }
    }
}