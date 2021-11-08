using System;
using Terraria;

namespace AQMod.Content.WorldEvents.AzureCurrents
{
    [Obsolete("Remove later")]
    public interface IWind
    {
        void ApplyOntoNPC(NPC npc);
        void ApplyOntoProjectile(Projectile projectile);
        void ApplyOntoPlayer(Player player);
    }
}