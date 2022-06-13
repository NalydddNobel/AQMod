using Aequus.NPCs.Boss;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Summons
{
    public class GalacticStarfruit : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = ItemID.Sets.SortingPriorityBossSpawns[ItemID.WormFood];
            this.SetResearch(3);
        }

        public override void SetDefaults()
        {
            Item.DefaultToHoldUpItem();
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Green;
            Item.consumable = true;
            Item.maxStack = 999;
            Item.value = Item.buyPrice(gold: 10);
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.dayTime /*&& !Glimmer.IsGlimmerEventCurrentlyActive()*/ && !NPC.AnyNPCs(ModContent.NPCType<OmegaStarite>());
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                SoundEngine.PlaySound(SoundID.Roar, player.position);
                Main.hardMode = !Main.hardMode;
                LanternNight.GenuineLanterns = !LanternNight.GenuineLanterns;
                player.Teleport(AequusSystem.Structures.GetLocation("CrabCrevice").GetValueOrDefault(Microsoft.Xna.Framework.Point.Zero).ToWorldCoordinates());
            }
            return true;
        }
    }
}