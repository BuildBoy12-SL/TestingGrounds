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
        internal static readonly string RecordingsDirectory = Path.Combine(SaveStateDirectory, "Recordings");

        private static readonly Harmony Harmony = new(nameof(TestingGrounds).ToLowerInvariant());

        private static PlayerHandlers _playerHandlers;
        private static ServerHandlers _serverHandlers;
        internal static TestingGrounds Singleton;

        public override void OnEnabled()
        {
            if (!Directory.Exists(FileDirectory)) Directory.CreateDirectory(FileDirectory);
            if (!Directory.Exists(SaveStateDirectory)) Directory.CreateDirectory(SaveStateDirectory);
            if (!Directory.Exists(RecordingsDirectory)) Directory.CreateDirectory(RecordingsDirectory);

            _playerHandlers = new PlayerHandlers(this);
            _serverHandlers = new ServerHandlers();
            Singleton = this;

            PlayerEvents.Shooting += _playerHandlers.OnShoot;
            ServerEvents.RoundEnded += _serverHandlers.OnRoundEnded;
            ServerEvents.RoundStarted += _serverHandlers.OnRoundStart;

            Harmony.PatchAll();
        }

        public override void OnDisabled()
        {
            PlayerEvents.Shooting -= _playerHandlers.OnShoot;
            ServerEvents.RoundEnded -= _serverHandlers.OnRoundEnded;
            ServerEvents.RoundStarted -= _serverHandlers.OnRoundStart;

            _playerHandlers = null;
            _serverHandlers = null;
            Singleton = null;

            Methods.KillAllCoroutines();
            Harmony.UnpatchAll(Harmony.Id);
        }

        public override string Name => "TestingGrounds";
        public override string Author => "Build";
    }
}