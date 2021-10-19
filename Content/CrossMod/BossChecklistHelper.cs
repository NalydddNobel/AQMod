using AQMod.Common;
using AQMod.Content.WorldEvents.Siege;
using AQMod.Items;
using AQMod.Items.Accessories;
using AQMod.Items.Accessories.Rings;
using AQMod.Items.Accessories.Shoes;
using AQMod.Items.Armor.Crab;
using AQMod.Items.GrapplingHooks;
using AQMod.Items.Placeable;
using AQMod.Items.Tools;
using AQMod.Items.Tools.Markers;
using AQMod.Items.Vanities;
using AQMod.Items.Vanities.CursorDyes;
using AQMod.Items.Vanities.Dyes;
using AQMod.Items.Vanities.Pets;
using AQMod.Items.Weapons.Magic;
using AQMod.Items.Weapons.Melee;
using AQMod.Items.Weapons.Melee.Flails;
using AQMod.Items.Weapons.Melee.Yoyos;
using AQMod.Items.Weapons.Ranged;
using AQMod.Items.Weapons.Ranged.Bullet;
using AQMod.Items.Weapons.Summon;
using AQMod.Localization;
using AQMod.NPCs.Boss.Crabson;
using AQMod.NPCs.Boss.Starite;
using AQMod.NPCs.Monsters.AquaticEvent;
using AQMod.NPCs.Monsters.CosmicEvent;
using AQMod.NPCs.Monsters.DemonicEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Content.CrossMod
{
    internal class BossChecklistHelper
    {
        private Mod _bossChecklist;
        private Mod _aQMod;
        private BossChecklistHelper(Mod mod, AQMod aQMod)
        {
            _bossChecklist = mod;
            _aQMod = aQMod;
        }

        public static void Setup(AQMod aQMod)
        {
            try
            {
                var bossChecklist = ModLoader.GetMod("BossChecklist");
                if (bossChecklist == null)
                    return;
                var b = new BossChecklistHelper(bossChecklist, aQMod);
                b.AddBoss(6.1f,
                    ModContent.NPCType<OmegaStarite>(),
                    AQText.chooselocalizationtext(en_US: "Omega Starite", zh_Hans: "终末之星"),
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
                        ModContent.ItemType<Raygun>(),
                        ModContent.ItemType<MagicWand>(),
                        ModContent.ItemType<CosmicEnergy>(),
                        ItemID.SoulofFlight,
                        ItemID.FallenStar,
                    },
                    "Summoned by using an [i:" + ModContent.ItemType<NovaFruit>() + "] or [i:" + ModContent.ItemType<UltimateStarfruit>() + "] at night. Can also be summoned by interacting with the sword located at the center of the Glimmer Event",
                    "AQMod/Assets/Textures/BossChecklist_OmegaStarite");
                b.AddBoss(
                    2f,
                    ModContent.NPCType<JerryCrabson>(),
                    AQText.chooselocalizationtext("Jerry Crabson", "巨蟹蛤"),
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
                    "AQMod/Assets/Textures/BossChecklist_JerryCrabson");
                b.AddEvent(
                    0.5f,
                    new List<int>() {
                        ModContent.NPCType<ArrowCrab>(),
                        ModContent.NPCType<SoliderCrabs>(),
                        ModContent.NPCType<HermitCrab>(),
                        ModContent.NPCType<StriderCrab>(),
                    },
                    AQText.chooselocalizationtext("Crab Season", "蟹季"),
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
                    "AQMod/Assets/Textures/BossChecklist_CrabSeason",
                    null);
                b.AddEvent(
                    2.1f,
                    new List<int>() {
                ModContent.NPCType<Starite>(),
                ModContent.NPCType<SuperStarite>(),
                ModContent.NPCType<HyperStarite>(),
                    },
                    AQText.chooselocalizationtext("Glimmer Event", "微光事件"),
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
                    ModContent.ItemType<SpaceShot>(),
                    ModContent.ItemType<StariteStaff>(),
                    ModContent.ItemType<MoonShoes>(),
                    ModContent.ItemType<Ultranium>(),
                    },
                    "Happens naturally at night. Can be summoned with a [i:" + ModContent.ItemType<MythicStarfruit>() + "] or [i:" + ModContent.ItemType<UltimateStarfruit>() + "]. Ends when the sun rises",
                    "AQMod/Assets/Textures/BossChecklist_GlimmerEvent",
                    null);
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
                foreach (var upgrade in DemonSiege._upgrades)
                {
                    if (summonItems != "")
                    {
                        summonItems += ", ";
                    }
                    summonItems += "[i:" + upgrade.baseItem + "]";
                    items.Add(upgrade.rewardItem);
                }
                b.AddEvent(
                4.1f,
                new List<int>() {
                        ModContent.NPCType<Cindera>(),
                        ModContent.NPCType<Magmalbubble>(),
                        ModContent.NPCType<TrapImp>(),
                        ModContent.NPCType<Trapper>(),
                },
                AQText.chooselocalizationtext("Demon Siege", "恶魔围攻"),
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
                "An event that happens when you are trying to upgrade a demonite or crimtane item. Can be summoned using: " + summonItems + " at a Gore Nest.",
                "AQMod/Assets/Textures/BossChecklist_DemonSiege",
                null);
            }
            catch (Exception e)
            {
                AQMod.Instance.Logger.Error("An error occured when loading boss checklist entries: " + e.Message);
            }
        }

        public void AddBoss(float progression, int boss, string bossName, Func<bool> isDowned, int summonItem, List<int> collectibles, List<int> loot, string summonDescription = "",
                string portraitTexture = null,
                string bossHeadTexture = null,
                string despawnMessage = null, Func<bool> available = null)
        {
            _bossChecklist.Call(
            "AddBoss",
            progression,
            boss,
            _aQMod,
            bossName,
            isDowned,
            summonItem,
            collectibles,
            loot,
            summonDescription,
            despawnMessage,
            portraitTexture,
            bossHeadTexture,
            available);
        }

        public void AddEvent(float progression, List<int> enemies, string eventName, Func<bool> isDowned, int summonItem, List<int> collectibles, List<int> loot, string summonDescription = "",
                string portraitTexture = null,
                string despawnMessage = null)
        {
            _bossChecklist.Call("AddEvent",
           progression,
           enemies,
           _aQMod,
           eventName,
           isDowned,
           summonItem,
           collectibles,
           loot,
           summonDescription,
           despawnMessage,
           portraitTexture);
        }
    }
}