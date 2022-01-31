using AQMod.Content.World.Events.DemonSiege;
using AQMod.Items;
using AQMod.Items.Accessories;
using AQMod.Items.Armor;
using AQMod.Items.Dyes;
using AQMod.Items.Dyes.Cursor;
using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Pets;
using AQMod.Items.Placeable.Furniture;
using AQMod.Items.Potions;
using AQMod.Items.Tools;
using AQMod.Items.Tools.Axe;
using AQMod.Items.Tools.Fishing;
using AQMod.Items.Tools.Map;
using AQMod.Items.Weapons.Magic;
using AQMod.Items.Weapons.Melee;
using AQMod.Items.Weapons.Ranged;
using AQMod.Items.Weapons.Summon;
using AQMod.Localization;
using AQMod.NPCs.Bosses;
using AQMod.NPCs.Monsters.DemonSiege;
using AQMod.NPCs.Monsters.GaleStreams;
using AQMod.NPCs.Monsters.GlimmerEvent;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Common.CrossMod
{
    internal static class BossChecklistSupport
    {
        public static void AddSupport(AQMod aQMod)
        {
            if (!AQMod.bossChecklist.active)
            {
                return;
            }
            AQMod.bossChecklist.mod.Call(
            "AddBoss",
            2f,
            ModContent.NPCType<JerryCrabson>(),
            aQMod,
            AQText.chooselocalizationtext(
                en_US: "Crabson",
                zh_Hans: "巨蟹蛤"),
            (Func<bool>)(() => WorldDefeats.DownedCrabson),
            ModContent.ItemType<MushroomClam>(),
            new List<int>()
            {
                ModContent.ItemType<CrabsonTrophy>(),
                ModContent.ItemType<CrabsonMask>(),
            },
            new List<int>() {
                ModContent.ItemType<Crabax>(),
                ModContent.ItemType<Crabsol>(),
                ModContent.ItemType<JerryClawFlail>(),
                ModContent.ItemType<CinnabarBow>(),
                ModContent.ItemType<Bubbler>(),
                ModContent.ItemType<AquaticEnergy>(),
            },
            AQText.chooselocalizationtext(
                en_US: "Summoned by using a [i:" + ModContent.ItemType<MushroomClam>() + "] at the beach.",
                zh_Hans: null),
            null,
            "AQMod/Assets/BossChecklist/JerryCrabson",
            null,
            null);
            AQMod.bossChecklist.mod.Call(
            "AddBoss",
            6f,
            ModContent.NPCType<OmegaStarite>(),
            aQMod,
            AQText.chooselocalizationtext(
                en_US: "Omega Starite",
                zh_Hans: "终末之星",
                ru_RU: "Омега Жизнезвезда"),
            (Func<bool>)(() => WorldDefeats.DownedStarite),
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
                ModContent.ItemType<LightMatter>(),
                ItemID.FallenStar,
            },
            AQText.chooselocalizationtext(
                en_US: "Summoned by using an [i:" + ModContent.ItemType<NovaFruit>() + "] at night. Can also be summoned by interacting with the sword located at the center of the Glimmer Event.",
                zh_Hans: "在夜晚使用 [i:" + ModContent.ItemType<NovaFruit>() + "] 召唤. 也可以与微光事件中心的剑交互来召唤.",
                ru_RU: "Можно призвать используя [i:" + ModContent.ItemType<NovaFruit>() + "] ночью. Также можно призвать взаимодействуя с мечом который расположен в центре Мерцающего События."),
            null,
            "AQMod/Assets/BossChecklist/OmegaStarite",
            null,
            (Func<bool>)(() => WorldDefeats.OmegaStariteIntroduction || WorldDefeats.OmegaStariteIntroduction || Main.hardMode));

            AQMod.bossChecklist.mod.Call(
            "AddMiniBoss",
            6.67f,
            ModContent.NPCType<RedSprite>(),
            aQMod,
            AQText.chooselocalizationtext(
                en_US: "Red Sprite",
                zh_Hans: "红色精灵"),
            (Func<bool>)(() => WorldDefeats.DownedRedSprite),
            0,
            new List<int>()
            {
                ModContent.ItemType<RedSpriteTrophy>(),
                ModContent.ItemType<RedSpriteMask>(),
                ModContent.ItemType<RedSpriteDye>(),
            },
            new List<int>()
            {
                ItemID.NimbusRod,
                ModContent.ItemType<Nimrod>(),
                ModContent.ItemType<RetroGoggles>(),
                ItemID.SoulofFlight,
                ModContent.ItemType<AtmosphericEnergy>(),
                ModContent.ItemType<Fluorescence>(),
                ModContent.ItemType<PeeledCarrot>(),
            },
            AQText.chooselocalizationtext(
                en_US: "Occasionally appears during the Gale Streams!",
                zh_Hans: "偶尔出现在紊流风暴中!",
                ru_RU: "Иногда появляется во время Штормовых Потоков!"),
            null,
            "AQMod/Assets/BossChecklist/RedSprite",
            null,
            null);

            AQMod.bossChecklist.mod.Call(
            "AddMiniBoss",
            6.68f,
            ModContent.NPCType<SpaceSquid>(),
            aQMod,
            AQText.chooselocalizationtext(
                en_US: "Space Squid",
                zh_Hans: null),
            (Func<bool>)(() => WorldDefeats.DownedSpaceSquid),
            0,
            new List<int>()
            {
                ModContent.ItemType<SpaceSquidTrophy>(),
                ModContent.ItemType<SpaceSquidMask>(),
                ModContent.ItemType<FrostbiteDye>(),
            },
            new List<int>()
            {
                ModContent.ItemType<RetroGoggles>(),
                ItemID.SoulofFlight,
                ModContent.ItemType<AtmosphericEnergy>(),
                ModContent.ItemType<SiphonTentacle>(),
                ModContent.ItemType<PeeledCarrot>(),
            },
            AQText.chooselocalizationtext(
                        en_US: "Occasionally appears during the Gale Streams!",
                        zh_Hans: "偶尔出现在紊流风暴中!",
                        ru_RU: "Иногда появляется во время Штормовых Потоков!"),
            null,
            "AQMod/Assets/BossChecklist/SpaceSquid",
            null,
            null);

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
                zh_Hans: "风速大于40 mph时开始, 风速小于34 mph时结束. 你可以使用 [i:" + ModContent.ItemType<Items.Tools.TheFan>() + "] 更改风速",
                ru_RU: "Начинается когда скорость ветра превышает 40 миль в час, и заканчивается когда скорость ветра ниже 34 миль в час. Также закончится если скорость ветра превисит 300 миль в час. Вы можете изменять скорость ветра используя [i:" + ModContent.ItemType<Items.Tools.TheFan>() + "]"),
            null,
            "AQMod/Content/World/BossChecklistGaleStreams",
            "AQMod/Content/World/GaleStreams",
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
            (Func<bool>)(() => WorldDefeats.DownedGaleStreams),
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