namespace TestingGrounds.Classes.SaveState
{
    using UnityEngine;

    public class SavedPlayer
    {
        public Inventory.SyncListItemInfo Inventory { get; set; }
        public string UserId { get; set; }
        public RoleType Role { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
    }
}