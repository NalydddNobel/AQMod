using AequusRemake.Core.ContentGeneration;
using AequusRemake.Core.Util.Helpers;
using Terraria.Localization;

namespace AequusRemake.Content.CrossMod.SplitSupport.Photography;

internal class InstancedPosterItem : InstancedTileItem {
    private AlbumQuestInfo _quest;

    public InstancedPosterItem(AlbumQuestInfo Quest)
        : base(ModContent.GetInstance<Poster>(), Quest.Frame, IdDictHelper.GetNPCName(Quest.NPC.GetId()), rarity: ItemRarityID.LightRed, value: Item.sellPrice(gold: 1)) {
        _quest = Quest;
    }

    public override LocalizedText DisplayName => Split.GetLocalization("Items.Prints").WithFormatArgs(NPCLoader.GetNPC(_quest.NPC.GetId()).DisplayName);
    public override LocalizedText Tooltip => LocalizedText.Empty;
}
