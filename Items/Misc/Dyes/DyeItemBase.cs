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
        public virtual Ref<Effect> Effect => FromAssetFolder("ParentDyeShader");
        public abstract string Pass { get; }
        public virtual int Rarity => ItemRarityID.Blue;
        public virtual int Value => Item.sellPrice(gold: 0, silver: 50);
        public virtual ArmorShaderData CreateShaderData()
        {
            return new ArmorShaderData(Effect, Pass);
        }
        internal Ref<Effect> FromAssetFolder(string name)
        {
            return new Ref<Effect>(ModContent.Request<Effect>("Aequus/Assets/Effects/" + name, AssetRequestMode.ImmediateLoad).Value);
        }
        internal Ref<Effect> GetDyeShader()
        {
            return new Ref<Effect>(ModContent.Request<Effect>($"{Texture}Shader", AssetRequestMode.ImmediateLoad).Value);
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
            if (!Main.dedServ)
            {
                GameShaders.Armor.BindShader(Type, CreateShaderData());
            }
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 9999;
            Item.value = Value;
            Item.rare = Rarity;
        }
    }
}