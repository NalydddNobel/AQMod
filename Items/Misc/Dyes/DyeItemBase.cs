using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Dyes
{
    public abstract class DyeItemBase : ModItem
    {
        public virtual Ref<Effect> Effect => new Ref<Effect>(ModContent.Request<Effect>("Aequus/Assets/Effects/ParentDyeShader", AssetRequestMode.ImmediateLoad).Value);
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
                GameShaders.Armor.BindShader(Type, CreateShaderData());
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