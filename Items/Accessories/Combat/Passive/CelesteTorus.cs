using Aequus.Common.Items;
using Aequus.Common.Projectiles.Global;
using Aequus.Common.Utilities;
using Aequus.Content;
using Aequus.Content.CrossMod;
using Aequus.Items.Accessories.Combat.Passive;
using Aequus.Projectiles.Misc.Friendly;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Combat.Passive {
    public class CelesteTorus : ModItem, ItemHooks.IUpdateItemDye {
        #region Drawing
        public const float DimensionZMultiplier = 0.03f;

        private static readonly List<CelesteTorusDrawData> DrawData = new();
        private static readonly List<CelesteTorusDrawData> DrawDataReversed = new();

        public static void AddDrawData(CelesteTorusDrawData drawData) {
            DrawData.Add(drawData);
            DrawDataReversed.Insert(0, drawData);
        }
        public static void ClearDrawData() {
            DrawData.Clear();
            DrawDataReversed.Clear();
        }

        public delegate bool CullingRule(float scale);
        public delegate Color ColorRule(CelesteTorusDrawData drawData, Vector2 worldPosition, Vector3 threeDimensionalOffset, Vector2 drawPosition, float scale);

        /// <summary>
        /// Do not run this method if "<see cref="DrawData"/>.Count == 0".
        /// </summary>
        /// <param name="players"></param>
        /// <param name="cullingRule"></param>
        /// <param name="colorRule"></param>
        /// <param name="texture"></param>
        /// <param name="frame"></param>
        /// <param name="origin"></param>
        /// <param name="DefaultDye"></param>
        /// <param name="GlobalScale"></param>
        /// <param name="End"></param>
        /// <param name="Reversed"></param>
        public static void DrawDrawData(IEnumerable<Player> players, CullingRule cullingRule, ColorRule colorRule, Texture2D texture, Rectangle frame, Vector2 origin, int DefaultDye = 0, float GlobalScale = 1f, bool End = false, bool Reversed = false) {
            float multiplierScaleZ = 0.07f;
            float multiplierPositionZ = 0.03f;
            int oldDye = -1;
            var drawDataList = !Reversed ? DrawData : DrawDataReversed;
            foreach (var drawData in drawDataList) {
                if (!players.Contains(drawData.Player)) {
                    continue;
                }

                int dye = DefaultDye;
                if (drawData.Player.TryGetModPlayer<AequusPlayer>(out var aequus)) {
                    dye = aequus.cCelesteTorus == 0 ? DefaultDye : aequus.cCelesteTorus;
                }
                if (oldDye != dye) {
                    if (End) {
                        Main.spriteBatch.End();
                    }

                    if (dye != 0) {
                        Main.spriteBatch.Begin_World(shader: true);
                    }
                    else {
                        Main.spriteBatch.Begin_World(shader: false);
                    }
                    End = true;
                }

                for (int i = 0; i < drawData.OrbsCount; i++) {
                    var v = drawData.GetVector3(i);
                    float layerValue = ViewHelper.GetViewScale(1f, v.Z * multiplierScaleZ);
                    if (cullingRule(layerValue)) {
                        var center = drawData.WorldPosition + new Vector2(v.X, v.Y);

                        var drawPosition = ViewHelper.GetViewPoint(center, v.Z * multiplierPositionZ) - Main.screenPosition;
                        DrawData dd = new(
                            texture,
                            drawPosition.Floor(),
                            frame,
                            colorRule(drawData, center, v, drawPosition, layerValue),
                            0f,
                            origin,
                            Math.Min(layerValue, 1.5f) * GlobalScale * drawData.Scale,
                            SpriteEffects.None,
                            0
                        );

                        if (dye != 0) {
                            GameShaders.Armor.Apply(dye, drawData.Player, dd);
                        }

                        dd.Draw(Main.spriteBatch);
                    }
                }
            }
            if (End) {
                Main.spriteBatch.End();
            }
        }
        internal static void DrawGlows(IEnumerable<Player> players) {
            if (DrawData.Count == 0) {
                return;
            }

            var texture = AequusTextures.Bloom0;
            DrawDrawData(
                players,
                AlwaysTrueCullingRule,
                GlowColorRule,
                texture,
                texture.Bounds,
                texture.Size() / 2f,
                DefaultDye: GameShaders.Armor.GetShaderIdFromItemId(ItemID.StardustDye),
                GlobalScale: 0.7f,
                End: false
            );
        }
        internal static void DrawOrbs(CullingRule cullingRule, bool Reversed, IEnumerable<Player> players) {
            if (DrawData.Count == 0) {
                return;
            }

            Main.instance.LoadProjectile(ModContent.ProjectileType<CelesteTorusProj>());
            var texture = TextureAssets.Projectile[ModContent.ProjectileType<CelesteTorusProj>()].Value;
            DrawDrawData(
                players,
                cullingRule,
                OrbsColorRule,
                texture,
                texture.Bounds,
                texture.Size() / 2f,
                DefaultDye: 0,
                End: false,
                Reversed: Reversed
            );
        }
        public static bool BackOrbsCullingRule(float scale) {
            return scale < 1f;
        }
        public static bool FrontOrbsCullingRule(float scale) {
            return scale >= 1f;
        }
        public static bool AlwaysTrueCullingRule(float scale) {
            return true;
        }
        public static Color OrbsColorRule(CelesteTorusDrawData drawData, Vector2 worldPosition, Vector3 threeDimensionalOffset, Vector2 drawPosition, float scale) {
            return Color.White;
        }
        public static Color GlowColorRule(CelesteTorusDrawData drawData, Vector2 worldPosition, Vector3 threeDimensionalOffset, Vector2 drawPosition, float scale) {
            return (Main.tenthAnniversaryWorld ? Color.HotPink with { A = 0 } : Color.White with { A = 0 } * 0.5f) ;
        }
        #endregion

        public override void SetStaticDefaults() {
            SentryAccessoriesDatabase.OnAI.Add(Type, SentryAccessoriesDatabase.ApplyEquipFunctional_AI);
        }

        public override void Unload() {
            DrawData.Clear();
        }

        public override void SetDefaults() {
            Item.width = 20;
            Item.height = 20;
            Item.damage = 70;
            Item.knockBack = 2f;
            Item.accessory = true;
            Item.rare = ItemDefaults.RarityOmegaStarite;
            Item.value = ItemDefaults.ValueOmegaStarite;
            Item.expert = !ModSupportSystem.DoExpertDropsInClassicMode();
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            var aequus = player.Aequus();
            if (aequus.accCelesteTorus != null)
                aequus.celesteTorusDamage++;
            aequus.accCelesteTorus = Item;
            if (aequus.ProjectilesOwned(ModContent.ProjectileType<CelesteTorusProj>()) <= 0) {
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<CelesteTorusProj>(),
                    0, 0f, player.whoAmI, player.Aequus().projectileIdentity + 1);
            }
        }

        void ItemHooks.IUpdateItemDye.UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
            player.Aequus().cCelesteTorus = dyeItem.dye;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage) {
            damage += player.Aequus().celesteTorusDamage - 1f;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            tooltips.RemoveCritChance();
        }
    }

    public record struct CelesteTorusDrawData(Player Player, Vector3 Rotation, Vector2 WorldPosition, float Radius, float Scale, int OrbsCount) {
        public Vector3 GetVector3(int index) {
            return CelesteTorusProj.GetRot(index, Rotation, Radius, OrbsCount);
        }
    }
}

