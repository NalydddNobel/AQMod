using Aequus.Content.Music;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Terraria.ModLoader;

namespace Aequus.Common.Music;

public class MusicBoxLoader : ModSystem {
    public override void Load() {
        foreach (var musicPath in Mod.GetFileNames().Where((p) => p.EndsWith(".ogg") && p.Contains("Music/"))) {
            string safeMusicPath = Path.ChangeExtension(musicPath, "")[..^1];
            string musicName = safeMusicPath[(musicPath.LastIndexOf("/")+1)..];
            Mod.Logger.Debug($"Generating Music Box (Path: {safeMusicPath})");
            Mod.AddContent(new InstancedMusicBox(safeMusicPath, musicName));
            CollectionsMarshal.GetValueRefOrAddDefault(MusicLinks.Links, musicName, out _);
        }
    }
}