using AQMod.Items.Materials.Energies;
using AQMod.NPCs.Boss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.BossItems.Starite
{
    public class StariteBag : BossBagItem
    {
        protected override bool CanDropDevArmor => false;

        protected override int rarity => AQItem.Rarities.OmegaStariteRare;

        public override int BossBagNPC => ModContent.NPCType<OmegaStarite>();

        public override void OpenBossBag(Player player)
        {
            if (Main.rand.NextBool(7))
                player.QuickSpawnItem(ModContent.ItemType<OmegaStariteMask>());
            //player.QuickSpawnItem(ModContent.ItemType<CelesteTorus>());
            //player.QuickSpawnItem(ModContent.ItemType<CosmicTelescope>());
            //int[] choices = new int[]
            //{
            //    ModContent.ItemType<MagicWand>(),
            //    ModContent.ItemType<Raygun>(),
            //};
            //player.QuickSpawnItem(choices[Main.rand.Next(choices.Length)]);
            player.QuickSpawnItem(ModContent.ItemType<CosmicEnergy>(), 5 + Main.rand.Next(3));
            player.QuickSpawnItem(ItemID.FallenStar, 20 + Main.rand.Next(10));
            player.QuickSpawnItem(ItemID.SoulofFlight, 8 + Main.rand.Next(4));
        }
    }
}