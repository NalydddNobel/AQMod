using Aequus.Buffs.Cooldowns;
using Aequus.Common.Primitives;
using Aequus.Content;
using Aequus.Content.CrossMod;
using Aequus.Items.Accessories.Misc;
using Aequus.Particles;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Passive
{
    [AutoloadEquip(EquipType.Back, EquipType.Front)]
    public class Stormcloak : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.damage = 50;
            Item.accessory = true;
            Item.rare = ItemDefaults.RarityDustDevil;
            Item.value = ItemDefaults.ValueDustDevil;
            Item.expert = !ModSupportSystem.DoExpertDropsInClassicMode();
            Item.Aequus().itemGravityCheck = 255;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().accDustDevilExpert = Item;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveKnockback();
            tooltips.RemoveCritChance();
        }

        public static void UpdateAccessory(Item item, Player player, AequusPlayer aequus)
        {
            if (item != null)
            {
                bool onCooldown = player.HasBuff<StormcloakCooldown>();
                var l = GetBlowableProjectiles(player, item, onlyMine: onCooldown);
                Vector2 widthMethod(float p) => new Vector2(7f) * (float)Math.Sin(p * MathHelper.Pi);
                Color colorMethod(float p) => Color.White.UseA(150) * 0.45f * (float)Math.Sin(p * MathHelper.Pi);
                for (int i = 0; i < l.Count + (onCooldown ? 0 : 1); i++)
                {
                    var v = Main.rand.NextVector2Unit();
                    var d = Dust.NewDustPerfect(player.Center + v * Main.rand.NextFloat(30f, 100f), ModContent.DustType<MonoDust>(), v.RotatedBy(MathHelper.PiOver2 * player.direction) * Main.rand.NextFloat(8f), newColor: new Color(128, 128, 128, 0));
                    d.noLight = true;
                    if (Main.rand.NextBool(3))
                    {
                        var prim = new TrailRenderer(TrailTextures.Trail[4].Value, TrailRenderer.DefaultPass, widthMethod, colorMethod);
                        float rotation = player.direction * 0.45f;
                        var particle = ParticleSystem.New<StormcloakTrailParticle>(ParticleLayer.AbovePlayers).Setup(prim, player.Center + v * Main.rand.NextFloat(35f, 90f), v.RotatedBy(MathHelper.PiOver2 * player.direction) * 10f,
                            scale: Main.rand.NextFloat(0.85f, 1.5f), trailLength: 10);
                        particle.StretchTrail(v.RotatedBy(MathHelper.PiOver2 * -player.direction) * 2f);
                        particle.rotationValue = rotation / 4f;
                        particle.prim.GetWidth = (p) => widthMethod(p) * particle.Scale;
                        particle.prim.GetColor = (p) => colorMethod(p) * particle.Rotation * Math.Min(particle.Scale, 1.5f);
                    }
                }

                if (l.Count > 0)
                {
                    if (aequus.accDustDevilExpertThrowTimer == 1)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_BetsySummon.WithPitchOffset(-0.44f).WithVolumeScale(2f), player.Center);
                    }
                    if (!onCooldown)
                        aequus.accDustDevilExpertThrowTimer++;
                    foreach (var proj in l)
                    {
                        proj.position += player.velocity * 0.95f;
                        var v = proj.DirectionTo(player.Center);
                        float size = Math.Max(proj.Size.Length(), player.Size.Length()) + proj.velocity.Length();
                        if (proj.Distance(player.Center) < size)
                        {
                            proj.Center = player.Center - v * size;
                        }
                        int i = proj.FindTargetWithLineOfSight();
                        float tornadoValue = 0.8f;
                        if (onCooldown && i != -1)
                        {
                            var toEnemy = proj.DirectionTo(Main.npc[i].Center);
                            proj.velocity = Vector2.Normalize(Vector2.Lerp(proj.velocity, toEnemy, 0.8f)) * proj.velocity.Length();
                            if ((proj.velocity - toEnemy * proj.velocity.Length()).Length() < 16f)
                            {
                                tornadoValue = 0.2f;
                            }
                        }
                        proj.velocity = Vector2.Normalize(Vector2.Lerp(proj.velocity, v.RotatedBy((MathHelper.PiOver2 - 0.1f) * -player.direction) + v,
                            Math.Clamp(1f - proj.Distance(player.Center) / 500f, 0f, tornadoValue))) * Math.Clamp(proj.velocity.Length() + 0.07f, 0.5f, 32f);
                        proj.extraUpdates = 0;
                        proj.Aequus().enemyRebound = (byte)(aequus.ExpertBoost ? 2 : 1);
                        proj.Aequus().sourceItemUsed = item.type;
                        proj.ArmorPenetration = 10;
                        proj.timeLeft = Math.Max(proj.timeLeft, 180);
                        proj.friendly = true;
                        proj.penetrate = 1;
                        proj.owner = player.whoAmI;
                        proj.netUpdate = true;
                        proj.hostile = false;
                        if (onCooldown)
                        {
                            var d = Dust.NewDustDirect(proj.position - new Vector2(32f, 32f), proj.width + 64, proj.height + 64, ModContent.DustType<MonoDust>(), newColor: new Color(128, 128, 128, 0));
                            d.velocity += v.RotatedBy(MathHelper.PiOver2 * player.direction) * Main.rand.NextFloat(8f);
                        }
                    }
                    if (aequus.accDustDevilExpertThrowTimer > 120)
                    {
                        for (int k = 0; k < l.Count; k++)
                        {
                            int i = l[k].FindTargetWithLineOfSight();
                            if (i != -1)
                            {
                                l[k].originalDamage = l[k].damage = player.GetWeaponDamage(item);
                                l.RemoveAt(k);
                                k--;
                            }
                        }
                        SoundEngine.PlaySound(SoundID.DD2_BetsysWrathShot.WithPitchOffset(-0.2f).WithVolumeScale(2f), player.Center);
                        aequus.accDustDevilExpertThrowTimer = 0;
                        player.AddBuff(ModContent.BuffType<StormcloakCooldown>(), 300);
                    }
                }
                else
                {
                    if (aequus.accDustDevilExpertThrowTimer > 0)
                        player.AddBuff(ModContent.BuffType<StormcloakCooldown>(), 300);
                    aequus.accDustDevilExpertThrowTimer = 0;
                }
            }
            else
            {
                if (aequus.accDustDevilExpertThrowTimer > 0)
                    player.AddBuff(ModContent.BuffType<StormcloakCooldown>(), 180);
                aequus.accDustDevilExpertThrowTimer = 0;
            }
        }

        public static List<Projectile> GetBlowableProjectiles(Player player, Item item, bool onlyMine = false)
        {
            var projectiles = new List<Projectile>();
            var rect = Utils.CenteredRectangle(player.Center, new Vector2(onlyMine ? 640f : 320f));
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].Colliding(Main.projectile[i].getRect(), rect) && PushableEntities.ProjectileIDs.Contains(Main.projectile[i].type))
                {
                    if ((Main.projectile[i].hostile || Main.player[Main.projectile[i].owner].hostile && Main.player[Main.projectile[i].owner].team != player.team) && !onlyMine ||
                        Main.projectile[i].Aequus().sourceItemUsed == item.type)
                    {
                        projectiles.Add(Main.projectile[i]);
                        if (projectiles.Count >= 3 * item.EquipmentStacks(1))
                            return projectiles;
                    }
                }
            }
            return projectiles;
        }

        public override void AddRecipes()
        {
            ModContent.GetInstance<TheReconstructionGlobalItem>().addEntry(Type);
        }
    }
}