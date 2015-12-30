using System;
using Akka.Actor;

namespace SampleClientNode
{
    public class GreetActor : ReceiveActor
    {
        public GreetActor()
        {           
            Receive<GreetMessage>(message =>
            {
                Console.WriteLine(message.Message);
            });
        }
    }
}