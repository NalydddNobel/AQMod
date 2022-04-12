using Terraria.ModLoader;

namespace Aequus.NPCs.Boss
{
    public sealed class DustDevil : ModNPC
    {
        public override string Texture => Aequus.TextureNone;

        public override void SetDefaults()
        {
            NPC.lifeMax = 18000;
            NPC.defense = 16;
        }
    }
}