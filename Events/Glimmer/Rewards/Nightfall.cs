using Aequus.Buffs;
using Aequus.Content;
using Aequus.Items;
using Aequus.Particles.Dusts;
using Aequus.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Events.Glimmer.Rewards
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
    }

    public class NightfallDebuff : ModBuff
    {
        public override string Texture => Aequus.PlaceholderDebuff;

        public static SoundStyle InflictDebuffSound => SoundID.Item4.WithPitch(0.6f).WithVolume(0.5f);

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            AequusBuff.PlayerStatusBuff.Add(Type);
        }

        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            var aequus = npc.Aequus();
            aequus.nightfallStacks = (byte)Math.Min(aequus.nightfallStacks + 1, 20);
            return false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            var aequus = npc.Aequus();
            aequus.nightfallStacks = Math.Max(aequus.nightfallStacks, (byte)1);
        }

        public static void AddBuff(NPC npc, int time)
        {
            AequusBuff.ApplyBuff<NightfallDebuff>(npc, time, out bool canPlaySound);
            if (canPlaySound)
            {
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    PacketSystem.SyncSound(SoundPacket.InflictNightfall, npc.Center);
                }
                SoundEngine.PlaySound(InflictDebuffSound, npc.Center);
            }
        }
    }

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
            Projectile.timeLeft = 60;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (Projectile.ai[0] > 0f)
            {
                Projectile.timeLeft = 60;
                Projectile.alpha = 0;
                if (Projectile.localAI[0] == 0f)
                {
                    Projectile.Center = Main.npc[(int)Projectile.ai[0] - 1].Center + Projectile.velocity;
                    Projectile.velocity = Vector2.Zero;
                    Projectile.localAI[0] = Main.rand.NextFloat(MathHelper.TwoPi);
                    for (int i = 0; i < 10; i++)
                    {
                        var d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<MonoDust>(), newColor: Color.Lerp(Color.White, Color.HotPink, 0.6f).UseA(75), Scale: Main.rand.NextFloat(1f, 1.5f));
                        d.velocity = (Projectile.localAI[0] + MathHelper.PiOver2).ToRotationVector2() * i / 2f * (Main.rand.NextBool() ? -1f : 1f);
                    }
                }
                Projectile.rotation = Projectile.localAI[0];
                Projectile.scale += 0.33f;
                if (Projectile.scale > 3f)
                {
                    Projectile.Kill();
                }
                return;
            }
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
            Projectile.velocity *= 0.98f;
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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.velocity = (Projectile.Center - target.Center) * 0.5f;
            Projectile.ai[0] = target.whoAmI + 1f;
            Projectile.damage = 0;
            NightfallDebuff.AddBuff(target, 600);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int trailLength);
            Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, Color.Lerp(Color.White, Color.HotPink, 0.6f).UseA(150) * Projectile.Opacity, Projectile.rotation, origin, new Vector2(1f, Projectile.scale) * 0.4f, SpriteEffects.None, 0f);
            return false;
        }
    }
}