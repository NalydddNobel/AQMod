using AQMod.Common;
using AQMod.Items;
using AQMod.Items.Accessories;
using AQMod.Items.Accessories.Rings;
using AQMod.Items.Accessories.Shoes;
using AQMod.Items.Armor;
using AQMod.Items.Armor.Crab;
using AQMod.Items.BossItems.Crabson;
using AQMod.Items.BossItems.Starite;
using AQMod.Items.Energies;
using AQMod.Items.GrapplingHooks;
using AQMod.Items.Tools;
using AQMod.Items.Tools.Markers;
using AQMod.Items.Vanities;
using AQMod.Items.Vanities.CursorDyes;
using AQMod.Items.Vanities.Dyes;
using AQMod.Items.Weapons.Magic;
using AQMod.Items.Weapons.Melee;
using AQMod.Items.Weapons.Melee.Flails;
using AQMod.Items.Weapons.Melee.Yoyos;
using AQMod.Items.Weapons.Ranged.Bows;
using AQMod.Items.Weapons.Ranged.Bullet;
using AQMod.Items.Weapons.Summon;
using AQMod.NPCs.CrabSeason;
using AQMod.NPCs.Crabson;
using AQMod.NPCs.Glimmer;
using AQMod.NPCs.SiegeEvent;
using AQMod.NPCs.Starite;
using System;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod
{
    internal static class bosschecklist
    {
        private static bosschecklistEntry_SINGLE_BOSS OmegaStarite => new bosschecklistEntry_SINGLE_BOSS(
                6.1f,
                ModContent.NPCType<OmegaStarite>(),
                "Omega Starite",
                () => WorldDefeats.DownedStarite,
                ModContent.ItemType<NovaFruit>(),
                new List<int>() {
                    ModContent.ItemType<OmegaStariteTrophy>(),
                    ModContent.ItemType<OmegaStariteMask>(),
                    ModContent.ItemType<DragonBall>(),
                    ModContent.ItemType<EnchantedDye>(),
                    ModContent.ItemType<RainbowOutlineDye>(),
                    ModContent.ItemType<DiscoDye>(),
                },
                new List<int>() {
                    ModContent.ItemType<CelesteTorus>(),
                    ModContent.ItemType<UltimateSword>(),
                    ModContent.ItemType<CosmicTelescope>(),
                    ModContent.ItemType<Galactium>(),
                    ModContent.ItemType<Raygun>(),
                    ModContent.ItemType<MagicWand>(),
                    ModContent.ItemType<CosmicEnergy>(),
                    ItemID.SoulofFlight,
                    ItemID.FallenStar,
                },
                "Summoned by using an [i:" + ModContent.ItemType<NovaFruit>() + "] or [i:" + ModContent.ItemType<UltimateStarfruit>() + "] at night. Can also be summoned by interacting with the sword located at the center of the Glimmer Event",
                "AQMod/Assets/Textures/BossChecklist_OmegaStarite"
            );

        private static bosschecklistEntry_SINGLE_BOSS JerryCrabson => new bosschecklistEntry_SINGLE_BOSS(
                2f,
                ModContent.NPCType<JerryCrabson>(),
                "Jerry Crabson",
                () => WorldDefeats.DownedCrabson,
                ModContent.ItemType<MushroomClam>(),
                new List<int>()
                {
                    ModContent.ItemType<CrabsonTrophy>(),
                    ModContent.ItemType<CrabsonMask>(),
                },
                new List<int>() {
                    ModContent.ItemType<Crabsol>(),
                    ModContent.ItemType<JerryClawFlail>(),
                    ModContent.ItemType<CinnabarBow>(),
                    ModContent.ItemType<Bubbler>(),
                    ModContent.ItemType<AquaticEnergy>(),
                },
                "Summoned by using a [i:" + ModContent.ItemType<MushroomClam>() + "] at the beach.",
                "AQMod/Assets/Textures/BossChecklist_JerryCrabson"
            );

        private static bosschecklistEntry_EVENT CrabSeason => new bosschecklistEntry_EVENT(
                0.5f,
                new List<int>() {
                    ModContent.NPCType<ArrowCrab>(),
                    ModContent.NPCType<SoliderCrabs>(),
                    ModContent.NPCType<HermitCrab>(),
                    ModContent.NPCType<StriderCrab>(),
                },
                "Crab Season",
                () => WorldDefeats.DownedCrabson,
                0,
                new List<int>()
                {
                    ModContent.ItemType<FishyFins>(),
                },
                new List<int>()
                {
                    ModContent.ItemType<CrabShell>(),
                    ModContent.ItemType<HermitShell>(),
                    ModContent.ItemType<StriderCarapace>(),
                    ModContent.ItemType<StriderPalms>(),
                    ModContent.ItemType<StriderHook>(),
                },
                "Begins naturally and ends naturally at random times. You can check the time when the event begins and ends using a [i:" + ModContent.ItemType<CrabClock>() + "].",
                null,
                "AQMod/Assets/Textures/BossChecklist_CrabSeason"
            );

        private static bosschecklistEntry_EVENT GlimmerEvent => new bosschecklistEntry_EVENT(
                2.1f,
                new List<int>() {
                ModContent.NPCType<Starite>(),
                ModContent.NPCType<SuperStarite>(),
                ModContent.NPCType<HyperStarite>(),
                },
                "Glimmer Event",
                () => WorldDefeats.DownedGlimmer,
                ModContent.ItemType<MythicStarfruit>(),
                new List<int>()
                {
                    ModContent.ItemType<CelesitalEightBall>(),
                    ModContent.ItemType<HypnoDye>(),
                    ModContent.ItemType<OutlineDye>(),
                    ModContent.ItemType<ScrollDye>(),
                },
                new List<int>()
                {
                    ModContent.ItemType<CosmicEnergy>(),
                    ItemID.Nazar,
                    ModContent.ItemType<RetroGoggles>(),
                    ModContent.ItemType<StariteBlade>(),
                    ModContent.ItemType<StariteSpinner>(),
                    ModContent.ItemType<SpaceShot>(),
                    ModContent.ItemType<StariteStaff>(),
                    ModContent.ItemType<MoonShoes>(),
                    ModContent.ItemType<Ultranium>(),
                },
                "Happens naturally at night. Can be summoned with a [i:" + ModContent.ItemType<MythicStarfruit>() + "] or [i:" + ModContent.ItemType<UltimateStarfruit>() + "]. Ends when the sun rises",
                null,
                "AQMod/Assets/Textures/BossChecklist_GlimmerEvent"
            );

        private static bosschecklistEntry_EVENT DemonSiege { get
        {
                var items = new List<int>()
                {
                    ModContent.ItemType<DemonicEnergy>(),
                    ModContent.ItemType<DegenerationRing>(),
                    ModContent.ItemType<PowPunch>(),
                    ItemID.MagmaStone,
                    ItemID.LavaCharm,
                    ItemID.ObsidianRose,
                };
                string summonItems = "";
                foreach (var upgrade in Content.WorldEvents.DemonSiege.DemonSiege._upgrades)
                {
                    if (summonItems != "")
                    {
                        summonItems += ", ";
                    }
                    summonItems += "[i:" + upgrade.baseItem + "]";
                    items.Add(upgrade.rewardItem);
                }
                return new bosschecklistEntry_EVENT(
                4.1f,
                new List<int>() {
                    ModContent.NPCType<Cindera>(),
                    ModContent.NPCType<Magmalbubble>(),
                    ModContent.NPCType<TrapImp>(),
                    ModContent.NPCType<Trapper>(),
                },
                "Demon Siege",
                () => WorldDefeats.DownedDemonSiege,
                ItemID.LightsBane,
                new List<int>()
                {
                    ModContent.ItemType<DemonicCursorDye>(),
                    ModContent.ItemType<HellBeamDye>(),
                    ItemID.LavaLamp,
                    ItemID.HellboundBanner,
                    ItemID.HellHammerBanner,
                    ItemID.HelltowerBanner,
                    ItemID.LostHopesofManBanner,
                    ItemID.ObsidianWatcherBanner,
                    ItemID.LavaEruptsBanner,
                },
                items,
                "An event that happens when you are trying to upgrade a demonite or crimtane item. Can be summoned using: " + summonItems,
                null,
                "AQMod/Assets/Textures/BossChecklist_DemonSiege"
                );
            }
        }

        internal static void setup(AQMod aQMod)
        {
            try
            {
                var bossChecklist = ModLoader.GetMod("BossChecklist");
                if (bossChecklist == null)
                    return;
                OmegaStarite.addentry(bossChecklist, aQMod);
                JerryCrabson.addentry(bossChecklist, aQMod);
                GlimmerEvent.addentry(bossChecklist, aQMod);
                CrabSeason.addentry(bossChecklist, aQMod);
                DemonSiege.addentry(bossChecklist, aQMod);
            }
            catch
            {
            }
        }
    }

    internal interface IbosschecklistEntry
    {
        void addentry(Mod _bossChecklist, Mod _aQMod);
    }

    internal struct bosschecklistEntry_SINGLE_BOSS : IbosschecklistEntry
    {
        private readonly float _progression;
        private readonly int _boss;
        private readonly string _bossName;
        private readonly Func<bool> _downedFlag;
        private readonly int _summonItem;
        private readonly List<int> _collectibles;
        private readonly List<int> _loot;
        private readonly string _summonDesc;
        private readonly string _portraitTexture;
        private readonly string _bossHeadTexture;
        private readonly string _despawnMessage;
        private readonly Func<bool> _available;

        public bosschecklistEntry_SINGLE_BOSS(float progression, int boss, string bossName, Func<bool> isDowned, int summonItem, List<int> collectibles, List<int> loot, string summonDescription = "",
            string portraitTexture = null,
            string bossHeadTexture = null,
            string despawnMessage = null, Func<bool> available = null)
        {
            _progression = progression;
            _boss = boss;
            _bossName = bossName;
            _downedFlag = isDowned;
            _summonItem = summonItem;
            _collectibles = collectibles;
            _loot = loot;
            _summonDesc = summonDescription;
            _portraitTexture = portraitTexture;
            _bossHeadTexture = bossHeadTexture;
            _despawnMessage = despawnMessage;
            _available = available;
        }

        public void addentry(Mod _bossChecklist, Mod _aQMod)
        {
            _bossChecklist.Call(
            "AddBoss",
            _progression,
            _boss,
            _aQMod,
            _bossName,
            _downedFlag,
            _summonItem,
            _collectibles,
            _loot,
            _summonDesc,
            _despawnMessage,
            _portraitTexture,
            _bossHeadTexture,
            _available);
        }
    }

    internal struct bosschecklistEntry_EVENT : IbosschecklistEntry
    {
        private readonly float _progression;
        private readonly List<int> _enemies;
        private readonly string _eventName;
        private readonly Func<bool> _downedFlag;
        private readonly int _summonItem;
        private readonly List<int> _collectibles;
        private readonly List<int> _loot;
        private readonly string _summonDesc;
        private readonly string _portraitTexture;
        private readonly string _despawnMessage;

        public bosschecklistEntry_EVENT(float priority, List<int> npcs, string name, Func<bool> isDowned, int summon, List<int> collectibles, List<int> loot, string description, string eventEndingMessage, string texture)
        {
            _progression = priority;
            _enemies = npcs;
            _eventName = name;
            _downedFlag = isDowned;
            _summonItem = summon;
            _collectibles = collectibles;
            _loot = loot;
            _summonDesc = description;
            _despawnMessage = eventEndingMessage;
            _portraitTexture = texture;
        }

        public void addentry(Mod _bossChecklist, Mod _aQMod)
        {
            _bossChecklist.Call("AddEvent",
                   _progression,
                   _enemies,
                   _aQMod,
                   _eventName,
                   _downedFlag,
                   _summonItem,
                   _collectibles,
                   _loot,
                   _summonDesc,
                   _despawnMessage,
                   _portraitTexture);
        }
    }
}