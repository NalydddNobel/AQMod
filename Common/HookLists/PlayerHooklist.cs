using AQMod.Buffs.Temperature;
using AQMod.Content.Players;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Common.HookLists
{
    public sealed class PlayerHooklist : HookList
    {
        [LoadHook(typeof(Main), "DrawPlayers", BindingFlags.NonPublic | BindingFlags.Instance)]
        internal static void Main_DrawPlayers(On.Terraria.Main.orig_DrawPlayers orig, Main self)
        {
            List<AQPlayer> active = new List<AQPlayer>();
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (Main.player[i].active && !Main.player[i].outOfRange)
                    {
                        active.Add(Main.player[i].GetModPlayer<AQPlayer>());
                    }
                }
            }
            else
            {
                active.Add(Main.LocalPlayer.GetModPlayer<AQPlayer>());
            }
            foreach (var p in active)
            {
                p.PreDrawAllPlayers(self);
            }
            orig(self);
            foreach (var p in active)
            {
                p.PostDrawAllPlayers(self);
            }
        }

        [LoadHook(typeof(Main), nameof(Main.DrawPlayer), BindingFlags.Public | BindingFlags.Instance)]
        internal static void Main_DrawPlayer(On.Terraria.Main.orig_DrawPlayer orig, Main self, Player drawPlayer, Vector2 Position, float rotation, Vector2 rotationOrigin, float shadow)
        {
            var aQPlayer = drawPlayer.GetModPlayer<AQPlayer>();
            aQPlayer.PreDrawPlayer(self, Position, rotation, rotationOrigin, shadow);
            orig(self, drawPlayer, Position, rotation, rotationOrigin, shadow);
            aQPlayer.PostDrawPlayer(self, Position, rotation, rotationOrigin, shadow);
        }

        [LoadHook(typeof(Player), nameof(Player.DropTombstone), BindingFlags.Public | BindingFlags.Instance)]
        internal static void Player_DropTombstone(On.Terraria.Player.orig_DropTombstone orig, Player self, int coinsOwned, Terraria.Localization.NetworkText deathText, int hitDirection)
        {
            var tombstonesPlayer = self.GetModPlayer<TombstonesPlayer>();
            if (tombstonesPlayer.disableTombstones)
            {
                return;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && tombstonesPlayer.CreateTombstone(coinsOwned, deathText, hitDirection))
            {
                orig(self, coinsOwned, deathText, hitDirection);
            }
        }

        [LoadHook(typeof(Player), "Update", BindingFlags.Public | BindingFlags.Instance)]
        internal static void StaticManipulation(On.Terraria.Player.orig_Update orig, Player self, int i)
        {
            if (self.whoAmI == i)
            {
                var aQPlayer = self.GetModPlayer<AQPlayer>();
                if (aQPlayer.fakeDaytime == 1 && !Main.dayTime)
                {
                    AQPlayer.setDaytime = false;
                    Main.dayTime = true;
                }
            }
            try
            {
                orig(self, i);
            }
            catch
            {
            }
            if (AQPlayer.setDaytime != null)
            {
                Main.dayTime = AQPlayer.setDaytime.Value;
                AQPlayer.setDaytime = null;
            }
        }

        [LoadHook(typeof(Chest), "SetupShop", BindingFlags.Public | BindingFlags.Instance)]
        internal static void ApplyCustomDiscount(On.Terraria.Chest.orig_SetupShop orig, Chest self, int type)
        {
            var plr = Main.LocalPlayer;
            bool discount = plr.discount;
            plr.discount = false;

            orig(self, type);

            plr.discount = discount;
            if (discount)
            {
                float discountPercentage = plr.GetModPlayer<AQPlayer>().discountPercentage;
                for (int i = 0; i < Chest.maxItems; i++)
                {
                    if (self.item[i] != null && self.item[i].type != ItemID.None)
                        self.item[i].value = (int)(self.item[i].value * discountPercentage);
                }
            }
        }

        [LoadHook(typeof(Player), "HorizontalMovement", BindingFlags.Public | BindingFlags.Instance)]
        internal static void MovementEffects(On.Terraria.Player.orig_HorizontalMovement orig, Player self)
        {
            orig(self);
            var aQPlayer = self.GetModPlayer<AQPlayer>();
            if (aQPlayer.redSpriteWind != 0 && !(self.mount.Active && self.velocity.Y == 0f && (self.controlLeft || self.controlRight)))
            {
                float windDirection = Math.Sign(aQPlayer.redSpriteWind) * 0.07f;
                if (Math.Abs(Main.windSpeed) > 0.5f)
                {
                    windDirection *= 1.37f;
                }
                if (self.velocity.Y != 0f)
                {
                    windDirection *= 1.5f;
                }
                if (self.controlLeft || self.controlRight)
                {
                    windDirection *= 0.8f;
                }
                if (Math.Sign(self.direction) != Math.Sign(windDirection))
                {
                    self.accRunSpeed -= Math.Abs(windDirection) * 20f;
                    self.maxRunSpeed -= Math.Abs(windDirection) * 20f;
                }
                if (windDirection < 0f && self.velocity.X > windDirection)
                {
                    self.velocity.X += windDirection;
                    if (self.velocity.X < windDirection)
                    {
                        self.velocity.X = windDirection;
                    }
                }
                if (windDirection > 0f && self.velocity.X < windDirection)
                {
                    self.velocity.X += windDirection;
                    if (self.velocity.X > windDirection)
                    {
                        self.velocity.X = windDirection;
                    }
                }

                if (!self.controlLeft && !self.controlRight)
                {
                    self.legFrameCounter = -1.0;
                    self.legFrame.Y = 0;
                }
            }
            aQPlayer.redSpriteWind = 0;
        }

        [LoadHook(typeof(Player), "HitTile", BindingFlags.Public | BindingFlags.Instance)]
        internal static void ModifyPickaxePower(On.Terraria.Player.orig_PickTile orig, Player self, int x, int y, int pickPower)
        {
            if (self.GetModPlayer<AQPlayer>().pickBreak)
            {
                pickPower /= 2;
            }
            orig(self, x, y, pickPower);
        }

        [LoadHook(typeof(Player), "AddBuff", BindingFlags.Public | BindingFlags.Instance)]
        internal static void OnAddBuff(On.Terraria.Player.orig_AddBuff orig, Player self, int type, int time1, bool quiet)
        {
            if (type >= Main.maxBuffTypes)
            {
                var modBuff = ModContent.GetModBuff(type);
                if (modBuff is TemperatureDebuff)
                {
                    for (int i = 0; i < Player.MaxBuffs; i++)
                    {
                        if (self.buffTime[i] > 0)
                        {
                            if (self.buffType[i] == type)
                            {
                                orig(self, type, time1, quiet);
                                return;
                            }
                            if (self.buffType[i] > Main.maxBuffTypes)
                            {
                                var otherModBuff = ModContent.GetModBuff(self.buffType[i]);
                                if (otherModBuff is TemperatureDebuff)
                                {
                                    self.DelBuff(i);
                                    orig(self, type, time1, quiet);
                                    return;
                                }
                            }
                        }
                    }
                    orig(self, type, time1, quiet);
                    return;
                }
            }
            if (AQBuff.Sets.Instance.FoodBuff.Contains(type))
            {
                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    if (self.buffTime[i] > 16 && self.buffType[i] != type && AQBuff.Sets.Instance.FoodBuff.Contains(self.buffType[i]))
                    {
                        self.DelBuff(i);
                        i--;
                    }
                }
            }
            orig(self, type, time1, quiet);
        }
    }
}