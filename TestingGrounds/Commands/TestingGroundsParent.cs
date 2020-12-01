namespace TestingGrounds.Commands
{
    using CommandSystem;
    using SubCommands;
    using SubCommands.Ruler;
    using SubCommands.SaveState;
    using System;

    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class TestingGroundsParent : ParentCommand
    {
        public TestingGroundsParent()
        {
            LoadGeneratedCommands();
        }
        
        public sealed override void LoadGeneratedCommands()
        {
            RegisterCommand(new ItemGun());
            RegisterCommand(new Load());
            RegisterCommand(new Save());
            RegisterCommand(new Saves());
            RegisterCommand(new Speed());
            RegisterCommand(new Ruler());
            RegisterCommand(new EndRuler());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Please enter a valid subcommand!\nAvailable: [load, save, saves, speed, itemgun]";
            return false;
        }
        
        public override string Command => "testinggrounds";
        public override string[] Aliases => new[] {"tgr"};
        public override string Description => "Parent for all TestingGrounds commands to isolate from other commands.";
    }
}