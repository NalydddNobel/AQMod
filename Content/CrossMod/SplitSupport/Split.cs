using Aequus.Content.Boss.UltraStariteMiniboss;
using Aequus.Content.Critters;
using Aequus.Content.CrossMod.SplitSupport.Photography;
using Aequus.Content.Town.SkyMerchantNPC;
using Aequus.Items.Unused.SlotMachines;
using Aequus.Items.Vanity.Pets;
using Aequus.NPCs.Monsters;
using Aequus.NPCs.Monsters.Night;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod.SplitSupport {
    internal class Split : ModSupport<Split> {
        public override void PostSetupContent() {

            if (Instance.TryFind("AnxiousnessPotion", out ModItem modItem)) {
                SlotMachineSystem.DefaultPotions.Add(modItem.Type);
            }
            if (Instance.TryFind("PurifyingPotion", out modItem)) {
                SlotMachineSystem.DefaultPotions.Add(modItem.Type);
            }
            if (Instance.TryFind("DiligencePotion", out modItem)) {
                SlotMachineSystem.DefaultPotions.Add(modItem.Type);
            }
            if (Instance.TryFind("AttractionPotion", out modItem)) {
                SlotMachineSystem.DefaultPotions.Add(modItem.Type);
            }

            RegisterPhotographyPage(
                order: 10f, 
                specialReward: ModContent.ItemType<FamiliarPickaxe>(),
                new AlbumQuestInfo[6] {
                    new(0, ModContent.NPCType<BreadOfCthulhu>(), ItemID.WoodenCrate, ModContent.ItemType<PosterBreadOfCthulhu>()),
                    new(1, ModContent.NPCType<BloodMimic>(), ItemID.WoodenCrate, ModContent.ItemType<PosterBloodMimic>()),
                    new(2, ModContent.NPCType<UltraStarite>(), ItemID.WoodenCrate, ModContent.ItemType<PosterUltraStarite>()),
                    new(3, ModContent.NPCType<Heckto>(), ItemID.WoodenCrate, ModContent.ItemType<PosterHeckto>()),
                    new(4, ModContent.NPCType<Oblivision>(), ItemID.WoodenCrate, ModContent.ItemType<PosterOblivision>()),
                    new(5, ModContent.NPCType<SkyMerchant>(), ItemID.WoodenCrate, ModContent.ItemType<PosterSkyMerchant>()),
                });
        }

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
    }
}