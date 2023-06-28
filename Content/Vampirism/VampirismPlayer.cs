using Aequus.Buffs.Debuffs;
using Aequus.Content.Vampirism.Buffs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus {
    public partial class AequusPlayer {
        public ushort _vampirismData;

        /// <summary>
        /// Returns true if this player is going through a vampire transformation, or is a vampire.
        /// </summary>
        public bool HasVampirism => _vampirismData != 0;

        /// <summary>
        /// Returns true if this player is a vampire.
        /// <para>
        /// For detecting if the player is undergoing a vampire transformation, check <see cref="HasVampirism"/>.
        /// </para>
        /// </summary>
        public bool IsVampire { 
            get => _vampirismData == ushort.MaxValue; 
            set {
                if (value) {
                    _vampirismData = ushort.MaxValue; // Set to max value to force this player to be a vampire.
                }
                else if (IsVampire) { // Only reset if the player is a full vampire.
                    _vampirismData = 0;
                }
            }  
        }

        public bool vampireDay;
        public bool vampireNight;

        private bool IsSunnyDay() {
            return Main.dayTime && !Main.eclipse;
        }

        private void Initialize_Vampire() {
            _vampirismData = 0;
        }

        private void ResetEffects_Vampire() {
            vampireDay = false;
            vampireNight = false;
        }

        private void SaveData_Vampire(TagCompound tag) {
            if (_vampirismData != 0) {
                tag["Vampirism"] = (int)_vampirismData;
            }
        }

        private void LoadData_Vampire(TagCompound tag) {
            _vampirismData = (ushort)tag.GetInt("Vampirism");
        }

        private bool CheckDaytimeState() {
            if (Player.position.Y > Main.worldSurface * 16f && !Aequus.ZenithSeed) {
                return false;
            }

            var tileCoords = Player.Center.ToTileCoordinates();
            if (!WorldGen.InWorld(tileCoords.X, tileCoords.Y, 40)) {
                return false;
            }

            var tile = Main.tile[tileCoords];
            if (tile.WallType > WallID.None && (WallID.Sets.AllowsWind[tile.WallType] || WallID.Sets.Transparent[tile.WallType])) {
                return true;
            }
            for (int j = -2; j > -10; j--) {
                if (Main.tile[tileCoords.X, tileCoords.Y + j].IsFullySolid()) {
                    return false;
                }
            }

            return true;
        }

        private void PreUpdateBuffs_Vampire() {
            if (!IsVampire) {
                if (_vampirismData > 0) {
                    _vampirismData--;
                    if (_vampirismData == 0) {
                        _vampirismData = ushort.MaxValue;
                        Player.ClearBuff(ModContent.BuffType<VampirismBuff>());
                    }
                    else if (_vampirismData > 4) {
                        GiveVampirism(4);
                    }
                }
                return;
            }

            vampireNight = !IsSunnyDay();

            if (!vampireNight) {
                vampireDay = CheckDaytimeState();
                if (vampireDay) {
                    Player.AddBuff(Main.raining ? ModContent.BuffType<VampirismDayRain>() : ModContent.BuffType<VampirismDay>(), 2);
                    for (int i = 0; i < Player.MaxBuffs; i++) {
                        if (Player.buffTime[i] > 0 && BuffID.Sets.IsWellFed[Player.buffType[i]]) {
                            Player.DelBuff(i);
                            break;
                        }
                    }
                }
            }
            else {
                Player.AddBuff(Main.eclipse ? ModContent.BuffType<VampirismNightEclipse>() : ModContent.BuffType<VampirismNight>(), 2);
            }
        }

        private void UpdateDead_Vampire() {
            if (!IsVampire && !Aequus.ZenithSeed) {
                _vampirismData = 0;
            }
        }

        private void UpdateEquips_Vampire() {
            if (!IsVampire) {
                return;
            }

            if (vampireNight) {
                if (Player.wingsLogic > 0)
                    Player.wingTimeMax = (int)(Player.wingTimeMax * 1.5f);
            }
        }

        private void PostUpdateEquips_Vampire() {
            if (!IsVampire) {
                return;
            }

            if (vampireDay) {
                Player.statLifeMax2 = (int)Math.Clamp(Player.statLifeMax2 * 0.6f + Player.statDefense, 100, Player.statLifeMax2);
            }
        }

        private void UpdateBadLifeRegen_Vampire() {
            if (!IsVampire) {
                return;
            }

            if (vampireDay) {
                if (Player.lifeRegen > 0) {
                    Player.lifeRegen = 0;
                }
            }
            if ((Player.onFire || Player.onFire2 || Player.onFire3 || Player.onFrostBurn || Player.onFrostBurn2 || Player.HasBuff<BlueFire>()) && Player.lifeRegen < 0) {
                Player.lifeRegen *= 2;
            }
        }

        private void OnHitAnything_Vampire(float x, float y, Entity victim) {
            if (!IsVampire || vampireDay) {
                return;
            }

            if (!Player.moonLeech && Player.lifeSteal > 0f && Main.myPlayer == Player.whoAmI && Main.rand.NextBool(8)) {
                int amtHealed = 2;

                Player.lifeSteal -= amtHealed;
                Projectile.NewProjectile(
                    Player.GetSource_OnHit(victim, "Aequus:VampireLifesteal"), 
                    new Vector2(x, y),
                    Vector2.Normalize(Player.Center - new Vector2(x, y)) * 10f, 
                    ProjectileID.VampireHeal, 0, 0f, Player.whoAmI, Player.whoAmI, amtHealed);
            }
        }

        private void ModifyDrawInfo_Vampire(ref PlayerDrawSet drawInfo) {
            if (!IsVampire) {
                return;
            }

            float brightness = (drawInfo.colorEyes.R + drawInfo.colorEyes.G + drawInfo.colorEyes.B) / 255f;
            if (brightness < 0.2f) {
                brightness = 0.2f;
            }
            else if (brightness > 0.95f) {
                brightness = 0.95f;
            }
            drawInfo.colorEyes = Color.Lerp(drawInfo.colorEyes, new Color(255, 10, 10, drawInfo.colorEyes.A), brightness + (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 4f) * 0.05f));

            float skinSaturation = 0.5f;
            if (vampireDay) {
                skinSaturation /= 2f;
            }
            drawInfo.colorBodySkin = drawInfo.colorBodySkin.SaturationMultiply(skinSaturation);
            drawInfo.colorHead = drawInfo.colorHead.SaturationMultiply(skinSaturation);
            drawInfo.colorLegs = drawInfo.colorLegs.SaturationMultiply(skinSaturation);
        }

        public void GiveVampirism(int time) {
            _vampirismData = Math.Max((ushort)time, _vampirismData);
            if (_vampirismData > 0) {
                Player.AddBuff(ModContent.BuffType<VampirismBuff>(), _vampirismData);
            }
        }
    }
}