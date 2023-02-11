using Aequus.Buffs.Misc.Empowered;
using Aequus.Common;
using Aequus;
using Aequus.Graphics;
using Aequus.Items.Accessories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Aequus.Common.Utilities;

namespace Aequus.Buffs
{
    public class AequusBuff : GlobalBuff, IPostSetupContent
    {
        public static HashSet<int> ConcoctibleBuffIDsBlacklist { get; private set; }
        public static HashSet<int> IsFire { get; private set; }
        public static HashSet<int> DontChangeDuration { get; private set; }
        public static HashSet<int> ForcedPositiveBuff { get; private set; }

        public static HashSet<int> PlayerDoTBuff { get; private set; }
        public static HashSet<int> PlayerStatusBuff { get; private set; }

        public static List<int> DemonSiegeEnemyImmunity { get; private set; }

        public static Dictionary<int, List<int>> PotionConflicts { get; private set; }

        /// <summary>
        /// Used to prevent right click effects when the <see cref="PotionCanteen"/> is equipped.
        /// </summary>
        public static List<int> preventRightClick;

        public override void Load()
        {
            PlayerDoTBuff = new HashSet<int>();
            PlayerStatusBuff = new HashSet<int>();
            ConcoctibleBuffIDsBlacklist = new HashSet<int>();
            DontChangeDuration = new HashSet<int>();
            ForcedPositiveBuff = new HashSet<int>();
            IsFire = new HashSet<int>();
            PotionConflicts = new Dictionary<int, List<int>>();
            DemonSiegeEnemyImmunity = new List<int>()
            {
                BuffID.OnFire,
                BuffID.OnFire3,
                BuffID.CursedInferno,
                BuffID.ShadowFlame,
                BuffID.Ichor,
                BuffID.Oiled,
            };
            On.Terraria.NPC.AddBuff += NPC_AddBuff;
            On.Terraria.Player.AddBuff += Player_AddBuff; ;
            On.Terraria.Player.AddBuff_DetermineBuffTimeToAdd += Player_AddBuff_DetermineBuffTimeToAdd;
            On.Terraria.Player.QuickBuff_ShouldBotherUsingThisBuff += Player_QuickBuff_ShouldBotherUsingThisBuff;

            preventRightClick = new List<int>();
        }

        public static bool isDebuff(int type)
        {
            if (Main.debuff[type])
                return true;
            return IsFire.Contains(type);
        }

        private static void addPotionConflict(int buffID, int conflictor)
        {
            if (!PotionConflicts.ContainsKey(buffID))
            {
                PotionConflicts[buffID] = new List<int>() { conflictor };
                return;
            }
            if (PotionConflicts[buffID].Contains(conflictor))
                return;
            PotionConflicts[buffID].Add(conflictor);
        }
        public static void AddPotionConflict(int buffID, int buffID2)
        {
            addPotionConflict(buffID, buffID2);
            addPotionConflict(buffID2, buffID);
        }

        private static void Player_AddBuff(On.Terraria.Player.orig_AddBuff orig, Player player, int type, int timeToAdd, bool quiet, bool foodHack)
        {
            if (PotionConflicts.TryGetValue(EmpoweredBuffBase.GetDepoweredBuff(type), out var l) && l != null)
            {
                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    if (l.Contains(EmpoweredBuffBase.GetDepoweredBuff(player.buffType[i])))
                    {
                        player.DelBuff(i);
                    }
                }
            }
            orig(player, type, timeToAdd, quiet, foodHack);
        }

        private static int Player_AddBuff_DetermineBuffTimeToAdd(On.Terraria.Player.orig_AddBuff_DetermineBuffTimeToAdd orig, Player player, int type, int time1)
        {
            int amt = orig(player, type, time1);
            if (Main.buffNoTimeDisplay[type] || DontChangeDuration.Contains(type))
            {
                return amt;
            }

            player.Aequus().DetermineBuffTimeToAdd(type, ref amt);
            return amt;
        }

