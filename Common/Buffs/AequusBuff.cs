using Aequus.Common.DataSets;
using Aequus.Common.Effects;
using Aequus.Common.Utilities;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using static Aequus.Common.Buffs.BuffHooks;

namespace Aequus.Common.Buffs;

public class AequusBuff : GlobalBuff {
    /// <summary>Used to prevent right click effects.</summary>
    public static List<int> preventRightClick;

    public override void Load() {
        On_NPC.AddBuff += NPC_AddBuff;
        On_Player.AddBuff += Player_AddBuff;
        On_Player.AddBuff_DetermineBuffTimeToAdd += Player_AddBuff_DetermineBuffTimeToAdd;
        On_Player.QuickBuff_ShouldBotherUsingThisBuff += Player_QuickBuff_ShouldBotherUsingThisBuff;
        preventRightClick = new List<int>();
    }

    /// <summary>
    /// Helper method for debuffs, since some debuffs aren't actually bad, or some aren't actually marked as debuffs
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsDebuff(int type) {
        return !BuffSets.NotTypicalDebuff.Contains(type) && (Main.debuff[type] || BuffSets.ProbablyFireDebuff.Contains(type));
    }

    public static void AddPotionConflict(int buffID, int buffID2) {
        BuffSets.AddBuffConflicts(buffID, buffID2);
    }

    private static void Player_AddBuff(On_Player.orig_AddBuff orig, Player player, int type, int timeToAdd, bool quiet, bool foodHack) {
        var onAddBuff = BuffLoader.GetBuff(type) as IOnAddBuff;
        onAddBuff?.PreAddBuff(player, ref timeToAdd, ref quiet, ref foodHack);
        if (BuffSets.BuffConflicts.TryGetValue(type, out var l) && l != null) {
            for (int i = 0; i < Player.MaxBuffs; i++) {
                if (l.Contains(player.buffType[i])) {
                    player.DelBuff(i);
                }
            }
        }
        orig(player, type, timeToAdd, quiet, foodHack);
        onAddBuff?.PostAddBuff(player, timeToAdd, quiet, foodHack);
    }

    private static int Player_AddBuff_DetermineBuffTimeToAdd(On_Player.orig_AddBuff_DetermineBuffTimeToAdd orig, Player player, int type, int time1) {
        int amt = orig(player, type, time1);
        if (Main.buffNoTimeDisplay[type] || BuffSets.DontChangeDuration.Contains(type) || BuffSets.NotTypicalDebuff.Contains(type)) {
            return amt;
        }

        player.Aequus().DetermineBuffTimeToAdd(type, ref amt);
        return amt;
    }

    private static bool Player_QuickBuff_ShouldBotherUsingThisBuff(On_Player.orig_QuickBuff_ShouldBotherUsingThisBuff orig, Player player, int attemptedType) {
        if (!orig(player, attemptedType)) {
            return false;
        }

        if (BuffSets.BuffConflicts.TryGetValue(attemptedType, out var l) && l != null) {
            for (int i = 0; i < Player.MaxBuffs; i++) {
                if (l.Contains(player.buffType[i])) {
                    return false;
                }
            }
        }

        return true;
    }

    private static void NPC_AddBuff(On_NPC.orig_AddBuff orig, NPC npc, int type, int time, bool quiet) {
        var onAddBuff = BuffLoader.GetBuff(type) as IOnAddBuff;
        onAddBuff?.PreAddBuff(npc, ref time, ref quiet);
        if (Main.debuff[type] || BuffSets.ProbablyFireDebuff.Contains(type)) {
            var player = AequusPlayer.CurrentPlayerContext();
            if (player != null) {
                var aequus = player.Aequus();
                time = (int)(time * aequus.DebuffsInfliction.GetBuffMultipler(player, type));
                if (aequus.accResetEnemyDebuffs && !npc.HasBuff(type)) {
                    for (int i = 0; i < NPC.maxBuffs; i++) {
                        if (npc.buffTime[i] > 0 && npc.buffType[i] > 0 && IsDebuff(npc.buffType[i])) {
                            npc.buffTime[i] = Math.Max(npc.buffTime[i], 180);
                        }
                    }
                }
                if (aequus.soulCrystalDamage > npc.Aequus().debuffDamage) {
                    npc.Aequus().debuffDamage = (byte)aequus.soulCrystalDamage;
                    if (Main.netMode != NetmodeID.SinglePlayer) {
                        var p = Aequus.GetPacket(PacketType.SendDebuffFlatDamage);
                        p.Write(npc.whoAmI);
                        p.Write((byte)aequus.soulCrystalDamage);
                        p.Send();
                    }
                }
            }
        }
        orig(npc, type, time, quiet);
        onAddBuff?.PostAddBuff(npc, time, quiet);
    }

