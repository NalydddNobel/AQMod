using AQMod.Assets;
using AQMod.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.NPCs.Starite
{
    public class OrbCollider : ModProjectile
    {
        public override string Texture => AQMod.ModName + "/" + TextureCache.None;

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.tileCollide = false;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.hide = true; // seems useless but there's nothing to draw anyways, so why even try to run the draw method at all?
        }

        public override void AI()
        {
            var omegaStarite = Main.npc[(int)projectile.ai[0]];
            if (!omegaStarite.active || omegaStarite.ai[0] == -1f)
                return;
            projectile.timeLeft = 2;
            projectile.Center = omegaStarite.Center;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            var omegaStarite = (OmegaStarite)Main.npc[(int)projectile.ai[0]].modNPC;
            for (int i = 0; i < omegaStarite.orbs.Count; i++)
            {
                Vector2 collisionCenter = new Vector2(omegaStarite.orbs[i].position.X, omegaStarite.orbs[i].position.Y);
                Rectangle rect = Utils.CenteredRectangle(collisionCenter, new Vector2(50f, 50f));
                if (rect.Intersects(targetHitbox))
                    return true;
            }
            return false;
        }
    }
}