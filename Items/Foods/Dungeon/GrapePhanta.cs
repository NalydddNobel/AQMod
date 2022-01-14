using AQMod.Items.DrawOverlays;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Foods.Dungeon
{
    public class GrapePhanta : ModItem, IItemOverlaysWorldDraw, IItemOverlaysPlayerDraw
    {
        private static readonly GlowmaskOverlay _overlay = new GlowmaskOverlay(AQUtils.GetPath<GrapePhanta>("_Glow"));
        IOverlayDrawWorld IItemOverlaysWorldDraw.WorldDraw => _overlay;
        IOverlayDrawPlayerUse IItemOverlaysPlayerDraw.PlayerDraw => _overlay;

        public override void SetDefaults()
        {
            item.width = 14;
            item.height = 30;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 10;
            item.useTime = 10;
            item.useTurn = true;
            item.UseSound = SoundID.Item3;
            item.maxStack = 999;
            item.consumable = true;
            item.rare = ItemRarityID.Yellow;
            item.value = Item.buyPrice(silver: 20);
            item.buffType = ModContent.BuffType<Buffs.Foods.GrapePhanta>();
            //item.buffType = BuffID.WellFed;
            item.buffTime = 28800;
        }
    }
}