namespace Aequus.Projectiles.Misc.Friendly {
    public class CelesteTorusProj : ModProjectile {
        public Vector3 rotation;
        public Vector3 rotation2;
        public bool show2ndRing;
        public float currentRadius;
        public Rectangle[] _hitboxesCache;
        public int hitboxesCount;

        public override void SetDefaults() {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hide = true;
            Projectile.netImportant = true;
            Projectile.ignoreWater = true;
            Projectile.manualDirectionChange = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
            _hitboxesCache = new Rectangle[13];
        }

        public override bool? CanCutTiles() => false;

        private void UpdateHitboxes() {
            hitboxesCount = 5;
            for (int i = 0; i < 5; i++) {
                var pos = GetRot(i, rotation, currentRadius);
                var collisionCenter = Projectile.Center + new Vector2(pos.X, pos.Y);
                _hitboxesCache[i] = Utils.CenteredRectangle(collisionCenter, new Vector2(Projectile.width, Projectile.width));
            }
            if (show2ndRing) {
                hitboxesCount += 8;
                for (int i = 0; i < 8; i++) {
                    var pos = GetRot(i, rotation2, currentRadius * 2f, 8);
                    var collisionCenter = Projectile.Center + new Vector2(pos.X, pos.Y);
                    _hitboxesCache[i + 5] = Utils.CenteredRectangle(collisionCenter, new Vector2(Projectile.width, Projectile.width) * 1.2f);
                }
            }
        }

