﻿using Akka.Actor;

namespace WinTail
{
    class Program
    {
        public static ActorSystem MyActorSystem;

        static void Main(string[] args)
        {
            // Initialize MyActorSystem.
            MyActorSystem = ActorSystem.Create("MyActorSystem");
            
            // Time to make your first actors!
            var consoleWritorActor = 
                MyActorSystem.ActorOf(Props.Create(() => new ConsoleWriterActor()));
            var consoleReaderActor =
                MyActorSystem.ActorOf(Props.Create(() => new ConsoleReaderActor(consoleWritorActor)));
            
            // Tell console reader to begin
            consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

            // Blocks the main thread from exiting until the actor system is shut down.
            MyActorSystem.WhenTerminated.Wait();
        }
    }
}