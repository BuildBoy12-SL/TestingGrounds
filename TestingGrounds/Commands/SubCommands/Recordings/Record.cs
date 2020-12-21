namespace TestingGrounds.Commands.SubCommands.Recordings
{
    using CommandSystem;
    using Exiled.Permissions.Extensions;
    using MEC;
    using System;
    using System.IO;

    public class Record : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("tg.save"))
            {
                response = "Permission denied. Required: tg.save";
                return false;
            }

            if (arguments.Count < 2)
            {
                response =
                    "Usage: record <name> <seconds> (delete / (force -> If a directory with the same name should be overwritten))";
                return false;
            }

            string directory = arguments.At(0);
            string fullDirectory = Path.Combine(TestingGrounds.SaveStateDirectory, directory);
            if (Directory.Exists(fullDirectory))
            {
                if (arguments.Count != 3)
                {
                    response = "A save already exists with that name!" +
                               "\nSpecify the \"force\" parameter to overwrite or \"delete\" parameter to remove.";
                    return false;
                }

                switch (arguments.At(2))
                {
                    case "d":
                    case "delete":
                        Directory.Delete(fullDirectory);
                        response = $"Recording under \"{directory}\" have been deleted.";
                        return false;
                    case "f":
                    case "force":
                        sender.Respond("SAVE#Overwriting..");
                        break;
                    default:
                        response = "A save already exists with that name and an incorrect parameter was specified." +
                                   "\nSpecify the \"force\" parameter to overwrite or \"delete\" parameter to remove.";
                        return false;
                }
            }

            if (!int.TryParse(arguments.At(1), out int seconds))
            {
                response = "Please enter in a valid number as an argument.";
                return false;
            }

            Methods.CoroutineHandles.Add(Timing.RunCoroutine(Methods.Record(directory, seconds)));
            response = $"The next {seconds} seconds will now be recorded.";
            return true;
        }

        public string Command => "record";
        public string[] Aliases => Array.Empty<string>();
        public string Description => "Records a given amount of seconds.";
    }
}