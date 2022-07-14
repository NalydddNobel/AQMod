using Aequus.Content.CrossMod;
using Aequus.Content.Necromancy.Aggression;
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
        public static GhostInfo Five => new GhostInfo(4f, 2000f);

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
                slotsUsed = value == null ? null : IModCallable.UnboxIntoInt(value);
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

        public override string ToString()
        {
            return AequusHelpers.ValueText("PowerNeeded", PowerNeeded)
                .c().valueText("slotsUsed", slotsUsed)
                .c().valueText("despawnPriority", despawnPriority)
                .c().valueText("Aggro", Aggro);
        }
    }
}