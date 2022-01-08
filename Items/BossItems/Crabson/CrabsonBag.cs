using AQMod.Items.Materials.Energies;
using AQMod.Items.Tools;
using AQMod.Items.Tools.Axe;
using AQMod.Items.Tools.Fishing;
using AQMod.Items.Weapons.Magic;
using AQMod.Items.Weapons.Melee.Flail;
using AQMod.Items.Weapons.Ranged;
using AQMod.NPCs.Boss.Crabson;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.BossItems.Crabson
{
    public class CrabsonBag : ModItem
    {
        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 24;
            item.height = 24;
            item.rare = ItemRarityID.Blue;
            item.expert = true;
        }

        public override bool CanRightClick() => true;

        public override void OpenBossBag(Player player)
        {
            player.QuickSpawnItem(ModContent.ItemType<Crabax>());
            if (Main.rand.NextBool(3))
                player.QuickSpawnItem(ModContent.ItemType<CrabClock>());
            if (Main.rand.NextBool())
                player.QuickSpawnItem(ModContent.ItemType<CrabRod>());
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

        public override int BossBagNPC => ModContent.NPCType<JerryCrabson>();
    }
}