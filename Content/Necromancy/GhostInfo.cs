using Aequus.Content.CrossMod;
using System;
using Terraria;

namespace Aequus.Content.Necromancy
{
    public struct GhostInfo : IModCallArgSettable
    {
        /// <summary>
        /// For enemies which cannot be turned into player zombies
        /// </summary>
        public static GhostInfo Invalid => new GhostInfo(0f);
        public static GhostInfo Critter => new GhostInfo(0.1f, 800f);
        public static GhostInfo One => new GhostInfo(1f, 800f);
        public static GhostInfo Two => new GhostInfo(2f, 800f);
        public static GhostInfo Three => new GhostInfo(3f, 1000f);
        public static GhostInfo Four => new GhostInfo(4f, 1250f);

        public float PowerNeeded;
        public float ViewDistance;
        public float? PrioritizePlayerMultiplier;
        public float? TimeLeftMultiplier;
        public int? SlotsUsed;
        public AggroForcer.IEnemyAggressor Aggro;

        public GhostInfo(float power, float view = 800f)
        {
            PowerNeeded = power;
            ViewDistance = view;
            PrioritizePlayerMultiplier = null;
            TimeLeftMultiplier = null;
            SlotsUsed = null;
            Aggro = null;
        }

        public GhostInfo WithAggro(AggroForcer.IEnemyAggressor aggro)
        {
            Aggro = aggro;
            return this;
        }

        public IModCallArgSettable HandleArg(string name, object value)
        {
            if (name == "PrioritizePlayerMultiplier")
            {
                // Call(..., "PrioritizePlayerMultiplier", 1f);
                PrioritizePlayerMultiplier = value == null ? null : IModCallable.UnboxIntoFloat(value);
            }
            else if (name == "TimeLeftMultiplier")
            {
                // Call(..., "TimeLeftMultiplier", 1f);
                TimeLeftMultiplier = value == null ? null : IModCallable.UnboxIntoFloat(value);
            }
            else if (name == "SlotsUsed")
            {
                // Call(..., "SlotsUsed", 1);
                SlotsUsed = value == null ? null : IModCallable.UnboxIntoInt(value);
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
                            WithAggro(AggroForcer.Eclipse);
                            break;

                        case "MartianMadness":
                            WithAggro(AggroForcer.MartianMadness);
                            break;

                        case "PirateInvasion":
                            WithAggro(AggroForcer.PirateInvasion);
                            break;

                        case "GoblinArmy":
                            WithAggro(AggroForcer.GoblinArmy);
                            break;

                        case "DayTime":
                            WithAggro(AggroForcer.DayTime);
                            break;

                        case "NightTime":
                            WithAggro(AggroForcer.NightTime);
                            break;
                    }
                }
                // Call(..., "Aggro", OnPreAIMethodForNPC);
                else if (value is Action<NPC> action)
                {
                    WithAggro(new AggroForcer.ModCallCustom(action, null));
                }
                // Call(..., "Aggro", new ValueTuple<Action<NPC>, Action<NPC>>(Method1, Method2));
                else if (value is ValueTuple<Action<NPC>, Action<NPC>> actionPair)
                {
                    WithAggro(new AggroForcer.ModCallCustom(actionPair.Item1, actionPair.Item2));
                }
                else
                {
                    Aequus.Instance.Logger.Error("Invalid aggro parameters. Please either use magic string text, an Action<NPC>, or a ValueTuple<Action<NPC>, Action<NPC>>.");
                }
            }
            else
            {
                IModCallArgSettable.DoesntExistReport(name, this);
                return this;
            }
            IModCallArgSettable.SuccessReport(name, value, this);
            return this;
        }
    }
}