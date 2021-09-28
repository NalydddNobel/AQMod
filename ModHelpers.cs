using AQMod.Common;
using AQMod.Items.Accessories;
using AQMod.Items.Accessories.Shoes;
using AQMod.Items.BossItems.Crabson;
using AQMod.Items.BossItems.Starite;
using AQMod.Items.Energies;
using AQMod.Items.Tools.Markers;
using AQMod.Items.Vanities;
using AQMod.Items.Vanities.Dyes;
using AQMod.Items.Weapons.Magic;
using AQMod.Items.Weapons.Melee;
using AQMod.Items.Weapons.Melee.Flails;
using AQMod.Items.Weapons.Melee.Yoyos;
using AQMod.Items.Weapons.Ranged.Bows;
using AQMod.Items.Weapons.Ranged.Bullet;
using AQMod.Items.Weapons.Summon;
using AQMod.NPCs.Crabson;
using AQMod.NPCs.Glimmer;
using AQMod.NPCs.Starite;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod
{
    internal static class ModHelpers
    {
        public class BossChecklist
        {
            private Mod _bossChecklist;
            private Mod _aQMod;

            public static void SetupBossChecklistEntries(AQMod aQMod)
            {
                try
                {
                    var bossChecklist = ModLoader.GetMod("BossChecklist");
                    if (bossChecklist == null)
                        return;

                    var bc = new BossChecklist();
                    bc._aQMod = aQMod;
                    bc._bossChecklist = bossChecklist;

                    bc.GlimmerEvent();
                    bc.OmegaStarite();
                    bc.JerryCrabson();
                }
                catch (Exception e)
                {
                    aQMod.Logger.Warn("Error occured when setting up boss checklist entries");
                    aQMod.Logger.Warn(e.Message);
                    aQMod.Logger.Warn(e.StackTrace);
                }
            }

            private void GlimmerEvent()
            {
                AddEvent(
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
                "AQMod/Assets/Textures/Extras/Extra_35");
                ;
            }

            private void OmegaStarite()
            {
                AddBoss(6.1f,
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
                    ItemID.FallenStar
                },
                "Summoned by using an [i:" + ModContent.ItemType<NovaFruit>() + "]or [i:" + ModContent.ItemType<UltimateStarfruit>() + "] at night. Can also be summoned by interacting with the sword located at the center of the Glimmer Event",
                "AQMod/Assets/Textures/Extras/Extra_1");
            }

            private void JerryCrabson()
            {
                AddBoss(2f,
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
                //"Summoned by using an [i:" + ModContent.ItemType<MushroomClam>() + "] at the beach. Summons naturally at the end of the Crab Season Event",
                "Summoned by using a [i:" + ModContent.ItemType<MushroomClam>() + "] at the beach.",
                "AQMod/Assets/Textures/Extras/Extra_38");
            }

            private void AddToCollectibles(string name, List<int> values, string mod = "Terraria")
            {
                _bossChecklist.Call(
                    "AddToBossCollection",
                    mod,
                    name,
                    values);
            }

            private void AddToLoot(string name, List<int> values, string mod = "Terraria")
            {
                _bossChecklist.Call(
                    "AddToBossLoot",
                    mod,
                    name,
                    values);
            }

            private void AddBoss(float progression, int boss, string bossName, Func<bool> isDowned, int summonItem, List<int> collectibles, List<int> loot, string summonDescription = "", string portraitTexture = null, string bossHeadTexture = null, string despawnMessage = null, Func<bool> available = null)
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

            private void AddEvent(float priority, List<int> npcs, string name, Func<bool> isDowned, int summon, List<int> collectibles, List<int> loot, string description, string eventEndingMessage, string texture)
            {
                _bossChecklist.Call("AddEvent",
                       priority,
                       npcs,
                       _aQMod,
                       name,
                       isDowned,
                       summon,
                       collectibles,
                       loot,
                       description,
                       eventEndingMessage,
                       texture);
            }
        }

        public class Fargowiltas
        {
            private Mod _fargowiltas;
            private Mod _aQMod;

            public static bool Active { get; private set; }

            public static void SetupBossSummons(AQMod aQMod)
            {
                try
                {
                    var fargos = ModLoader.GetMod("Fargowiltas");
                    if (fargos == null)
                        return;
                    Active = true;
                    var f = new Fargowiltas();
                    f._aQMod = aQMod;
                    f._fargowiltas = fargos;
                    f.AddSummon(1.5f, "MushroomClam", () => WorldDefeats.DownedCrabson, Item.buyPrice(gold: 8));
                }
                catch (Exception e)
                {
                    aQMod.Logger.Warn("Error occured when setting up fargo boss summons");
                    aQMod.Logger.Warn(e.Message);
                    aQMod.Logger.Warn(e.StackTrace);
                }
            }

            private void AddSummon(float sort, string itemName, Func<bool> checkFlag, int price)
            {
                _fargowiltas.Call("AddSummon", sort, "AQMod", itemName, checkFlag, price);
            }
        }
    }
}