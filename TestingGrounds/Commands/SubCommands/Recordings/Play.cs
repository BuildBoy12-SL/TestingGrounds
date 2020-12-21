namespace TestingGrounds.Commands.SubCommands.Recordings
{
    using CommandSystem;
    using Exiled.Permissions.Extensions;
    using MEC;
    using System;
    using System.IO;

    public class Play : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("tg.load"))
            {
                response = "Permission denied. Required: tg.load";
                return false;
            }

            string directory = "Recordings";
            if (arguments.Count >= 1)
                directory = arguments.At(0);

            string fullDirectory = Path.Combine(TestingGrounds.RecordingsDirectory, directory);
            if (!File.Exists(fullDirectory))
            {
                response = "The specified file does not exist.";
                return false;
            }

            Methods.CoroutineHandles.Add(Timing.RunCoroutine(Methods.PlayRecording(fullDirectory)));
            response = "Playing..";
            return true;
        }

        public string Command => "play";
        public string[] Aliases => Array.Empty<string>();
        public string Description => "Plays saved recordings.";
    }
}