using System;
using Akka.Actor;

namespace DistributedEventStream.Core.Actors
{
    public static class ActorSelectionExtensions
    {
        public static bool CanBeResolved(this ActorSelection actorSelection, TimeSpan timeoutPeriod)
        {
            IActorRef targetActor = null;
            try
            { 
                targetActor = actorSelection.ResolveOne(timeoutPeriod).Result;
            }
            catch (AggregateException)
            {                
                // Ignore the error
            }

            return targetActor != null;
        }
    }
}