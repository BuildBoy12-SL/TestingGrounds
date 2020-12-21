namespace TestingGrounds
{
    using Classes.SaveState;
    using Exiled.API.Extensions;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Grenades;
    using Hints;
    using MEC;
    using Mirror;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using UnityEngine;

    public class Methods
    {
        public static readonly List<CoroutineHandle> CoroutineHandles = new();

        public static IEnumerator<float> Record(string directory, int seconds)
        {
            Log.Debug($"A new {seconds} second recording with the name of \"{directory}\" has started.",
                TestingGrounds.Singleton.Config.ShowDebug);
            string updatedDirectory = Path.Combine(TestingGrounds.RecordingsDirectory, directory);
            if (!File.Exists(updatedDirectory))
                File.Create(updatedDirectory).Close();

            for (int i = 0; i < Application.targetFrameRate * seconds; i++)
            {
                using (StreamWriter sw = File.AppendText(updatedDirectory))
                {
                    sw.WriteLine(GetData());
                }

                yield return Timing.WaitForOneFrame;
            }

            Log.Debug($"Recording \"{directory}\" has ended.", TestingGrounds.Singleton.Config.ShowDebug);
        }

        private static string GetData()
        {
            var savedPlayers =
                Player.List.Select(player => new SavedPlayer
                {
                    Inventory = player.Inventory.items,
                    UserId = player.UserId,
                    Position = player.Position,
                    Role = player.Role,
                    Rotation = player.Rotation
                }).ToList();

            var savedItems =
                (from pickup in UnityEngine.Object.FindObjectsOfType<Pickup>()
                    let transform = pickup.transform
                    select
                        new SavedItem
                        {
                            Durability = pickup.durability, Item = pickup.itemId, Position = transform.position,
                            Rotation = transform.rotation
                        }).ToList();

            var savedDoors =
                (from door in Map.Doors
                    select
                        new SavedDoor
                        {
                            IsDestroyed = door.Networkdestroyed, IsLocked = door.Networklocked,
                            IsOpen = door.NetworkisOpen,
                            Position = door.transform.position
                        }).ToList();

            var saveState = new SaveState
            {
                Seed = GameObject.Find("Host").GetComponent<RandomSeedSync>().seed,
                SavedItems = savedItems,
                SavedPlayers = savedPlayers,
                SavedDoors = savedDoors
            };

            return JsonConvert.SerializeObject(saveState, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        public static IEnumerator<float> PlayRecording(string directory)
        {
            Log.Debug($"Playing recording with name of \"{directory}\".", TestingGrounds.Singleton.Config.ShowDebug);
            foreach (string line in File.ReadAllLines(directory))
            {
                SaveState saveState = JsonConvert.DeserializeObject<SaveState>(line);
                LoadState(saveState);
                yield return Timing.WaitForOneFrame;
            }

            Log.Debug($"Playback of \"{directory}\" has ended.", TestingGrounds.Singleton.Config.ShowDebug);
        }

        public static void SaveState(string directory)
        {
            File.WriteAllText(directory, GetData());
        }

        public static void LoadState(SaveState saveState)
        {
            if (saveState == null)
                return;

            if (saveState.SavedPlayers != null)
            {
                foreach (var savedPlayer in saveState.SavedPlayers)
                {
                    Player ply = Player.Get(savedPlayer.UserId);
                    if (ply == null)
                        return;

                    Timing.CallDelayed(0.1f, () =>
                    {
                        if (ply.Role != savedPlayer.Role)
                            ply.Role = savedPlayer.Role;
                        ply.ResetInventory(savedPlayer.Inventory.ToList());
                        Timing.CallDelayed(0.1f, () =>
                        {
                            ply.Position = savedPlayer.Position;
                            ply.Rotation = savedPlayer.Rotation;
                        });
                    });
                }
            }

            if (saveState.SavedItems != null)
            {
                foreach (Pickup pickup in Object.FindObjectsOfType<Pickup>())
                    NetworkServer.Destroy(pickup.gameObject);

                foreach (SavedItem savedItem in saveState.SavedItems)
                    savedItem.Item.Spawn(savedItem.Durability, savedItem.Position, savedItem.Rotation);
            }

            if (saveState.SavedDoors != null)
            {
                foreach (var door in Map.Doors)
                {
                    foreach (SavedDoor savedDoor in saveState.SavedDoors)
                    {
                        if (savedDoor.Position != door.transform.position)
                            continue;

                        door.Networkdestroyed = savedDoor.IsDestroyed;
                        door.Networklocked = savedDoor.IsLocked;
                        door.SetStateWithSound(savedDoor.IsOpen);
                    }
                }
            }

            if (saveState == State.SaveState)
                State.SaveState = null;
        }

        // Nabbed from RogerFK ThrowItemsSL [https://github.com/RogerFK/ThrowItemsSL/blob/master/ThrowItems.cs#L62]
        public static IEnumerator<float> ThrowWhenRigidBody(Rigidbody rigidbody, Vector3 dir)
        {
            yield return Timing.WaitUntilFalse(() => rigidbody == null);
            rigidbody.transform.Translate(new Vector3(0, 0.5f, 0), Space.Self);
            rigidbody.AddForce(dir * 17, ForceMode.Impulse);
            Vector3 rand = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-100f, 1f))
                .normalized;
            rigidbody.angularVelocity = rand.normalized * 10;
        }

        // Has to be separate because cool video game doesn't like passing through the rb itself.
        public static IEnumerator<float> ThrowWhenRigidBody(Pickup pickup, Vector3 dir)
        {
            yield return Timing.WaitUntilFalse(() => pickup.Rb == null);
            pickup.Rb.transform.Translate(new Vector3(0, 0.5f, 0), Space.Self);
            pickup.Rb.AddForce(dir * 17, ForceMode.Impulse);
            Vector3 rand = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-100f, 1f))
                .normalized;
            pickup.Rb.angularVelocity = rand.normalized * 10;
        }

        // Nabbed from KoukoCocoa AdminTools [https://github.com/KoukoCocoa/AdminTools/blob/V2.6/AdminTools/EventHandlers.cs#L262]
        public static void SpawnGrenadeOnPlayer(Player player, GrenadeType type, float timer = 3f)
        {
            Vector3 spawnRandom = new Vector3(Random.Range(0f, 2f), Random.Range(0f, 2f), Random.Range(0f, 2f));
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
                grenade.FullInitData(gm, player.Position, Quaternion.Euler(grenade.throwStartAngle),
                    grenade.throwLinearVelocityOffset, grenade.throwAngularVelocity, player.Team);
                CoroutineHandles.Add(Timing.RunCoroutine(ThrowWhenRigidBody(grenade.rb,
                    (player.ReferenceHub.PlayerCameraReference.forward + new Vector3(0, 0.25f, 0)).normalized)));
            }
            else
            {
                grenade = Object.Instantiate(gs.grenadeInstance).GetComponent<Scp018Grenade>();
                grenade.InitData(gm, spawnRandom, player.ReferenceHub.PlayerCameraReference.forward);
            }

            NetworkServer.Spawn(grenade.gameObject);
        }

        public static IEnumerator<float> DoDistance(Player ply)
        {
            while (true)
            {
                if (!Physics.Raycast(ply.CameraTransform.position, ply.CameraTransform.forward,
                    out RaycastHit hit))
                    continue;

                ply.HintDisplay.Show(new TextHint($"<u>Ruler</u>\n{Vector3.Distance(State.Ruler[ply], hit.point)}",
                    new HintParameter[]
                    {
                        new StringHintParameter("")
                    },
                    HintEffectPresets.FadeInAndOut(0f, 0f), 2f));
                yield return Timing.WaitForSeconds(1f);
            }
        }

        public static void KillAllCoroutines()
        {
            foreach (CoroutineHandle coroutineHandle in CoroutineHandles)
                Timing.KillCoroutines(coroutineHandle);

            CoroutineHandles.Clear();
        }
    }
}