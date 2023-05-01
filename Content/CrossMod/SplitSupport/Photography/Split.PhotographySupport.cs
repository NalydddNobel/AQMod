using Aequus.Content.Boss.UltraStariteMiniboss;
using Aequus.Content.CrossMod.SplitSupport.Photography;
using Aequus.Content.NPCs.Critters;
using Aequus.Content.Town.SkyMerchantNPC;
using Aequus.Items.Vanity.Pets;
using Aequus.NPCs.Monsters;
using Aequus.NPCs.Monsters.Night;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod.SplitSupport {
    internal partial class Split {

        private record struct AlbumQuestInfo(int Frame, int[] NPCIds, int Envelope, int PosterItem, Predicate<NPC> SpecialCondition = null) {
            public AlbumQuestInfo(int Frame, int NPCId, int Envelope, int PosterItem, Predicate<NPC> SpecialCondition = null)
                : this(Frame, new int[1] { NPCId, }, Envelope, PosterItem, SpecialCondition) {

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order">Sorting value for this album.</param>
        /// <param name="specialReward">Special reward for this album</param>
        /// <param name="values">Must have a length of 6.</param>
        private void RegisterPhotographyPage(float order, int specialReward, AlbumQuestInfo[] values) {

            var args = new object[35];
            args[0] = "Photography.AddPage";
            args[1] = Mod;
            args[2] = order;
            args[3] = specialReward;
            args[4] = Main.dedServ ? null : AequusTextures.Icons.Value;

            for (int i = 0; i < 6; i++) {
                int index = 5 + i * 5;
                args[index] = values[i].Frame;
                args[index + 1] = values[i].NPCIds;
                args[index + 2] = values[i].Envelope;
                args[index + 3] = values[i].PosterItem;
                args[index + 4] = values[i].SpecialCondition;
            }

            Instance.Call(args);
        }

        private void LoadPhotographySupport() {

            int spaceEnvelope = GetItem("BlueSkyEnvelope", ItemID.FloatingIslandFishingCrate);
            int hellEnvelope = GetItem("FieryEnvelope", ItemID.ObsidianLockbox);
            int dungeonEnvelope = GetItem("DungeonEnvelope", ItemID.DungeonFishingCrateHard);
            int bloodMoonEnvelope = GetItem("HorrificEnvelope", ItemID.DungeonFishingCrateHard);

            RegisterPhotographyPage(
                order: 10f,
                specialReward: ModContent.ItemType<FamiliarPickaxe>(),
                new AlbumQuestInfo[6] {
                    new(0, ModContent.NPCType<BreadOfCthulhu>(), ModContent.ItemType<EnvelopeUndergroundOcean>(), ModContent.ItemType<PosterBreadOfCthulhu>()),
                    new(1, ModContent.NPCType<BloodMimic>(), bloodMoonEnvelope, ModContent.ItemType<PosterBloodMimic>()),
                    new(2, ModContent.NPCType<UltraStarite>(), ModContent.ItemType<EnvelopeGlimmer>(), ModContent.ItemType<PosterUltraStarite>()),
                    new(3, ModContent.NPCType<Heckto>(), dungeonEnvelope, ModContent.ItemType<PosterHeckto>()),
                    new(4, ModContent.NPCType<Oblivision>(), hellEnvelope, ModContent.ItemType<PosterOblivision>()),
                    new(5, ModContent.NPCType<SkyMerchant>(), spaceEnvelope, ModContent.ItemType<PosterSkyMerchant>()),
                });
        }
    }
}
