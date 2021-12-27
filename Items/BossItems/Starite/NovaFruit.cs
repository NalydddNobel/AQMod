using AQMod.Common;
using AQMod.Items.DrawOverlays;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Vanities.Dyes;
using AQMod.NPCs.Boss;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.BossItems.Starite
{
    public class NovaFruit : ModItem, IItemOverlaysWorldDraw, IItemOverlaysDrawInventory, IItemOverlaysPlayerDraw
    {
        private static readonly ShaderOverlay _shaderOverlay = new ShaderOverlay(AQUtils.GetPath<NovaFruit>(), () => ModContent.ItemType<EnchantedDye>());
        IOverlayDrawWorld IItemOverlaysWorldDraw.WorldDraw => _shaderOverlay;
        IOverlayDrawInventory IItemOverlaysDrawInventory.InventoryDraw => _shaderOverlay;
        IOverlayDrawPlayerUse IItemOverlaysPlayerDraw.PlayerDraw => _shaderOverlay;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = ItemID.Sets.SortingPriorityBossSpawns[ItemID.Abeemination];
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Pink;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<OmegaStarite>());
        }

        public override bool? UseItem(Player player)
        {
            //if (Main.myPlayer == player.whoAmI)
            //    GlimmerEventSky._glimmerLight = 1f;
            NPC.NewNPC((int)player.position.X, (int)player.position.Y - 1600, ModContent.NPCType<OmegaStarite>(), 0, OmegaStarite.PHASE_NOVA, 0f, 0f, 0f, player.whoAmI);
            AQUtils.BroadcastMessage("Mods.AQMod.Common.AwakenedOmegaStarite", Constants.ChatColors.BossMessage);
            SoundEngine.PlaySound(SoundID.Roar, player.position, 0);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CosmicEnergy>()
                .AddIngredient(ItemID.HellstoneBar, 15)
                .AddIngredient(ItemID.CrystalShard, 8)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}