using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Assets.Gores.Gems
{
    public abstract class GemGores : ModGore
    {
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.numFrames = 3;
            gore.frame = (byte)Main.rand.Next(3);
            gore.scale = Main.rand.NextFloat(0.5f, 1.1f);
            gore.drawOffset = new Vector2(0f, Main.rand.Next(3) * 2 + 6) * gore.scale;
        }
    }

    public class AmethystGore : GemGores
    {
    }
    public class TopazGore : GemGores
    {
    }
    public class SapphireGore : GemGores
    {
    }
    public class EmeraldGore : GemGores
    {
    }
    public class RubyGore : GemGores
    {
    }
    public class DiamondGore : GemGores
    {
    }
}