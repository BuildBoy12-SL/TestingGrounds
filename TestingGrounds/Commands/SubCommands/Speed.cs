namespace TestingGrounds.Commands.SubCommands
{
    using CommandSystem;
    using Exiled.Permissions.Extensions;
    using System;
    
    public class Speed : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender as CommandSender).CheckPermission("tg.speed"))
            {
                response = "Permission denied. Required: tg.speed";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Please enter a multiplier for speed.";
                return false;
            }
            
            if (!float.TryParse(arguments.At(0), out var multiplier))
            {
                response = "Please enter a multiplier for speed.";
                return false;
            }
            
            ServerConfigSynchronizer.Singleton.NetworkHumanWalkSpeedMultiplier = multiplier;
            response = "Multiplier changed successfully.";
            return true;
        }

        public string Command => "speed";
        public string[] Aliases => new string[0];
        public string Description => "Change the default walk speed.";
    }
}