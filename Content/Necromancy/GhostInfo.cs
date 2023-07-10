using Aequus.Common.CrossMod.ModCalls;
using Aequus.Content.Necromancy.Aggression;
using System;
using Terraria;
using Terraria.ID;

namespace Aequus.Content.Necromancy {
    public struct GhostInfo : ILegacyCallHandler
    {
        /// <summary>
        /// For enemies which cannot be turned into player zombies
        /// </summary>
        public static GhostInfo Invalid => new GhostInfo(0f);
        public static GhostInfo Critter => new GhostInfo(0.1f, 800f);
        /// <summary>
        /// Tier for pre-Evil enemies
        /// </summary>
        public static GhostInfo One => new GhostInfo(1f, 800f);
        /// <summary>
        /// Tier for post-Evils/Skeletron/Underworld enemies
        /// </summary>
        public static GhostInfo Two => new GhostInfo(2f, 800f);
        /// <summary>
        /// Tier for pre-Golem enemies, and pre-Hardmode minibosses
        /// </summary>
        public static GhostInfo Three => new GhostInfo(3f, 1000f);
        /// <summary>
        /// Tier for pre-Golem Hardmode minibosses and strong/special enemies (Wraiths and Giant Tortises are here too.)
        /// </summary>
        public static GhostInfo Four => new GhostInfo(4f, 1250f);
        /// <summary>
        /// Tier for post-Golem Minibosses and post-ML enemies
        /// </summary>
        public static GhostInfo Five => new GhostInfo(5f, 2000f);

        public float PowerNeeded;
        public float ViewDistance;
        public float? PrioritizePlayerMultiplier;
        public float? TimeLeftMultiplier;
        public int? slotsUsed;
        public IEnemyAggressor Aggro;
        public int despawnPriority;
        public bool DontModifyVelocity;

        public int SlotsUsed => slotsUsed.GetValueOrDefault(1);

        public GhostInfo(float power, float view = 800f)
        {
            PowerNeeded = power;
            ViewDistance = view;
            PrioritizePlayerMultiplier = null;
            TimeLeftMultiplier = null;
            slotsUsed = null;
            Aggro = null;
            despawnPriority = 1;
            DontModifyVelocity = false;
        }

        public GhostInfo WithAggro(IEnemyAggressor aggro)
        {
            Aggro = aggro;
            return this;
        }

        public static GhostInfo Autogenerate(NPC npc)
        {
            if (npc.boss)
                return Invalid;

            if (NPCID.Sets.CountsAsCritter[npc.type])
            {
                return Critter;
            }

            int calcedTier = TierCalc(npc);

            switch (calcedTier)
            {
                case 0:
                    return One;

                case 1:
                    return Two;

                case 2:
                    return Three;

                case 3:
                    return Four;

                default:
                    if (calcedTier >= 4)
                    {
                        return new GhostInfo(5, 2000f);
                    }
                    break;
            }

            return Invalid;
        }
        public static int TierCalc(NPC npc)
        {
            int c = (int)((1f - Math.Pow(1f - Math.Sqrt(npc.lifeMax + npc.defense * 5 + npc.damage) / 100f, 2f)) * 100f) / 20;
            if (c > 3)
            {
                c--;
            }
            return c;
        }

        public bool EnoughPower(float power)
        {
            return (PowerNeeded > 0f && PowerNeeded <= power) || power >= 100f;
        }

        public ILegacyCallHandler HandleArg(string name, object value)
        {
            if (name == "PrioritizePlayerMultiplier")
            {
                // Call(..., "PrioritizePlayerMultiplier", 1f);
                PrioritizePlayerMultiplier = value == null ? null : Helper.UnboxFloat.Unbox(value);
            }
            else if (name == "TimeLeftMultiplier")
            {
                // Call(..., "TimeLeftMultiplier", 1f);
                TimeLeftMultiplier = value == null ? null : Helper.UnboxFloat.Unbox(value);
            }
            else if (name == "SlotsUsed")
            {
                // Call(..., "SlotsUsed", 1);
                slotsUsed = value == null ? null : Helper.UnboxInt.Unbox(value);
            }
            else if (name == "Aggro")
            {
                // Call(..., "Aggro", null);
                if (value == null)
                {
                    Aggro = null;
                }
                // Call(..., "Aggro", "Eclipse");
                else if (value is string text)
                {
                    switch (text)
                    {
                        default:
                            Aequus.Instance.Logger.Error("There is no aggro forcer called '" + text + "'");
                            return this;

                        case "Eclipse":
                            WithAggro(AggressionType.Eclipse);
                            break;

                        case "MartianMadness":
                            WithAggro(AggressionType.MartianMadness);
                            break;

                        case "PirateInvasion":
                            WithAggro(AggressionType.PirateInvasion);
                            break;

                        case "GoblinArmy":
                            WithAggro(AggressionType.GoblinArmy);
                            break;

                        case "DayTime":
                            WithAggro(AggressionType.DayTime);
                            break;

                        case "NightTime":
                            WithAggro(AggressionType.NightTime);
                            break;
                    }
                }
                // Call(..., "Aggro", OnPreAIMethodForNPC);
                else if (value is Action<NPC> action)
                {
                    WithAggro(new AggressionType.ModCallCustom(action, null));
                }
                // Call(..., "Aggro", new ValueTuple<Action<NPC>, Action<NPC>>(Method1, Method2));
                else if (value is ValueTuple<Action<NPC>, Action<NPC>> actionPair)
                {
                    WithAggro(new AggressionType.ModCallCustom(actionPair.Item1, actionPair.Item2));
                }
                else
                {
                    Aequus.Instance.Logger.Error("Invalid aggro parameters. Please either use magic strings, an Action<NPC>, or a ValueTuple<Action<NPC>, Action<NPC>>.");
                }
            }
            else
            {
                ILegacyCallHandler.DoesntExistReport(name, this);
                return this;
            }
            ILegacyCallHandler.SuccessReport(name, value, this);
            return this;
        }

        public override string ToString()
        {
            return $"{nameof(PowerNeeded)}:{PowerNeeded}, {nameof(slotsUsed)}:{slotsUsed}, {nameof(despawnPriority)}:{despawnPriority}, {nameof(Aggro)}:{(Aggro == null ? "null" : Aggro.GetType().Name)}";
        }
    }
}