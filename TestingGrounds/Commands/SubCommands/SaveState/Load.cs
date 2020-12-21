namespace TestingGrounds.Commands.SubCommands.SaveState
{
    using Classes.SaveState;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using Newtonsoft.Json;
    using System;
    using System.IO;
    
    public class Load : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("tg.load"))
            {
                response = "Permission denied. Required: tg.load";
                return false;
            }

            if (arguments.Count != 1 && arguments.Count != 2)
            {
                response = "Usage: load <save name> (force -> if round should restart to load save)";
                return false;
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(TestingGrounds.SaveStateDirectory);
            foreach (var file in directoryInfo.GetFiles())
            {
                if (file.Name != arguments.At(0))
                    continue;

                string saveFile = file.FullName;
                string lines = File.ReadAllText(saveFile);
                SaveState saveState = JsonConvert.DeserializeObject<SaveState>(lines);
                if (arguments.Count == 2)
                {
                    if (arguments.At(1) == "f" || arguments.At(1) == "force")
                    {
                        State.SaveState = saveState;
                        Round.Restart();
                        response = "Loading save in new round.";
                        return true;
                    }
                }

                Methods.LoadState(saveState);
                response = "Save loaded successfully.";
                return true;
            }

            response = "No save found with that name.";
            return false;
        }

        public string Command => "load";
        public string[] Aliases => new string[0];
        public string Description => "Loads a saved game.";
    }
}