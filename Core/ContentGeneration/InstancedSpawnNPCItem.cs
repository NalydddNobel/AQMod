using Terraria.Localization;

namespace AequusRemake.Core.ContentGeneration;

internal class InstancedSpawnNPCItem : InstancedModItem {
    protected readonly ModNPC _parent;
    protected readonly int _value;
    protected readonly int _rarity;
    protected readonly int _bait;
    protected readonly bool _lavaBait;

    public InstancedSpawnNPCItem(ModNPC npc, int value, int rarity, int bait = 0, bool lavaBait = false, string nameSuffix = "") : base(npc.Name + nameSuffix, npc.Texture + "Item") {
        _parent = npc;
        _value = value;
        _rarity = rarity;
        _bait = bait;
        _lavaBait = lavaBait;
    }

    public override string LocalizationCategory => "NPCs";

    public override LocalizedText DisplayName => _parent.DisplayName;
    public override LocalizedText Tooltip => _parent.GetLocalization("CritterItemTooltip");

    public override void SetStaticDefaults() {
        ItemSets.IsLavaBait[Type] = _lavaBait;
        Item.ResearchUnlockCount = 5;
    }

    public override void SetDefaults() {
        Item.DefaultToCapturedCritter(_parent.Type);
        Item.bait = _bait;
        Item.rare = _rarity;
        Item.value = _value;
    }
}
