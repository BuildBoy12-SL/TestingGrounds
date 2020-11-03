namespace TestingGrounds
{
    using Classes.SaveState;
    using Exiled.API.Features;
    using System.Collections.Generic;
    
    public static class State
    {
        // internal static float SpeedMultiplier;
        internal static SaveState SaveState = null;
        internal static readonly Dictionary<Player, ItemType> AlteredGuns = new Dictionary<Player, ItemType>();
    }
}