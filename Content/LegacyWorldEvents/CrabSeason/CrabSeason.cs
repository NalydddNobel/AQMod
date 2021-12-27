using AQMod.Common;
using AQMod.Common.CrossMod.BossChecklist;
using AQMod.Content.World.Events;
using AQMod.Items.Tools;
using AQMod.Localization;
using AQMod.NPCs.Monsters.CrabSeason;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.LegacyWorldEvents.CrabSeason
{
    public sealed class CrabSeason : WorldEvent
    {
        public static Color TextColor => new Color(18, 226, 213, 255);

        public static int crabSeasonTimer;
        public static int crabSeasonTimerRate = 1;
        public const int CrabSeasonTimerMax = 162000;
        public const int CrabSeasonTimerMin = 72000;

        public static bool Active => crabSeasonTimer < 0;
        public static short CrabsonCachedID { get; set; } = -1;

        internal override EventEntry? BossChecklistEntry => new EventEntry(
            () => WorldDefeats.DownedCrabSeason,
            0.5f,
            new List<int>() {
                ModContent.NPCType<ArrowCrab>(),
                ModContent.NPCType<SoliderCrabs>(),
                ModContent.NPCType<HermitCrab>(),
                ModContent.NPCType<StriderCrab>(),
            },
            AQText.chooselocalizationtext("Crab Season", "蟹季"),
            0,
            new List<int>()
            {
                ModContent.ItemType<Items.Materials.CrabShell>(),
                ModContent.ItemType<Items.Armor.HermitShell>(),
                ModContent.ItemType<Items.Armor.StriderCarapace>(),
                ModContent.ItemType<Items.Armor.StriderPalms>(),
                ModContent.ItemType<Items.Tools.GrapplingHooks.StriderHook>(),
            },
            new List<int>()
            {
                ModContent.ItemType<Items.Vanities.FishyFins>(),
            },
            AQText.chooselocalizationtext(
                en_US: "Begins naturally and ends naturally at random times. You can check the time when the event begins and ends using a [i:" + ModContent.ItemType<CrabClock>() + "].",
                zh_Hans: null),
            "AQMod/Assets/BossChecklist/CrabSeason",
            "");

        public static bool InActiveZone(Player player)
        {
            return Active && CrabsonCachedID == -1 && player.ZoneBeach;
        }

        public static void UpdateWorld()
        {
            if (crabSeasonTimer > 0)
            {
                if (crabSeasonTimer - crabSeasonTimerRate <= 0)
                {
                    Activate();
                    if (Main.LocalPlayer.HeldItem.modItem is CrabClock)
                        AQMod.BroadcastMessage(AQText.ModText("Common.CrabSeasonWarning").Value, TextColor);
                }
            }
            crabSeasonTimer -= crabSeasonTimerRate;
            if (crabSeasonTimer <= -CrabSeasonTimerMin)
            {
                Deactivate();
                if (Main.LocalPlayer.HeldItem.modItem is CrabClock)
                    AQMod.BroadcastMessage(AQText.ModText("Common.CrabSeasonEnding").Value, TextColor);
            }
            crabSeasonTimerRate = 1;
        }

        public static void Activate()
        {
            if (crabSeasonTimer > 0)
                crabSeasonTimer = 0;
        }

        public static void Deactivate()
        {
            if (crabSeasonTimer < 0)
                crabSeasonTimer = Main.rand.Next(CrabSeasonTimerMin, CrabSeasonTimerMax);
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(crabSeasonTimer);
        }

        public override void NetReceive(BinaryReader reader)
        {
            crabSeasonTimer = reader.ReadInt32();
        }
    }
}