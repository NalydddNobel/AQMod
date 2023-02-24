using Aequus;
using Aequus.Buffs;
using Aequus.Buffs.Debuffs;
using Aequus.Content;
using Aequus.Items;
using Aequus.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Events.DemonSiege.Upgrades
{
    [GlowMask]
    [LegacyName("BallisticScreecher")]
    public class BombarderRod : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
            DemonSiegeSystem.RegisterSacrifice(new SacrificeData(ItemID.CrimsonRod, Type, UpgradeProgressionType.PreHardmode));
        }

        public override void SetDefaults()
        {
            Item.SetWeaponValues(18, 2f, 11);
            Item.DamageType = DamageClass.Magic;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.width = 32;
            Item.height = 32;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemDefaults.RarityDemonSiege;
            Item.shoot = ModContent.ProjectileType<BombarderRodProj>();
            Item.shootSpeed = 8.5f;
            Item.mana = 6;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item88.WithVolume(0.5f).WithPitchOffset(0.8f);
            Item.value = ItemDefaults.ValueDemonSiege;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += Vector2.Normalize(velocity) * 38f;
            velocity = velocity.RotatedBy(Main.rand.NextFloat(-0.05f, 0.05f));
        }
    }
}

namespace Aequus.Projectiles.Magic
{
    public class BombarderRodProj : ModProjectile
    {
        public override string Texture => $"{Aequus.VanillaTexture}Projectile_{ProjectileID.Flamelash}";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = Main.projFrames[ProjectileID.Flamelash];
            this.SetTrail(16);
            PushableEntities.AddProj(Type);
            AequusProjectile.HeatDamage.Add(Type);
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 120;
            Projectile.alpha = 200;
        }

