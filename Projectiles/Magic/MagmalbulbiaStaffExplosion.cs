using AQMod.Assets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Magic
{
    internal class MagmalbulbiaStaffExplosion : ModProjectile
    {
        public override string Texture => "AQMod/" + LegacyTextureCache.None;

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.timeLeft = 2;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.aiStyle = -1;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (projectile.ai[0] > 0)
            {
                projectile.active = false;
            }
            projectile.ai[0]++;
        }

        public static void Explode(Vector2 center, int size, int dmg, float kb, int owner)
        {
            int p = Projectile.NewProjectile(center, Vector2.Zero, ModContent.ProjectileType<MagmalbulbiaStaffExplosion>(), dmg, kb, owner);
            if (p >= 0)
            {
                Main.projectile[p].width = size * 2;
                Main.projectile[p].height = size * 2;
                Vector2 position = center - new Vector2(size, size);
                Main.projectile[p].position = position;
                Main.PlaySound(SoundID.Item14, center);
                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDust(position, Main.projectile[p].width, Main.projectile[p].height, 31);
                }
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(position, Main.projectile[p].width, Main.projectile[p].height, DustID.Fire);
                }
                Dust.NewDust(position, Main.projectile[p].width, Main.projectile[p].height, 228, 0f, 0f, 0, default(Color), 0.75f);
            }
        }
    }
}