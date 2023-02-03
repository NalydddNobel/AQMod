using Aequus.Biomes;
using Aequus.Biomes.DemonSiege;
using Aequus.Items.Boss.Summons;
using Aequus.Items.Tools;
using Aequus.NPCs.Boss.Crabson;
using Aequus.NPCs.Boss.DustDevil;
using Aequus.NPCs.Boss.OmegaStarite;
using Aequus.NPCs.Monsters.Night.Glimmer;
using Aequus.NPCs.Monsters.Sky.GaleStreams;
using Aequus.NPCs.Monsters.Underworld;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    internal class BossChecklist : ModSupport<BossChecklist>
    {
        internal enum BossEntryType
        {
            Boss,
            MiniBoss,
        }
        private void AddBossEntry(BossEntryType type, string bossName, List<int> npcIDs, float progression, Func<bool> downed, Func<bool> available, List<int> extraDrops, List<int> spawnItems)
        {
            try
            {
                Instance.Call(
                    $"Add{type}",
                    Mod,
                    $"$Mods.Aequus.NPCName.{bossName}",
                    npcIDs,
                    progression,
                    downed,
                    available,
                    extraDrops,
                    spawnItems,
                    $"$Mods.Aequus.BossChecklist.{bossName}.1",
                    null,
                    new Action<SpriteBatch, Rectangle, Color>((spriteBatch, rect, color) =>
                    {
                        var tex = Mod.Assets.Request<Texture2D>("Assets/UI/BossChecklist/" + bossName, AssetRequestMode.ImmediateLoad).Value;
                        var sourceRect = tex.Bounds;
                        float scale = Math.Min(1f, (float)rect.Width / sourceRect.Width);
                        spriteBatch.Draw(tex, rect.Center.ToVector2(), sourceRect, color, 0f, sourceRect.Size() / 2, scale, SpriteEffects.None, 0);
                    })
                );
            }
            catch (Exception ex)
            {
                Mod.Logger.Error($"{ex.Message}\n{ex.StackTrace}");
            }
        }

        private void AddEventEntry(string eventName, List<int> npcIDs, float progression, Func<bool> downed, Func<bool> available, List<int> drops, List<int> spawnItems)
        {
            try
            {
                Instance.Call(
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
            catch (Exception ex)
            {
                Mod.Logger.Error($"{ex.Message}\n{ex.StackTrace}");
            }
        }

        private void AddBossEntries()
        {
            AddBossEntry(
                BossEntryType.Boss,
                    "Crabson",
                    new List<int>() { ModContent.NPCType<Crabson>() },
                    Crabson.BossProgression, // After Blood Moon, before Glimmer
                    () => AequusWorld.downedCrabson,
                    null,
                    null,
                    new List<int>() { ModContent.ItemType<HypnoticPearl>(), });

            AddBossEntry(
                BossEntryType.Boss,
                "OmegaStarite",
                new List<int>() { ModContent.NPCType<OmegaStarite>() },
                OmegaStarite.BossProgression, // Right before WoF
                () => AequusWorld.downedOmegaStarite,
                null,
                null,
                new List<int>() { ModContent.ItemType<SupernovaFruit>(), });

            AddBossEntry(
                BossEntryType.Boss,
                "DustDevil",
                new List<int>() { ModContent.NPCType<DustDevil>() },
                DustDevil.BossProgression, // After Queen Slime, before Twins
                () => AequusWorld.downedDustDevil,
                null,
                null,
                new List<int>() { ModContent.ItemType<TornadoInABottle>(), });
        }
        private void AddMiniBossEntries()
        {
            AddBossEntry(
                BossEntryType.MiniBoss,
                "UltraStarite",
                new List<int>() { ModContent.NPCType<UltraStarite>() },
                UltraStarite.BossProgression, // Fought in late Pre Hardmode
                () => AequusWorld.downedUltraStarite,
                null,
                null,
                null);

            AddBossEntry(
                BossEntryType.MiniBoss,
                "RedSprite",
                new List<int>() { ModContent.NPCType<RedSprite>() },
                GaleStreamsBiome.EventProgression + 0.01f, // Fought in Gale Streams
                () => AequusWorld.downedRedSprite,
                null,
                null,
                null);

            AddBossEntry(
                BossEntryType.MiniBoss,
                "SpaceSquid",
                new List<int>() { ModContent.NPCType<SpaceSquid>() },
                GaleStreamsBiome.EventProgression + 0.011f, // Fought in Gale Streams
                () => AequusWorld.downedSpaceSquid,
                null,
                null,
                null);
        }
        private void AddEventEntries()
        {
            AddEventEntry(
                "Glimmer",
                new List<int>() { ModContent.NPCType<Starite>(), ModContent.NPCType<SuperStarite>(), ModContent.NPCType<HyperStarite>(), ModContent.NPCType<UltraStarite>(), },
                2.75f,
                () => AequusWorld.downedEventCosmic,
                null,
                null,
                new List<int>() { ModContent.ItemType<GalacticStarfruit>(), });

            AddEventEntry(
                "DemonSiege",
                new List<int>() { ModContent.NPCType<Cindera>(), ModContent.NPCType<Magmabubble>(), ModContent.NPCType<TrapperImp>(), },
                5.5f,
                () => AequusWorld.downedEventDemon,
                null,
                null,
                new List<int>() { ModContent.ItemType<UnholyCoreSmall>(), });

            AddEventEntry(
                "GaleStreams",
                new List<int>() { ModContent.NPCType<StreamingBalloon>(), ModContent.NPCType<Vraine>(), ModContent.NPCType<WhiteSlime>(), ModContent.NPCType<RedSprite>(), ModContent.NPCType<SpaceSquid>(), },
                GaleStreamsBiome.EventProgression,
                () => AequusWorld.downedEventAtmosphere,
                null,
                null,
                new List<int>() { ModContent.ItemType<Pumpinator>(), });

        }
        private void FixText()
        {
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

        public override void PostSetupContent()
        {
            if (Instance == null)
                return;

            AddBossEntries();
            AddMiniBossEntries();
            AddEventEntries();
            FixText();
        }
    }
}