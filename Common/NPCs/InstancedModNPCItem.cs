using Aequus.Core.ContentGeneration;
using Terraria.Localization;

namespace Aequus.Common.NPCs;

internal class InstancedModNPCItem : InstancedModItem {
    protected readonly ModNPC _modNPC;
    protected readonly int _value;
    protected readonly int _rarity;
    protected readonly int _bait;
    protected readonly bool _lavaBait;

    public InstancedModNPCItem(ModNPC npc, int value, int rarity, int bait, bool lavaBait) : base(npc.Name, npc.Texture + "Item") {
        _modNPC = npc;
        _value = value;
        _rarity = rarity;
        _bait = bait;
        _lavaBait = lavaBait;
    }

    public override string LocalizationCategory => "NPCs";

    public override LocalizedText DisplayName => _modNPC.DisplayName;
    public override LocalizedText Tooltip => _modNPC.GetLocalization("CritterItemTooltip");

    public override void SetStaticDefaults() {
        ItemID.Sets.IsLavaBait[Type] = _lavaBait;
        Item.ResearchUnlockCount = 5;
    }

    public override void SetDefaults() {
        Item.DefaultToCapturedCritter(_modNPC.Type);
        Item.bait = _bait;
        Item.rare = _rarity;
        Item.value = _value;
    }
}