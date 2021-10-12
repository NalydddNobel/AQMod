using AQMod.Common.Utilities;
using AQMod.Items.Accessories;
using AQMod.Items.Tools.Markers;
using AQMod.Items.Vanities;
using AQMod.Items.Weapons.Magic;
using AQMod.Items.Weapons.Melee;
using AQMod.Items.Weapons.Ranged.Bullet;
using AQMod.NPCs.Boss.Starite;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items
{
    public class StariteBag : ModItem
    {
        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 24;
            item.height = 24;
            item.rare = ItemRarityID.LightRed;
            item.expert = true;
        }

        public override bool CanRightClick() => true;

        public override void OpenBossBag(Player player)
        {
            if (Main.rand.NextBool(7))
                player.QuickSpawnItem(ModContent.ItemType<OmegaStariteMask>());
            player.QuickSpawnItem(ModContent.ItemType<CelesteTorus>());
            player.QuickSpawnItem(ModContent.ItemType<CosmicTelescope>());
            int[] choices = new int[]
            {
                ModContent.ItemType<MagicWand>(),
                ModContent.ItemType<Galactium>(),
                ModContent.ItemType<Raygun>(),
            };
            player.QuickSpawnItem(choices[Main.rand.Next(choices.Length)]);
            player.QuickSpawnItem(ModContent.ItemType<CosmicEnergy>(), Main.rand.NextVRand(5, 8));
            player.QuickSpawnItem(ItemID.FallenStar, Main.rand.NextVRand(20, 30));
            player.QuickSpawnItem(ItemID.SoulofFlight, Main.rand.NextVRand(8, 12));
        }

        public override int BossBagNPC => ModContent.NPCType<OmegaStarite>();
    }
}