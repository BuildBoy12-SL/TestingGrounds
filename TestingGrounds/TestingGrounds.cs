namespace TestingGrounds
{
    using Exiled.API.Features;
    using Handlers;
    using HarmonyLib;
    using System.IO;
    using PlayerEvents = Exiled.Events.Handlers.Player;
    using ServerEvents = Exiled.Events.Handlers.Server;

    public class TestingGrounds : Plugin<Config>
    {
        private static readonly string FileDirectory = Path.Combine(Paths.Configs, "TestingGrounds");
        internal static readonly string SaveStateDirectory = Path.Combine(FileDirectory, "Saves");

        private static readonly Harmony Harmony = new Harmony(nameof(TestingGrounds).ToLowerInvariant());
        
        private static PlayerHandlers _playerHandlers;
        private static ServerHandlers _serverHandlers;
        internal static TestingGrounds Instance;

        public override void OnEnabled()
        {
            if (!Directory.Exists(FileDirectory)) Directory.CreateDirectory(FileDirectory);
            if (!Directory.Exists(SaveStateDirectory)) Directory.CreateDirectory(SaveStateDirectory);
            
            _playerHandlers = new PlayerHandlers();
            _serverHandlers = new ServerHandlers();
            
            PlayerEvents.Shooting += _playerHandlers.OnShoot;
            ServerEvents.RoundStarted += _serverHandlers.OnRoundStart;
            
            Instance = this;
            Harmony.PatchAll();
        }

        public override void OnDisabled()
        {
            PlayerEvents.Shooting -= _playerHandlers.OnShoot;
            ServerEvents.RoundStarted -= _serverHandlers.OnRoundStart;
            
            _playerHandlers = null;
            _serverHandlers = null;
            
            Instance = null;
            Harmony.UnpatchAll(nameof(TestingGrounds).ToLowerInvariant());
        }

        public override string Name => "TestingGrounds";
        public override string Author => "Build";
    }
}