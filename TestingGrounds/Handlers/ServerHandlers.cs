namespace TestingGrounds.Handlers
{
    using Exiled.Events.EventArgs;
    using UnityEngine;

    public class ServerHandlers
    {
        public void OnRoundStart()
        {
            Methods.LoadState(State.SaveState);
            State.CurrentSeed = GameObject.Find("Host").GetComponent<RandomSeedSync>().seed;
            State.NextSeed = 0;
        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            Methods.KillAllCoroutines();
        }
    }
}