namespace TestingGrounds.Commands.SubCommands.SaveState
{
    using CommandSystem;
    using Exiled.Permissions.Extensions;
    using System;
    using System.IO;
    using System.Text;
    
    using static TestingGrounds;
    
    public class Saves : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender as CommandSender).CheckPermission("tg.save"))
            {
                response = "Permission denied. Required: tg.save";
                return false;
            }

            StringBuilder stringBuilder = new StringBuilder("\n");

            DirectoryInfo directoryInfo = new DirectoryInfo(SaveStateDirectory);
            foreach (var file in directoryInfo.GetFiles())
            {
                stringBuilder.AppendLine(file.Name);
            }

            response = stringBuilder.ToString();
            return true;
        }

        public string Command => "saves";
        public string[] Aliases => new string[0];
        public string Description => "Lists all current saves.";
    }
}