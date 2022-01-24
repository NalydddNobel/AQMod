using AQMod.Items.DrawOverlays;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Foods
{
    public class NeutronJuice : ModItem, IItemOverlaysWorldDraw, IItemOverlaysPlayerDraw
    {
        private static readonly GlowmaskOverlay _overlay = new GlowmaskOverlay(AQUtils.GetPath<NeutronJuice>("_Glow"));
        IOverlayDrawWorld IItemOverlaysWorldDraw.WorldDraw => _overlay;
        IOverlayDrawPlayerUse IItemOverlaysPlayerDraw.PlayerDraw => _overlay;

        public override void SetDefaults()
        {
            item.width = 10;
            item.height = 10;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 15;
            item.useTime = 15;
            item.useTurn = true;
            item.UseSound = SoundID.Item3;
            item.maxStack = 999;
            item.consumable = true;
            item.rare = ItemRarityID.Blue;
            item.value = Item.buyPrice(silver: 20);
            item.buffType = ModContent.BuffType<Buffs.Foods.NeutronYogurt>();
            item.buffTime = 36000;
        }
    }
}