using Aequus.Projectiles.Misc;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Ranged {
    public class FlameblasterWind : PumpinatorProj
    {
        public override bool OnlyPushHostilePlayers => true;
        public override bool PushUIObjects => false;
        public override bool PushItems => false;
        public override bool OnlyPushHostileProjectiles => true;
        public override bool PushMyProjectiles => true;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 90;
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.extraUpdates = 3;
        }

        public override float GetWindSpeed(Vector2 entityLocation, Vector2 entityVelocity, Vector2 wantedVelocity)
        {
            return Math.Max(entityVelocity.Length(), wantedVelocity.Length() * (Projectile.extraUpdates + 1));
        }
    }
}