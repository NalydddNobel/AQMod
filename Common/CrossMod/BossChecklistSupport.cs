using AQMod.Content.World.Events;
using AQMod.Items.Accessories;
using AQMod.Items.Accessories.Summon;
using AQMod.Items.Armor.Vanity.BossMasks;
using AQMod.Items.Dyes;
using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Misc;
using AQMod.Items.Misc.Cursor.Demonic;
using AQMod.Items.Misc.Summons;
using AQMod.Items.Placeable.Furniture;
using AQMod.Items.Potions.Foods;
using AQMod.Items.Tools;
using AQMod.Items.Tools.Fishing;
using AQMod.Items.Tools.Map;
using AQMod.Items.Weapons.Magic;
using AQMod.Items.Weapons.Melee;
using AQMod.Items.Weapons.Ranged;
using AQMod.Items.Weapons.Summon;
using AQMod.Localization;
using AQMod.NPCs.Bosses;
using AQMod.NPCs.Monsters.DemonSiegeMonsters;
using AQMod.NPCs.Monsters.GaleStreamsMonsters;
using AQMod.NPCs.Monsters.GlimmerMonsters;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Common.CrossMod
{
    public sealed class BossChecklistSupport
    {
        public static class ProgressionReference
        {
            public static float JerryCrabson = 2f;

            public static float OmegaStarite = 5.99f;
            internal static float WallofFlesh = 6f;

            public static float RedSprite = 6.67f;
            public static float SpaceSquid = 6.68f;
        }

        private static void AddBoss(float progression, AQUtils.ArrayInterpreter<int> boss, string bossName, AQUtils.ArrayInterpreter<int> loot, AQUtils.ArrayInterpreter<int> collectibles, Func<bool> isDowned, int summonItem = 0, string howToSummon = null,
            string bossHeadTexture = null, string bossPortraitTexture = null, Func<bool> isAvailable = null, string despawnMessage = null, bool miniBoss = false)
        {
            AQMod.bossChecklist.Call(
                miniBoss ? "AddMiniBoss" : "AddBoss",
                progression,
                boss.Arr.ToList(),
                AQMod.Instance,
                bossName,
                isDowned,
                summonItem,
                collectibles.Arr.ToList(),
                loot.Arr.ToList(),
                howToSummon,
                despawnMessage,
                bossPortraitTexture,
                bossHeadTexture,
                isAvailable);
        }
        internal static void SetupContent(AQMod aQMod)
        {
            if (!AQMod.bossChecklist.IsActive)
            {
                return;
            }

            AQText.AdjustTranslation("BC.Spawn.Crabson", "BC.Spawn.Crabson.1", (s) => string.Format(s, AQText.Item<MushroomClam>()));
            AQText.AdjustTranslation("BC.Spawn.OmegaStarite", "BC.Spawn.OmegaStarite.1", (s) => string.Format(s, AQText.Item<NovaFruit>()));
            AQText.AdjustTranslation("BC.Spawn.GlimmerEvent", "BC.Spawn.GlimmerEvent.1", (s) => string.Format(s, AQText.Item<MythicStarfruit>()));
            AQText.AdjustTranslation("BC.Spawn.GaleStreams", "BC.Spawn.GaleStreams.1", (s) => string.Format(s, AQText.Item<TheFan>()));

            // Jerry Crabson

            AddBoss(ProgressionReference.JerryCrabson, ModContent.NPCType<Crabson>(),
                "$Mods.AQMod.NPCName.Crabson",
                new int[] { ModContent.ItemType<Crabax>(), ModContent.ItemType<AquaticEnergy>(), ModContent.ItemType<JerryClawFlail>(), ModContent.ItemType<CinnabarBow>(), ModContent.ItemType<Bubbler>(), },
                new int[] { ModContent.ItemType<CrabsonTrophy>(), ModContent.ItemType<CrabsonMask>() },
                () => WorldDefeats.DownedCrabson,
                summonItem: ModContent.ItemType<MushroomClam>(),
                howToSummon: "$Mods.AQMod.BC.Spawn.Crabson.1",
                bossPortraitTexture: "AQMod/Assets/BossChecklist/JerryCrabson");


            // Omega Starite

            AddBoss(ProgressionReference.OmegaStarite, ModContent.NPCType<OmegaStarite>(),
                "$Mods.AQMod.NPCName.OmegaStarite",
                new int[] { ModContent.ItemType<CelesteTorus>(), ModContent.ItemType<UltimateSword>(), ModContent.ItemType<CosmicTelescope>(), ModContent.ItemType<Raygun>(), ModContent.ItemType<MagicWand>(), ModContent.ItemType<CosmicEnergy>(), ModContent.ItemType<LightMatter>(), ItemID.FallenStar, },
                new int[] { ModContent.ItemType<OmegaStariteTrophy>(), ModContent.ItemType<OmegaStariteMask>(), ModContent.ItemType<DragonBall>(), ModContent.ItemType<EnchantedDye>(), ModContent.ItemType<RainbowOutlineDye>(), ModContent.ItemType<DiscoDye>(), },
                () => WorldDefeats.DownedStarite,
                summonItem: ModContent.ItemType<NovaFruit>(),
                howToSummon: "$Mods.AQMod.BC.Spawn.OmegaStarite.1",
                bossPortraitTexture: "AQMod/Assets/BossChecklist/OmegaStarite",
                isAvailable: () => WorldDefeats.OmegaStariteIntroduction || WorldDefeats.OmegaStariteIntroduction || Main.hardMode);


            // Red Sprite

            AddBoss(ProgressionReference.RedSprite, ModContent.NPCType<RedSprite>(),
                "$Mods.AQMod.NPCName.RedSprite",
                new int[] { ItemID.NimbusRod, ModContent.ItemType<Nimrod>(), ModContent.ItemType<RetroGoggles>(), ItemID.SoulofFlight, ModContent.ItemType<AtmosphericEnergy>(), ModContent.ItemType<Fluorescence>(), ModContent.ItemType<PeeledCarrot>(), },
                new int[] { ModContent.ItemType<RedSpriteTrophy>(), ModContent.ItemType<RedSpriteMask>(), ModContent.ItemType<RedSpriteDye>(), },
                () => WorldDefeats.DownedRedSprite,
                howToSummon: "$Mods.AQMod.BC.Spawn.GaleStreamsMiniboss",
                bossPortraitTexture: "AQMod/Assets/BossChecklist/RedSprite",
                miniBoss: true);

            // Space Squid

            AddBoss(ProgressionReference.SpaceSquid, ModContent.NPCType<SpaceSquid>(),
                "$Mods.AQMod.NPCName.SpaceSquid",
                new int[] { ModContent.ItemType<RetroGoggles>(), ItemID.SoulofFlight, ModContent.ItemType<AtmosphericEnergy>(), ModContent.ItemType<SiphonTentacle>(), ModContent.ItemType<PeeledCarrot>(), },
                new int[] { ModContent.ItemType<SpaceSquidTrophy>(), ModContent.ItemType<SpaceSquidMask>(), ModContent.ItemType<FrostbiteDye>(), },
                () => WorldDefeats.DownedSpaceSquid,
                howToSummon: "$Mods.AQMod.BC.Spawn.GaleStreamsMiniboss",
                bossPortraitTexture: "AQMod/Assets/BossChecklist/SpaceSquid",
                miniBoss: true);

            //bossChecklist.Call("AddEvent",
            //progression,
            //enemies,
            //_aQMod,
            //eventName,
            //isDowned,
            //summonItem,
            //collectibles,
            //loot,
            //summonDescription,
            //despawnMessage,
            //portraitTexture,
            //bossHead,
            //available);            

            AQMod.bossChecklist.mod.Call("AddEvent",
            2.1f,
            new List<int>() {
                ModContent.NPCType<Starite>(),
                ModContent.NPCType<SuperStarite>(),
                ModContent.NPCType<HyperStarite>(),
            },
            aQMod,
            "$Mods.AQMod.GlimmerEvent",
            (Func<bool>)(() => WorldDefeats.DownedGlimmer),
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
                ModContent.ItemType<StariteStaff>(),
                ModContent.ItemType<MoonShoes>(),
                ModContent.ItemType<Ultranium>(),
            },
            "$Mods.AQMod.BC.Spawn.GlimmerEvent.1",
            null,
            "AQMod/Content/World/BossChecklistGlimmerEvent",
            "AQMod/Content/World/GlimmerEvent",
            null);

            AQMod.bossChecklist.mod.Call("AddEvent",
            6.66f,
            new List<int>() {
                ModContent.NPCType<Vraine>(),
                ModContent.NPCType<WhiteSlime>(),
                ModContent.NPCType<StreamingBalloon>(),
                ModContent.NPCType<RedSprite>(),
                ModContent.NPCType<SpaceSquid>(),
            },
            aQMod,
            "$Mods.AQMod.GaleStreams",
            (Func<bool>)(() => WorldDefeats.DownedGaleStreams),
            0,
            new List<int>()
            {
                ModContent.ItemType<RedSpriteTrophy>(),
                ModContent.ItemType<SpaceSquidTrophy>(),
                ModContent.ItemType<CensorDye>(),
                ModContent.ItemType<RedSpriteDye>(),
                ModContent.ItemType<FrostbiteDye>(),
            },
            new List<int>()
            {
                ItemID.NimbusRod,
                ModContent.ItemType<Vrang>(),
                ModContent.ItemType<Umystick>(),
                ModContent.ItemType<Nimrod>(),
                ModContent.ItemType<AtmosphericEnergy>(),
                ModContent.ItemType<Fluorescence>(),
                ModContent.ItemType<SiphonTentacle>(),
                ItemID.SoulofFlight,
                ModContent.ItemType<PeeledCarrot>(),
                ModContent.ItemType<CinnamonRoll>(),
            },
            "$Mods.AQMod.BC.Spawn.GaleStreams.1",
            null,
            "AQMod/Content/World/BossChecklistGaleStreams",
            "AQMod/Content/World/GaleStreams",
            null);

            var items = new List<int>()
                {
                    ModContent.ItemType<DemonicEnergy>(),
                    ModContent.ItemType<DegenerationRing>(),
                    ItemID.MagmaStone,
                    ItemID.LavaCharm,
                    ItemID.ObsidianRose,
                };
            string summonItems = "";
            foreach (var upgrade in DemonSiege.Upgrades)
            {
                if (summonItems != "")
                    summonItems += ", ";
                summonItems += "[i:" + upgrade.baseItem + "]";
                items.Add(upgrade.rewardItem);
            }

            AQText.AdjustTranslation("BC.Spawn.DemonSiege", "BC.Spawn.DemonSiege.1", (s) => string.Format(s, summonItems));

            AQMod.bossChecklist.mod.Call("AddEvent",
            4.1f,
            new List<int>()
            {
                ModContent.NPCType<Cindera>(),
                ModContent.NPCType<Magmalbubble>(),
                ModContent.NPCType<TrapImp>(),
                ModContent.NPCType<Trapper>(),
            },
            aQMod,
            "$Mods.AQMod.DemonSiege",
            (Func<bool>)(() => WorldDefeats.DownedDemonSiege),
            0,
            new List<int>()
            {
                ModContent.ItemType<DemonicCursorDye>(),
                ModContent.ItemType<HellBeamDye>(),
            },
            items,
            "$Mods.AQMod.BC.Spawn.DemonSiege.1",
            null,
            "AQMod/Content/World/BossChecklistDemonSiege",
            "AQMod/Content/World/DemonSiege",
            null);
        }
    }
}