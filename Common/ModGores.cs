using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common
{
    public static class ModGores
    {
        public static int GoreType(string name)
        {
            return Aequus.Instance.Find<ModGore>(name).Type;
        }
        public static Gore DeathGore(this NPC npc, string name, Vector2 offset = default(Vector2), Vector2? velocity = null)
        {
            return Gore.NewGoreDirect(npc.GetSource_Death("Aequus:Gore"), npc.Center + offset, velocity ?? npc.velocity, GoreType(name));
        }
    }
}