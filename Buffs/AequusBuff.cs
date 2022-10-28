using Aequus.Buffs.Empowered;
using Aequus.Common;
using Aequus.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs
{
    public class AequusBuff : GlobalBuff, IPostSetupContent
    {
        public static HashSet<int> ConcoctibleBuffsBlacklist { get; private set; }
        public static HashSet<int> CountsAsFire { get; private set; }
        public static HashSet<int> DontChangeDuration { get; private set; }
        public static HashSet<int> CountsAsBuff { get; private set; }

        public static List<int> DemonSiegeEnemyImmunity { get; private set; }

        public override void Load()
        {
            ConcoctibleBuffsBlacklist = new HashSet<int>()
            {
                BuffID.Tipsy,
                BuffID.Honey,
                BuffID.Lucky,
            };
            DontChangeDuration = new HashSet<int>()
            {
                BuffID.Campfire,
                BuffID.HeartLamp,
                BuffID.Sunflower,
                BuffID.Lucky,
            };
            CountsAsBuff = new HashSet<int>()
            {
                BuffID.Tipsy,
            };
            CountsAsFire = new HashSet<int>()
            {
                BuffID.OnFire,
                BuffID.OnFire3,
                BuffID.Frostburn,
                BuffID.Frostburn2,
            };
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
            On.Terraria.Player.QuickBuff_ShouldBotherUsingThisBuff += Player_QuickBuff_ShouldBotherUsingThisBuff;
        }

        private static bool Player_QuickBuff_ShouldBotherUsingThisBuff(On.Terraria.Player.orig_QuickBuff_ShouldBotherUsingThisBuff orig, Player self, int attemptedType)
        {
            if (!orig(self, attemptedType))
                return false;

            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                if (self.buffType[i] < Main.maxBuffTypes)
                    continue;
                var modBuff = BuffLoader.GetBuff(self.buffType[i]);
                if (modBuff is not EmpoweredBuffBase empoweredBuff)
                    continue;

                if (attemptedType == empoweredBuff.OriginalBuffType)
                    return false;
            }
            return true;
        }

        private static void NPC_AddBuff(On.Terraria.NPC.orig_AddBuff orig, NPC self, int type, int time, bool quiet)
        {
            if (Main.debuff[type])
            {
                var player = AequusPlayer.CurrentPlayerContext();
                if (player != null)
                {
                    time = (int)(time * player.Aequus().DebuffsInfliction.ApplyBuffMultipler(player, type));
                }
            }

            orig(self, type, time, quiet);
        }

        void IPostSetupContent.PostSetupContent(Aequus aequus)
        {
            var l = new List<int>();
            for (int i = 0; i < BuffLoader.BuffCount; i++)
            {
                if (BuffID.Sets.IsFedState[i])
                {
                    ConcoctibleBuffsBlacklist.Add(i);
                }
            }
        }

        public override void Unload()
        {
            CountsAsBuff?.Clear();
            CountsAsBuff = null;
            DontChangeDuration?.Clear();
            DontChangeDuration = null;
            CountsAsFire?.Clear();
            CountsAsFire = null;
            DemonSiegeEnemyImmunity?.Clear();
            DemonSiegeEnemyImmunity = null;
        }

        public override void PostDraw(SpriteBatch spriteBatch, int type, int buffIndex, BuffDrawParams drawParams)
        {
            if (Main.LocalPlayer.Aequus().boundedPotionIDs.Contains(type))
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

        public static bool InflictAndPlaySound(NPC target, int type, int time, SoundStyle sound)
        {
            if (target.life <= 0)
            {
                return false;
            }

            bool hasBuffOld = target.HasBuff(type);

            target.AddBuff(type, time);

            bool hasBuff = target.HasBuff(type);
            if (!hasBuffOld && hasBuff)
            {
                SoundEngine.PlaySound(sound);
            }
            return hasBuff;
        }

        public static bool InflictAndPlaySound<T>(NPC target, int time, SoundStyle sound) where T : ModBuff
        {
            return InflictAndPlaySound(target, ModContent.BuffType<T>(), time, sound);
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
    }
}