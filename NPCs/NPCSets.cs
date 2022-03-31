using Aequus.NPCs.Monsters.Sky;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs
{
    public sealed class NPCSets : ModType
    {
        protected sealed override void Register()
        {
        }

        public static HashSet<int> WindUpdates { get; private set; }

        public override void Load()
        {
            WindUpdates = new HashSet<int>();
        }

        public override void SetupContent()
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
                if (WindUpdates.Contains(p.Key))
                {
                    continue;
                }
                try
                {
                    var projectile = p.Value;
                    if (windAIStyles.Contains(projectile.aiStyle))
                    {
                        WindUpdates.Add(p.Key);
                    }
                }
                catch (Exception e)
                {
                    var l = Aequus.Instance.Logger;
                    l.Error("An error occured when doing algorithmic checks for sets for {" + Lang.GetNPCName(p.Key).Value + ", ID: " + p.Key + "}", e);
                }
            }

            WindUpdates.Remove(NPCID.BloodSquid);
        }
    }
}