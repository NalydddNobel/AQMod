using Terraria.Graphics.Shaders;

namespace AQMod.Items.Vanities.Dyes
{
    public class CensorDye : DyeItem
    {
        public override string Pass => "CensorPass";
        public override void ModifyArmorShaderData(ArmorShaderData data)
        {
            data.UseOpacity(4f);
        }
    }
}