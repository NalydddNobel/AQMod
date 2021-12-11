using AQMod.Assets;
using AQMod.Effects.Dyes;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Vanities.Dyes
{
    public abstract class DyeItem : ModItem
    {
        public virtual Ref<Effect> Effect => new Ref<Effect>(EffectCache.ParentPixelShader);
        public abstract string Pass { get; }
        public virtual int Rarity => ItemRarityID.Blue;
        public virtual int Value => Item.sellPrice(gold: 1, silver: 50);
        public virtual ArmorShaderData CreateShaderData()
        {
            return new ArmorShaderData(Effect, Pass);
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
            item.height = 20;
            item.maxStack = 99;
            item.value = Value;
            item.rare = Rarity;
        }
    }
}