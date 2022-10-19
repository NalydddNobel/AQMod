using Aequus.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Melee
{
    public class PhaseDiscProj : ValariProj
    {
        public override string Texture => AequusHelpers.GetPath<PhaseDisc>();

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.extraUpdates = 2;
            Projectile.manualDirectionChange = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 1200;
        }

        public override void AI()
        {
            if ((int)Projectile.ai[0] == 0)
            {
                Projectile.direction = Projectile.velocity.X < 0f ? -1 : 1;
                switch (Main.GetMoonPhase())
                {
                    case MoonPhase.Full:
                        Projectile.penetrate += 3;
                        break;
                    case MoonPhase.ThreeQuartersAtLeft:
                    case MoonPhase.ThreeQuartersAtRight:
                        Projectile.penetrate += 2;
                        break;
                    case MoonPhase.HalfAtLeft:
                    case MoonPhase.HalfAtRight:
                        Projectile.penetrate += 1;
                        break;
                }
            }
            Projectile.ai[0]++;
            if (Projectile.ai[0] > 60f)
                Projectile.tileCollide = false;
            if (Projectile.ai[0] > 46f)
            {
                float speed = Math.Max((Main.player[Projectile.owner].Center - Projectile.Center).Length() / 60f, 10f) + Projectile.ai[0] * 0.0003f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Main.player[Projectile.owner].Center - Projectile.Center) * speed, Math.Max(1f - (Main.player[Projectile.owner].Center - Projectile.Center).Length() / 40f, 0.04f));
                if ((Projectile.Center - Main.player[Projectile.owner].Center).Length() < 20f)
                {
                    Projectile.Kill();
                }
            }
            Projectile.rotation += 0.125f;
            var hitbox = Projectile.getRect();
            for (int i = 0; i < Main.maxItems; i++)
            {
                if (Main.item[i] != null && Main.item[i].active && Main.item[i].getRect().Intersects(hitbox))
                {
                    Main.item[i].Center = Main.player[Projectile.owner].Center;
                    Main.timeItemSlotCannotBeReusedFor[i] = 2;
                    if (!Main.item[i].instanced)
                    {
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i);
                    }
                    break;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 offset = new Vector2(Projectile.width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var color = new Color(40, 80, 150, 40);
            var origin = frame.Size() / 2f;
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                float progress = 1f / ProjectileID.Sets.TrailCacheLength[Projectile.type] * i;
                Main.spriteBatch.Draw(texture, Projectile.oldPos[i] + offset - Main.screenPosition, frame, color * (1f - progress), Projectile.rotation, origin, Math.Max(Projectile.scale * (1f - progress), 0.1f), effects, 0f);
            }

            Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, null, new Color(255, 255, 255, 255), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}