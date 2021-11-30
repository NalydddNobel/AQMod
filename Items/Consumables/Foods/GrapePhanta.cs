using AQMod.Assets.ItemOverlays;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Consumables.Foods
{
    public class GrapePhanta : ModItem, ISpecialFood
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                AQMod.ItemOverlays.Register(new GlowmaskOverlay(this.GetPath("_Glow"), new Color(200, 200, 200, 0)), item.type);
            }
        }

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
            item.buffType = BuffID.WellFed;
            item.buffTime = 28800;
        }

        int ISpecialFood.ChangeBuff(Player player)
        {
            return ModContent.BuffType<Buffs.Foods.GrapePhanta>();
        }
    }
}