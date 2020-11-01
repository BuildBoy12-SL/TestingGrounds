namespace TestingGrounds.Commands.SaveState
{
    using Classes.SaveState;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Linq;
    using UnityEngine;
    
    using static TestingGrounds;
    
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Save : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender as CommandSender).CheckPermission("tg.save"))
            {
                response = "Permission denied. Required: tg.save";
                return false;
            }

            if (arguments.Count > 2)
            {
                response = "Usage: save (name) (delete / (force -> If a file with the same name should be overwritten))";
                return false;
            }
            
            string fileName = arguments.Count > 0 ? arguments.At(0) : "Save";
            
            if (Directory.GetFiles(SaveStateDirectory).Any(file => file.EndsWith(fileName)))
            {
                if (arguments.Count != 2)
                {
                    response = "A save already exists with that name!" +
                               "\nSpecify the \"force\" parameter to overwrite or \"delete\" parameter to remove.";
                    return false;
                }

                switch (arguments.At(1))
                {
                    case "d":
                    case "delete":
                        File.Delete(Path.Combine(SaveStateDirectory, fileName));
                        response = $"Save \"{fileName}\" deleted.";
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
            
            var savedPlayers = 
                Player.List.Select(player => new SavedPlayer
                {
                    Inventory = player.Inventory.items,
                    UserId = player.UserId,
                    Position = player.Position,
                    Role = player.Role,
                    Rotation = player.Rotation
                })
                .ToList();

            var savedItems = 
                (from pickup in UnityEngine.Object.FindObjectsOfType<Pickup>() let transform = pickup.transform select 
                    new SavedItem {Durability = pickup.durability, Item = pickup.itemId, Position = transform.position, Rotation = transform.rotation}).ToList();

            var savedDoors =
                (from door in Map.Doors select
                    new SavedDoor {IsDestroyed = door.Networkdestroyed, IsLocked = door.Networklocked, IsOpen = door.NetworkisOpen, Position = door.transform.position}).ToList();

            var saveState = new SaveState
            {
                Seed = GameObject.Find("Host").GetComponent<RandomSeedSync>().seed,
                SavedItems = savedItems,
                SavedPlayers = savedPlayers,
                SavedDoors = savedDoors
            };

            var serialized = JsonConvert.SerializeObject(saveState, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            
            File.WriteAllText(Path.Combine(SaveStateDirectory, fileName), serialized);
            response = $"Game saved with the name \"{fileName}\"";
            return true;
        }

        public string Command => "save";
        public string[] Aliases => new string[0];
        public string Description => "Saves the state of the current round.";
    }
}