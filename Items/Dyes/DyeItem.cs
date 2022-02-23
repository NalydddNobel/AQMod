using AQMod.Assets;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Dyes
{
    public abstract class DyeItem : ModItem
    {
        public virtual Ref<Effect> Effect => new Ref<Effect>(LegacyEffectCache.ParentPixelShader);
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
                GameShaders.Armor.BindShader(item.type, CreateShaderData());
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