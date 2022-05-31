using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Ranged.Birds
{
    public class SlingshotBirdProj : ModProjectile
    {
        public static SoundStyle ShootSound { get; private set; }

        public override string Texture => Aequus.VanillaTexture + "Item_" + ItemID.Bird;

        public int ItemTexture = ItemID.Bird;

        public virtual float Gravity => 0.1f;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                ShootSound = new SoundStyle("Aequus/Sounds/Items/Slingshot/shoot", 2);
            }
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.noEnchantments = true;
            Projectile.timeLeft = 1200;
            Projectile.extraUpdates = 1;
            Projectile.ignoreWater = true;
            ItemTexture = ItemID.Bird;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo itemWithAmmo)
            {
                ItemTexture = itemWithAmmo.AmmoItemIdUsed;
            }
        }

        public override void AI()
        {
            var player = Main.player[Projectile.owner];
            if (player.channel && (int)Projectile.ai[0] == 0)
            {
                player.heldProj = Projectile.whoAmI;
                Projectile.timeLeft = Math.Max(Projectile.timeLeft, 1200);
                player.itemTime = Math.Max(player.itemTime, 16);
                player.itemAnimation = Math.Max(player.itemAnimation, 16);

                Projectile.velocity.Normalize();
                if (Projectile.ai[1] == 0)
                {
                    Projectile.ai[1] = Projectile.velocity.ToRotation();
                }
                Projectile.Center = player.Center + Projectile.ai[1].ToRotationVector2() * 24f;
                Projectile.position.Y -= 10f;
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.velocity = Vector2.Normalize(Main.MouseWorld - Projectile.Center);

                    GetShootingStats(out int predictiveTrailType, out float speed, out float ai0);

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * speed, predictiveTrailType, 0, 0f, Projectile.owner, ai0);
                }
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            else
            {
                Projectile.rotation += Projectile.velocity.Length() * 0.025f * Projectile.direction;
                if (Projectile.ai[0] == 0)
                {
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= Main.player[Projectile.owner].HeldItem.shootSpeed;
                    if (Main.myPlayer == player.whoAmI)
                    {
                        float cursorSpeed = Main.player[Projectile.owner].Distance(Main.MouseWorld);
                        if (cursorSpeed > 320f)
                        {
                            cursorSpeed = 320f;
                        }
                        Projectile.velocity *= cursorSpeed / 320f;
                    }
                    if (Main.netMode != NetmodeID.Server)
                    {
                        SoundEngine.PlaySound(ShootSound, player.Center);
                    }
                    Projectile.ai[0] = 1f;
                    Projectile.netUpdate = true;
                }
                Projectile.velocity.Y += Gravity;
                if (Projectile.velocity.Y > 20f)
                {
                    Projectile.velocity.Y = 20f;
                }
            }
            Projectile.spriteDirection = Projectile.direction;
        }

        public virtual void GetShootingStats(out int predictiveTrailType, out float speed, out float ai0_Gravity)
        {
            predictiveTrailType = ModContent.ProjectileType<BirdProjPrediction>();
            speed = Main.player[Projectile.owner].HeldItem.shootSpeed;
            float cursorSpeed = Main.player[Projectile.owner].Distance(Main.MouseWorld);
            if (cursorSpeed > 320f)
            {
                cursorSpeed = 320f;
            }
            speed *= cursorSpeed / 320f;
            ai0_Gravity = Gravity;
        }

        public override bool? CanDamage()
        {
            return Projectile.velocity.Length() > 2f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X && oldVelocity.X.Abs() > 2f)
            {
                Projectile.velocity.X = oldVelocity.X * -0.9f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y.Abs() > 2f)
            {
                Projectile.velocity.Y = oldVelocity.Y * -0.9f;
            }
            Projectile.velocity *= 0.8f;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadItem(ItemTexture);

            var texture = TextureAssets.Item[ItemTexture].Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor),
                Projectile.spriteDirection == -1 ? Projectile.rotation - MathHelper.Pi : Projectile.rotation, texture.Size() / 2f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            return false;
        }
    }

    public class BirdProjPrediction : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.noEnchantments = true;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 120;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Projectile.velocity.Y += Projectile.ai[0];
            if (Projectile.velocity.Y > 20f)
            {
                Projectile.velocity.Y = 20f;
            }
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<BirdPredictionDust>(), Velocity: Vector2.Zero, newColor: new Color(255, 255, 255, 255));
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X && oldVelocity.X.Abs() > 2f)
            {
                Projectile.velocity.X = oldVelocity.X * -0.75f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y.Abs() > 2f)
            {
                Projectile.velocity.Y = oldVelocity.Y * -0.75f;
            }
            Projectile.velocity *= 0.75f;
            return false;
        }
    }

    public class BirdPredictionDust : ModDust
    {
        public override string Texture => AequusHelpers.GetPath<MonoDust>();

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return new Color(255, 255, 255, 128);
        }

        public override bool Update(Dust dust)
        {
            dust.alpha++;
            if (dust.alpha > 1)
            {
                dust.active = false;
            }
            return false;
        }
    }
}