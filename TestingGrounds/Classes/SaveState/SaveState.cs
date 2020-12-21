namespace TestingGrounds.Classes.SaveState
{
    using System.Collections.Generic;

    public class SaveState
    {
        public int Seed { get; set; }
        public List<SavedPlayer> SavedPlayers { get; set; }
        public List<SavedItem> SavedItems { get; set; }
        public List<SavedDoor> SavedDoors { get; set; }
    }
}