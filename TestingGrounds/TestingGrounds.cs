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
        
        private static readonly PlayerHandlers PlayerHandlers = new PlayerHandlers();
        private static readonly ServerHandlers ServerHandlers = new ServerHandlers();

        public override void OnEnabled()
        {
            if (!Directory.Exists(FileDirectory))
                Directory.CreateDirectory(FileDirectory);
            if (!Directory.Exists(SaveStateDirectory))
                Directory.CreateDirectory(SaveStateDirectory);
            
            PlayerEvents.Shooting += PlayerHandlers.OnShoot;
            ServerEvents.RoundStarted += ServerHandlers.OnRoundStart;
            Harmony.PatchAll();
            
            // State.SpeedMultiplier = GameCore.ConfigFile.ServerConfig.GetFloat("stamina_balance_walk_speed", 1.2f);
        }

        public override void OnDisabled()
        {
            PlayerEvents.Shooting -= PlayerHandlers.OnShoot;
            ServerEvents.RoundStarted -= ServerHandlers.OnRoundStart;
            Harmony.UnpatchAll(nameof(TestingGrounds).ToLowerInvariant());
        }

        public override string Name => "TestingGrounds";
        public override string Author => "Build";
    }
}