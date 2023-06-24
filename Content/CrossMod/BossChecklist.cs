using Aequus.Content.Events.DemonSiege;
using Aequus.Content.Events.GaleStreams;
using Aequus.Content.Events.GlimmerEvent.Misc;
using Aequus.Items.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Aequus.NPCs.Monsters.Event.DemonSiege;
using Aequus.NPCs.Monsters.Event.Glimmer;
using Aequus.NPCs.Monsters.Event.GaleStreams;
using Aequus.NPCs.RedSprite;
using Aequus.NPCs.SpaceSquid;
using Aequus.NPCs.Monsters.BossMonsters.Crabson;
using Aequus.NPCs.Monsters.BossMonsters.DustDevil;
using Aequus.NPCs.Monsters.BossMonsters.OmegaStarite;
using Aequus.Items.Misc.Spawners;
using Aequus.NPCs.Monsters.Event.Glimmer.UltraStarite;

namespace Aequus.Content.CrossMod {
    internal class BossChecklist : ModSupport<BossChecklist>
    {
        public const float GaleStreams_Progression = 8.1f;

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
                    $"$Mods.Aequus.BossChecklist.{bossName}",
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

                string biomeManager = "BiomeManager";
                if (!Mod.TryFind<ModBiome>(eventName + biomeManager, out var _)) {
                    biomeManager = "Biome";
                }
                if (!Mod.TryFind<ModBiome>(eventName + biomeManager, out var _)) {
                    return;
                }

                Instance.Call(
                    $"AddEvent",
                    Aequus.Instance,
                    $"$Mods.Aequus.BiomeName.{eventName}{biomeManager}",
                    npcIDs,
                    progression,
                    downed,
                    available,
                    drops,
                    spawnItems,
                    $"$Mods.Aequus.BossChecklist.{eventName}",
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
                GaleStreams_Progression + 0.01f, // Fought in Gale Streams
                () => AequusWorld.downedRedSprite,
                null,
                null,
                null);

            AddBossEntry(
                BossEntryType.MiniBoss,
                "SpaceSquid",
                new List<int>() { ModContent.NPCType<SpaceSquid>() },
                GaleStreams_Progression + 0.011f, // Fought in Gale Streams
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
                4.6f,
                () => AequusWorld.downedEventCosmic,
                null,
                null,
                new List<int>() { ModContent.ItemType<GalacticStarfruit>(), });

            AddEventEntry(
                "DemonSiege",
                new List<int>() { ModContent.NPCType<Cindera>(), ModContent.NPCType<Magmabubble>(), ModContent.NPCType<TrapperImp>(), },
                6.1f,
                () => AequusWorld.downedEventDemon,
                null,
                null,
                new List<int>() { ModContent.ItemType<UnholyCoreSmall>(), });

            AddEventEntry(
                "GaleStreams",
                new List<int>() { ModContent.NPCType<StreamingBalloon>(), ModContent.NPCType<Vraine>(), ModContent.NPCType<WhiteSlime>(), ModContent.NPCType<RedSprite>(), ModContent.NPCType<SpaceSquid>(), },
                GaleStreams_Progression,
                () => AequusWorld.downedEventAtmosphere,
                null,
                null,
                new List<int>() { ModContent.ItemType<Pumpinator>(), });

        }
        private void FixText() {
            TextHelper.ModifyText("BossChecklist.Crabson", TextHelper.Modifications.UpdateItemCommands);
            TextHelper.ModifyText("BossChecklist.OmegaStarite", TextHelper.Modifications.UpdateItemCommands);
            TextHelper.ModifyText("BossChecklist.DustDevil", TextHelper.Modifications.UpdateItemCommands);
            TextHelper.ModifyText("BossChecklist.Glimmer", TextHelper.Modifications.UpdateItemCommands);
            TextHelper.ModifyText("BossChecklist.GaleStreams", TextHelper.Modifications.UpdateItemCommands);

            string demonSiegeItemList = "";
            foreach (var d in DemonSiegeSystem.RegisteredSacrifices)
            {
                if (d.Value.Hide)
                    continue;

                if (demonSiegeItemList.Length != 0)
                    demonSiegeItemList += ", ";

                demonSiegeItemList += TextHelper.ItemCommand(d.Key);
            }
            TextHelper.ModifyText("BossChecklist.DemonSiege", t => t.SetValue(t.Value.FormatWith(new { ItemList = demonSiegeItemList, })));
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