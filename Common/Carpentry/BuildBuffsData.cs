using Aequus.Common.Carpentry.Results;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Carpentry;

public readonly struct BuildBuffsData {
    public readonly string SaveKey;
    public readonly Dictionary<int, List<Rectangle>> WorldData;

    public BuildBuffsData(string saveKey) {
        SaveKey = saveKey;
        WorldData = new();
    }

    public void Clear() {
        WorldData?.Clear();
    }

    public void SaveData(TagCompound tag) {
        try {
            for (int i = 0; i < BuildChallengeLoader.BuildChallengeCount; i++) {
                TrimBuildingBuffsInWorld(i);
            }
        }
        catch {
        }
        Dictionary<string, List<Rectangle>> dictionary = new();
        foreach (var item in WorldData) {
            if (item.Value == null || item.Value.Count == 0) {
                continue;
            }
            dictionary[BuildChallengeLoader.registeredBuildChallenges[item.Key].FullName] = item.Value;
        }
        tag[SaveKey] = dictionary;
    }

    public void LoadData(TagCompound tag) {
        if (!tag.TryGet<Dictionary<string, List<Rectangle>>>(SaveKey, out var dictionary)) {
            return;
        }

        foreach (var item in dictionary) {
            if (!ModContent.TryFind(item.Key, out BuildChallenge buildChallenge)) {
                continue;
            }
            WorldData[buildChallenge.Type] = item.Value;
        }
    }

    public bool AnyBuild(int id) {
        return WorldData.ContainsKey(id);
    }

    public bool AddBuild(int buildChallengeId, Rectangle rectangle) {
        if (!AnyBuild(buildChallengeId)) {
            WorldData[buildChallengeId] = new() { rectangle };
            return true;
        }

        var l = WorldData[buildChallengeId];
        for (int i = 0; i < l.Count; i++) {
            if (l[i].Intersects(rectangle)) {
                return false;
            }
        }
        WorldData[buildChallengeId].Add(rectangle);
        return true;
    }

    public bool RemoveBuild(int buildChallengeId, int x, int y) {
        if (!AnyBuild(buildChallengeId)) {
            return false;
        }
        var l = WorldData[buildChallengeId];
        for (int i = 0; i < l.Count; i++) {
            if (l[i].Contains(x, y)) {
                l.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public void TrimBuildingBuffsInWorld(int id) {
        if (!WorldData.TryGetValue(id, out var list) || list == null || list.Count == 0) {
            return;
        }
        var buildChallenge = BuildChallengeLoader.registeredBuildChallenges[id];
        var result = new IStepResults[buildChallenge.Steps.Length];
        for (int i = 0; i < list.Count; i++) {
            try {
                ScanInfo scanInfo = new(list[i]);
                HighlightInfo highlightInfo = new(scanInfo.Area);
                buildChallenge.Scan(result, ref highlightInfo, in scanInfo);
                for (int j = 0; j < result.Length; i++) {
                    if (result[j].ResultType == StepResultType.Fail) {
                        list.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }
            catch (Exception ex) {
                Aequus.Instance.Logger.Error($"Error from {buildChallenge.FullName} when evaluating.\n{ex}");
            }
        }
    }

    public void NetSend(BinaryWriter writer) {
        int total = 0;
        foreach (var b in WorldData) {
            if (b.Value != null && b.Value.Count > 0) {
                total++;
            }
        }
        writer.Write(total);
        foreach (var pair in WorldData) {
            var b = pair.Value;
            if (b.Count < 0) {
                continue;
            }
            var l = b;
            writer.Write(pair.Key);
            writer.Write(l.Count);
            for (int j = 0; j < l.Count; j++) {
                writer.Write(l[j].X);
                writer.Write(l[j].Y);
                writer.Write(l[j].Width);
                writer.Write(l[j].Height);
            }
        }
    }

    public void NetReceive(BinaryReader reader) {
        int buildingBuffLocationsCount = reader.ReadInt32();
        Clear();
        for (int i = 0; i < buildingBuffLocationsCount; i++) {
            int type = reader.ReadInt32();
            int listCount = reader.ReadInt32();
            for (int j = 0; j < listCount; j++) {
                var r = new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                AddBuild(type, r);
            }
        }
    }
}
