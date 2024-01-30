using Aequus.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Renaming;

public sealed class RenamingSystem : ModSystem {
    #region Text
    public const Char CommandChar = '{';
    public const Char CommandEndChar = '}';

    #region Commands 
    public static Dictionary<Char, Action<String, String, List<DecodedText>>> Commands { get; private set; } = new() {
        ['$'] = LanguageKeyCommand,
    };

    private static void LanguageKeyCommand(String fullCommand, String commandInput, List<DecodedText> output) {
        String languageText = Language.GetTextValue(commandInput, Lang.CreateDialogSubstitutionObject());

        output.Add(new DecodedText(fullCommand, languageText, commandInput == languageText ? DecodeType.FailedCommand : DecodeType.LanguageKey));
    }
    #endregion

    public static Dictionary<DecodeType, Color> DecodeColors { get; private set; } = new() {
        [DecodeType.None] = Color.White,
        [DecodeType.FailedCommand] = new Color(255, 120, 120),
        [DecodeType.LanguageKey] = new Color(255, 255, 80),
    };

    private static readonly List<DecodedText> _decodedTextList = new();

    public static String GetPlainDecodedText(String input) {
        _decodedTextList.Clear();
        DecodeText(input, _decodedTextList);

        String result = "";
        foreach (var i in _decodedTextList) {
            result += i.Output;
        }
        return result;
    }

    public static String GetColoredDecodedText(String input, Boolean pulse = false) {
        _decodedTextList.Clear();
        DecodeText(input, _decodedTextList);

        String result = "";
        foreach (var i in _decodedTextList) {
            result += i.Type != DecodeType.None ? ChatCommandInserts.ColorCommand(i.Output, DecodeColors[i.Type], pulse) : i.Output;
        }

        return result;
    }

    public static void DecodeText(String input, List<DecodedText> output) {
        String text = "";
        for (Int32 i = 0; i < input.Length; i++) {
            if (input[i] == '\\' && i < input.Length - 2 && input[i + 1] == 'n') {
                FlushOldOutput();
                text = "";
                output.Add(new DecodedText("\\n", "\n", DecodeType.LanguageKey));
                continue;
            }
            if (input[i] == CommandChar) {
                FlushOldOutput();
                text = "";

                String subString = input[i..];
                Int32 endIndex = subString.IndexOf(CommandEndChar);
                if (endIndex == -1) {
                    output.Add(new DecodedText(subString, subString, DecodeType.FailedCommand));
                    break;
                }

                i += endIndex + 1;
                subString = subString[..(endIndex + 1)];
                if (subString.Length < 3) {
                    output.Add(new DecodedText(subString, subString, DecodeType.FailedCommand));
                    continue;
                }

                if (Commands.TryGetValue(subString[1], out var command)) {
                    command(subString, subString[2..^1], output);
                }
                else {
                    output.Add(new DecodedText(subString, subString, DecodeType.FailedCommand));
                    continue;
                }
                continue;
            }

            text += input[i];
        }
        FlushOldOutput();

        void FlushOldOutput() {
            if (!String.IsNullOrEmpty(text)) {
                output.Add(new DecodedText(text, text, DecodeType.None));
            }
        }
    }
    #endregion

    public static readonly Dictionary<Int32, RenamedNPCMarker> RenamedNPCs = new();
    public static readonly Queue<Int32> RemoveQueue = new();
    internal static readonly List<Rectangle> SpawnRectangles = new();
    private static readonly Vector2 PlayerBoxSize = new Vector2(1968f, 1200f);

    public static Int32 UpdateRate = 60;
    private static Int32 _gameTime;

    public override void Load() {
        IOHooks.PreSaveWorld += EnsureTagCompoundContents;
    }

    public override void ClearWorld() {
        SyncList.Clear();
        RenamedNPCs.Clear();
    }

    public override void SaveWorldData(TagCompound tag) {
        lock (RenamedNPCs) {
            if (RenamedNPCs.Count > 0) {
                tag["NPCs"] = RenamedNPCs.Values.Select(n => n.Save()).ToList();
            }
        }
    }

