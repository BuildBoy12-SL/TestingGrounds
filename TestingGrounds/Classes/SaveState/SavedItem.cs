namespace TestingGrounds.Classes.SaveState
{
    using UnityEngine;

    public class SavedItem
    {
        public float Durability { get; set; }
        public ItemType Item { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
    }
}