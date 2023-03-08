using Aequus.Common.Net;
using Aequus.Common.Recipes;
using Aequus.Content;
using Aequus.Items.Weapons.Magic;
using Aequus.Items.Weapons.Summon.Minion;
using Aequus.Particles;
using Aequus.Particles.Dusts;
using Aequus.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic
{
    [LegacyName("WowHat")]
    public class Nightfall : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.SetWeaponValues(9, 1f);
            Item.DefaultToMagicWeapon(ModContent.ProjectileType<NightfallProj>(), 24, 12f, true);
            Item.mana = 10;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemDefaults.RarityGlimmer;
            Item.value = ItemDefaults.ValueGlimmer;
            Item.UseSound = SoundID.Item8;
        }

        public override void AddRecipes()
        {
            AequusRecipes.CreateShimmerTransmutation(Type, ModContent.ItemType<StariteStaff>());
        }
    }

    public class NightfallOnHitPacket : PacketHandler
    {
        public override PacketType LegacyPacketType => PacketType.NightfallOnHit;

        public void Send(NPC npc)
        {
            var p = GetPacket();
            p.Write((byte)npc.whoAmI);
            p.Send();
        }

        public override void Receive(BinaryReader reader)
        {
            byte npc = reader.ReadByte();
            if (npc >= Main.maxNPCs)
                return;

            if (Main.netMode == NetmodeID.Server)
            {
                Send(Main.npc[npc]);
            }

            NightfallProj.ApplyOnHitEffect(Main.npc[npc], out int _);
        }
    }
}

namespace Aequus.Projectiles.Magic
{
    public class NightfallProj : ModProjectile
    {
        public override string Texture => $"{Aequus.VanillaTexture}Projectile_{ProjectileID.RainbowCrystalExplosion}";

        public override void SetStaticDefaults()
        {
            PushableEntities.AddProj(Type);
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 30;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.timeLeft < 30)
            {
                Projectile.alpha += 8;
                if (Projectile.alpha > 255)
                    Projectile.alpha = 255;
            }
            else if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 20;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 4;
            height = 4;
            fallThrough = true;
            return true;
        }

        public override void Kill(int timeLeft)
        {
        }

        public static void ApplyOnHitEffect(NPC target, out int fallDamage)
        {
            int j = 0;
            var oldPosition = target.position;
            var tileCoordinates = target.Bottom.ToTileCoordinates();
            int x = tileCoordinates.X;
            for (; j < 30; j++)
            {
                int y = tileCoordinates.Y + j;
                if (!WorldGen.InWorld(x, y, fluff: 40))
                    break;

                if (Main.tile[x, y].IsFullySolid())
                    break;
            }

            fallDamage = 0;
            if (j <= 1)
                return;

            float damage = j * 5;
            if (damage > 20)
            {
                // all damage above 20 only adds 2.5
                damage = Helper.ScaleDown(damage, 20f, 0.5f);
            }
            if (damage > 50)
            {
                // all damage above 50 only adds 1.25
                damage = Helper.ScaleDown(damage, 50f, 0.5f);
            }

            fallDamage = (int)damage;

            target.velocity.Y -= 3f;
            target.position.Y = (tileCoordinates.Y + j) * 16f - target.height;

            for (int k = 0; k < j; k++)
            {
                ParticleSystem.New<DashBlurParticle>(ParticleLayer.AboveNPCs)
                    .Setup(
                    new(oldPosition.X + Main.rand.Next(target.width), oldPosition.Y + k * 16f + Main.rand.Next(target.height)),
                    Vector2.UnitY * Main.rand.NextFloat(4f, 8f),
                    Color.BlueViolet with { A = 60 },
                    2f,
                    0f);

                var d = Dust.NewDustDirect(
                    oldPosition with { Y = oldPosition.Y + k * 16f },
                    target.width,
                    target.height,
                    ModContent.DustType<MonoDust>(),
                    0f, 1f,
                    0, Color.HotPink with { A = 0, }, Scale: Main.rand.NextFloat(0.5f, 1.5f));
                d.velocity.X *= 0.1f;
                d.noGravity = false;
            }
            Collision.HitTiles(target.position with { Y = target.position.Y + target.height, }, Vector2.UnitY, target.width, 16);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.knockBackResist <= 0.02f || target.life <= 0 || target.Hitbox.InSolidCollision())
                return;

            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModContent.GetInstance<NightfallOnHitPacket>().Send(target);
            }
            ApplyOnHitEffect(target, out int fallDamage);

            if (fallDamage > 0)
            {
                target.StrikeNPC(fallDamage, 0f, 0);
                ScreenShake.SetShake(fallDamage / 3f, where: target.Center);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int trailLength);
            Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, Color.Lerp(Color.White, Color.HotPink, 0.6f).UseA(150) * Projectile.Opacity, Projectile.rotation, origin, new Vector2(1f, Projectile.scale) * 0.4f, SpriteEffects.None, 0f);
            return false;
        }
    }
}