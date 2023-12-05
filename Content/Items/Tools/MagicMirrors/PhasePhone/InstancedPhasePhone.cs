using Aequus.Common.Items;
using Aequus.Common.Items.Components;
using Aequus.Content.Items.Tools.MagicMirrors.PhaseMirror;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Tools.MagicMirrors.PhasePhone;

[Autoload(false)]
internal class InstancedPhasePhone : InstancedModItem, IPhaseMirror, ITransformItem {
    public List<(int, int, Dust)> DustEffectCache { get; set; }

    public int UseAnimationMax => 64;

    private readonly int _shellPhoneClone;
    private readonly string _nameSuffix;
    
    private ModItem _shellPhoneConvert;
    private Func<int> _dustTypeFactory;
    private Func<Color> _customColorFactory;
    private Action<Player> _teleportAction;

    public InstancedPhasePhone(string nameSuffix, int shellPhone) : base($"PhasePhone{nameSuffix}", $"{typeof(InstancedPhasePhone).NamespaceFilePath()}/PhasePhone{nameSuffix}") {
        _nameSuffix = nameSuffix;
        _shellPhoneClone = shellPhone;
    }

    public override LocalizedText DisplayName {
        get {
            if (!string.IsNullOrEmpty(_nameSuffix)) {
                return this.GetCategoryText($"PhasePhone.{_nameSuffix}.DisplayName", ()=> $"Phase Phone ({_nameSuffix})");
            }
            return base.DisplayName;
        }
    }

    public override LocalizedText Tooltip {
        get {
            if (!string.IsNullOrEmpty(_nameSuffix)) {
                return this.GetCategoryText("PhasePhone.Tooltip").WithFormatArgs(this.GetCategoryText($"PhasePhone.{_nameSuffix}.Tooltip", () => $""));
            }
            return base.Tooltip.WithFormatArgs("");
        }
    }

    public InstancedPhasePhone ConvertInto(InstancedPhasePhone other) {
        _shellPhoneConvert = other;
        return this;
    }

    public InstancedPhasePhone WithDust(int dustId) {
        _dustTypeFactory += () => dustId;
        return this;
    }
    public InstancedPhasePhone WithDynamicDust(Func<int> dustFactory) {
        _dustTypeFactory += dustFactory;
        return this;
    }

    public InstancedPhasePhone WithDustColors(Func<Color> colorFactory) {
        _customColorFactory += colorFactory;
        return this;
    }

    public InstancedPhasePhone WithTeleportLocation(Action<Player> teleportLocation) {
        _teleportAction += teleportLocation;
        return this;
    }

    public override void SetStaticDefaults() {
        var loader = ModContent.GetInstance<PhasePhoneInstantiator>();
        ContentSamples.CreativeResearchItemPersistentIdOverride[Type] = loader.PhasePhone.Type;
        ItemID.Sets.ShimmerCountsAsItem[Type] = loader.PhasePhone.Type;
        ItemID.Sets.WorksInVoidBag[Type] = true;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(_shellPhoneClone);
        Item.rare++;
        Item.useTime = UseAnimationMax;
        Item.useAnimation = UseAnimationMax;
        DustEffectCache = new();
    }

    public override void UseStyle(Player player, Rectangle heldItemFrame) {
        if (player.altFunctionUse != 2 && !player.JustDroppedAnItem) {
            IPhaseMirror.UsePhaseMirror(player, Item, this);
        }
    }

    public override void HoldItem(Player player) {
    }

    // TODO: Make this support other modded info accessories which add their ingredients to the PDA.
    public override void UpdateInfoAccessory(Player player) {
        player.GetModPlayer<AequusPlayer>().infiniteWormhole = true;
        player.accWatch = 3;
        player.accCompass = 1;
        player.accDepthMeter = 1;
        player.accCalendar = true;
        player.accDreamCatcher = true;
        player.accFishFinder = true;
        player.accJarOfSouls = true;
        player.accOreFinder = true;
        player.accStopwatch = true;
        player.accThirdEye = true;
        player.accWeatherRadio = true;
        player.accCritterGuide = true;
    }

    public void Teleport(Player player, Item item, IPhaseMirror me) {
        if (_teleportAction != null) {
            _teleportAction.Invoke(player);
        }
        else {
            player.Spawn(PlayerSpawnContext.RecallFromItem);
        }
    }

    public void GetPhaseMirrorDust(Player player, Item item, IPhaseMirror me, out int dustType, out Color dustColor) {
        dustType = _dustTypeFactory?.Invoke() ?? DustID.MagicMirror;
        dustColor = _customColorFactory?.Invoke() ?? default;
    }

    public void Transform(Player player) {
        Item.Transform(_shellPhoneConvert.Type);
        SoundEngine.PlaySound(SoundID.Unlock);
    }

    public override void OnCreated(ItemCreationContext context) {
        if (context is RecipeItemCreationContext) {
            Item.Transform(_shellPhoneConvert.Type);
        }
    }
}