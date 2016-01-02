using Akka.Actor;

namespace DistributedEventStream.Core.Messages
{
    public static class MessageExtensions
    {
        public static void TellFrom<TMessage>(this TMessage message, ActorSystem sourceActorSystem, string targetActorAddress)
        {            
            sourceActorSystem?.ActorSelection(targetActorAddress)?
                .Tell(message);
        }
    }
}