        private static bool Player_QuickBuff_ShouldBotherUsingThisBuff(On.Terraria.Player.orig_QuickBuff_ShouldBotherUsingThisBuff orig, Player player, int attemptedType)
        {
            if (!orig(player, attemptedType))
                return false;
            if (PotionConflicts.TryGetValue(EmpoweredBuffBase.GetDepoweredBuff(attemptedType), out var l) && l != null)
            {
                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    if (l.Contains(EmpoweredBuffBase.GetDepoweredBuff(player.buffType[i])))
                    {
                        return false;
                    }
                }
            }
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                if (player.buffType[i] < Main.maxBuffTypes)
                    continue;
                var modBuff = BuffLoader.GetBuff(player.buffType[i]);
                if (modBuff is not EmpoweredBuffBase empoweredBuff)
                    continue;

                if (attemptedType == empoweredBuff.OriginalBuffType)
                    return false;
            }
            return true;
        }

        private static void NPC_AddBuff(On.Terraria.NPC.orig_AddBuff orig, NPC npc, int type, int time, bool quiet)
        {
            if (Main.debuff[type] || IsFire.Contains(type))
            {
                var player = AequusPlayer.CurrentPlayerContext();
                if (player != null)
                {
                    var aequus = player.Aequus();
                    time = (int)(time * aequus.DebuffsInfliction.GetBuffMultipler(player, type));
                    if (aequus.accResetEnemyDebuffs && !npc.HasBuff(type))
                    {
                        for (int i = 0; i < NPC.maxBuffs; i++)
                        {
                            if (npc.buffTime[i] > 0 && npc.buffType[i] > 0 && isDebuff(npc.buffType[i]))
                            {
                                npc.buffTime[i] = Math.Max(npc.buffTime[i], 180);
                            }
                        }
                    }
                    if (aequus.debuffDamage > npc.Aequus().debuffDamage)
                    {
                        npc.Aequus().debuffDamage = (byte)aequus.debuffDamage;
                        if (Main.netMode != NetmodeID.SinglePlayer)
                        {
                            var p = Aequus.GetPacket(PacketType.SendDebuffFlatDamage);
                            p.Write(npc.whoAmI);
                            p.Write((byte)aequus.debuffDamage);
                            p.Send();
                        }
                    }
                }
            }

            orig(npc, type, time, quiet);
        }

        void IPostSetupContent.PostSetupContent(Aequus aequus)
        {
            var contentFile = new ContentArrayFile("BuffSets", BuffID.Search);
            contentFile.AddToHashSet("ConcoctionBlacklist", ConcoctibleBuffIDsBlacklist);
            contentFile.AddToHashSet("DontChangeDuration", DontChangeDuration);
            contentFile.AddToHashSet("ForcedPositiveBuff", ForcedPositiveBuff);
            contentFile.AddToHashSet("IsFire", IsFire);
            for (int i = 0; i < BuffLoader.BuffCount; i++)
            {
                if (BuffID.Sets.IsFedState[i])
                {
                    ConcoctibleBuffIDsBlacklist.Add(i);
                }
            }
        }

        public override void Unload()
        {
            PotionConflicts?.Clear();
            PotionConflicts = null;
            ConcoctibleBuffIDsBlacklist?.Clear();
            ConcoctibleBuffIDsBlacklist = null;
            ForcedPositiveBuff?.Clear();
            ForcedPositiveBuff = null;
            DontChangeDuration?.Clear();
            DontChangeDuration = null;
            IsFire?.Clear();
            IsFire = null;
            DemonSiegeEnemyImmunity?.Clear();
            DemonSiegeEnemyImmunity = null;
        }

        public override void PostDraw(SpriteBatch spriteBatch, int type, int buffIndex, BuffDrawParams drawParams)
        {
            if (Main.LocalPlayer.Aequus().BoundedPotionIDs.Contains(type))
            {
                var buffSB = new SpriteBatchCache(Main.spriteBatch);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);

                var dd = new DrawData(drawParams.Texture, drawParams.Position, drawParams.Texture.Bounds, drawParams.DrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                EffectsSystem.VerticalGradient.ShaderData.UseColor(Color.Lerp(Color.Transparent, Color.HotPink, AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 5f + MathHelper.Pi, 0f, 0.5f)) * Main.cursorAlpha);
                EffectsSystem.VerticalGradient.ShaderData.UseSecondaryColor(Color.Lerp(Color.Transparent, Color.BlueViolet, AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 5f, 0f, 0.5f)) * Main.cursorAlpha);
                EffectsSystem.VerticalGradient.ShaderData.Apply(dd);

                dd.color.A = 0;
                dd.Draw(Main.spriteBatch);

                Main.spriteBatch.End();
                buffSB.Begin(Main.spriteBatch);
            }
        }

        public override bool RightClick(int type, int buffIndex)
        {
            return !preventRightClick.Contains(type);
        }

        public static bool ApplyBuff(Entity target, int type, int time, out bool canPlaySound)
        {
            var entity = new EntityCommons(target);
            canPlaySound = false;
            if (entity.life <= 0)
            {
                return false;
            }

            bool hasBuffOld = entity.HasBuff(type);

            entity.AddBuff(type, time);

            bool hasBuff = entity.HasBuff(type);
            canPlaySound = !hasBuffOld && hasBuff;
            return hasBuff;
        }

        public static bool ApplyBuff<T>(Entity target, int time, out bool canPlaySound) where T : ModBuff
        {
            return ApplyBuff(target, ModContent.BuffType<T>(), time, out canPlaySound);
        }

        public static bool AddStaticImmunity(int npc, params int[] buffList)
        {
            return AddStaticImmunity(npc, false, buffList);
        }
        public static bool AddStaticImmunity(int npc, bool isWhipBuff, params int[] buffList)
        {
            if (!NPCID.Sets.DebuffImmunitySets.TryGetValue(npc, out var value))
            {
                NPCID.Sets.DebuffImmunitySets.Add(npc, new NPCDebuffImmunityData() { SpecificallyImmuneTo = buffList });
                return true;
            }

            if (value == null)
            {
                value = NPCID.Sets.DebuffImmunitySets[npc] = new NPCDebuffImmunityData();
            }

            if (value.ImmuneToAllBuffsThatAreNotWhips && !isWhipBuff)
            {
                return true;
            }

            if (value.SpecificallyImmuneTo == null)
            {
                value.SpecificallyImmuneTo = buffList;
                return true;
            }

            var list = new List<int>(buffList);
            for (int i = 0; i < value.SpecificallyImmuneTo.Length; i++)
            {
                list.Remove(value.SpecificallyImmuneTo[i]);
            }
            buffList = list.ToArray();
            if (buffList.Length <= 0)
            {
                return true;
            }

            Array.Resize(ref value.SpecificallyImmuneTo, value.SpecificallyImmuneTo.Length + buffList.Length);
            int k = 0;
            for (int j = value.SpecificallyImmuneTo.Length - buffList.Length; j < value.SpecificallyImmuneTo.Length; j++)
            {
                value.SpecificallyImmuneTo[j] = buffList[k];
                k++;
            }
            return true;
        }

        public static void SaveBuffID(TagCompound tag, string key, int buffID)
        {
            if (buffID >= Main.maxBuffTypes)
            {
                var modBuff = BuffLoader.GetBuff(buffID);
                tag[$"{key}Key"] = $"{modBuff.Mod.Name}:{modBuff.Name}";
                return;
            }
            tag[$"{key}ID"] = buffID;
        }
        public static int LoadBuffID(TagCompound tag, string key)
        {
            if (tag.TryGet($"{key}Key", out string buffKey))
            {
                var val = buffKey.Split(":");
                if (ModLoader.TryGetMod(val[0], out var mod))
                {
                    if (mod.TryFind<ModBuff>(val[1], out var modBuff))
                    {
                        return modBuff.Type;
                    }
                }
            }
            return tag.Get<int>($"{key}ID");
        }
    }
}