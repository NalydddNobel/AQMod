using Aequus.Biomes;
using Aequus.Biomes.Glimmer;
using Aequus.Content.AnalysisQuests;
using Aequus.NPCs.Boss;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Boss.Summons
{
    public class GalacticStarfruit : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = ItemID.Sets.SortingPriorityBossSpawns[ItemID.WormFood];
            SacrificeTotal = 3;
            AnalysisSystem.IgnoreItem.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.DefaultToHoldUpItem();
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Green;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(gold: 2);
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.dayTime && !GlimmerBiome.EventActive && !NPC.AnyNPCs(ModContent.NPCType<OmegaStarite>());
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            GlimmerSystem.BeginEvent();
            return GlimmerBiome.EventActive;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FallenStar, 5)
                .AddIngredient(ItemID.DemoniteBar, 1)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}