    public override void LoadWorldData(TagCompound tag) {
        lock (RenamedNPCs) {
            if (tag.TryGet<List<TagCompound>>("NPCs", out var list)) {
                foreach (var t in list) {
                    RenamedNPCs[Guid.NewGuid().GetHashCode()] = RenamedNPCMarker.Load(t);
                }
            }
        }
    }

    public override void PreUpdateEntities() {
        SyncMarkers();

        _gameTime++;
        if (_gameTime < UpdateRate) {
            return;
        }

        _gameTime = 0;
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            return;
        }

        lock (RenamedNPCs) {
            SpawnRectangles.Clear();
            for (Int32 i = 0; i < Main.maxPlayers; i++) {
                if (Main.player[i].active && !Main.player[i].dead) {
                    SpawnRectangles.Add(Utils.CenteredRectangle(Main.player[i].Center, PlayerBoxSize));
                }
            }

            foreach (var queuedRemoval in RemoveQueue) {
                RenamedNPCs.Remove(queuedRemoval);
            }

            foreach (var npc in RenamedNPCs) {
                var marker = npc.Value;
                if (marker.IsTrackingNPC) {
                    if (marker.IsTrackedNPCValid) {
                        Main.npc[marker.TrackNPC].EncourageDespawn(600);
                        continue;
                    }
                    else {
                        marker.TrackNPC = -1;
                    }
                }

                for (Int32 j = 0; j < SpawnRectangles.Count; j++) {
                    if (SpawnRectangles[j].Intersects(marker.SpawnBox)) {
                        _gameTime = UpdateRate;

                        Int32 newNPC = NPC.NewNPC(new EntitySource_Misc("Aequus: Name Tag Respawn"), marker.tileX * 16 + 8, marker.tileY * 16 + 8, marker.type);
                        if (newNPC == Main.maxNPCs) {
                            break;
                        }

                        Main.npc[newNPC].whoAmI = newNPC;
                        marker.SetupNPC(Main.npc[newNPC]);

                        if (Main.npc[newNPC].TryGetGlobalNPC<RenamedNPCMarkerManager>(out var markerHandler)) {
                            markerHandler.MarkerId = npc.Key;
                        }

                        if (Main.netMode == NetmodeID.Server) {
                            NetMessage.SendData(MessageID.SyncNPC, number: newNPC);
                        }
                        return;
                    }
                }
            }
        }
    }

    public static void EnsureTagCompoundContents(Boolean toCloud) {
        foreach (var marker in RenamedNPCs.Values) {
            if (marker.IsTrackingNPC && marker.IsTrackedNPCValid) {
                marker.UpdateTagCompound(Main.npc[marker.TrackNPC]);
            }
        }
    }

    public static void Remove(Int32 id, Boolean quiet = false) {
        if (!RenamedNPCs.ContainsKey(id)) {
            return;
        }

        if (!quiet) {
            if (Main.netMode != NetmodeID.SinglePlayer) {
                ModContent.GetInstance<PacketRemoveMarker>().Send(id);
            }

            if (Main.netMode == NetmodeID.MultiplayerClient) {
                return;
            }
        }

        RenamedNPCs.Remove(id);
    }

    #region Syncing 
    private static readonly List<(Int32, RenamedNPCMarker)> SyncList = new();
    private static Int32 markerListIndex;
    private static Boolean syncMarkers;

    private void SyncMarkers() {
        if (Main.netMode != NetmodeID.Server) {
            return;
        }
        if (markerListIndex < 0) {
            if (!syncMarkers) {
                return;
            }

            markerListIndex = 0;
            syncMarkers = false;
        }

        lock (RenamedNPCs) {
            if (markerListIndex == 0) {
                SyncList.Clear();
                SyncList.AddRange(RenamedNPCs.Select(p => (p.Key, p.Value)));
            }
        }

        var packetHandler = ModContent.GetInstance<PacketAddMarker>();
        Int32 endIndex = Math.Min(markerListIndex + 10, SyncList.Count);
        for (Int32 i = markerListIndex; i < endIndex; i++) {
            packetHandler.Send(SyncList[i].Item1, SyncList[i].Item2);
        }

        if (markerListIndex >= SyncList.Count) {
            markerListIndex = -1;
        }
    }

    public override void NetSend(BinaryWriter writer) {
        syncMarkers = true;
    }

    public override void NetReceive(BinaryReader reader) {
    }
    #endregion
}