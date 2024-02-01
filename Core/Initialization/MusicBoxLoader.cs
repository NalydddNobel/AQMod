using Aequus.Content.Music;
using Aequus.Core.ContentGeneration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Aequus.Core.Initialization;

public class MusicBoxLoader : ModSystem {
    public override void Load() {
        foreach (var musicPath in Mod.GetFileNames().Where((p) => p.EndsWith(".ogg") && p.Contains("Music/"))) {
            string safeMusicPath = Path.ChangeExtension(musicPath, "")[..^1];
            string musicName = safeMusicPath[(musicPath.LastIndexOf("/") + 1)..];
#if DEBUG
            Mod.Logger.Debug($"Generating Music Box (Path: {safeMusicPath})");
#endif
            Mod.AddContent(new InstancedMusicBox(safeMusicPath, musicName));
            CollectionsMarshal.GetValueRefOrAddDefault(MusicLinks.Links, musicName, out _);
        }
    }
}