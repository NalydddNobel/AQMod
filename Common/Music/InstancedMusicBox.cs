using Aequus.Common.Tiles;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Common.Music;

internal class InstancedMusicBox : InstancedTile {
    protected ModItem MusicBoxItem { get; private set; }

    internal string _musicPath;
    internal string _musicName;

    public InstancedMusicBox(string musicPath, string musicName) : base($"{musicName}MusicBox", $"Aequus/Content/Music/{musicName}MusicBox") {
        _musicPath = musicPath;
        _musicName = musicName;
    }

    public override void Load() {
        MusicBoxItem = new InstancedMusicBoxItem(this);
        Mod.AddContent(MusicBoxItem);
    }

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.Origin = new Point16(0, 1);
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.addTile(Type);

        AddMapEntry(ColorHelper.ColorFurniture, Language.GetText("MapObject.MusicBox"));

        MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, _musicPath), MusicBoxItem.Type, Type);
    }

    public override void MouseOver(int i, int j) {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = MusicBoxItem.Type;
    }
}