using AQMod.Assets.Effects;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Vanities.Dyes
{
    public abstract class DyeItem : ModItem
    {
        public virtual Ref<Effect> Effect => new Ref<Effect>(EffectCache.ParentPixelShader.Value);
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
                GameShaders.Armor.BindShader(Item.type, CreateShaderData());
            }
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 99;
            Item.value = Value;
            Item.rare = Rarity;
        }
    }
}