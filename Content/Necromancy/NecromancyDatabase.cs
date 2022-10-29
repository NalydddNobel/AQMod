using Aequus.Common;
using Aequus.Common.Utilities;
using Aequus.Content.CrossMod;
using Aequus.Content.CrossMod.ModCalls;
using Aequus.Content.Necromancy.Aggression;
using Aequus.Items.Weapons.Summon.Necro;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Necromancy
{
    public class NecromancyDatabase : LoadableType, IAddRecipes
    {
        public static List<int> NecromancyDebuffs { get; private set; }
        public static Dictionary<int, GhostInfo> NPCs { get; private set; }
        public static HashSet<string> AutogeneratorIgnoreMods { get; private set; }

        public override void Load()
        {
            NecromancyDebuffs = new List<int>();
            AggressionType.LoadAggressions();
            PopulateLegacyEnemyStats();
            AutogeneratorIgnoreMods = new HashSet<string>() { "Aequus", };
        }
        private static void PopulateLegacyEnemyStats()
        {
            NPCs = new Dictionary<int, GhostInfo>()
            {
                [NPCID.DuneSplicerBody] = GhostInfo.Invalid,
                [NPCID.DuneSplicerTail] = GhostInfo.Invalid,
                [NPCID.Gnome] = GhostInfo.Invalid,
                [NPCID.MotherSlime] = GhostInfo.Invalid,
                [NPCID.GiantWormBody] = GhostInfo.Invalid,
                [NPCID.GiantWormTail] = GhostInfo.Invalid,
                [NPCID.BoneSerpentBody] = GhostInfo.Invalid,
                [NPCID.BoneSerpentTail] = GhostInfo.Invalid,
                [NPCID.DevourerBody] = GhostInfo.Invalid,
                [NPCID.DevourerTail] = GhostInfo.Invalid,
                [NPCID.TombCrawlerBody] = GhostInfo.Invalid,
                [NPCID.TombCrawlerTail] = GhostInfo.Invalid,
                [NPCID.DungeonGuardian] = GhostInfo.Invalid,
                [NPCID.DarkCaster] = GhostInfo.Invalid,
                [NPCID.FireImp] = GhostInfo.Invalid,
                [NPCID.ManEater] = GhostInfo.Invalid,
                [NPCID.Snatcher] = GhostInfo.Invalid,
                [NPCID.Tim] = GhostInfo.Invalid,
                [NPCID.AngryTrapper] = GhostInfo.Invalid,
            };
        }

        void IAddRecipes.AddRecipes(Aequus aequus)
        {
            if (Aequus.LogMore)
            {
                Aequus.Instance.Logger.Info("Loading necromancy stats...");
            }
            LoadEntriesFile();
            AutogenEntries();
            AutogenPriorities();
        }
        public static void LoadEntriesFile()
        {
            var val = Aequus.GetContentFile("Necromancy");
            foreach (var modDict in val)
            {
                if (modDict.Key == "Vanilla")
                {
                    foreach (var data in modDict.Value)
                    {
                        string name = data.Key;
                        bool checkBanners = false;
                        if (name[0] == '!')
                        {
                            checkBanners = true;
                            name = name[1..];
                        }
                        int id = NPCID.Search.GetId(name);
                        var info = ReadGhostInfo(data.Value);
                        //Main.NewText(Lang.GetNPCName(id) + ": " + info);
                        if (checkBanners)
                        {
                            foreach (var i in AequusHelpers.AllWhichShareBanner(id))
                            {
                                NPCs[i] = info;
                            }
                            continue;
                        }
                        NPCs[id] = info;
                    }
                }
                else if (ModLoader.TryGetMod(modDict.Key, out var mod))
                {
                    if (Aequus.LogMore)
                    {
                        Aequus.Instance.Logger.Info($"Loading necromancy stats for {modDict.Key}...");
                    }
                    foreach (var data in modDict.Value)
                    {
                        string name = data.Key;
                        bool checkBanners = false;
                        if (name[0] == '!')
                        {
                            checkBanners = true;
                            name = name[1..];
                        }
                        if (mod.TryFind<ModNPC>(name, out var modNPC))
                        {
                            if (Aequus.LogMore)
                            {
                                Aequus.Instance.Logger.Info($"{data.Key}/{modNPC.Type}: {data.Value}");
                            }
                            var info = ReadGhostInfo(data.Value);
                            if (checkBanners)
                            {
                                foreach (var i in AequusHelpers.AllWhichShareBanner(modNPC.Type))
                                {
                                    NPCs[i] = info;
                                }
                                continue;
                            }
                            NPCs[modNPC.Type] = info;
                        }
                    }
                }
            }
        }
        public static GhostInfo ReadGhostInfo(string text)
        {
            IEnemyAggressor aggressor = null;
            int type;
            if (text.Contains(';'))
            {
                var val = text.Split(';');
                if (!int.TryParse(val[0].Trim(), out type))
                    type = -1;
                AggressionType.AggressorFromName.TryGetValue(val[1].Trim(), out aggressor);
            }
            else
            {
                if (!int.TryParse(text, out type))
                    type = -1;
            }

            switch (type) 
            {
                case 1:
                    return GhostInfo.One.WithAggro(aggressor);
                case 2:
                    return GhostInfo.Two.WithAggro(aggressor);
                case 3:
                    return GhostInfo.Three.WithAggro(aggressor);
                case 4:
                    return GhostInfo.Four.WithAggro(aggressor);
                case 5:
                    return GhostInfo.Five.WithAggro(aggressor);
            }
            return GhostInfo.Invalid;
        }
        public static void AutogenEntries()
        {
            for (int i = Main.maxNPCTypes; i < NPCLoader.NPCCount; i++)
            {
                var n = ContentSamples.NpcsByNetId[i];
                var md = n.ModNPC;
                if (AutogeneratorIgnoreMods.Contains(md.Mod.Name) || NPCs.ContainsKey(n.type))
                {
                    continue;
                }

                NPCs.Add(i, GhostInfo.Autogenerate(n));
            }
        }
        public static void AutogenPriorities()
        {
            foreach (int i in NPCs.Keys)
            {
                var g = NPCs[i];
                var n = ContentSamples.NpcsByNetId[i];
                g.despawnPriority = CalcPriority(CalcDamageTiering(g.PowerNeeded, n.boss, n.noGravity), n.life, n.damage, n.defense, g.slotsUsed.GetValueOrDefault(1));
                NPCs[i] = g;
            }
        }
        public static float CalcDamageTiering(float powerNeeded, bool boss, bool flies)
        {
            float tiering = powerNeeded;
            if (boss)
            {
                tiering += 10f;
            }
            if (flies)
            {
                tiering *= 2f;
            }
            return tiering;
        }
        public static int CalcPriority(float tiering, int life, int damage, int defense, int slotsUsed)
        {
            return (int)((life + damage * 3 + defense * 2) * slotsUsed * tiering);
        }

        /// <summary>
        /// Adds a NecroStats data for an npc index in <see cref="NPCs"/>
        /// <para>Parameter 1: NPC Type (short)</para>
        /// <para>Parameter 2: Tier (float), <see cref="ZombieScepter"/> is tier 1, <see cref="Insurgency"/> is tier 4</para>
        /// <para>Parameter 3 (Optional): View range (float), how close a slave needs to be to an enemy in order for it to target it. Defaults to 800</para>
        /// <para>Parameter 4+ (Optional): Two paired arguments. One string and one value</para>
        /// <para>A successful mod call would look like:</para>
        /// <code>aequus.Call("NecroStats", ModContent.NPCType{...}(), 1f);</code> OR
        /// <code>aequus.Call("NecroStats", ModContent.NPCType{...}(), 1f, 800f);</code> OR
        /// <code>aequus.Call("NecroStats", ModContent.NPCType{...}(), 1f, 800f, "PrioritizePlayerMultiplier", 4f);</code>
        /// <para>Please handle these mod calls in <see cref="Mod.PostSetupContent"/>. As buff immunities are setup in <see cref="IAddRecipes.AddRecipes(Aequus)"/></para>
        /// </summary>
        /// <param name="callingMod"></param>
        /// <param name="args"></param>
        /// <returns>'Success' when correctly handled. 'Failure' when improperly handled</returns>
        public static object CallAddNecromancyData(Mod callingMod, object[] args)
        {
            int npc = 0;
            try
            {
                AequusHelpers.UnboxInt.TryUnbox(args[2], out npc);
                AequusHelpers.UnboxFloat.TryUnbox(args[3], out float tier);
                float viewDistance = 800f;
                if (args.Length > 4)
                {
                    AequusHelpers.UnboxFloat.TryUnbox(args[4], out viewDistance);
                }
                if (Aequus.LogMore)
                    Aequus.Instance.Logger.Info("Adding necromancy data for: " + Lang.GetNPCName(npc) + " -- Tier: " + tier + ", SightRange: " + viewDistance + " --");
                var stats = new GhostInfo(tier, viewDistance);
                stats = (GhostInfo)ICrapModCallHandler.HandleArgs(stats, args.Length >= 5 ? 5 : 4, args);
                NPCs.Set(npc, stats);
            }
            catch (Exception ex)
            {
                string name = "Unknown";
                if (npc > NPCID.NegativeIDCount && npc < NPCLoader.NPCCount)
                {
                    name = Lang.GetNPCName(npc).Value;
                }
                Aequus.Instance.Logger.Error("Failed handling a mod call for NecroStats. {NPC Type: " + npc + ", Potential Name: " + name + "}", ex);
                return ModCallManager.Failure;
            }

            return ModCallManager.Success;
        }
        public static object CallAddNecromancyModBlacklist(Mod callingMod, object[] args)
        {
            AutogeneratorIgnoreMods.Add(callingMod.Name);
            return ModCallManager.Success;
        }

        /// <summary>
        /// Attempts to get the data for this NPC ID, otherwise throws an exception. Does not take netIDs into account
        /// <para>If you are wanting to use a safer method, please use <see cref="TryGet(int, out GhostInfo)"/></para>
        /// </summary>
        /// <param name="type">the NPC ID.</param>
        /// <returns></returns>
        public static GhostInfo Get(int type)
        {
            return NPCs[type];
        }
        /// <summary>
        /// Attempts to get the data for this NPC ID, otherwise throws an exception. Does not take netIDs into account
        /// <para>If you are wanting to use a safer method, please use <see cref="TryGet(NPC, out GhostInfo)"/></para>
        /// </summary>
        /// <param name="npc">the NPC instance</param>
        /// <returns></returns>
        public static GhostInfo Get(NPC npc)
        {
            return Get(npc.type);
        }

        /// <summary>
        /// Tries to get a <see cref="GhostInfo"/> value using the given NPC Net ID. Returns false if no info exists, and the popped <see cref="GhostInfo"/> would be useless.
        /// </summary>
        /// <param name="netID">The Net ID (Basically the same as npc.type, but is negative for specialized enemies)</param>
        /// <param name="value">The released <see cref="GhostInfo"/> value, when this method returns false this is set to default(GhostInfo)</param>
        /// <returns>Whether or not there is <see cref="GhostInfo"/> related to this NPC Net ID</returns>
        public static bool TryGet(int netID, out GhostInfo value)
        {
            if (netID < 0 && NPCs.ContainsKey(netID))
            {
                value = Get(netID);
                return true;
            }
            return NPCs.TryGetValue(NPCID.FromNetId(netID), out value);
        }
        /// <summary>
        /// Tries to get a <see cref="GhostInfo"/> value using the given NPC. Returns false if no info exists, and the popped <see cref="GhostInfo"/> would be useless.
        /// </summary>
        /// <param name="npc">The NPC instance</param>
        /// <param name="value">The released <see cref="GhostInfo"/> value, when this method returns false this is set to default(GhostInfo)</param>
        /// <returns>Whether or not there is <see cref="GhostInfo"/> related to this NPC Net ID</returns>
        public static bool TryGet(NPC npc, out GhostInfo value)
        {
            return TryGet(npc.netID, out value);
        }
    }
}