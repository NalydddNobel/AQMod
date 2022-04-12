using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Aequus.Common.ID
{
    public sealed class MovementWindWhitelistSetsProvider : SetsProviderBase
    {
        public static HashSet<int> NPCWindWhitelist { get; private set; }

        public override void InitalizeMiscEntries()
        {
            NPCWindWhitelist = new HashSet<int>();
        }

        public override void LoadAutomaticEntries()
        {
            List<int> windAIStyles = new List<int>()
            {
                0,
                NPCAIStyleID.Slime,
                NPCAIStyleID.DemonEye,
                NPCAIStyleID.Fighter,
                NPCAIStyleID.Fairy,
                NPCAIStyleID.AncientLight,
                NPCAIStyleID.BabyMothron,
                NPCAIStyleID.Balloon,
                NPCAIStyleID.Bat,
                NPCAIStyleID.Bird,
                NPCAIStyleID.Butterfly,
                NPCAIStyleID.Caster,
                NPCAIStyleID.Creeper,
                NPCAIStyleID.CritterWorm,
                NPCAIStyleID.Dragonfly,
                NPCAIStyleID.Duck,
                NPCAIStyleID.DukeFishronBubble,
                NPCAIStyleID.ElfCopter,
                NPCAIStyleID.EnchantedSword,
                NPCAIStyleID.Firefly,
                NPCAIStyleID.Flying,
                NPCAIStyleID.FlyingFish,
                NPCAIStyleID.GiantTortoise,
                NPCAIStyleID.GraniteElemental,
                NPCAIStyleID.Herpling,
                NPCAIStyleID.HoveringFighter,
                NPCAIStyleID.Jellyfish,
                NPCAIStyleID.Ladybug,
                NPCAIStyleID.ManEater,
                NPCAIStyleID.Mimic,
                NPCAIStyleID.MothronEgg,
                NPCAIStyleID.NebulaFloater,
                NPCAIStyleID.Passive,
                NPCAIStyleID.Piranha,
                NPCAIStyleID.PlanteraTentacle,
                NPCAIStyleID.Seahorse,
                NPCAIStyleID.SmallStarCell,
                NPCAIStyleID.Spell,
                NPCAIStyleID.Spider,
                NPCAIStyleID.Spore,
                NPCAIStyleID.StarCell,
                NPCAIStyleID.TheHungry,
                NPCAIStyleID.Unicorn,
                NPCAIStyleID.Vulture,
                NPCAIStyleID.WaterStrider,
            };

            foreach (var p in ContentSamples.NpcsByNetId)
            {
                if (NPCWindWhitelist.Contains(p.Key))
                {
                    continue;
                }
                try
                {
                    var projectile = p.Value;
                    if (windAIStyles.Contains(projectile.aiStyle))
                    {
                        NPCWindWhitelist.Add(p.Key);
                    }
                }
                catch (Exception e)
                {
                    var l = Aequus.Instance.Logger;
                    l.Error("An error occured when doing algorithmic checks for sets for {" + Lang.GetNPCName(p.Key).Value + ", ID: " + p.Key + "}", e);
                }
            }
        }

        public override void RemoveUnwantedEntries()
        {
            NPCWindWhitelist.Remove(NPCID.BloodSquid);
        }
    }
}