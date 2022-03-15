using AQMod.Items.Armor.Vanity.BossMasks;
using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Tools.Map;
using AQMod.Items.Weapons.Magic;
using AQMod.Items.Weapons.Ranged;
using AQMod.NPCs.Bosses;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Expert
{
    public class StariteBag : TreasureBag
    {
        protected override int InternalRarity => ItemRarityID.LightRed;
        public override int BossBagNPC => ModContent.NPCType<OmegaStarite>();

        public override void OpenBossBag(Player player)
        {
            AQMod.AequusDeveloperItems(player, hardmode: true);
            if (Main.rand.NextBool(7))
                player.QuickSpawnItem(ModContent.ItemType<OmegaStariteMask>());
            player.QuickSpawnItem(ModContent.ItemType<CelesteTorus>());
            if (Main.rand.NextBool(3))
                player.QuickSpawnItem(ModContent.ItemType<CosmicTelescope>());
            int[] choices = new int[]
            {
                ModContent.ItemType<MagicWand>(),
                ModContent.ItemType<Raygun>(),
            };
            player.QuickSpawnItem(choices[Main.rand.Next(choices.Length)]);
            player.QuickSpawnItem(ModContent.ItemType<CosmicEnergy>(), Main.rand.NextVRand(5, 8));
            player.QuickSpawnItem(ItemID.FallenStar, Main.rand.NextVRand(20, 30));
            player.QuickSpawnItem(ModContent.ItemType<LightMatter>(), Main.rand.NextVRand(18, 24));
        }
    }
}