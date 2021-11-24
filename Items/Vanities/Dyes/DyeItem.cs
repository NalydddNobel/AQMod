using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Vanities.Dyes
{
    public abstract class DyeItem : ModItem
    {
        public virtual Ref<Effect> Effect => new Ref<Effect>();
        public abstract string Pass { get; }
        public virtual int Rarity => ItemRarityID.Blue;
        public virtual int Value => Item.sellPrice(gold: 1, silver: 50);
        public virtual void ModifyArmorShaderData(ArmorShaderData data)
        {
        }

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                var effect = Effect;
                mod.Logger.Info("Binding " + effect.Value.Name + " to " + Name);
                var shaderData = new ArmorShaderData(effect, Pass);
                ModifyArmorShaderData(shaderData);
                GameShaders.Armor.BindShader(item.type, shaderData);
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