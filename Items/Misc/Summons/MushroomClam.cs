using AQMod.Items.Materials;
using AQMod.Items.Recipes;
using AQMod.Localization;
using AQMod.NPCs.Bosses;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Misc.Summons
{
    public class MushroomClam : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[item.type] = ItemID.Sets.SortingPriorityBossSpawns[ItemID.SlimeCrown];
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Blue;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
        }

        public override bool CanUseItem(Player player)
        {
            return (player.ZoneBeach || player.Biomes().zoneCrabCrevice) && !NPC.AnyNPCs(ModContent.NPCType<Crabson>());
        }

        public override bool UseItem(Player player)
        {
            NPC.NewNPC((int)player.position.X, (int)player.position.Y + 1000, ModContent.NPCType<Crabson>(), 0, 0f, 0f, 0f, 0f, player.whoAmI);
            Main.NewText(AQText.ModText("Common.AwakenedCrabson"), Coloring.BossMessage);
            Main.PlaySound(SoundID.Roar, player.position, 0);
            return true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<CrabShell>(), 4);
            r.AddRecipeGroup(AQRecipeGroups.AnyNobleMushroom);
            r.AddTile(TileID.DemonAltar);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}