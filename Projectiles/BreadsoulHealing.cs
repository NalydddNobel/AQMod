using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public class BreadsoulHealing : ModProjectile
    {
        public static void SpawnCluster(Player owner, Vector2 position, int amount, int hpHeal = 30)
        {
            int buffTime = hpHeal * 60 / amount;
        }
    }
}