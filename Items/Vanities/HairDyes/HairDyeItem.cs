using AQMod.Effects.Dyes;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Dyes;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Vanities.HairDyes
{
    public abstract class HairDyeItem : ModItem
    {
        protected virtual int Rarity => ItemRarityID.Green;
        protected virtual int Value => Item.buyPrice(gold: 5);
        internal virtual HairShaderData CreateShaderData()
        {
            return new LegacyHairShaderData().UseLegacyMethod(GetHairClr);
        }
        protected virtual Color GetHairClr(Player player, Color hairColor, ref bool useLighting)
        {
            return new Color(255, 255, 255, 255);
        }

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                DyeBinder.AddDye(this);
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 26;
            item.maxStack = 999;
            item.value = Value;
            item.rare = Rarity;
            item.UseSound = SoundID.Item3;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useTurn = true;
            item.useAnimation = 17;
            item.useTime = 17;
            item.consumable = true;
        }
    }
}