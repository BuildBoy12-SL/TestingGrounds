namespace TestingGrounds.Handlers
{
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.Events.EventArgs;
    using MEC;
    using UnityEngine;
    
    public class PlayerHandlers
    {
        public PlayerHandlers(TestingGrounds testingGrounds) => _testingGrounds = testingGrounds;
        private readonly TestingGrounds _testingGrounds;
        
        public void OnShoot(ShootingEventArgs ev)
        {
            if (!State.AlteredGuns.ContainsKey(ev.Shooter))
                return;

            int itemIndex = ev.Shooter.Inventory.GetItemIndex();
            if (ev.Shooter.Inventory.items[itemIndex].durability - _testingGrounds.Config.ItemGunConsumedAmmo < 0)
                return;
            
            ev.IsAllowed = false;
            ev.Shooter.Inventory.items.ModifyDuration(itemIndex, ev.Shooter.Inventory.items[itemIndex].durability - _testingGrounds.Config.ItemGunConsumedAmmo);
            ItemType item = State.AlteredGuns[ev.Shooter];
            if (item.IsThrowable())
            {
                switch (item)
                {
                    case ItemType.SCP018:
                        Methods.SpawnGrenadeOnPlayer(ev.Shooter, GrenadeType.Scp018);
                        return;
                    
                    case ItemType.GrenadeFrag:
                        Methods.SpawnGrenadeOnPlayer(ev.Shooter, GrenadeType.FragGrenade);
                        return;
                    
                    case ItemType.GrenadeFlash:
                        Methods.SpawnGrenadeOnPlayer(ev.Shooter, GrenadeType.Flashbang);
                        return;
                }
            }
                
            Pickup pickup = State.AlteredGuns[ev.Shooter].Spawn(item.ItemDur(), ev.Shooter.Position);
            Methods.CoroutineHandles.Add(Timing.RunCoroutine(Methods.ThrowWhenRigidBody(pickup,
                (ev.Shooter.ReferenceHub.PlayerCameraReference.forward + new Vector3(0, 0.25f, 0)).normalized)));
        }
    }
}