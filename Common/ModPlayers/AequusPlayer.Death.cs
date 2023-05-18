using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus {
    public partial class AequusPlayer : ModPlayer {
        private void Load_DeathMsgHook() {
            On_Lang.CreateDeathMessage += On_Lang_CreateDeathMessage;
        }

        private NetworkText GetDeathMessage(string key, int variants, params NetworkText[] networkTexts) {
            return GetDeathMessage(key + Main.rand.Next(variants), networkTexts);
        }
        private NetworkText GetDeathMessage(string key, params NetworkText[] networkTexts) {
            return NetworkText.FromKey("Mods.Aequus.DeathMessage." + key, networkTexts);
        }

        private static NetworkText On_Lang_CreateDeathMessage(On_Lang.orig_CreateDeathMessage orig, string deadPlayerName, int plr, int npc, int proj, int other, int projType, int plrItemType) {
            Player dyingPlayer = Array.Find(Main.player, p => p.name.Equals(deadPlayerName));
            if (dyingPlayer != null && dyingPlayer.TryGetModPlayer<AequusPlayer>(out var aequusPlayer)) {
                var customReason = aequusPlayer.ChangeDeathReason(deadPlayerName, dyingPlayer, plr, npc, proj, other, projType, plrItemType);
                if (customReason != null) {
                    return customReason;
                }
            }

            return orig(deadPlayerName, plr, npc, proj, other, projType, plrItemType);
        }

        private NetworkText ChangeDeathReason(string deadPlayerName, Player dyingPlayer, int plr, int npc, int proj, int other, int projType, int plrItemType) {
            var playerText = NetworkText.FromLiteral(deadPlayerName);

            if (plr == dyingPlayer.whoAmI) {
                return GetDeathMessage("SelfDamage.", 3, playerText);
            }

            var source = NetworkText.Empty;
            if (proj > 0) {
                source = NetworkText.FromKey(Lang.GetProjectileName(projType).Key);
            }
            if (npc > 0) {
                source = Main.npc[npc].GetGivenOrTypeNetName();
            }
            if (source == NetworkText.Empty) {
                return null;
            }

            bool bossAlive = Helper.AnyNPCWithCondition(Helper.IsABoss);
            if (NPC.AnyNPCs(NPCID.WallofFlesh)) {
                return GetDeathMessage("WallOfFlesh.", 4, playerText, source);
            }

            if (bossAlive && Main.rand.NextBool(Main.netMode == NetmodeID.SinglePlayer ? 2 : 4)) {
                return GetDeathMessage("Boss.", 20, playerText, source);
            }

            if (!Main.dayTime) {
                if (bossAlive && Main.rand.NextBool(3)) {
                    return GetDeathMessage("Boss.Nighttime", playerText, source);
                }
            }

            if (ZoneDemonSiege) {
                if (Main.rand.NextBool()) {
                    return GetDeathMessage("GoreNest.", 4, playerText, source);
                }
            }
            else if (Player.ZoneDungeon) {
                if (Main.rand.NextBool()) {
                    return GetDeathMessage("Dungeon.", 4, playerText, source);
                }
            }
            else if (Player.ZoneSnow) {
                if (Main.rand.NextBool()) {
                    return GetDeathMessage("SnowBiome.", 4, playerText, source);
                }
            }

            if (Main.netMode != NetmodeID.SinglePlayer) {
                List<Player> teammates = new();
                for (int i = 0; i < Main.maxPlayers; i++) {
                    if (i != Player.whoAmI && Main.player[i].active && !Main.player[i].DeadOrGhost && Main.player[i].team == Player.team) {
                        teammates.Add(Main.player[i]);
                    }
                }

                if (Main.rand.NextBool(3)) {
                    if (Player.ConsumedLifeCrystals < Player.LifeCrystalMax) {
                        foreach (var p in teammates) {
                            if (Player.ConsumedLifeCrystals + 3 < p.ConsumedLifeCrystals) {
                                return GetDeathMessage("Multiplayer.LifeCrystalSharing", playerText, NetworkText.FromLiteral(p.name));
                            }
                        }
                    }
                    if (Player.ConsumedLifeFruit < Player.LifeFruitMax) {
                        foreach (var p in teammates) {
                            if (Player.ConsumedLifeFruit + 3 < p.ConsumedLifeFruit) {
                                return GetDeathMessage("Multiplayer.LifeFruitSharing", playerText, NetworkText.FromLiteral(p.name));
                            }
                        }
                    }
                }

                if (bossAlive) {
                    if (Main.rand.NextBool()) {
                        if (teammates.Count == 0) {
                            return GetDeathMessage("Multiplayer.AllDead_", 6, playerText, source);
                        }
                        else {
                            return GetDeathMessage("Multiplayer.SomeoneAlive_", 2, playerText, NetworkText.FromLiteral(Main.rand.Next(teammates).name));
                        }
                    }

                    if (Main.rand.NextBool(3)) {
                        return GetDeathMessage("Multiplayer.", 6, playerText, source);
                    }
                }
            }
            return null;
        }
    }
}