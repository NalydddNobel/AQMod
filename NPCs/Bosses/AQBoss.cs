using AQMod.Common;
using Terraria.ModLoader;

namespace AQMod.NPCs.Bosses
{
    public abstract class AQBoss : ModNPC, IModifiableMusicNPC
    {
        public abstract ModifiableMusic GetMusic();
    }
}