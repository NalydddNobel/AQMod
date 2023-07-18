using Aequus;
using Aequus.Common.CrossMod;
using Aequus.Common.DataSets;
using Aequus.Common.Items;
using Aequus.Common.Projectiles;
using Aequus.Content;
using Aequus.Items.Equipment.Accessories.Combat.Passive.Stormcloak;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Items.Equipment.Accessories.Combat.Passive.Stormcloak {
    [AutoloadEquip(EquipType.Back, EquipType.Front)]
    public class Stormcloak : ModItem {
        public virtual int SpawnCheck => 60;
        public virtual int Amount => 3;
        public virtual int Cooldown => 300;
        public virtual int Distance => 160;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Amount);

        public override void SetDefaults() {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemDefaults.RarityDustDevil;
            Item.value = ItemDefaults.ValueDustDevil;
            Item.expert = !ModSupportCommons.DoExpertDropsInClassicMode();
            Item.Aequus().itemGravityCheck = 255;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().accStormcloak = this;
        }

        //public static void UpdateAccessory(Item item, Player player, AequusPlayer aequus) {
        //    if (item != null) {
        //        bool onCooldown = player.HasBuff<StormcloakCooldown>();
        //        var l = GetBlowableProjectiles(player, item, onlyMine: onCooldown);
        //        Vector2 widthMethod(float p) => new Vector2(7f) * (float)Math.Sin(p * MathHelper.Pi);
        //        Color colorMethod(float p) => Color.White.UseA(150) * 0.45f * (float)Math.Sin(p * MathHelper.Pi);
        //        for (int i = 0; i < l.Count + (onCooldown ? 0 : 1); i++) {
        //            var v = Main.rand.NextVector2Unit();
        //            var d = Dust.NewDustPerfect(player.Center + v * Main.rand.NextFloat(30f, 100f), ModContent.DustType<MonoDust>(), v.RotatedBy(MathHelper.PiOver2 * player.direction) * Main.rand.NextFloat(8f), newColor: new Color(128, 128, 128, 0));
        //            d.noLight = true;
        //            if (Main.rand.NextBool(3)) {
        //                var prim = new TrailRenderer(TrailTextures.Trail[4].Value, TrailRenderer.DefaultPass, widthMethod, colorMethod);
        //                float rotation = player.direction * 0.45f;
        //                var particle = ParticleSystem.New<StormcloakTrailParticle>(ParticleLayer.AbovePlayers).Setup(prim, player.Center + v * Main.rand.NextFloat(35f, 90f), v.RotatedBy(MathHelper.PiOver2 * player.direction) * 10f,
        //                    scale: Main.rand.NextFloat(0.85f, 1.5f), trailLength: 10);
        //                particle.StretchTrail(v.RotatedBy(MathHelper.PiOver2 * -player.direction) * 2f);
        //                particle.rotationValue = rotation / 4f;
        //                particle.prim.GetWidth = (p) => widthMethod(p) * particle.Scale;
        //                particle.prim.GetColor = (p) => colorMethod(p) * particle.Rotation * Math.Min(particle.Scale, 1.5f);
        //            }
        //        }

        //        if (l.Count > 0) {
        //            if (aequus.accDustDevilExpertThrowTimer == 1) {
        //                SoundEngine.PlaySound(SoundID.DD2_BetsySummon.WithPitchOffset(-0.44f).WithVolumeScale(2f), player.Center);
        //            }
        //            if (!onCooldown)
        //                aequus.accDustDevilExpertThrowTimer++;
        //            foreach (var proj in l) {
        //                proj.position += player.velocity * 0.95f;
        //                var v = proj.DirectionTo(player.Center);
        //                float size = Math.Max(proj.Size.Length(), player.Size.Length()) + proj.velocity.Length();
        //                if (proj.Distance(player.Center) < size) {
        //                    proj.Center = player.Center - v * size;
        //                }
        //                int i = proj.FindTargetWithLineOfSight();
        //                float tornadoValue = 0.8f;
        //                if (onCooldown && i != -1) {
        //                    var toEnemy = proj.DirectionTo(Main.npc[i].Center);
        //                    proj.velocity = Vector2.Normalize(Vector2.Lerp(proj.velocity, toEnemy, 0.8f)) * proj.velocity.Length();
        //                    if ((proj.velocity - toEnemy * proj.velocity.Length()).Length() < 16f) {
        //                        tornadoValue = 0.2f;
        //                    }
        //                }
        //                proj.velocity = Vector2.Normalize(Vector2.Lerp(proj.velocity, v.RotatedBy((MathHelper.PiOver2 - 0.1f) * -player.direction) + v,
        //                    Math.Clamp(1f - proj.Distance(player.Center) / 500f, 0f, tornadoValue))) * Math.Clamp(proj.velocity.Length() + 0.07f, 0.5f, 32f);
        //                proj.extraUpdates = 0;
        //                proj.Aequus().specialState = (byte)((aequus.accStormcloak.Aequus().equipEmpowerment?.addedStacks) > 0 ? 2 : 1);
        //                proj.Aequus().sourceItemUsed = item.type;
        //                proj.ArmorPenetration = 10;
        //                proj.timeLeft = Math.Max(proj.timeLeft, 180);
        //                proj.friendly = true;
        //                proj.penetrate = 1;
        //                proj.owner = player.whoAmI;
        //                proj.netUpdate = true;
        //                proj.hostile = false;
        //                if (onCooldown) {
        //                    var d = Dust.NewDustDirect(proj.position - new Vector2(32f, 32f), proj.width + 64, proj.height + 64, ModContent.DustType<MonoDust>(), newColor: new Color(128, 128, 128, 0));
        //                    d.velocity += v.RotatedBy(MathHelper.PiOver2 * player.direction) * Main.rand.NextFloat(8f);
        //                }
        //            }
        //            if (aequus.accDustDevilExpertThrowTimer > 120) {
        //                for (int k = 0; k < l.Count; k++) {
        //                    int i = l[k].FindTargetWithLineOfSight();
        //                    if (i != -1) {
        //                        l[k].originalDamage = l[k].damage = player.GetWeaponDamage(item);
        //                        l.RemoveAt(k);
        //                        k--;
        //                    }
        //                }
        //                SoundEngine.PlaySound(SoundID.DD2_BetsysWrathShot.WithPitchOffset(-0.2f).WithVolumeScale(2f), player.Center);
        //                aequus.accDustDevilExpertThrowTimer = 0;
        //                player.AddBuff(ModContent.BuffType<StormcloakCooldown>(), 300);
        //            }
        //        }
        //        else {
        //            if (aequus.accDustDevilExpertThrowTimer > 0)
        //                player.AddBuff(ModContent.BuffType<StormcloakCooldown>(), 300);
        //            aequus.accDustDevilExpertThrowTimer = 0;
        //        }
        //    }
        //    else {
        //        if (aequus.accDustDevilExpertThrowTimer > 0)
        //            player.AddBuff(ModContent.BuffType<StormcloakCooldown>(), 180);
        //        aequus.accDustDevilExpertThrowTimer = 0;
        //    }
        //}

        //public static List<Projectile> GetBlowableProjectiles(Player player, Item item, bool onlyMine = false) {
        //    var projectiles = new List<Projectile>();
        //    var rect = Utils.CenteredRectangle(player.Center, new Vector2(onlyMine ? 640f : 320f));
        //    for (int i = 0; i < Main.maxProjectiles; i++) {
        //        if (Main.projectile[i].active && Main.projectile[i].Colliding(Main.projectile[i].getRect(), rect) && PushableEntities.ProjectileIDs.Contains(Main.projectile[i].type)) {
        //            if ((Main.projectile[i].hostile || Main.player[Main.projectile[i].owner].hostile && Main.player[Main.projectile[i].owner].team != player.team) && !onlyMine ||
        //                Main.projectile[i].Aequus().sourceItemUsed == item.type) {
        //                projectiles.Add(Main.projectile[i]);
        //                if (projectiles.Count >= 3 * item.EquipmentStacks(1))
        //                    return projectiles;
        //            }
        //        }
        //    }
        //    return projectiles;
        //}
    }

    public class StormcloakProj : ModProjectile {
        public override string Texture => AequusTextures.None.Path;

        public Vector3 transformedPosition;
        public float radius;
        public int projectileTakenIdentity;
        public int Index { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int Cooldown { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }

        public override void SetDefaults() {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            radius = 60f;
            projectileTakenIdentity = -1;
        }

        public override bool? CanDamage() {
            return false;
        }

        private void ShootProj(Player player, AequusPlayer aequus, Projectile projectile) {
            for (int i = 0; i < Main.maxProjectiles; i++) {
                var proj = Main.projectile[i];
                if (!proj.active || proj.ModProjectile is not StormcloakProj stormcloakProj) {
                    continue;
                }

                stormcloakProj.Cooldown = Math.Max(30, stormcloakProj.Cooldown);
            }

            var aequusProjectile = projectile.Aequus();

            Cooldown = aequus.accStormcloak.Cooldown;
            projectileTakenIdentity = -1;
            projectile.netUpdate = true;
            aequusProjectile.sourceProjIdentity = -1;
            aequusProjectile.sourceProj = -1;
            aequusProjectile.sourceProjType = ProjectileID.None;
            Projectile.netUpdate = true;
            projectile.velocity = Vector2.Normalize(Main.MouseWorld - projectile.Center) / Math.Max(Projectile.extraUpdates + 1, 1) * 14f;

            int dustWidth = Math.Max(projectile.width, Projectile.width * 2);
            int dustHeight = Math.Max(projectile.height, Projectile.height * 2);
            var dustPosition = Projectile.Center - new Vector2(dustWidth / 2f, dustHeight / 2f);
            int dustType = ModContent.DustType<MonoDust>();
            for (int i = 0; i < 90; i++) {
                Dust.NewDust(dustPosition, dustWidth, dustHeight, dustType, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, newColor: Color.White * 0.5f);
            }
        }

        private void PickupProj(Player player, AequusPlayer aequus, Projectile projectile) {
            var gotoPosition = Projectile.Center + new Vector2(0f, -8f);
            var difference = gotoPosition - projectile.Center;
            if (difference.LengthSquared() < 600f) {
                projectile.Center = gotoPosition;
                projectile.velocity *= 0.1f;
            }
            else {
                projectile.velocity = difference / Math.Max(projectile.extraUpdates + 1, 1) / 4f;
            }

            var aequusProjectile = projectile.Aequus();
            aequusProjectile.specialState = aequus.accStormcloak?.Item?.GetEquipEmpowerment()?.HasAbilityBoost == true ? ProjectileSpecialStates.StormcloakPickupBoosted : ProjectileSpecialStates.StormcloakPickup;
            aequusProjectile.sourceItemUsed = aequus.accStormcloak.Type;
            aequusProjectile.sourceProjIdentity = Projectile.identity;
            aequusProjectile.sourceProjType = Projectile.type;
            projectile.ArmorPenetration = 10;
            projectile.timeLeft = Math.Max(projectile.timeLeft, 180);
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.owner = player.whoAmI;
            projectile.netUpdate = true;
            projectile.hostile = false;
            projectile.damage = 50;
        }

        private void FindProjToPickup(Player player, AequusPlayer aequus) {
            var rect = Utils.CenteredRectangle(player.Center, new(aequus.accStormcloak.Distance));
            int stormcloakItemID = aequus.accStormcloak.Type;
            for (int i = 0; i < Main.maxProjectiles; i++) {
                var proj = Main.projectile[i];
                if (!proj.active || !proj.IsHostile(player)
                    || proj.Aequus().sourceItemUsed == stormcloakItemID
                    || !PushableEntities.ProjectileIDs.Contains(Main.projectile[i].type)
                    || !proj.Colliding(proj.getRect(), rect)) {
                    continue;
                }

                PickupProj(player, aequus, proj);
                projectileTakenIdentity = proj.identity;
                Cooldown = 180;
                SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack);
            }
        }

        public override void AI() {
            var player = Main.player[Projectile.owner];
            var aequus = player.Aequus();
            if (aequus.accStormcloak == null || aequus.accStormcloak.Amount <= Index) {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 2;
            float timer = player.miscCounterNormalized;
            transformedPosition = Vector3.Transform(new(radius, 0f, 0f),
                Matrix.CreateFromYawPitchRoll(0f,
                MathHelper.PiOver2 - Helper.Wave(timer * 15f, -0.2f, 0.2f),
                -timer * 4f * MathHelper.TwoPi + MathHelper.TwoPi / aequus.accStormcloak.Amount * Index));
            Projectile.Center = player.Center + new Vector2(transformedPosition.X, transformedPosition.Y);

            if (Cooldown > 0) {
                Cooldown--;
                if (projectileTakenIdentity <= -1) {
                    return;
                }
            }

            if (projectileTakenIdentity > -1) {
                int proj = Helper.FindProjectileIdentity(Projectile.owner, projectileTakenIdentity);
                if (proj == -1 && Cooldown <= 0) {
                    projectileTakenIdentity = -1;
                    return;
                }

                var projInstance = Main.projectile[projectileTakenIdentity];
                PickupProj(player, aequus, projInstance);
                if (player.controlUseItem && Cooldown <= 0) {
                    ShootProj(player, aequus, projInstance);
                }
                aequus.accStormcloakProjectilesCollected++;
                return;
            }

            FindProjToPickup(player, aequus);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
            if (transformedPosition.Z > 0f) {
                overPlayers.Add(index);
            }
            else {
                behindProjectiles.Add(index);
            }
        }
    }
}

