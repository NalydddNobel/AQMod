using Aequus.Biomes;
using Aequus.Biomes.DemonSiege;
using Aequus.Common;
using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Healing;
using Aequus.Items.Armor.Vanity;
using Aequus.Items.Consumables;
using Aequus.Items.Consumables.Foods;
using Aequus.Items.Consumables.LootBags;
using Aequus.Items.Consumables.Summons;
using Aequus.Items.Misc;
using Aequus.Items.Misc.Dyes;
using Aequus.Items.Misc.Energies;
using Aequus.Items.Pets;
using Aequus.Items.Placeable.Furniture.BossTrophies;
using Aequus.Items.Placeable.Furniture.Paintings;
using Aequus.Items.Tools.Misc;
using Aequus.Items.Weapons.Magic;
using Aequus.Items.Weapons.Melee;
using Aequus.Items.Weapons.Summon;
using Aequus.NPCs;
using Aequus.NPCs.Boss;
using Aequus.NPCs.Monsters.Night.Glimmer;
using Aequus.NPCs.Monsters.Sky.GaleStreams;
using Aequus.NPCs.Monsters.Underworld;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    public class ModSupport : IPostSetupContent, IAddRecipes
    {
        public static Mod CalamityMod => CalamityModSupport.CalamityMod;
        public static Mod Polarities => PolaritiesSupport.Polarities;
        public static Mod TrueTooltips { get; private set; }
        public static Mod Fargowiltas { get; private set; }
        public static Mod ColoredDamageTypes { get; private set; }
        public static Mod BossChecklist { get; private set; }

        void ILoadable.Load(Mod mod)
        {
        }

        void IPostSetupContent.PostSetupContent(Aequus aequus)
        {
            TrueTooltips = FindMod(nameof(TrueTooltips));

            Fargowiltas = FindMod(nameof(Fargowiltas));

            ColoredDamageTypes = FindMod(nameof(ColoredDamageTypes));

            BossChecklist = FindMod(nameof(BossChecklist));


            if (BossChecklist != null)
            {
                BossChecklistSupport();
            }

            if (ColoredDamageTypes != null)
            {
                ColoredDamageTypesSupport();
            }
        }
        private static Mod FindMod(string name)
        {
            return ModLoader.TryGetMod(name, out var value) ? value : null;
        }
        private static void BossChecklistSupport()
        {
            static void AddBossEntry(string type, string bossName, List<int> npcIDs, float progression, Func<bool> downed, Func<bool> available, List<int> drops, List<int> spawnItems)
            {
                BossChecklist.Call(
                    $"Add{type}",
                    Aequus.Instance,
                    $"$Mods.Aequus.NPCName.{bossName}",
                    npcIDs,
                    progression,
                    downed,
                    available,
                    drops,
                    spawnItems,
                    $"$Mods.Aequus.BossChecklist.{bossName}.1",
                    null,
                    new Action<SpriteBatch, Rectangle, Color>((spriteBatch, rect, color) =>
                    {
                        var tex = Aequus.Instance.Assets.Request<Texture2D>("Assets/UI/BossChecklist/" + bossName, AssetRequestMode.ImmediateLoad).Value;
                        var sourceRect = tex.Bounds;
                        float scale = Math.Min(1f, (float)rect.Width / sourceRect.Width);
                        spriteBatch.Draw(tex, rect.Center.ToVector2(), sourceRect, color, 0f, sourceRect.Size() / 2, scale, SpriteEffects.None, 0);
                    })
                );
            }

            AddBossEntry(
                "Boss",
                "OmegaStarite",
                new List<int>() { ModContent.NPCType<OmegaStarite>() },
                6.99f, // Right before WoF
                () => AequusWorld.downedOmegaStarite,
                null,
                new List<int>()
                {
                    ModContent.ItemType<OmegaStariteRelic>(),
                    ModContent.ItemType<DragonBall>(),
                    ModContent.ItemType<OmegaStariteBag>(),
                    ModContent.ItemType<OmegaStariteTrophy>(),
                    ModContent.ItemType<OriginPainting>(),
                    ModContent.ItemType<OmegaStariteMask>(),
                    ModContent.ItemType<SupernovaFruit>(),
                    ModContent.ItemType<UltimateSword>(),
                    ModContent.ItemType<CosmicEnergy>(),
                    ModContent.ItemType<AstralCookie>(),
                    ModContent.ItemType<ScrollDye>(),
                    ModContent.ItemType<OutlineDye>(),
                },
                new List<int>() { ModContent.ItemType<SupernovaFruit>(), });

            AddBossEntry(
                "Boss",
                "Crabson",
                new List<int>() { ModContent.NPCType<Crabson>() },
                2.66f, // After Blood Moon, before Glimmer
                () => AequusWorld.downedCrabson,
                null,
                new List<int>()
                {
                    ModContent.ItemType<CrabsonRelic>(),
                    ModContent.ItemType<CrabsonBag>(),
                    ModContent.ItemType<CrabsonTrophy>(),
                    ModContent.ItemType<CrabsonMask>(),
                    ModContent.ItemType<HypnoticPearl>(),
                    ModContent.ItemType<AquaticEnergy>(),
                    ModContent.ItemType<Mendshroom>(),
                },
                new List<int>() { ModContent.ItemType<HypnoticPearl>(), });

            AddBossEntry(
                "Boss",
                "DustDevil",
                new List<int>() { ModContent.NPCType<DustDevil>() },
                8.5f, // After Queen Slime, before Twins
                () => AequusWorld.downedDustDevil,
                null,
                new List<int>()
                {
                    ModContent.ItemType<DustDevilRelic>(),
                    ModContent.ItemType<DustDevilBag>(),
                    ModContent.ItemType<TornadoInABottle>(),
                    ModContent.ItemType<AtmosphericEnergy>(),
                },
                new List<int>() { ModContent.ItemType<TornadoInABottle>(), });

            const float GaleStreams = 8.1f;

            AddBossEntry(
                "Miniboss",
                "RedSprite",
                new List<int>() { ModContent.NPCType<RedSprite>() },
                GaleStreams + 0.01f, // Fought in Gale Streams
                () => AequusWorld.downedRedSprite,
                null,
                new List<int>()
                {
                    ModContent.ItemType<RedSpriteRelic>(),
                    ModContent.ItemType<LightningRod>(),
                    ModContent.ItemType<RedSpriteTrophy>(),
                    ModContent.ItemType<RedSpriteMask>(),
                    ModContent.ItemType<Moro>(),
                    ModContent.ItemType<Fluorescence>(),
                    ModContent.ItemType<AtmosphericEnergy>(),
                    ItemID.SoulofFlight,
                    ModContent.ItemType<ScorchingDye>(),
                },
                null);

            AddBossEntry(
                "Miniboss",
                "SpaceSquid",
                new List<int>() { ModContent.NPCType<SpaceSquid>() },
                GaleStreams + 0.015f, // Fought in Gale Streams
                () => AequusWorld.downedSpaceSquid,
                null,
                new List<int>()
                {
                    ModContent.ItemType<SpaceSquidRelic>(),
                    ModContent.ItemType<ToySpaceGun>(),
                    ModContent.ItemType<SpaceSquidTrophy>(),
                    ModContent.ItemType<SpaceSquidMask>(),
                    ModContent.ItemType<FrozenTear>(),
                    ModContent.ItemType<AtmosphericEnergy>(),
                    ItemID.SoulofFlight,
                    ModContent.ItemType<FrostbiteDye>(),
                },
                null);

            static void AddEventEntry(string eventName, List<int> npcIDs, float progression, Func<bool> downed, Func<bool> available, List<int> drops, List<int> spawnItems)
            {
                BossChecklist.Call(
                    $"AddEvent",
                    Aequus.Instance,
                    $"$Mods.Aequus.BiomeName.{eventName}Biome",
                    npcIDs,
                    progression,
                    downed,
                    available,
                    drops,
                    spawnItems,
                    $"$Mods.Aequus.BossChecklist.{eventName}.1",
                    new Action<SpriteBatch, Rectangle, Color>((spriteBatch, rect, color) =>
                    {
                        var tex = Aequus.Instance.Assets.Request<Texture2D>("Assets/UI/BossChecklist/" + eventName, AssetRequestMode.ImmediateLoad).Value;
                        var sourceRect = tex.Bounds;
                        float scale = Math.Min(1f, (float)rect.Width / sourceRect.Width);
                        spriteBatch.Draw(tex, rect.Center.ToVector2(), sourceRect, color, 0f, sourceRect.Size() / 2, scale, SpriteEffects.None, 0);
                    }),
                    $"Aequus/Assets/UI/BestiaryIcons/{eventName}"
                );
            }

            AddEventEntry(
                "Glimmer",
                new List<int>() { ModContent.NPCType<Starite>(), ModContent.NPCType<SuperStarite>(), ModContent.NPCType<HyperStarite>(), ModContent.NPCType<UltraStarite>(), },
                2.75f,
                () => AequusWorld.downedEventCosmic,
                null,
                new List<int>()
                {
                    ModContent.ItemType<SuperStarSword>(),
                    ModContent.ItemType<WowHat>(),
                    ModContent.ItemType<StariteStaff>(),
                    ModContent.ItemType<HyperCrystal>(),
                    ItemID.Nazar,
                    ModContent.ItemType<NeutronYogurt>(),
                    ModContent.ItemType<CelesitalEightBall>(),
                },
                new List<int>() { ModContent.ItemType<GalacticStarfruit>(), });

            AddEventEntry(
                "DemonSiege",
                new List<int>() { ModContent.NPCType<Cindera>(), ModContent.NPCType<Magmabubble>(), ModContent.NPCType<TrapperImp>(), },
                5.5f,
                () => AequusWorld.downedEventDemon,
                null,
                new List<int>()
                {
                    ItemID.LavaCharm,
                    ItemID.MagmaStone,
                    ItemID.ObsidianRose,
                    ModContent.ItemType<DemonicEnergy>(),
                    ModContent.ItemType<HellBeamDye>(),
                },
                null);

            AddEventEntry(
                "GaleStreams",
                new List<int>() { ModContent.NPCType<StreamingBalloon>(), ModContent.NPCType<Vraine>(), ModContent.NPCType<WhiteSlime>(), ModContent.NPCType<RedSprite>(), ModContent.NPCType<SpaceSquid>(), },
                GaleStreams,
                () => AequusWorld.downedEventAtmosphere,
                null,
                new List<int>()
                {
                    ModContent.ItemType<Umystick>(),
                    ModContent.ItemType<Vrang>(),
                    ModContent.ItemType<CinnamonRoll>(),
                    ModContent.ItemType<CensorDye>(),
                },
                new List<int>() { ModContent.ItemType<Pumpinator>(), });

            AequusText.NewFromDict("BossChecklist.Crabson", ".1", new { HypnoticPearl = AequusText.ItemText<HypnoticPearl>(), });
            AequusText.NewFromDict("BossChecklist.OmegaStarite", ".1", new { SupernovaFruit = AequusText.ItemText<SupernovaFruit>(), });
            AequusText.NewFromDict("BossChecklist.DustDevil", ".1", new { ThunderstormInABottle = AequusText.ItemText<TornadoInABottle>(), });

            AequusText.NewFromDict("BossChecklist.Glimmer", ".1", new { GalacticStarfruit = AequusText.ItemText<GalacticStarfruit>(), });
            string demonSiegeItemList = "";
            foreach (var d in DemonSiegeSystem.RegisteredSacrifices)
            {
                if (d.Value.Hide)
                    continue;

                if (demonSiegeItemList.Length != 0)
                    demonSiegeItemList += ", ";

                demonSiegeItemList += AequusText.ItemText(d.Key);
            }
            AequusText.NewFromDict("BossChecklist.DemonSiege", ".1", new { ItemList = demonSiegeItemList, });
            AequusText.NewFromDict("BossChecklist.GaleStreams", ".1", new { Pumpinator = AequusText.ItemText<Pumpinator>(), });
        }
        private static void ColoredDamageTypesSupport()
        {
            ColoredDamageTypes.Call("AddDamageType", NecromancyDamageClass.Instance,
                NecromancyDamageClass.NecromancyDamageColor, NecromancyDamageClass.NecromancyDamageColor, NecromancyDamageClass.NecromancyDamageColor * 1.25f);
        }

        void IAddRecipes.AddRecipes(Aequus aequus)
        {
            if (Fargowiltas != null)
            {
                ShopQuotes.Database.GetNPC(AequusHelpers.NPCType(Fargowiltas, "Squirrel")).WithColor(Color.Gray * 1.66f);
            }
        }

        void ILoadable.Unload()
        {
            TrueTooltips = null;
            Fargowiltas = null;
            ColoredDamageTypes = null;
        }
    }
}