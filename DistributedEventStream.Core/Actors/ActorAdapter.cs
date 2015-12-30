using Akka.Actor;
using Akka.Event;
using DistributedEventStream.Core.Messages;

namespace DistributedEventStream.Core.Actors
{
    public class ActorAdapter : ReceiveActor
    {
        private readonly string _targetAddress;

        public ActorAdapter(string targetAddress)
        {
            _targetAddress = targetAddress;

            Receive<IForwardMessage>(message =>
            {
                var logger = Context.GetLogger();
                logger.Debug($"Forwarding message to address '{_targetAddress}'");
                Context.ActorSelection(_targetAddress).Tell(message);
            });
        }
    }
}