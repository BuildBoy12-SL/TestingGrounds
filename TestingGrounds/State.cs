namespace TestingGrounds
{
    using Classes.SaveState;
    using Exiled.API.Features;
    using System.Collections.Generic;
    using UnityEngine;
    
    public static class State
    {
        internal static SaveState SaveState = null;
        internal static readonly Dictionary<Player, ItemType> AlteredGuns = new Dictionary<Player, ItemType>();
        internal static readonly Dictionary<Player, Vector3> Ruler = new Dictionary<Player, Vector3>();
    }
}