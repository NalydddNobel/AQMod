using Aequus.Biomes.DemonSiege;
using Aequus.Common;
using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Healing;
using Aequus.Items.Armor.Vanity;
using Aequus.Items.Boss.Bags;
using Aequus.Items.Boss.Summons;
using Aequus.Items.Consumables.Foods;
using Aequus.Items.Misc;
using Aequus.Items.Misc.Dyes.Ancient;
using Aequus.Items.Misc.Energies;
using Aequus.Items.Pets;
using Aequus.Items.Pets.Light;
using Aequus.Items.Placeable.Furniture.BossTrophies;
using Aequus.Items.Placeable.Furniture.Paintings;
using Aequus.Items.Tools;
using Aequus.Items.Weapons.Magic;
using Aequus.Items.Weapons.Melee;
using Aequus.Items.Weapons.Summon.Minion;
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
    internal class BossChecklistSupport : IPostSetupContent
    {
        public static Mod BossChecklist { get; private set; }

        void ILoadable.Load(Mod mod)
        {
        }

        void IPostSetupContent.PostSetupContent(Aequus aequus)
        {
            BossChecklist = null;
            if (ModLoader.TryGetMod("BossChecklist", out var bossChecklist))
            {
                BossChecklist = bossChecklist;
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
                        ModContent.ItemType<Fluorescence>(),
                        ModContent.ItemType<AtmosphericEnergy>(),
                        ItemID.SoulofFlight,
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
                        ModContent.ItemType<Nightfall>(),
                        ModContent.ItemType<StariteStaff>(),
                        ModContent.ItemType<HyperCrystal>(),
                        ItemID.Nazar,
                        ModContent.ItemType<NeutronYogurt>(),
                        ModContent.ItemType<AstralCookie>(),
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
                        ModContent.ItemType<AncientHellBeamDye>(),
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
                    },
                    new List<int>() { ModContent.ItemType<Pumpinator>(), });

                AequusText.NewFromDict("BossChecklist.Crabson", ".1", new { HypnoticPearl = AequusText.ItemCommand<HypnoticPearl>(), });
                AequusText.NewFromDict("BossChecklist.OmegaStarite", ".1", new { SupernovaFruit = AequusText.ItemCommand<SupernovaFruit>(), });
                AequusText.NewFromDict("BossChecklist.DustDevil", ".1", new { ThunderstormInABottle = AequusText.ItemCommand<TornadoInABottle>(), });

                AequusText.NewFromDict("BossChecklist.Glimmer", ".1", new { GalacticStarfruit = AequusText.ItemCommand<GalacticStarfruit>(), });
                string demonSiegeItemList = "";
                foreach (var d in DemonSiegeSystem.RegisteredSacrifices)
                {
                    if (d.Value.Hide)
                        continue;

                    if (demonSiegeItemList.Length != 0)
                        demonSiegeItemList += ", ";

                    demonSiegeItemList += AequusText.ItemCommand(d.Key);
                }
                AequusText.NewFromDict("BossChecklist.DemonSiege", ".1", new { ItemList = demonSiegeItemList, });
                AequusText.NewFromDict("BossChecklist.GaleStreams", ".1", new { Pumpinator = AequusText.ItemCommand<Pumpinator>(), });
            }
        }

        void ILoadable.Unload()
        {
            BossChecklist = null;
        }
    }
}