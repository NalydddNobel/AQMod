using Aequus.Core.ContentGeneration;
using Aequus.Core.DataSets;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aequus.Content.Music;

public class MusicLinks : DataSet {
    [JsonProperty]
    public static Dictionary<string, string> Links { get; private set; } = new();

    public override void PostSetupContent() {
        foreach (var m in Mod.GetContent<InstancedMusicBoxItem>()) {
            if (Links.TryGetValue(m._musicBox._musicName, out string link)) {
                m.Link = link;
            }
        }
    }
}