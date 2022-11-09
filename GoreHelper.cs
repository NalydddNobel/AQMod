using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus
{
    public static class GoreHelper
    {
        public static int GoreType(string name)
        {
            return Aequus.Instance.Find<ModGore>(name).Type;
        }
        public static Gore DeathGore(this NPC npc, string name, Vector2 offset = default(Vector2), Vector2? velocity = null)
        {
            var g = Gore.NewGoreDirect(npc.GetSource_Death(), npc.Center + offset, velocity ?? npc.velocity, GoreType(name));
            g.position.X -= g.Width / 2f;
            g.position.Y -= g.Height / 2f;
            return g;
        }
    }
}