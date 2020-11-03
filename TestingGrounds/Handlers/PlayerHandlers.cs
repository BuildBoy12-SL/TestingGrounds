namespace TestingGrounds.Handlers
{
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.Events.EventArgs;
    using MEC;
    using UnityEngine;
    
    public class PlayerHandlers
    { 
        public void OnShoot(ShootingEventArgs ev)
        {
            if (!State.AlteredGuns.ContainsKey(ev.Shooter))
                return;

            int itemIndex = ev.Shooter.Inventory.GetItemIndex();
            if (ev.Shooter.Inventory.items[itemIndex].durability - 1f < 1)
                return;
            
            ev.IsAllowed = false;
            ev.Shooter.Inventory.items.ModifyDuration(itemIndex, ev.Shooter.Inventory.items[itemIndex].durability - 1f);
            if (State.AlteredGuns[ev.Shooter].IsThrowable())
            {
                switch (State.AlteredGuns[ev.Shooter])
                {
                    case ItemType.SCP018:
                        Methods.SpawnGrenadeOnPlayer(ev.Shooter, GrenadeType.Scp018);
                        break;
                    
                    case ItemType.GrenadeFrag:
                        Methods.SpawnGrenadeOnPlayer(ev.Shooter, GrenadeType.FragGrenade);
                        break;
                    
                    case ItemType.GrenadeFlash:
                        Methods.SpawnGrenadeOnPlayer(ev.Shooter, GrenadeType.Flashbang);
                        break;
                }
                
                return;
            }
                
            var pickup = State.AlteredGuns[ev.Shooter].Spawn(State.AlteredGuns[ev.Shooter].ItemDur(), ev.Shooter.Position);
            Timing.RunCoroutine(Methods.ThrowWhenRigidBody(pickup,
                (ev.Shooter.ReferenceHub.PlayerCameraReference.forward + new Vector3(0, 0.25f, 0)).normalized));
        }
    }
}