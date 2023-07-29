using Microsoft.Xna.Framework;
using System.Diagnostics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Carpentry {
    [LegacyName("CarpenterBountyPlayer")]
    public partial class CarpentryPlayer : ModPlayer {
        public bool CanClaimFreeShutterstockerGift { get; internal set; }
        public int SelectedBounty { get; set; }

        public readonly BuildChallengeSaveData CollectedBounties = new("CollectedBounties");

        public override void Initialize() {
            CollectedBounties.Clear();
            CanClaimFreeShutterstockerGift = false;
            SelectedBounty = -1;
        }

        public override void SaveData(TagCompound tag) {
            tag["FreeCamera"] = CanClaimFreeShutterstockerGift;
            if (SelectedBounty > 0) {
                tag["SelectedBounty"] = BuildChallengeLoader.registeredBuildChallenges[SelectedBounty];
            }
            CollectedBounties.SaveData(tag);
        }

        public override void LoadData(TagCompound tag) {
            CanClaimFreeShutterstockerGift = tag.Get("freeCameraGift", defaultValue: false);
            SelectedBounty = tag.TryGet("SelectedBounty", out string selectedBounty) && BuildChallengeLoader.TryFind(selectedBounty, out var buildChallenge) ? buildChallenge.Type : -1;
            CollectedBounties.LoadData(tag);
        }

        public override ModPlayer Clone(Player newEntity) {
            var clone = (CarpentryPlayer)base.Clone(newEntity);
            clone.CollectedBounties.Clone(clone.CollectedBounties);
            clone.SelectedBounty = SelectedBounty;
            return clone;
        }

        public override void CopyClientState(ModPlayer clientClone) {
            var clone = (CarpentryPlayer)clientClone;
            clone.CollectedBounties.Clone(clone.CollectedBounties);
            clone.SelectedBounty = SelectedBounty;
        }

        public override void SendClientChanges(ModPlayer clientPlayer) {
        }

        public override void PostUpdate() {
            if (Main.myPlayer != Player.whoAmI || Main.GameUpdateCount % 10 != 0 || NPC.AnyDanger(quickBossNPCCheck: true, ignorePillarsAndMoonlordCountdown: true))
                return;

            CheckBuffBuildings();
        }

        public void CheckBuffBuildings() {
#if DEBUG
            var stopwatch = new Stopwatch();
            stopwatch.Start();
#endif
            int val = Player.Aequus().buildingBuffRange;
            var screenRectangle = new Rectangle((int)Player.Center.X / 16 - val, (int)Player.Center.Y / 16 - val, val * 2, val * 2);

            foreach (var pair in CarpentrySystem.BuildingBuffs.WorldData) {
                if (pair.Key <= 0) {
                    continue;
                }
                int buffId = BuildChallengeLoader.registeredBuildChallenges[pair.Key].BuildBuffType;
                if (buffId > 0) {
                    foreach (var r in pair.Value) {
                        if (r.Intersects(screenRectangle)) {
                            Player.AddBuff(buffId, 60);
                            break;
                        }
                    }
                }
            }
#if DEBUG
            stopwatch.Stop();
#endif
        }

        public bool HasUnclaimedBounty() {
            return CarpentrySystem.CompletedBounties.ContainsAny((t) => !CollectedBounties.ContainsChallenge(t));
        }
    }
}