namespace Aequus {
    public partial class AequusPlayer {
        public Stormcloak accStormcloak;
        public int accStormcloakProjectilesCollected;
        public float accStormcloakEffectAlpha;

        private void ResetEffects_Stormcloak() {
            float amount;
            if (accStormcloak == null) {
                amount = 0f;
            }
            else {
                amount = 1f / accStormcloak.Amount * accStormcloakProjectilesCollected;
            }

            if (accStormcloakEffectAlpha > amount) {
                accStormcloakEffectAlpha -= 0.02f;
                if (accStormcloakEffectAlpha < amount) {
                    accStormcloakEffectAlpha = amount;
                }
            }
            else if (accStormcloakEffectAlpha < amount) {
                accStormcloakEffectAlpha += 0.02f;
                if (accStormcloakEffectAlpha > amount) {
                    accStormcloakEffectAlpha = amount;
                }
            }

            accStormcloak = null;
            accStormcloakProjectilesCollected = 0;
        }

        private void CheckStormcloakProj(int index) {
            for (int i = 0; i < Main.maxProjectiles; i++) {
                var proj = Main.projectile[i];
                if (!proj.active || proj.owner != Player.whoAmI || proj.ModProjectile is not StormcloakProj stormcloakProj) {
                    continue;
                }

                if (stormcloakProj.Index == index) {
                    return;
                }
            }

            if (Main.myPlayer == Player.whoAmI) {
                Projectile.NewProjectile(
                    Player.GetSource_Accessory(accStormcloak.Item),
                    Player.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<StormcloakProj>(),
                    50,
                    0f,
                    Player.whoAmI,
                    index
                );
            }
        }
        private void UpdateStormcloak() {
            if (accStormcloak == null || Main.myPlayer != Player.whoAmI) {
                return;
            }

            int spawnTime = accStormcloak.SpawnCheck;
            int timer = Player.miscCounter;
            if (timer % spawnTime == 0) {
                int index = timer % (spawnTime * accStormcloak.Amount) / spawnTime;
                //Main.NewText(index);
                CheckStormcloakProj(index);
            }
        }

