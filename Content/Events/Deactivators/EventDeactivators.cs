using Aequus.Common.Items.Components;
using Aequus.Core.ContentGeneration;
using Terraria.Audio;
using Terraria.Localization;

namespace Aequus.Content.Events.Deactivators;
public class EventDeactivators : ModSystem {
    public static ModItem BloodMoon { get; private set; }
    public static ModItem Glimmer { get; private set; }
    public static ModItem Eclipse { get; private set; }

    public override bool IsLoadingEnabled(Mod mod) {
        return Aequus.DEBUG_MODE;
    }

    public override void Load() {
        BloodMoon = new EventDeactivatorItem("BloodMoonDeactivator", AequusTextures.BloodMoonDeactivator.Path, ItemRarityID.Green, Item.buyPrice(gold: 15));
        Glimmer = new EventDeactivatorItem("GlimmerDeactivator", AequusTextures.GlimmerDeactivator.Path, ItemRarityID.Green, Item.buyPrice(gold: 15));
        Eclipse = new EventDeactivatorItem("EclipseDeactivator", AequusTextures.EclipseDeactivator.Path, ItemRarityID.Yellow, Item.buyPrice(gold: 25));

        Mod.AddContent(BloodMoon);
        Mod.AddContent(Glimmer);
        Mod.AddContent(Eclipse);
    }

    public override void Unload() {
        BloodMoon = null;
        Glimmer = null;
        Eclipse = null;
    }

    private class EventDeactivatorItem : InstancedModItem, ITransformItem {
        public EventDeactivatorDisabledItem DisabledVariant { get; private set; }

        private readonly int _rarity;
        private readonly int _value;

        public EventDeactivatorItem(string name, string texture, int rarity, int value) : base(name, texture) {
            _rarity = rarity;
            _value = value;
        }

        public override LocalizedText Tooltip => this.GetCategoryText("EventGlasses.Tooltip")
            .WithFormatArgs(base.Tooltip, Language.GetText("Mods.Aequus.Items.CommonTooltips.DisableItem"));

        public override void Load() {
            DisabledVariant = new EventDeactivatorDisabledItem(this);
            Mod.AddContent(DisabledVariant);
        }

        public void Transform(Player player) {
            Item.Transform(DisabledVariant.Type);
            SoundEngine.PlaySound(SoundID.Grab);
        }

        public override void SetDefaults() {
            Item.width = 24;
            Item.height = 24;
            Item.rare = _rarity;
            Item.value = _value;
        }
    }

    private class EventDeactivatorDisabledItem : InstancedModItem, ITransformItem {
        public readonly EventDeactivatorItem _parent;

        public EventDeactivatorDisabledItem(EventDeactivatorItem parent) : base(parent.Name + "Disabled", parent.Texture + "Inactive") {
            _parent = parent;
        }

        public void Transform(Player player) {
            Item.Transform(_parent.Type);
            SoundEngine.PlaySound(SoundID.Grab);
        }

        public override LocalizedText DisplayName => this.GetCategoryText("EventGlasses.DisabledName")
            .WithFormatArgs(_parent.DisplayName, this.GetCategoryText("EventGlasses.Folded"));
        public override LocalizedText Tooltip => this.GetCategoryText("EventGlasses.Tooltip")
            .WithFormatArgs(_parent.GetLocalization("Tooltip"), Language.GetText("Mods.Aequus.Items.CommonTooltips.EnableItem"));

        public override void SetStaticDefaults() {
            ContentSamples.CreativeResearchItemPersistentIdOverride[Type] = _parent.Type;
        }

        public override void SetDefaults() {
            Item.width = 24;
            Item.height = 24;
            Item.rare = _parent.Item.rare;
            Item.value = _parent.Item.value;
        }
    }
}
