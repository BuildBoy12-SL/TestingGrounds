namespace TestingGrounds.Commands
{
    using CommandSystem;
    using Exiled.Permissions.Extensions;
    using System;
    
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Speed : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                if (!(sender as CommandSender).CheckPermission("tg.speed"))
                {
                    response = "Permission denied. Required: tg.speed";
                    return false;
                }

                if (float.TryParse(arguments.At(0), out var multiplier))
                {
                    ServerConfigSynchronizer.Singleton.NetworkHumanWalkSpeedMultiplier = multiplier;
                    response = "Multiplier changed successfully.";
                    return true;
                }

                response = "Enter a valid num as an argument.";
                return false;
            }
            catch (IndexOutOfRangeException)
            {
                response = "Enter an argument for the speed modifier.";
                return false;
            }
        }

        public string Command => "speed";
        public string[] Aliases => new string[0];
        public string Description => "Change the default walk speed.";
    }
}