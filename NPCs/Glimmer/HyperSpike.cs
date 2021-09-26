using AQMod.Assets.Textures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.NPCs.Glimmer
{
    public class HyperSpike : ModProjectile
    {
        public override string Texture => AQMod.ModName + "/" + AQTextureAssets.None;

        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.aiStyle = -1;
        }

        public override void AI()
        {
            var npc = Main.npc[(int)projectile.ai[0]];
            if (npc.active && npc.type == ModContent.NPCType<HyperStarite>())
            {
                var armLength = new Vector2(npc.height * npc.scale + npc.ai[3] + 18f, 0f);
                float rotation = npc.rotation + MathHelper.TwoPi / 5f * projectile.ai[1];
                projectile.timeLeft = 16;
                projectile.Center = npc.Center + armLength.RotatedBy(rotation - MathHelper.PiOver2);
            }
            else
            {
                projectile.active = false;
            }
        }
    }
}