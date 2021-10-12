using AQMod.Common.Skies;
using AQMod.Common.Utilities;
using AQMod.Content.WorldEvents.Glimmer;
using AQMod.Localization;
using AQMod.NPCs.Boss.Starite;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items
{
    public class UltimateStarfruit : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[item.type] = 6;
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.LightPurple;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
        }

        public override bool CanUseItem(Player player)
        {
            if (Main.dayTime || NPC.AnyNPCs(ModContent.NPCType<OmegaStarite>()))
                return false;
            if (player.altFunctionUse != 2)
            {
                return AQMod.glimmerEvent.IsActive;
            }
            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                if (AQMod.glimmerEvent.IsActive)
                {
                    AQMod.glimmerEvent.Deactivate();
                    AQMod.BroadcastMessage(AQText.Key + "Common.GlimmerEventEnding", GlimmerEvent.TextColor);
                }
                else
                {
                    AQMod.glimmerEvent.Activate();
                    AQMod.BroadcastMessage(AQText.Key + "Common.GlimmerEventWarning", GlimmerEvent.TextColor);
                    Main.PlaySound(SoundID.Roar, player.position, 0);
                }
            }
            else
            {
                if (Main.myPlayer == player.whoAmI)
                    GlimmerEventSky._glimmerLight = 1f;
                NPC.NewNPC((int)player.position.X, (int)player.position.Y - 1600, ModContent.NPCType<OmegaStarite>(), 0, OmegaStarite.PHASE_NOVA, 0f, 0f, 0f, player.whoAmI);
                AQMod.BroadcastMessage(AQText.Key + "Common.AwakenedOmegaStarite", AQMod.BossMessage);
                Main.PlaySound(SoundID.Roar, player.position, 0);
            }
            return true;
        }

        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<NovaFruit>());
            recipe.AddIngredient(ModContent.ItemType<MythicStarfruit>(), 2);
            recipe.AddIngredient(ModContent.ItemType<CosmicEnergy>(), 5);
            recipe.AddIngredient(ItemID.SoulofFlight, 3);
            recipe.AddIngredient(ItemID.FallenStar, 10);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}