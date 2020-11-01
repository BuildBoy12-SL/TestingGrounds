namespace TestingGrounds
{
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using MEC;
    using Mirror;
    using System.Linq;
    
    public class Methods
    {
        public static void LoadState()
        {
            if (State.SaveState == null)
                return;
            
            if (State.SaveState.SavedPlayers != null)
                foreach (var savedPlayer in State.SaveState.SavedPlayers)
                {
                    Player ply = Player.Get(savedPlayer.UserId);
                
                    if (ply == null)
                        return;

                    Timing.CallDelayed(0.2f, () =>
                    {
                        ply.Role = savedPlayer.Role;
                        ply.ResetInventory(savedPlayer.Inventory.ToList());
                        Timing.CallDelayed(0.2f, () =>
                        {
                            ply.Position = savedPlayer.Position;
                            ply.Rotation = savedPlayer.Rotation;
                        });
                    });
                }

            if (State.SaveState.SavedItems != null)
            {
                foreach (var pickup in UnityEngine.Object.FindObjectsOfType<Pickup>())
                    NetworkServer.Destroy(pickup.gameObject);

                foreach (var savedItem in State.SaveState.SavedItems)
                    savedItem.Item.Spawn(savedItem.Durability, savedItem.Position, savedItem.Rotation);
            }


            if (State.SaveState.SavedDoors != null)
                foreach (var door in Map.Doors)
                {
                    foreach (var savedDoor in State.SaveState.SavedDoors.Where(savedDoor => savedDoor.Position == door.transform.position))
                    {
                        door.Networkdestroyed = savedDoor.IsDestroyed;
                        door.Networklocked = savedDoor.IsLocked;
                        door.NetworkisOpen = savedDoor.IsOpen;
                    }
                }
            
            State.SaveState = null;
        }
    }
}