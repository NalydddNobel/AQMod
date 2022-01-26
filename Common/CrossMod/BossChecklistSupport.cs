using AQMod.Assets;
using AQMod.Items;
using AQMod.Items.Accessories;
using AQMod.Items.Armor;
using AQMod.Items.Dyes;
using AQMod.Items.Foods;
using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Pets;
using AQMod.Items.Placeable.Furniture;
using AQMod.Items.Tools.Axe;
using AQMod.Items.Tools.Map;
using AQMod.Items.Weapons.Magic;
using AQMod.Items.Weapons.Melee;
using AQMod.Items.Weapons.Ranged;
using AQMod.Localization;
using AQMod.NPCs.Bosses;
using AQMod.NPCs.Monsters.GaleStreams;
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
                en_US: "Jerry Crabson",
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
                ModContent.ItemType<Items.Tools.Fishing.Nimrod>(),
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
            "AQMod/Assets/BossChecklist/GaleStreams",
            TexturePaths.EventIcons + "galestreams",
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
                ModContent.ItemType<Items.Tools.Fishing.Nimrod>(),
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
            "AQMod/Assets/BossChecklist/GaleStreams",
            TexturePaths.EventIcons + "galestreams",
            null);
        }
    }
}