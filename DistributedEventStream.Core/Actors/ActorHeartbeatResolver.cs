using System;
using Akka.Actor;
using DistributedEventStream.Core.Messages;

namespace DistributedEventStream.Core.Actors
{
    public class ActorHeartbeatResolver : ReceiveActor
    {
        private string _actorAddress;
        private readonly IActorRef _parentActor;
        private readonly TimeSpan _refreshRate;
        private readonly TimeSpan _timeoutPeriod;

        public ActorHeartbeatResolver(string actorAddress, IActorRef parentActor, TimeSpan refreshRate, TimeSpan timeoutPeriod)
        {
            _actorAddress = actorAddress;
            _parentActor = parentActor;
            _refreshRate = refreshRate;
            _timeoutPeriod = timeoutPeriod;

            Receive<ActorHeartbeatMessage>(message =>
            {
                if (message.ActorAddress != actorAddress)
                    return;

                if (Context.ActorSelection(_actorAddress).CanResolve(_timeoutPeriod))
                    return;

                _parentActor.Tell(new ActorDissasociation(_actorAddress));
                Self.Tell(PoisonPill.Instance);
            });
        }

        protected override void PreStart()
        {
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(1), _refreshRate, Self, new ActorHeartbeatMessage(_actorAddress),Self);           
        }
    }
}