    public override void PostDraw(SpriteBatch spriteBatch, int type, int buffIndex, BuffDrawParams drawParams) {
        if (Main.LocalPlayer.Aequus().BoundedPotionIDs.Contains(type)) {
            var buffSB = new LegacySpriteBatchCache(Main.spriteBatch);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);

            var dd = new DrawData(drawParams.Texture, drawParams.Position, drawParams.Texture.Bounds, drawParams.DrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            LegacyEffects.VerticalGradient.ShaderData.UseColor(Color.Lerp(Color.Transparent, Color.HotPink, Helper.Wave(Main.GlobalTimeWrappedHourly * 5f + MathHelper.Pi, 0f, 0.5f)) * Main.cursorAlpha);
            LegacyEffects.VerticalGradient.ShaderData.UseSecondaryColor(Color.Lerp(Color.Transparent, Color.BlueViolet, Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0f, 0.5f)) * Main.cursorAlpha);
            LegacyEffects.VerticalGradient.ShaderData.Apply(dd);

            dd.color.A = 0;
            dd.Draw(Main.spriteBatch);

            Main.spriteBatch.End();
            buffSB.Begin(Main.spriteBatch);
        }
    }

    public override bool RightClick(int type, int buffIndex) {
        return !preventRightClick.Contains(type);
    }

    public static bool ApplyBuff(Entity target, int type, int time, out bool canPlaySound) {
        var entity = new EntityCommons(target);
        canPlaySound = false;
        if (entity.Life <= 0) {
            return false;
        }

        bool hasBuffOld = entity.HasBuff(type);

        entity.AddBuff(type, time);

        bool hasBuff = entity.HasBuff(type);
        canPlaySound = !hasBuffOld && hasBuff;
        return hasBuff;
    }

    public static bool ApplyBuff<T>(Entity target, int time, out bool canPlaySound) where T : ModBuff {
        return ApplyBuff(target, ModContent.BuffType<T>(), time, out canPlaySound);
    }

    public static void AddStandardMovementDebuffImmunities(int buffType, bool bossImmune = true) {
        SetImmune(NPCID.MartianTurret, buffType);
        SetImmune(NPCID.MartianDrone, buffType);
        SetImmune(NPCID.MartianProbe, buffType);
        SetImmune(NPCID.MartianSaucer, buffType);
        SetImmune(NPCID.MartianSaucerCannon, buffType);
        SetImmune(NPCID.MartianSaucerCore, buffType);
        SetImmune(NPCID.MartianSaucerTurret, buffType);
        SetImmune(NPCID.MoonLordCore, buffType);
        SetImmune(NPCID.MoonLordHand, buffType);
        SetImmune(NPCID.MoonLordHead, buffType);
        SetImmune(NPCID.MoonLordLeechBlob, buffType);
        SetImmune(NPCID.GolemHead, buffType);

        if (bossImmune) {
            foreach (var n in ContentSamples.NpcsByNetId.Values) {
                if (n.boss) {
                    SetImmune(n.type, buffType);
                }
            }
        }
    }

    public static bool SetImmune(int npc, params int[] buffList) {
        return SetImmune(npc, false, buffList);
    }
    public static bool SetImmune(int npc, bool isWhipBuff, params int[] buffList) {
        for (int j = 0; j < buffList.Length; j++) {
            NPCID.Sets.SpecificDebuffImmunity[npc][buffList[j]] = true;
        }
        return true;
    }

    public static void SaveBuffID(TagCompound tag, string key, int buffID) {
        if (buffID >= BuffID.Count) {
            var modBuff = BuffLoader.GetBuff(buffID);
            tag[$"{key}Key"] = $"{modBuff.Mod.Name}:{modBuff.Name}";
            return;
        }
        tag[$"{key}ID"] = buffID;
    }
    public static int LoadBuffID(TagCompound tag, string key) {
        if (tag.TryGet($"{key}Key", out string buffKey)) {
            var val = buffKey.Split(":");
            if (ModLoader.TryGetMod(val[0], out var mod)) {
                if (mod.TryFind<ModBuff>(val[1], out var modBuff)) {
                    return modBuff.Type;
                }
            }
        }
        return tag.Get<int>($"{key}ID");
    }
}