        private void DrawStormcloakRing(float yDir, Color color) {
            if (accStormcloakProjectilesCollected <= 0 || accStormcloak == null) {
                return;
            }

            color *= accStormcloakEffectAlpha;
            FastRandom r = new(Player.name.GetHashCode());
            var windTexture = AequusTextures.Bullet;
            var dustTexture = AequusTextures.Particle;
            for (int i = 0; i < 60; i++) {
                Texture2D texture;
                Rectangle frame;
                Vector2 scale;
                float rotation = MathHelper.PiOver2;
                if (r.NextFloat(1f) < 0.1f) {
                    texture = windTexture;
                    frame = texture.Bounds;
                    scale = new(0.3f, 0.66f);
                }
                else {
                    texture = dustTexture;
                    frame = texture.Frame(verticalFrames: 3, frameY: r.Next(3));
                    scale = new(1f, 1f);
                    rotation += Main.GlobalTimeWrappedHourly * r.NextFloat(0.8f, 1.2f);
                }
                var origin = frame.Size() / 2f;
                float speed = r.NextFloat(0.8f, 4f);
                float progress = (Main.GlobalTimeWrappedHourly * speed + r.NextFloat(1f)) % 3f;
                float scaleWave = MathF.Sin(progress * MathHelper.Pi);
                float waveDistance = r.NextFloat(40f, 120f);
                float yWave = Helper.Wave(Main.GlobalTimeWrappedHourly * 2f, 0.5f, 1.5f);
                float yOffset = scaleWave * waveDistance * 0.3f * yWave;
                float xWave = MathF.Sin(progress * MathHelper.Pi - MathHelper.PiOver2) * yDir;
                var drawPosition = Player.Center + new Vector2(xWave * waveDistance, r.NextFloat(-20f, 14f) + yOffset * yDir);
                if (progress > 1f) {
                    continue;
                }
                Main.spriteBatch.Draw(
                    texture,
                    (drawPosition - Main.screenPosition).Floor(),
                    frame,
                    color,
                    rotation - xWave / 3f * yWave * yDir,
                    origin,
                    new Vector2(scale.X * scaleWave * scaleWave, scale.Y * scaleWave),
                    SpriteEffects.None,
                    0f
                );
            }
        }
        private void PreDraw_Stormcloak() {
            DrawStormcloakRing(-1f, Color.White * 0.33f);
        }
        private void PostDraw_Stormcloak() {
            DrawStormcloakRing(1f, Color.White * 0.7f);
        }
    }
}

