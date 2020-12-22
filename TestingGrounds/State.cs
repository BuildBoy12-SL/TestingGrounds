namespace TestingGrounds
{
    using Classes.SaveState;
    using Exiled.API.Features;
    using System.Collections.Generic;
    using UnityEngine;

    public static class State
    {
        internal static SaveState SaveState = null;
        internal static int CurrentSeed = 0;
        internal static int NextSeed = 0;
        internal static readonly Dictionary<Player, ItemType> AlteredGuns = new();
        internal static readonly Dictionary<Player, Vector3> Ruler = new();
    }
}