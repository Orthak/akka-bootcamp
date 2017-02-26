using System;
using Akka.Actor;

namespace WinTail
{
    /// <summary>
    /// Actor responsible for reading FROM the console. 
    /// Also responsible for calling <see cref="ActorSystem.Terminate"/>.
    /// </summary>
    class ConsoleReaderActor : UntypedActor
    {
        public const string StartCommand = "start";
        public const string ExitCommand = "exit";

        IActorRef _consoleWriterActor;

        public ConsoleReaderActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
            if (message.Equals(StartCommand))
                DoPrintInstructions();
            else if(message is Messages.InputError)
                _consoleWriterActor.Tell(message as Messages.InputError);

            GetAndValidateInput();
        }

        void DoPrintInstructions()
        {
            Console.WriteLine("Write whatever you want into the console!");
            Console.WriteLine("Some entries will pass validation, and some won't...");
            Console.WriteLine("Type \"exit\" to quit this application at any time.");
        }

        void GetAndValidateInput()
        {
            var message = Console.ReadLine();
            if(string.IsNullOrEmpty(message))
                // Signal that the user needs to supply an input, as the received
                // input was empty.
                Self.Tell(new Messages.NullInputError("No input received."));

            else if (string.Equals(message, ExitCommand, StringComparison.OrdinalIgnoreCase))
                // Shut down the actor system.
                Context.System.Terminate();
            else
            {
                var valid = IsValid(message);
                if (valid)
                {
                    _consoleWriterActor.Tell(new Messages.InputSuccess("Thank you! Message was valid."));

                    // Continue reading.
                    Self.Tell(new Messages.ContinueProcessing());
                }
                else
                    Self.Tell(new Messages.ValidationError("Invalid: Input had odd number of characters."));
                
            }
        }

        static bool IsValid(string message)
        {
            return message.Length % 2 == 0;
        }
    }
}