namespace TestingGrounds.Commands.SubCommands
{
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using System;

    public class Seed : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("tg.load"))
            {
                response = "Permission denied. Required: tg.load";
                return false;
            }

            if (arguments.Count == 0 || !int.TryParse(arguments.At(0), out int seed))
            {
                response = "The first argument must be an integer.";
                return false;
            }

            State.NextSeed = seed;
            Round.Restart();
            response = "Restarting round.";
            return true;
        }

        public string Command => "seed";
        public string[] Aliases => Array.Empty<string>();
        public string Description => "Sets the seed and restarts the round.";
    }
}