namespace TestingGrounds.Commands.SubCommands.Ruler
{
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using MEC;
    using RemoteAdmin;
    using System;
    using UnityEngine;

    public class Ruler : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender is PlayerCommandSender player))
            {
                response = "This command can only be executed from the game level.";
                return false;
            }

            var ply = Player.Get(player.SenderId);
            if (!ply.CheckPermission("tg.ruler"))
            {
                response = "Permission denied. Required: tg.ruler";
                return false;
            }

            if (!Physics.Raycast(ply.CameraTransform.position, ply.CameraTransform.forward, out RaycastHit hit))
            {
                response = "Please look at a valid point to start the ruler!";
                return false;
            }

            if (!State.Ruler.ContainsKey(ply))
            {
                State.Ruler.Add(ply, hit.point);
                Methods.CoroutineHandles.Add(Timing.RunCoroutine(Methods.DoDistance(ply), $"{ply.UserId}"));
            }
            else
                State.Ruler[ply] = hit.point;

            response = "Use the command 'endruler' to end the calculations.";
            return true;
        }

        public string Command => "ruler";
        public string[] Aliases => new[] {"measure", "length"};
        public string Description => "Measures the distance between two points.";
    }
}