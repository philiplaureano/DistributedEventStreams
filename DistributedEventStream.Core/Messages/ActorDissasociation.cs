namespace DistributedEventStream.Core.Messages
{
    public class ActorDissasociation : IActorDissasociation
    {
        public ActorDissasociation(string actorAddress)
        {
            ActorAddress = actorAddress;
        }

        public string ActorAddress { get; }
    }
}