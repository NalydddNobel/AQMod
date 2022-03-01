using AQMod.Buffs.Vampire;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Content.Players
{
    public sealed class VampirismPlayer : ModPlayer
    {
        public ushort Vampirism;

        public bool HasVampirism => Vampirism != 0;
        public bool IsVampire => Vampirism == ushort.MaxValue;

        public bool daylightBurning;
        private bool nightEffects;

        public override void ResetEffects()
        {
            nightEffects = !Main.dayTime && (player.ZoneOverworldHeight || player.ZoneSkyHeight);
            daylightBurning = false;
            if (IsVampire)
            {
            }
            else if (Vampirism > 0)
            {
                Vampirism--;
                if (Vampirism == 0)
                {
                    Vampirism = ushort.MaxValue;
                    player.ClearBuff(ModContent.BuffType<Vampirism>());
                }
                else
                {
                    GiveVampirism(1);
                }
            }
        }

        public void GiveVampirism(int time)
        {
            player.AddBuff(ModContent.BuffType<Vampirism>(), time);
            if (IsVampire || HasVampirism)
            {
                return;
            }
            Vampirism = (ushort)time;
        }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["Vampirism"] = (int)Vampirism,
            };
        }

        public override void Load(TagCompound tag)
        {
            Vampirism = (ushort)tag.GetInt("Vampirism");
        }

        public override void PreUpdateBuffs()
        {
            if (!IsVampire)
            {
                return;
            }

            player.aggro -= 1000;

            if (!nightEffects)
            {
                int x = ((int)player.position.X + player.width / 2) / 16;
                int y = ((int)player.position.Y + player.height / 2) / 16;
                if (Main.tile[x, y] == null)
                {
                    Main.tile[x, y] = new Tile();
                }
                bool burn = true;
                if (Main.tile[x, y].wall > 0)
                {
                    burn = false;
                }
                if (burn)
                {
                    for (int i = 1; i < 10; i++)
                    {
                        if (Main.tile[x, y - i] == null)
                        {
                            Main.tile[x, y - i] = new Tile();
                        }
                        if (Main.tile[x, y - i].active()
                            && !Main.tileSolidTop[Main.tile[x, y - i].type]
                            && Main.tileSolid[Main.tile[x, y - i].type])
                        {
                            burn = false;
                            break;
                        }
                    }
                }
                if (burn)
                {
                    player.AddBuff(ModContent.BuffType<VampirismDay>(), 16);
                    daylightBurning = true;
                }
                if (daylightBurning)
                {
                    for (int i = 0; i < Player.MaxBuffs; i++)
                    {
                        if (player.buffTime[i] > 0 && AQBuff.Sets.FoodBuff.Contains(player.buffType[i]))
                        {
                            player.DelBuff(i);
                            break;
                        }
                    }
                }
            }
        }

        public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff)
        {
            if (!IsVampire)
            {
                return;
            }

            if (nightEffects)
            {
                if (player.wingsLogic > 0)
                    player.wingTimeMax = (int)(player.wingTimeMax * 1.5f);
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
                player.allDamageMult += 0.1f;
                player.allDamageMult += 0.1f;
                player.moveSpeed *= 1.5f;
                player.accRunSpeed *= 1.5f;
                player.pickSpeed *= 1.5f;
                player.jumpSpeedBoost *= 1.5f;
            }
            if (daylightBurning)
            {
                int lifeMax = player.statLifeMax2;
                player.statLifeMax2 = (int)(player.statLifeMax2 * 0.6f + player.statDefense);
                player.statDefense = (int)(player.statDefense * 0.4f);
                player.allDamageMult *= 0.5f;
                player.minionKB *= 0.5f;
                if (player.statLifeMax2 > lifeMax)
                {
                    player.statLifeMax2 = lifeMax;
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
                if (player.lifeRegen > 0)
                {
                    player.lifeRegen = 0;
                }
            }
            if ((player.onFire || player.onFire2 || player.frostBurn) && player.lifeRegen < 0)
            {
                player.lifeRegen *= 2;
            }
        }

        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
        {
            if (!IsVampire)
            {
                return;
            }

            float brightness = (drawInfo.eyeColor.R + drawInfo.eyeColor.G + drawInfo.eyeColor.B) / 255f;
            if (brightness < 0.2f)
            {
                brightness = 0.2f;
            }
            else if (brightness > 0.95f)
            {
                brightness = 0.95f;
            }
            drawInfo.eyeColor = Color.Lerp(drawInfo.eyeColor, new Color(255, 10, 10, drawInfo.eyeColor.A), brightness + (float)(Math.Sin(Main.GlobalTime * 4f) * 0.05f));
        }
    }
}