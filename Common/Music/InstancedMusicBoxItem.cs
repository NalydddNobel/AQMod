using Aequus.Common.Items.Components;
using Aequus.Common.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Aequus.Common.Music;

internal sealed class InstancedMusicBoxItem : InstancedTileItem, IOnlineLink {
    public string Link { get; internal set; }
    public InstancedMusicBox _musicBox;

    internal InstancedMusicBoxItem(InstancedMusicBox musicBox) : base(musicBox, style: 0, nameSuffix: null, dropItem: true, rarity: ItemRarityID.LightRed, value: Item.sellPrice(gold: 2), researchSacrificeCount: null) {
        _musicBox = musicBox;
    }

    public override LocalizedText DisplayName => Language.GetOrRegister("Mods.Aequus.Music.MusicBoxName").WithFormatArgs(Language.GetOrRegister($"Mods.Aequus.Music.{_musicBox._musicName}"));
    public override LocalizedText Tooltip => LocalizedText.Empty;

    public override void SetStaticDefaults() {
        ItemID.Sets.CanGetPrefixes[Type] = false;
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
    }

    public override void SetDefaults() {
        Item.DefaultToMusicBox(_modTile.Type, 0);
    }
}