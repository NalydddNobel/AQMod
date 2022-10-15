using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Projectiles
{
    public class CustomTombstonesGlobalProj : GlobalProjectile
    {
        public static List<int> HellTombstones { get; private set; }

        public override void Load()
        {
            HellTombstones = new List<int>();
        }

        public override void Unload()
        {
            HellTombstones?.Clear();
            HellTombstones = null;
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_Misc misc && misc.Context == "PlayerDeath_TombStone")
            {
                var player = Main.player[projectile.owner];
                if (player.position.Y > Main.UnderworldLayer * 16f)
                {
                    projectile.Aequus().transform = Main.rand.Next(HellTombstones);
                }
            }
        }
    }
}