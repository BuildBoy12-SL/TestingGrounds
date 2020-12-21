namespace TestingGrounds.Handlers
{
    using Exiled.Events.EventArgs;

    public class ServerHandlers
    {
        public void OnRoundStart()
        {
            Methods.LoadState(State.SaveState);
        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            Methods.KillAllCoroutines();
        }
    }
}