namespace Aequus.Projectiles {
    public partial class AequusProjectile {
        private void PostAI_Stormcloak(Projectile projectile) {
            if (specialState == 0 || specialState > ProjectileSpecialStates.StormcloakPickupBoosted) {
                return;
            }
            projectile.friendly = true;
            projectile.hostile = false;
            if (projectile.type != ProjectileID.DemonScythe) {
                if (Main.netMode != NetmodeID.Server && sourceProjIdentity > -1) {
                    projectile.rotation = -MathHelper.PiOver2 + ProjectileSets.GetSpriteRotation(projectile.type);
                }
            }
            for (int i = 0; i < 3; i++) {
                if (Main.rand.NextBool(projectile.MaxUpdates)) {
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), -projectile.velocity.X * 0.2f, -projectile.velocity.Y * 0.2f, newColor: Color.White * Main.rand.NextFloat(0.5f, 1f));
                }
            }
        }

        private bool DrawBehind_CheckStormcloak(Projectile projectile, int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
            if (specialState <= 0 && specialState > ProjectileSpecialStates.StormcloakPickupBoosted) {
                return false;
            }

            if (sourceProj != -1 && Main.projectile[sourceProj].ModProjectile is StormcloakProj stormcloakProj) {
                stormcloakProj.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
            }
            return true;
        }
    }
}