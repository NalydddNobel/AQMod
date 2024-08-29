using Aequus.Common.ContentTemplates.Generic;
using Aequus.Common.Entities.Items;
using Aequus.Common.Recipes;
using Aequus.Content.Items.Tools.PocketWormhole;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Aequus.Content.Items.Tools.PhasePhone;

public sealed class PhasePhones : ModTexturedType, IAddRecipes, ILocalizedModType {
    public ModItem? PhasePhone { get; private set; }
    public ModItem? PhasePhoneHome { get; private set; }
    public ModItem? PhasePhoneSpawn { get; private set; }
    public ModItem? PhasePhoneOcean { get; private set; }
    public ModItem? PhasePhoneUnderworld { get; private set; }

    string ILocalizedModType.LocalizationCategory => "Items";

    // Get rid of the 's' in PhasePhones
    public override string Texture => base.Texture[..^1];

    protected sealed override void Register() {
        ModItem? previous = null;

        PhasePhone = Add("", new PhasePhoneInfo(ItemID.ShellphoneDummy, p => p.Spawn(PlayerSpawnContext.RecallFromItem)));

        PhasePhoneUnderworld = Add("Underworld", new PhasePhoneInfo(ItemID.ShellphoneHell,
            p => p.DemonConch(),
            DustTypeFactory: () => DustID.Lava
        ));
        PhasePhoneOcean = Add("Ocean", new PhasePhoneInfo(ItemID.ShellphoneOcean,
            p => p.MagicConch(),
            DustTypeFactory: Dust.dustWater
        ));
        PhasePhoneSpawn = Add("Spawn", new PhasePhoneInfo(ItemID.ShellphoneSpawn,
            p => p.Shellphone_Spawn(),
            DustTypeFactory: () => DustID.RainbowMk2,
            DustColorFactory: () => Main.rand.Next(4) switch {
                2 => Color.Yellow,
                3 => Color.White,
                _ => new(100, 255, 100),
            }
        ));
        PhasePhoneHome = Add("Home", new PhasePhoneInfo(ItemID.Shellphone, p => p.Spawn(PlayerSpawnContext.RecallFromItem)));
        // Manually set the Underworld's phone to transform into the Home variant.
        (PhasePhoneUnderworld as InstancedPhasePhone)!.Info.NextItem = PhasePhoneHome;

        ModItem Add(string nameSuffix, PhasePhoneInfo info) {
            InstancedPhasePhone next = new InstancedPhasePhone(this, nameSuffix, info with { NextItem = previous });
            Mod.AddContent(next);
            previous = next;
            return next;
        }
    }

    void IAddRecipes.AddRecipes(Aequus aequus) {
        Recipe.Create(PhasePhone!.Type)
            .AddRecipeGroup(AequusRecipes.Shellphone)
            .AddIngredient<PhaseMirror>()
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }
}

[Autoload(false)]
internal class InstancedPhasePhone(PhasePhones Parent, string Suffix, PhasePhoneInfo info) : InstancedModItem($"PhasePhone{Suffix}", $"{Parent.Texture}{Suffix}"), IPhaseMirror, ITransformItem {
    public PhasePhoneInfo Info = info;

    int NextItem => Info.NextItem?.Type ?? Instance<PhasePhones>().PhasePhoneHome!.Type;

    public override LocalizedText DisplayName => Parent.GetLocalization($"{Name}.DisplayName", () => $"Phase Phone ({Suffix})");

    public override LocalizedText Tooltip => Parent.GetLocalization("Tooltip").WithFormatArgs(Parent.GetLocalization($"{Name}.Tooltip", () => $""));

    public override void SetStaticDefaults() {
        var loader = ModContent.GetInstance<PhasePhones>();
        ContentSamples.CreativeResearchItemPersistentIdOverride[Type] = loader.PhasePhone!.Type;
        ItemID.Sets.ShimmerCountsAsItem[Type] = loader.PhasePhone.Type;
        ItemID.Sets.WorksInVoidBag[Type] = true;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(Info.CopyItem);
        Item.rare++;
        Item.useTime = 64;
        Item.useAnimation = 64;
    }

    public override void UpdateInfoAccessory(Player player) {
        player.GetModPlayer<AequusPlayer>().infiniteWormhole = true;
        // Other effects are handled automagically by Aequus.Common.Entites.Items.PDAEffects.
    }

    void IPhaseMirror.GetPhaseMirrorDust(Player player, out int dustType, out Color dustColor) {
        dustType = Info.DustTypeFactory?.Invoke() ?? DustID.MagicMirror;
        dustColor = Info.DustColorFactory?.Invoke() ?? default;
    }

    void IPhaseMirror.Teleport(Player player) {
        Main.NewText(DisplayName.Key);
        Info.TeleportAction(player);
    }

    void ITransformItem.Transform(Player player) {
        Item.Transform(NextItem);
        SoundEngine.PlaySound(SoundID.Unlock);
    }

    public override void OnCreated(ItemCreationContext context) {
        if (context is RecipeItemCreationContext || context is JourneyDuplicationItemCreationContext) {
            Item.Transform(NextItem);
        }
    }
}

internal record struct PhasePhoneInfo(int CopyItem, Action<Player> TeleportAction, Func<int>? DustTypeFactory = null, Func<Color>? DustColorFactory = null, ModItem? NextItem = null);