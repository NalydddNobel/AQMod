using AQMod.Items.Materials.Energies;
using AQMod.Items.Tools;
using AQMod.Items.Weapons.Magic;
using AQMod.Items.Weapons.Melee;
using AQMod.Items.Weapons.Ranged;
using AQMod.NPCs.Bosses;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items
{
    public class CrabsonBag : TreasureBagItem
    {
        protected override int InternalRarity => ItemRarityID.Blue;
        public override int BossBagNPC => ModContent.NPCType<JerryCrabson>();

        public override void OpenBossBag(Player player)
        {
            player.QuickSpawnItem(ModContent.ItemType<Crabax>());
            player.QuickSpawnItem(ModContent.ItemType<AquaticEnergy>(), Main.rand.NextVRand(5, 8));
            var choices = new List<int>()
            {
                ModContent.ItemType<Bubbler>(),
                ModContent.ItemType<CinnabarBow>(),
                ModContent.ItemType<JerryClawFlail>(),
            };
            int choice = Main.rand.Next(choices.Count);
            player.QuickSpawnItem(choices[choice]);
            choices.RemoveAt(choice);
            choice = Main.rand.Next(choices.Count);
            player.QuickSpawnItem(choices[choice]);
        }
    }
}