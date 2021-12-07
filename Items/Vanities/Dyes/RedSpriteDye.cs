using Microsoft.Xna.Framework;
using Terraria.Graphics.Shaders;

namespace AQMod.Items.Vanities.Dyes
{
    public class RedSpriteDye : DyeItem
    {
        public override string Pass => "RedSpritePass";
        public override int Rarity => AQItem.Rarities.GaleStreamsRare - 1;
        public override ArmorShaderData CreateShaderData => base.CreateShaderData.UseColor(new Color(140, 0, 21, 255));
    }
}