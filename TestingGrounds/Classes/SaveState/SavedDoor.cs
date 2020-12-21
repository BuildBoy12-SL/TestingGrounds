namespace TestingGrounds.Classes.SaveState
{
    using UnityEngine;

    public class SavedDoor
    {
        public bool IsDestroyed { get; set; }
        public bool IsLocked { get; set; }
        public bool IsOpen { get; set; }
        public Vector3 Position { get; set; }
    }
}