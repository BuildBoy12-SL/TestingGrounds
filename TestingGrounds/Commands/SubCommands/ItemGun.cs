namespace TestingGrounds.Commands.SubCommands
{
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using RemoteAdmin;
    using System;

    public class ItemGun : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender as CommandSender).CheckPermission("tg.itemgun"))
            {
                response = "Permission denied. Required: tg.itemgun";
                return false;
            }

            if (arguments.Count != 1 && arguments.Count != 2)
            {
                response = "Syntax: tg ig <ItemType/remove> (Player)";
                return false;
            }

            if (!Enum.TryParse(arguments.At(0), out ItemType item) && arguments.At(0).ToLower() != "remove")
            {
                response = "Please enter a valid ItemType.";
                return false;
            }

            Player ply;
            if (arguments.Count != 2)
            {
                if (sender is PlayerCommandSender player)
                    ply = Player.Get(player.SenderId);
                else
                {
                    response = "This command cannot be executed from the console without specifying a player.";
                    return false;
                }
            }
            else
                ply = Player.Get(arguments.At(1));

            if (ply == null)
            {
                response = "Player not found.";
                return false;
            }

            if (arguments.At(0).ToLower() == "remove")
            {
                State.AlteredGuns.Remove(ply);
                response = $"Player {ply.Nickname} removed from ItemGun.";
                return true;
            }
            
            if (State.AlteredGuns.ContainsKey(ply))
                State.AlteredGuns[ply] = item;
            else
                State.AlteredGuns.Add(ply, item);
            
            response = $"Guns for player {ply.Nickname} have been set to shoot {item}.";
            return true;
        }

        public string Command => "itemgun";
        public string[] Aliases => new[] {"ig"};
        public string Description => "Sets a users gun to fire items rather than bullets.";
    }
}