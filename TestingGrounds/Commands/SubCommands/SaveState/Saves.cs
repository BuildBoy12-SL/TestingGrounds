namespace TestingGrounds.Commands.SubCommands.SaveState
{
    using CommandSystem;
    using Exiled.Permissions.Extensions;
    using NorthwoodLib.Pools;
    using System;
    using System.IO;
    using System.Text;

    public class Saves : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("tg.save"))
            {
                response = "Permission denied. Required: tg.save";
                return false;
            }

            StringBuilder stringBuilder = StringBuilderPool.Shared.Rent("\n");
            DirectoryInfo directoryInfo = new DirectoryInfo(TestingGrounds.SaveStateDirectory);
            foreach (var file in directoryInfo.GetFiles())
            {
                stringBuilder.AppendLine(file.Name);
            }

            response = stringBuilder.ToString();
            StringBuilderPool.Shared.Return(stringBuilder);
            return true;
        }

        public string Command => "saves";
        public string[] Aliases => new string[0];
        public string Description => "Lists all current saves.";
    }
}