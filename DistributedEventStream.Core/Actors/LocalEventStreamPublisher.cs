using Akka.Actor;
using DistributedEventStream.Core.Messages;

namespace DistributedEventStream.Core.Actors
{
    public class LocalEventStreamPublisher : ReceiveActor
    {
        public LocalEventStreamPublisher()
        {
            Receive<IForwardMessage>(message =>
            {
                Context?.System.EventStream?.Publish(message);
            });
        }
    }
}