        public override void AI() {
            int projIdentity = (int)Projectile.ai[0] - 1;
            AequusPlayer aequus;
            if (projIdentity > -1) {
                projIdentity = Helper.FindProjectileIdentity(Projectile.owner, projIdentity);
                if (projIdentity == -1 || !Main.projectile[projIdentity].active || !Main.projectile[projIdentity].TryGetGlobalProjectile<SentryAccessoriesGlobalProj>(out var value)) {
                    Projectile.Kill();
                    return;
                }

                aequus = value.dummyPlayer?.GetModPlayer<AequusPlayer>();
                Projectile.Center = Main.projectile[projIdentity].Center;
            }
            else {
                aequus = Main.player[Projectile.owner].GetModPlayer<AequusPlayer>();
                Projectile.Center = Main.player[Projectile.owner].Center;
            }
            Projectile.scale = 1f;

            var player = Main.player[Projectile.owner];
            if (!player.active || player.dead || (aequus?.accCelesteTorus) == null) {
                return;
            }
            Projectile.timeLeft = 2;

            if (Projectile.active) {
                int damage = player.GetWeaponDamage(aequus.accCelesteTorus);
                if (Projectile.damage != damage) {
                    if (Projectile.damage < damage) {
                        Projectile.damage = Math.Min(Projectile.damage + 2, damage);
                    }
                    else {
                        Projectile.damage = Math.Max(Projectile.damage - 10, damage);
                    }
                }

                float playerPercent = player.statLife / (float)player.statLifeMax2;
                float gotoRadius = Math.Min((int)((float)Math.Sqrt(player.width * player.height) + 20f + player.wingTimeMax * 0.15f + player.wingTime * 0.15f + (1f - playerPercent) * 90f + player.statDefense), 600);
                currentRadius = MathHelper.Lerp(currentRadius, gotoRadius, 0.1f);
                Projectile.scale *= 0.8f + 0.2f * currentRadius / 100f;

                var center = Projectile.Center;
                bool danger = false;
                for (int i = 0; i < Main.maxNPCs; i++) {
                    if (Main.npc[i].CanBeChasedBy(Projectile) && Vector2.Distance(Main.npc[i].Center, center) < 2000f) {
                        danger = true;
                        break;
                    }
                }

                if (Main.myPlayer == Projectile.owner && Main.netMode != NetmodeID.SinglePlayer) {
                    if (rotation.X.Abs() > MathHelper.TwoPi || rotation.Y.Abs() > MathHelper.TwoPi || rotation.Z.Abs() > MathHelper.TwoPi || Main.GameUpdateCount % 60 == 0) {
                        Projectile.netUpdate = true;
                    }
                }

                rotation.X %= MathHelper.TwoPi;
                rotation.Y %= MathHelper.TwoPi;
                rotation.Z %= MathHelper.TwoPi;

                show2ndRing = (aequus.accCelesteTorus.Aequus()?.equipEmpowerment?.addedStacks).GetValueOrDefault(0) > 0;
                if (danger) {
                    rotation.X = rotation.X.AngleLerp(0f, 0.01f);
                    rotation.Y = rotation.Y.AngleLerp(0f, 0.0075f);
                    rotation.Z += 0.04f + (1f - playerPercent) * 0.0314f;

                    rotation2.X = rotation.X.AngleLerp(0f, 0.01f);
                    rotation2.Y = rotation.Y.AngleLerp(0f, 0.0075f);
                    rotation2.Z += 0.04f + (1f - playerPercent) * 0.0314f;
                }
                else {
                    rotation.X += 0.0157f;
                    rotation.Y += 0.01f;
                    rotation.Z += 0.0314f;

                    if (show2ndRing) {
                        rotation2.X += 0.0157f;
                        rotation2.Y += 0.0314f;
                        rotation2.Z += 0.011f;
                    }
                }

                Lighting.AddLight(Projectile.Center, new Vector3(0.3f, 0.3f, 0.8f));
                UpdateHitboxes();
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            for (int i = 0; i < hitboxesCount; i++) {
                if (_hitboxesCache[i].Intersects(targetHitbox)) {
                    return true;
                }
            }
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
            modifiers.HitDirectionOverride = target.position.X < Main.player[Projectile.owner].position.X ? -1 : 1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            Projectile.NewProjectile(
                Projectile.GetSource_OnHit(target), 
                Main.rand.NextFromRect(target.Hitbox), 
                Vector2.Zero,
                ModContent.ProjectileType<CelesteTorusOnHitProj>(),
                0,
                0f,
                Projectile.owner
            );
        }

        public override void SendExtraAI(BinaryWriter writer) {
            writer.Write(rotation.X);
            writer.Write(rotation.Y);
            writer.Write(rotation.Z);
            writer.Write(rotation2.X);
            writer.Write(rotation2.Y);
            writer.Write(rotation2.Z);
        }

        public override void ReceiveExtraAI(BinaryReader reader) {
            rotation.X = reader.ReadSingle();
            rotation.Y = reader.ReadSingle();
            rotation.Z = reader.ReadSingle();
            rotation2.X = reader.ReadSingle();
            rotation2.Y = reader.ReadSingle();
            rotation2.Z = reader.ReadSingle();
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
            CelesteTorus.AddDrawData(new(Main.player[Projectile.owner], rotation, Projectile.Center, currentRadius, Projectile.scale, 5));
            if (show2ndRing) {
                CelesteTorus.AddDrawData(new(Main.player[Projectile.owner], rotation2, Projectile.Center, currentRadius * 2f, Projectile.scale * 1.3f, 8));
            }
        }

        public static Vector3 GetRot(int i, Vector3 rotation, float currentRadius, int max = 5) {
            return Vector3.Transform(new Vector3(currentRadius, 0f, 0f), Matrix.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z + MathHelper.TwoPi / max * i));
        }
    }

    public class CelesteTorusOnHitProj : ModProjectile {
        public override string Texture => AequusTextures.None.Path;

        public override void SetDefaults() {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 10;
        }

        public override void AI() {

        }

        public override bool PreDraw(ref Color lightColor) {
            return base.PreDraw(ref lightColor);
        }
    }
}