namespace DistributedEventStream.Core.Messages
{
    public class ActorAssociation : IActorAssociation
    {
        public ActorAssociation(string actorAddress)
        {
            ActorAddress = actorAddress;
        }

        public string ActorAddress { get; }
    }
}