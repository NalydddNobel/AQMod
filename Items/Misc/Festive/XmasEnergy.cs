using Aequus.Graphics;
using Aequus.Items.Misc.Energies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Misc.Festive
{
    public class XmasEnergy : EnergyItemBase
    {
        public static StaticMiscShaderInfo EnergyShader;
        public static Asset<Texture2D> AuraTexture;
        public override ref StaticMiscShaderInfo Shader => ref EnergyShader;
        public override ref Asset<Texture2D> Aura => ref AuraTexture;
        protected override Vector3 LightColor => new Vector3(AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 2.5f, 0f, 0.5f), AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 2.5f + MathHelper.PiOver2, 0f, 0.5f), 0.1f);
        public override int Rarity => ItemRarityID.Yellow;
    }
}