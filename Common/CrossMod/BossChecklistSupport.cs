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
using Terraria.Localization;
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


            // Jerry Crabson

            AddBoss(ProgressionReference.JerryCrabson, ModContent.NPCType<Crabson>(),
                AQUtils.GetTextValue((GameCulture.English, "Jerry Crabson"),
                (GameCulture.Chinese, "巨蟹蛤")),
                new int[] { ModContent.ItemType<Crabax>(), ModContent.ItemType<AquaticEnergy>(), ModContent.ItemType<JerryClawFlail>(), ModContent.ItemType<CinnabarBow>(), ModContent.ItemType<Bubbler>(), },
                new int[] { ModContent.ItemType<CrabsonTrophy>(), ModContent.ItemType<CrabsonMask>() },
                () => WorldDefeats.DownedCrabson,
                summonItem: ModContent.ItemType<MushroomClam>(),
                howToSummon: AQUtils.GetTextValue((GameCulture.English, "Summoned by using a [i:" + ModContent.ItemType<MushroomClam>() + "] at the beach."),
                (GameCulture.Chinese, "在海滩处使用 [i:" + ModContent.ItemType<MushroomClam>() + "] 召唤.")),
                bossPortraitTexture: "AQMod/Assets/BossChecklist/JerryCrabson");


            // Omega Starite

            AddBoss(ProgressionReference.OmegaStarite, ModContent.NPCType<OmegaStarite>(),
                AQUtils.GetTextValue((GameCulture.English, "Omega Starite"), (GameCulture.Chinese, "终末之星"), (GameCulture.Russian, "Омега Жизнезвезда")),
                new int[] { ModContent.ItemType<CelesteTorus>(), ModContent.ItemType<UltimateSword>(), ModContent.ItemType<CosmicTelescope>(), ModContent.ItemType<Raygun>(), ModContent.ItemType<MagicWand>(), ModContent.ItemType<CosmicEnergy>(), ModContent.ItemType<LightMatter>(), ItemID.FallenStar, },
                new int[] { ModContent.ItemType<OmegaStariteTrophy>(), ModContent.ItemType<OmegaStariteMask>(), ModContent.ItemType<DragonBall>(), ModContent.ItemType<EnchantedDye>(), ModContent.ItemType<RainbowOutlineDye>(), ModContent.ItemType<DiscoDye>(), },
                () => WorldDefeats.DownedStarite,
                summonItem: ModContent.ItemType<NovaFruit>(),
                howToSummon: AQUtils.GetTextValue((GameCulture.English, "Summoned by using an [i:" + ModContent.ItemType<NovaFruit>() + "] at night. Can also be summoned by interacting with the sword located at the source of the Glimmer."),
                (GameCulture.Chinese, "在夜晚使用 [i:" + ModContent.ItemType<NovaFruit>() + "] 召唤. 也可以通过与微光之源处的剑交互来召唤."),
                (GameCulture.Russian, "Можно призвать используя [i:" + ModContent.ItemType<NovaFruit>() + "] ночью. Также можно призвать взаимодействуя с мечом который расположен в центре Мерцающего События.")),
                bossPortraitTexture: "AQMod/Assets/BossChecklist/OmegaStarite",
                isAvailable: () => WorldDefeats.OmegaStariteIntroduction || WorldDefeats.OmegaStariteIntroduction || Main.hardMode);


            string english = "Occasionally appears during the Gale Streams!";
            string chinese = "偶尔出现在紊流风暴中!";
            string russian = "Иногда появляется во время Штормовых Потоков!";

            // Red Sprite

            AddBoss(ProgressionReference.RedSprite, ModContent.NPCType<RedSprite>(),
                AQUtils.GetTextValue((GameCulture.English, "Red Sprite"), (GameCulture.Chinese, "红色精灵")),
                new int[] { ItemID.NimbusRod, ModContent.ItemType<Nimrod>(), ModContent.ItemType<RetroGoggles>(), ItemID.SoulofFlight, ModContent.ItemType<AtmosphericEnergy>(), ModContent.ItemType<Fluorescence>(), ModContent.ItemType<PeeledCarrot>(), },
                new int[] { ModContent.ItemType<RedSpriteTrophy>(), ModContent.ItemType<RedSpriteMask>(), ModContent.ItemType<RedSpriteDye>(), },
                () => WorldDefeats.DownedRedSprite,
                howToSummon: AQUtils.GetTextValue((GameCulture.English, english),
                (GameCulture.Chinese, chinese),
                (GameCulture.Russian, russian)),
                bossPortraitTexture: "AQMod/Assets/BossChecklist/RedSprite",
                miniBoss: true);

            // Space Squid

            AddBoss(ProgressionReference.SpaceSquid, ModContent.NPCType<SpaceSquid>(),
                AQUtils.GetTextValue((GameCulture.English, "Space Squid"), (GameCulture.Chinese, "太空乌贼")),
                new int[] { ModContent.ItemType<RetroGoggles>(), ItemID.SoulofFlight, ModContent.ItemType<AtmosphericEnergy>(), ModContent.ItemType<SiphonTentacle>(), ModContent.ItemType<PeeledCarrot>(), },
                new int[] { ModContent.ItemType<SpaceSquidTrophy>(), ModContent.ItemType<SpaceSquidMask>(), ModContent.ItemType<FrostbiteDye>(), },
                () => WorldDefeats.DownedSpaceSquid,
                howToSummon: AQUtils.GetTextValue((GameCulture.English, english),
                (GameCulture.Chinese, chinese),
                (GameCulture.Russian, russian)),
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
            AQText.chooselocalizationtext(
                en_US: "Glimmer Event",
                zh_Hans: "微光事件",
                ru_RU: "Мерцающее Событие"),
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
            AQText.chooselocalizationtext(
                en_US: "Happens naturally at night. Can alternatively summoned with a [i:" + ModContent.ItemType<MythicStarfruit>() + "]. Ends when the sun rises",
                zh_Hans: "在夜晚自然开始. 也可以使用 [i:" + ModContent.ItemType<MythicStarfruit>() + "] 召唤. 在太阳升起时结束.",
                ru_RU: "Натурально появляется ночью. Также можно призвать с помощью [i:" + ModContent.ItemType<MythicStarfruit>() + "]. Кончается когда всходит солнце."),
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
            AQText.chooselocalizationtext(
                en_US: "Gale Streams",
                zh_Hans: "紊流风暴",
                ru_RU: "Штормовые Потоки"),
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
            AQText.chooselocalizationtext(
                en_US: "Begins when the wind is above 40 mph, and ends when it's less than 34 mph. Will also end if the wind goes above 300 mph. You can modify the speed of the wind using [i:" + ModContent.ItemType<Items.Tools.TheFan>() + "]",
                zh_Hans: "风速大于 40 mph时开始, 风速小于 34 mph时结束. 你可以使用 [i:" + ModContent.ItemType<Items.Tools.TheFan>() + "] 更改风速",
                ru_RU: "Начинается когда скорость ветра превышает 40 миль в час, и заканчивается когда скорость ветра ниже 34 миль в час. Также закончится если скорость ветра превисит 300 миль в час. Вы можете изменять скорость ветра используя [i:" + ModContent.ItemType<Items.Tools.TheFan>() + "]"),
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
            AQText.chooselocalizationtext(
                   en_US: "Demon Siege",
                   zh_Hans: "恶魔围攻",
                   ru_RU: "Осада Демонов"),
            (Func<bool>)(() => WorldDefeats.DownedDemonSiege),
            0,
            new List<int>()
            {
                ModContent.ItemType<DemonicCursorDye>(),
                ModContent.ItemType<HellBeamDye>(),
            },
            items,
            AQText.chooselocalizationtext(
                   en_US: "Can be summoned using: " + summonItems + " at a Gore Nest.",
                   zh_Hans: "在血巢处使用 " + summonItems + " 召唤.",
                   ru_RU: "Можно призвать используя: " + summonItems + " в кровавом гнезде."),
            null,
            "AQMod/Content/World/BossChecklistDemonSiege",
            "AQMod/Content/World/DemonSiege",
            null);
        }
    }
}