using System;
using CommandSystem;
using Exiled.API.Features;
using MEC;
using RemoteAdmin;

namespace TestingGrounds.Commands.SubCommands.Ruler
{
    public class EndRuler : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender is PlayerCommandSender player))
            {
                response = "This command can only be executed from the game level.";
                return false;
            }
            
            var ply = Player.Get(player.SenderId);
            if (!State.Ruler.ContainsKey(ply))
            {
                response = "You are not currently using the ruler!";
                return false;
            }

            State.Ruler.Remove(ply);
            Timing.KillCoroutines(ply.UserId);
            response = "Ruler ended.";
            return false;
        }

        public string Command => "endruler";
        public string[] Aliases => new[] {"er"};
        public string Description => "Ends the ruler command.";
    }
}