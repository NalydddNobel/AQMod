using Aequus.Buffs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Players
{
    public sealed class VampirismPlayer : ModPlayer
    {
        public ushort vampirism;

        public bool HasVampirism => vampirism != 0;
        public bool IsVampire => vampirism == ushort.MaxValue;

        public bool daylightBurning;
        private bool nightEffects;

        public override void ResetEffects()
        {
            nightEffects = !Main.dayTime && (Player.ZoneOverworldHeight || Player.ZoneSkyHeight);
            daylightBurning = false;
            if (IsVampire)
            {
            }
            else if (vampirism > 0)
            {
                vampirism--;
                if (vampirism == 0)
                {
                    vampirism = ushort.MaxValue;
                    Player.ClearBuff(ModContent.BuffType<Vampirism>());
                }
                else
                {
                    GiveVampirism(1);
                }
            }
        }

        public void GiveVampirism(int time)
        {
            Player.AddBuff(ModContent.BuffType<Vampirism>(), time);
            if (IsVampire || HasVampirism)
            {
                return;
            }
            vampirism = (ushort)time;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["Vampirism"] = (int)vampirism;
        }

        public override void LoadData(TagCompound tag)
        {
            vampirism = (ushort)tag.GetInt("Vampirism");
        }

        public override void PreUpdateBuffs()
        {
            if (!IsVampire)
            {
                return;
            }

            Player.aggro -= 1000;

            if (!nightEffects)
            {
                int x = ((int)Player.position.X + Player.width / 2) / 16;
                int y = ((int)Player.position.Y + Player.height / 2) / 16;
                bool burn = true;
                if (Main.tile[x, y].WallType > 0)
                {
                    burn = false;
                }
                if (burn)
                {
                    for (int i = 1; i < 10; i++)
                    {
                        if (Main.tile[x, y - i].HasTile
                            && !Main.tileSolidTop[Main.tile[x, y - i].TileType]
                            && Main.tileSolid[Main.tile[x, y - i].TileType])
                        {
                            burn = false;
                            break;
                        }
                    }
                }
                if (burn)
                {
                    Player.AddBuff(ModContent.BuffType<VampirismDay>(), 16);
                    daylightBurning = true;
                }
                if (daylightBurning)
                {
                    for (int i = 0; i < Player.MaxBuffs; i++)
                    {
                        if (Player.buffTime[i] > 0 && BuffID.Sets.IsWellFed[Player.buffType[i]])
                        {
                            Player.DelBuff(i);
                            break;
                        }
                    }
                }
            }
        }

        public override void UpdateEquips()
        {
            if (!IsVampire)
            {
                return;
            }

            if (nightEffects)
            {
                if (Player.wingsLogic > 0)
                    Player.wingTimeMax = (int)(Player.wingTimeMax * 1.5f);
                Player.AddBuff(ModContent.BuffType<VampirismNight>(), 4);
            }
        }

        public override void PostUpdateEquips()
        {
            if (!IsVampire)
            {
                return;
            }

            if (nightEffects)
            {
                Player.GetDamage(DamageClass.Generic) += 0.1f;
                Player.moveSpeed *= 1.5f;
                Player.accRunSpeed *= 1.5f;
                Player.pickSpeed *= 1.5f;
                Player.jumpSpeedBoost *= 1.5f;
                Player.Aequus().ghostSlotsMax++;
            }
            if (daylightBurning)
            {
                int lifeMax = Player.statLifeMax2;
                Player.statLifeMax2 = (int)(Player.statLifeMax2 * 0.6f + Player.statDefense);
                Player.statDefense = (int)(Player.statDefense * 0.4f);
                Player.GetDamage(DamageClass.Generic) *= 0.5f;
                Player.GetKnockback(DamageClass.Generic) *= 0.5f;
                if (Player.statLifeMax2 > lifeMax)
                {
                    Player.statLifeMax2 = lifeMax;
                }
            }
        }

        public override void UpdateBadLifeRegen()
        {
            if (!IsVampire)
            {
                return;
            }

            if (daylightBurning)
            {
                if (Player.lifeRegen > 0)
                {
                    Player.lifeRegen = 0;
                }
            }
            if ((Player.onFire || Player.onFire2 || Player.frostBurn) && Player.lifeRegen < 0)
            {
                Player.lifeRegen *= 2;
            }
        }

        public override void OnHitAnything(float x, float y, Entity victim)
        {
            if (!IsVampire || daylightBurning)
            {
                return;
            }

            if (!Player.moonLeech && Player.lifeSteal > 0f && Main.myPlayer == Player.whoAmI && Main.rand.NextBool(8))
            {
                int amtHealed = 1;

                Player.lifeSteal -= amtHealed;
                Projectile.NewProjectile(Player.GetSource_OnHit(victim, "Aequus:VampireLifesteal"), new Vector2(x, y),
                    Vector2.Normalize(Player.Center - new Vector2(x, y)) * 10f, ProjectileID.VampireHeal, 0, 0f, Player.whoAmI, Player.whoAmI, 1f);
            }
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            if (!IsVampire)
            {
                return;
            }

            float brightness = (drawInfo.colorEyes.R + drawInfo.colorEyes.G + drawInfo.colorEyes.B) / 255f;
            if (brightness < 0.2f)
            {
                brightness = 0.2f;
            }
            else if (brightness > 0.95f)
            {
                brightness = 0.95f;
            }
            drawInfo.colorEyes = Color.Lerp(drawInfo.colorEyes, new Color(255, 10, 10, drawInfo.colorEyes.A), brightness + (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 4f) * 0.05f));
        }
    }
}