        public override void AI()
        {
            this.SetTrail(16);
            if ((int)Projectile.ai[0] == 0)
            {
                int dir = Main.rand.NextBool().ToDirectionInt();
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.PiOver2 * dir * 0.2f);
                Projectile.ai[1] = dir;
                Projectile.ai[0] += Main.rand.NextFloat(-5f, 5f);
                Projectile.netUpdate = true;
            }
            if (Projectile.alpha < 200 && Main.rand.NextBool(2))
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Scale: Main.rand.NextFloat(2f, 2.66f) * Projectile.Opacity);
                d.velocity *= 0.5f;
                d.velocity -= Projectile.velocity * 0.2f;
                d.noGravity = true;
            }
            if (Projectile.alpha < 200 && Main.rand.NextBool(5))
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Scale: Main.rand.NextFloat(0.5f, 1.66f) * Projectile.Opacity);
                d.velocity *= 0.5f;
                d.velocity -= Projectile.velocity * 0.2f;
                d.noGravity = true;
            }
            if (Projectile.timeLeft < 100)
            {
                Projectile.velocity *= 0.995f;
            }
            if (Projectile.timeLeft < 50)
            {
                Projectile.alpha += 6;
                Projectile.scale -= 0.0075f;
                Projectile.velocity *= 0.99f;
            }
            else
            {
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 5;
                    if (Projectile.alpha < 0)
                    {
                        Projectile.alpha = 0;
                    }
                }
            }

            var target = Projectile.FindTargetWithLineOfSight(240f);
            if (target != -1)
            {
                Projectile.velocity = Vector2.Normalize(Vector2.Lerp(Projectile.velocity,
                    Vector2.Normalize(Main.npc[target].Center - Projectile.Center) * Projectile.velocity.Length(), 0.05f)) * Projectile.velocity.Length();
            }
            else if (Projectile.ai[0] < 25f)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(0.04f * -Projectile.ai[1] * (Math.Max(Projectile.ai[0], 0f) / 25f));
            }
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            if (Collision.SolidCollision(Projectile.position - new Vector2(32f, 32f), Projectile.width + 64, Projectile.height + 64))
            {
                Projectile.ai[0]++;
            }
            if (Collision.SolidCollision(Projectile.position - new Vector2(16f, 16f), Projectile.width + 32, Projectile.height + 32))
            {
                Projectile.ai[0]++;
            }
            Projectile.ai[0]++;
            if ((int)Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = 1;
            }
        }

        public void AddBuffToPlayer(Player player)
        {
            AequusBuff.ApplyBuff<CrimsonHellfire>(player, 300, out bool canPlaySound);
            if (canPlaySound)
            {
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    PacketSystem.SyncSound(SoundPacket.InflictBurning2, player.Center);
                }
                SoundEngine.PlaySound(BlueFire.InflictDebuffSound.WithPitch(-0.2f));
            }
        }
        public void OnHit(Entity target)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (i == target.whoAmI)
                {
                    continue;
                }
                if (Main.npc[i].active && !Main.npc[i].friendly && Main.npc[i].Distance(target.Center) < 180f)
                {
                    CrimsonHellfire.AddBuff(Main.npc[i], 300);
                }
            }
            if (Main.player[Projectile.owner].hostile)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (Main.player[i].active && !Main.player[i].dead && !Main.player[i].ghost && Main.player[i].hostile && Main.player[i].team != Main.player[Projectile.owner].team
                        && Main.player[i].Distance(target.Center) < 180f)
                    {
                        AddBuffToPlayer(Main.player[i]);
                    }
                }
            }
            for (int i = 0; i < 75; i++)
            {
                var d = Dust.NewDustDirect(target.position, target.width, target.height, DustID.Torch, Scale: Main.rand.NextFloat(1f, 4f) * Projectile.Opacity);
                d.velocity *= 0.5f;
                d.velocity = Vector2.Normalize(target.Center - d.position) * Main.rand.NextFloat() * 16f;
                d.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (crit)
            {
                CrimsonHellfire.AddBuff(target, 300);
                OnHit(target);
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (crit)
            {
                AddBuffToPlayer(target);
                OnHit(target);
            }
        }
        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (crit)
            {
                AddBuffToPlayer(target);
                OnHit(target);
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 10;
            height = 10;
            fallThrough = true;
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var textureFrame = Projectile.Frame();
            var textureOrigin = textureFrame.Size() / 2f;

            var bloom = Textures.Bloom[0].Value;
            var bloomFrame = bloom.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            var bloomOrigin = bloomFrame.Size() / 2f;

            var center = Projectile.Center;
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            int trailLength = ProjectileID.Sets.TrailCacheLength[Type];
            for (int i = 0; i < trailLength; i++)
            {
                var p = AequusHelpers.CalcProgress(trailLength, i);
                Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.oldPos[i] + offset - Main.screenPosition, textureFrame,
                    Color.Lerp(new Color(255, 255, 255, 128), new Color(255, 10, 10, 128), 1f - p) * Projectile.Opacity * p * p * 0.5f, Projectile.rotation, textureOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(bloom, Projectile.position + offset - Main.screenPosition, null, CrimsonHellfire.FireColor * Projectile.Opacity * 2f,
                Projectile.rotation, bloomOrigin, Projectile.scale * 0.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.position + offset - Main.screenPosition, textureFrame,
               Color.White * Projectile.Opacity, Projectile.rotation, textureOrigin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode == NetmodeID.Server || (Projectile.alpha > 128 && Projectile.ai[0] > 25f))
            {
                return;
            }
            SoundEngine.PlaySound(SoundID.Item89.WithVolume(0.75f).WithPitchOffset(0.5f), Projectile.Center);
            var center = Projectile.Center;
            for (int i = 0; i < 20; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                d.velocity = (d.position - center) / 8f;
                d.noGravity = true;
                if (Main.rand.NextBool(3))
                {
                    d.velocity *= 2f;
                    d.scale *= 1.75f;
                    d.fadeIn = d.scale + Main.rand.NextFloat(0.5f, 0.75f);
                }
            }
        }
    }
}