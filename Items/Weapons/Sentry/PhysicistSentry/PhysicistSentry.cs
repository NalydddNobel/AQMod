using Aequus;
using Aequus.Common.Items;
using Aequus.Common.Items.Variants;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Items;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Sentry.PhysicistSentry {
    public class PhysicistSentry : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 25;
            AequusItemVariants.AddVariant(Type, ItemVariants.StrongerVariant, Condition.RemixWorld);
        }

        public override void SetDefaults() {
            Item.width = 16;
            Item.height = 16;
            Item.SetWeaponValues(20, 10f);
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.value = Item.buyPrice(silver: 10);
            Item.rare = ItemDefaults.RarityGlimmer;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<PhysicistSentryProj>();
            Item.shootSpeed = 10f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.consumable = true;
            Item.maxStack = Item.CommonMaxStack;

            if (Item.Variant == ItemVariants.StrongerVariant) {
                Item.damage = 35;
                Item.rare = ItemDefaults.RarityDungeon;
                Item.value = Item.buyPrice(gold: 1);
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            player.UpdateMaxTurrets();
            return true;
        }
    }

    public class PhysicistSentryProj : ModProjectile {
        public const int STATE_Thrown = 0;
        public const int STATE_Setup = 1;
        public const int STATE_Sentry = 2;

        public int State { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int Timer { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }

        public virtual int ProjectileShot => ModContent.ProjectileType<PhysicistSentryLightning>();

        public override void SetStaticDefaults() {
            this.SetTrail(4);
            Main.projFrames[Type] = 7;
        }

        public override void SetDefaults() {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.sentry = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = Projectile.SentryLifeTime;
        }

        public virtual void OnSetup() {
            Main.player[Projectile.owner].UpdateMaxTurrets();
        }

        public virtual int GetTarget(Vector2 projectilePosition) {
            return Projectile.GetMinionTarget(projectilePosition, out _, maxDistance: 500f);
        }

        public override void AI() {
            switch (State) {
                case STATE_Thrown: {
                        Projectile.rotation += Projectile.velocity.Length() * 0.1f;
                    }
                    break;

                case STATE_Sentry: {
                        int frameSpeed = 20;

                        var projectilePosition = Projectile.Bottom + new Vector2(0f, -30f);
                        int target = GetTarget(projectilePosition);

                        if (target == -1) {
                            Timer = 0;
                        }
                        else {
                            Timer++;
                        }
                        if (Timer > 50) {
                            frameSpeed /= 4;
                        }
                        if (Timer > 70) {
                            frameSpeed /= 2;
                        }
                        if (Timer >= 80 && Timer % 10 == 0) {
                            SoundEngine.PlaySound(SoundID.Item12, Projectile.Center);
                            Projectile.netUpdate = true;

                            if (Main.myPlayer == Projectile.owner) {
                                Projectile.NewProjectile(
                                    Projectile.GetSource_FromThis(),
                                    projectilePosition,
                                    Vector2.Normalize(Main.npc[target].Center - projectilePosition) * 10f,
                                    ProjectileShot,
                                    Projectile.damage,
                                    Projectile.knockBack,
                                    Projectile.owner
                                );
                            }
                        }
                        if (Timer > 100) {
                            Timer = 0;
                        }
                        Projectile.frameCounter++;
                        if (Projectile.frameCounter > frameSpeed) {
                            Projectile.frameCounter = 0;
                            Projectile.frame = Projectile.frame == 5 ? 6 : 5;
                        }
                    }
                    break;

                case STATE_Setup: {
                        if (Timer < 1) {
                            OnSetup();
                            break;
                        }
                        if (Timer == 1) {
                            SoundEngine.PlaySound(SoundID.DD2_DefenseTowerSpawn, Projectile.Center);
                        }

                        Timer++;
                        if (Timer > 60) {
                            Projectile.rotation = 0f;
                            Projectile.frameCounter++;
                            if (Projectile.frameCounter > 4) {
                                Projectile.frame++;
                                Projectile.frameCounter = 0;
                                if (Projectile.frame > 4) {
                                    Projectile.netUpdate = true;
                                    State = STATE_Sentry;
                                    Timer = 0;
                                }
                            }
                        }
                        else if (Timer >= 30) {
                            if (Timer == 30) {
                                Projectile.velocity.Y = -4f;
                            }
                            Projectile.rotation %= MathHelper.Pi;
                            Projectile.rotation *= 0.8f;
                        }
                        else {
                            Projectile.rotation += Projectile.velocity.Length() * 0.1f;
                        }
                    }
                    break;
            }
            Projectile.velocity.Y += 0.3f;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
            fallThrough = false;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            int state = State;
            if (state == STATE_Thrown && Projectile.velocity.Length() > 5f) {
                if (Projectile.velocity.X != oldVelocity.X) {
                    Projectile.velocity.X = -oldVelocity.X;
                }
                if (Projectile.velocity.Y != oldVelocity.Y) {
                    Projectile.velocity.Y = -oldVelocity.Y * 0.5f;
                }
            }
            else if (state == STATE_Thrown) {
                State = STATE_Setup;
                Timer = 0;
                Projectile.netUpdate = true;
            }
            else if (state == STATE_Setup && oldVelocity.Y != Projectile.velocity.Y) {
                if (Timer < 1) {
                    Timer = 1;
                    Projectile.netUpdate = true;
                }
            }

            Projectile.velocity.X *= 0.66f;
            return false;
        }

        public override bool PreDraw(ref Color lightColor) {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int trailLength);
            origin.Y = 42f;
            var drawPosition = Projectile.position + offset - Main.screenPosition;
            for (int i = 0; i < trailLength; i++) {
                var p = Helper.CalcProgress(trailLength, i);
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + offset - Main.screenPosition, frame, lightColor with { A = 0 } * p * 0.2f, Projectile.oldRot[i], origin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture, drawPosition, frame, lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(AequusTextures.PhysicistSentryProj_Glow, drawPosition, frame, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }

    public class PhysicistSentryLightning : ModProjectile {
        public override string Texture => $"{Aequus.VanillaTexture}Projectile_{ProjectileID.ThunderSpearShot}";

        public Vector2 baseVelocity;

        public override void SetStaticDefaults() {
            Main.projFrames[Type] = Main.projFrames[ProjectileID.ThunderSpearShot];
            this.SetTrail(10);
            ProjectileID.Sets.SentryShot[Type] = true;
        }

        public override void SetDefaults() {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 40;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.penetrate = -1;
        }

        public override void AI() {
            if (baseVelocity == Vector2.Zero) {
                baseVelocity = Projectile.velocity;
            }
            Projectile.velocity = baseVelocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f));
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.numUpdates == -1) {
                Projectile.LoopingFrame(3);
            }
            Projectile.Opacity -= 0.02f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            Projectile.NewProjectile(
                Projectile.GetSource_OnHit(target),
                Projectile.Center,
                Projectile.velocity.RotatedBy(Main.rand.NextFloat(-1f, 1f)),
                Type,
                (int)(Projectile.damage * 0.95f),
                Projectile.knockBack,
                Projectile.owner
            );
            Projectile.ai[0] = 1f;
            Projectile.damage = 0;
            Projectile.originalDamage = 0;
            Projectile.velocity *= 0.5f;
        }

        public override bool PreDraw(ref Color lightColor) {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int trailLength);
            var drawPosition = Projectile.position + offset - Main.screenPosition;
            var drawColor = Color.White with { A = 0 } * Projectile.Opacity;
            for (int i = 0; i < trailLength; i++) {
                var p = Helper.CalcProgress(trailLength, i);
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + offset - Main.screenPosition, frame, drawColor * p, Projectile.oldRot[i], origin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture, drawPosition, frame, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}