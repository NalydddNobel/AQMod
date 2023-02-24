using Aequus.Buffs;
using Aequus.Buffs.Debuffs;
using Aequus.Common.Utilities.Drawing;
using Aequus.Content;
using Aequus.Events.DemonSiege;
using Aequus.Graphics;
using Aequus.Graphics.Primitives;
using Aequus.Items;
using Aequus.Particles;
using Aequus.Particles.Dusts;
using Aequus.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Events.DemonSiege.Upgrades
{
    [GlowMask]
    public class HamaYumi : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DemonSiegeSystem.RegisterSacrifice(new SacrificeData(ItemID.DemonBow, Type, UpgradeProgressionType.PreHardmode));
        }

        public override void SetDefaults()
        {
            Item.damage = 32;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.width = 30;
            Item.height = 30;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemDefaults.RarityDemonSiege;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 12.5f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
            Item.value = ItemDefaults.ValueDemonSiege;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.knockBack = 6f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor.MaxRGBA(200);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<HamaYumiArrow>();
        }
    }
}

namespace Aequus.Projectiles.Ranged
{
    public class HamaYumiArrow : ModProjectile
    {
        public TrailRenderer prim;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
            this.SetTrail(10);
            PushableEntities.AddProj(Type);
            AequusProjectile.HeatDamage.Add(Type);
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.timeLeft = 28;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 200;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo ammo)
            {
                Projectile.ai[0] = ContentSamples.ItemsByType[ammo.AmmoItemIdUsed].shoot;
            }
            else
            {
                Projectile.ai[0] = ProjectileID.WoodenArrowFriendly;
            }
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 20;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            CorruptionHellfire.AddBuff(target, 120);
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_Death(), target.Center, Vector2.Normalize(Projectile.velocity) * 0.01f, ModContent.ProjectileType<HamaYumiExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner, target.whoAmI + 1);
            }
            Projectile.damage = (int)(Projectile.damage * 0.75f);
        }
        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            AequusBuff.ApplyBuff<CorruptionHellfire>(target, 120, out bool canPlaySound);
            if (canPlaySound)
            {
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    PacketSystem.SyncSound(SoundPacket.InflictBurning2, target.Center);
                }
                SoundEngine.PlaySound(BlueFire.InflictDebuffSound.WithPitch(-0.2f));
            }
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_Death(), target.Center, Vector2.Normalize(Projectile.velocity) * 0.01f, ModContent.ProjectileType<HamaYumiExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0);
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 2;
            height = 2;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            }
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Normalize(Projectile.velocity) * 0.01f, ModContent.ProjectileType<HamaYumiExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            return true;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity * 2f, (int)Projectile.ai[0], Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var origin = new Vector2(texture.Width / 2f, 8f);

            if (prim == null)
            {
                prim = TrailRenderer.NewRenderer(Projectile, 1, 8f, CorruptionHellfire.FireColor * 3);
            }

            prim.Draw(Projectile.oldPos);
            var frame = Projectile.Frame();

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, Color.White * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

            float opacity = 1f;
            if (Projectile.timeLeft < 12)
            {
                opacity = Projectile.timeLeft / 12f;
            }
            frame.Y += frame.Height;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, CorruptionHellfire.FireColor * Projectile.Opacity * opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class HamaYumiExplosion : ModProjectile
    {
        public override string Texture => "Aequus/Assets/Explosion1";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.DefaultToExplosion(90, DamageClass.Ranged, 20);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return CorruptionHellfire.BloomColor.UseA(0) * 5;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return (target.whoAmI + 1) == (int)Projectile.ai[0] ? false : null;
        }

        public override void AI()
        {
            if (Projectile.frame == 0 && Main.netMode != NetmodeID.Server)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                for (int i = 0; i < 5; i++)
                {
                    var v = Main.rand.NextVector2Unit();
                    ParticleSystem.New<BloomParticle>(ParticleLayer.BehindPlayers).Setup(Projectile.Center + v * Main.rand.NextFloat(16f), v * Main.rand.NextFloat(3f, 12f),
                        CorruptionHellfire.FireColor, CorruptionHellfire.BloomColor, 1.25f, 0.3f);
                }
                for (int i = 0; i < 15; i++)
                {
                    var v = Main.rand.NextVector2Unit();
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<VoidDust>(), v * Main.rand.NextFloat(1f, 12f), 0,
                        new Color(175, 50, 255), Main.rand.NextFloat(0.4f, 1.5f));
                }
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 2)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Type])
                {
                    Projectile.hide = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int _);
            Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
