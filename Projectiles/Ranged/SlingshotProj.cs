using Aequus.Graphics.Primitives;
using Aequus.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Ranged
{
    public class SlingshotProj : ModProjectile
    {

        public int ItemTexture = ItemID.Bird;
        public bool _playedSound;

        public virtual float Gravity => 0.1f;

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
                var position = Main.GetPlayerArmPosition(Projectile);
                float rotation = Projectile.direction == -1 ? Projectile.velocity.ToRotation() - MathHelper.Pi : Projectile.velocity.ToRotation();
                Projectile.Center = position;
                if (Main.myPlayer == Projectile.owner)
                {
                    var v = Projectile.velocity;
                    Projectile.velocity = Vector2.Normalize(Main.MouseWorld - Projectile.Center);

                    if (v.X != Projectile.velocity.X || v.Y != Projectile.velocity.Y)
                        Projectile.netUpdate = true;

                    GetShootingStats(out int predictiveTrailType, out float speed, out float ai0);

                    if (player.ownedProjectileCounts[predictiveTrailType] == 0)
                    {
                        player.ownedProjectileCounts[predictiveTrailType]++;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * speed, predictiveTrailType, 0, 0f, Projectile.owner, ai0);
                    }
                }
                Projectile.rotation = Projectile.velocity.ToRotation();
                AequusHelpers.ShootRotation(Projectile, MathHelper.WrapAngle(Projectile.rotation + (float)Math.PI / 2f));
                Projectile.Center -= new Vector2(-4f * Projectile.spriteDirection, 20f * player.gravDir).RotatedBy(rotation);
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
                    Projectile.ai[0] = 1f;
                    Projectile.netUpdate = true;
                }
                if (!_playedSound)
                {
                    SoundEngine.PlaySound(new SoundStyle("Aequus/Sounds/Items/Slingshot/shoot", 2), player.Center);
                    _playedSound = true;
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
            predictiveTrailType = ModContent.ProjectileType<BasicBirdPredictionLine>();
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
            if (Main.player[Projectile.owner].channel && (int)Projectile.ai[0] == 0)
            {
                DrawHeldSlingshot();
            }
            else
            {
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor),
                    Projectile.spriteDirection == -1 ? Projectile.rotation - MathHelper.Pi : Projectile.rotation, texture.Size() / 2f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
            return false;
        }

        public virtual void DrawHeldSlingshot()
        {
            Main.instance.LoadItem(ModContent.ItemType<Slingshot>());
            var texture = TextureAssets.Item[ModContent.ItemType<Slingshot>()];
            var topMask = TextureAssets.Projectile[Type];

            var position = Main.GetPlayerArmPosition(Projectile);
            var drawCoords = position;
            float rotation = Projectile.spriteDirection == -1 ? Projectile.velocity.ToRotation() - MathHelper.Pi : Projectile.velocity.ToRotation();
            var origin = new Vector2(topMask.Value.Width / 2f, topMask.Value.Height);
            int grav = (int)Main.player[Projectile.owner].gravDir;
            if (grav == -1)
            {
                rotation -= MathHelper.Pi;
            }
            var spriteEffects = Main.player[Projectile.owner].direction == -1 * grav ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(texture.Value, drawCoords - Main.screenPosition, null, AequusHelpers.GetColor(drawCoords),
                 rotation, origin, Projectile.scale, spriteEffects, 0);

            Main.EntitySpriteDraw(TextureAssets.Item[ItemTexture].Value, drawCoords - new Vector2(-4f * Projectile.spriteDirection * grav, (origin.Y - 10f + Math.Max(TextureAssets.Item[ItemTexture].Value.Height - 20, 0))).RotatedBy(rotation) - Main.screenPosition, null, Projectile.GetAlpha(AequusHelpers.GetColor(drawCoords)),
                Projectile.spriteDirection == -1 ? Projectile.rotation - MathHelper.Pi : Projectile.rotation + (grav == -1 ? -MathHelper.Pi : 0f), TextureAssets.Item[ItemTexture].Size() / 2f, Projectile.scale, Projectile.spriteDirection == 1 * grav ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            Main.EntitySpriteDraw(topMask.Value, drawCoords - Main.screenPosition, null, AequusHelpers.GetColor(drawCoords),
                 rotation, origin, Projectile.scale, spriteEffects, 0);
        }
    }

    public class BasicBirdPredictionLine : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;
        public virtual int TrailTicks => 120;
        public TrailRenderer testTrailRenderer;

        public override void SetStaticDefaults()
        {
            this.SetTrail(TrailTicks);
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.noEnchantments = true;
            Projectile.timeLeft = TrailTicks * 4;
            Projectile.extraUpdates = TrailTicks;
            Projectile.ignoreWater = true;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            this.SetTrail(TrailTicks);
            if (Projectile.numUpdates == Projectile.extraUpdates - 1)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].owner == Projectile.owner && (int)Main.projectile[i].ai[0] == 0 && Main.projectile[i].ModProjectile is SlingshotProj slingshot)
                    {
                        slingshot.GetShootingStats(out int type, out float speed, out float grav);
                        if (type != Projectile.type)
                            continue;
                        for (int k = 0; k < Projectile.oldPos.Length; k++)
                        {
                            Projectile.oldPos[k] = Vector2.Zero;
                        }
                        Projectile.Center = Main.projectile[i].Center;
                        if (!Projectile.netUpdate)
                            Projectile.netUpdate = Main.projectile[i].netUpdate;
                        Projectile.velocity = Vector2.Normalize(Main.projectile[i].velocity) * speed;
                        Projectile.ai[0] = grav;
                        Projectile.timeLeft = 3;
                        break;
                    }
                }
                if (Projectile.timeLeft != 3)
                {
                    Projectile.Kill();
                }
                return;
            }
            Projectile.timeLeft = 2;
            Projectile.velocity.Y += Projectile.ai[0];
            if (Projectile.velocity.Y > 20f)
            {
                Projectile.velocity.Y = 20f;
            }
            //if (Main.GameUpdateCount % 30 == 0)
            //    AequusHelpers.dustDebug(Projectile.position, DustID.CursedTorch);

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X && oldVelocity.X.Abs() > 2f)
            {
                Projectile.velocity.X = oldVelocity.X * -0.8f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y.Abs() > 2f)
            {
                Projectile.velocity.Y = oldVelocity.Y * -0.8f;
            }
            Projectile.velocity *= 0.8f;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Main.NewText("...");
            if (testTrailRenderer == null)
                testTrailRenderer = new TrailRenderer(TextureCache.Trail[1].Value, TrailRenderer.DefaultPass, (f) => new Vector2(4f), (f) => Color.White * f, drawOffset: Projectile.Size / 2f);
            testTrailRenderer.Draw(Projectile.oldPos);
            return false;
        }
    }
}