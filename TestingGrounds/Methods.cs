namespace TestingGrounds
{
    using Exiled.API.Extensions;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Grenades;
    using MEC;
    using Mirror;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    
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

        // Nabbed from RogerFK ThrowItemsSL [https://github.com/RogerFK/ThrowItemsSL/blob/master/ThrowItems.cs#L62]
        public static IEnumerator<float> ThrowWhenRigidBody(Rigidbody rigidbody, Vector3 dir)
        {
            yield return Timing.WaitUntilFalse(() => rigidbody == null);
            rigidbody.transform.Translate(new Vector3(0, 0.5f, 0), Space.Self);
            rigidbody.AddForce(dir * 17, ForceMode.Impulse);
            Vector3 rand = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-100f, 1f)).normalized;
            rigidbody.angularVelocity = rand.normalized * 10;
        }
        
        // Has to be separate because cool video game doesn't like passing through the rb itself.
        public static IEnumerator<float> ThrowWhenRigidBody(Pickup pickup, Vector3 dir)
        {
            yield return Timing.WaitUntilFalse(() => pickup.Rb == null);
            pickup.Rb.transform.Translate(new Vector3(0, 0.5f, 0), Space.Self);
            pickup.Rb.AddForce(dir * 17, ForceMode.Impulse);
            Vector3 rand = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-100f, 1f)).normalized;
            pickup.Rb.angularVelocity = rand.normalized * 10;
        }

        // Nabbed from KoukoCocoa AdminTools [https://github.com/KoukoCocoa/AdminTools/blob/V2.6/AdminTools/EventHandlers.cs#L262]
        public static void SpawnGrenadeOnPlayer(Player player, GrenadeType type, float timer = 3f)
        {
            Vector3 spawnrand = new Vector3(Random.Range(0f, 2f), Random.Range(0f, 2f), Random.Range(0f, 2f));
            GrenadeManager gm = player.ReferenceHub.gameObject.GetComponent<GrenadeManager>();
            GrenadeSettings gs = null;
            switch (type)
            {
                case GrenadeType.FragGrenade:
                    gs = gm.availableGrenades.FirstOrDefault(g => g.inventoryID == ItemType.GrenadeFrag);
                    break;
                case GrenadeType.Flashbang:
                    gs = gm.availableGrenades.FirstOrDefault(g => g.inventoryID == ItemType.GrenadeFlash);
                    break;
                case GrenadeType.Scp018:
                    gs = gm.availableGrenades.FirstOrDefault(g => g.inventoryID == ItemType.SCP018);
                    break;
            }

            Grenade grenade;
            if (type != GrenadeType.Scp018)
            { 
                grenade = Object.Instantiate(gs.grenadeInstance).GetComponent<Grenade>();
                grenade.fuseDuration = timer;
                grenade.FullInitData(gm, player.Position, Quaternion.Euler(grenade.throwStartAngle), grenade.throwLinearVelocityOffset, grenade.throwAngularVelocity);
                Timing.RunCoroutine(ThrowWhenRigidBody(grenade.rb,
                    (player.ReferenceHub.PlayerCameraReference.forward + new Vector3(0, 0.25f, 0)).normalized));
            }
            else
            {
                grenade = Object.Instantiate(gs.grenadeInstance).GetComponent<Scp018Grenade>();
                grenade.InitData(gm, spawnrand, player.ReferenceHub.PlayerCameraReference.forward);
            }
            
            NetworkServer.Spawn(grenade.gameObject);
        }
    }
}