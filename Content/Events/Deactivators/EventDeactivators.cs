using Aequus.Core.ContentGeneration;
using Aequus.Old.Content.Materials;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Reflection;

namespace Aequus.Content.Events.Deactivators;
public class EventDeactivators : ModSystem {
    public const string CD_KEY = "EventDeactivator";

    public static int CDTime { get; set; } = 420;

    public static ModItem BloodMoonItem { get; private set; }
    public static ModItem GlimmerItem { get; private set; }
    public static ModItem EclipseItem { get; private set; }

    public override void Load() {
        BloodMoonItem = new EventDeactivatorItem("BloodMoonDeactivator", AequusTextures.BloodMoonDeactivator.Path,
            ItemRarityID.Green, Item.buyPrice(gold: 15), p => p.GetModPlayer<EventDeactivatorPlayer>().accDisableBloodMoon = true);
        GlimmerItem = new EventDeactivatorItem("GlimmerDeactivator", AequusTextures.GlimmerDeactivator.Path,
            ItemRarityID.Green, Item.buyPrice(gold: 15), p => p.GetModPlayer<EventDeactivatorPlayer>().accDisableGlimmer = true);
        EclipseItem = new EventDeactivatorItem("EclipseDeactivator", AequusTextures.EclipseDeactivator.Path,
            ItemRarityID.Yellow, Item.buyPrice(gold: 25), p => p.GetModPlayer<EventDeactivatorPlayer>().accDisableEclipse = true);

        Mod.AddContent(BloodMoonItem);
        Mod.AddContent(GlimmerItem);
        Mod.AddContent(EclipseItem);

        On_NPC.SpawnNPC += SpawnNPCUndoFlags;
        On_Projectile.FishingCheck += OverrideFishingFlags;
        IL_NPC.SpawnNPC += SpawnNPCOverrideEventsPerPlayer;
        IL_Main.UpdateAudio += OverrideEventsForMusic;
        IL_Main.CalculateWaterStyle += OverrideBloodMoonWaterStyle;
    }

    public override void AddRecipes() {
        BloodMoonItem.CreateRecipe()
            .AddIngredient(ItemID.BlackLens)
            .AddIngredient<BloodyTearstone>(5)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        GlimmerItem.CreateRecipe()
            .AddIngredient(ItemID.BlackLens)
            .AddIngredient<StariteMaterial>(5)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        EclipseItem.CreateRecipe()
            .AddIngredient(ItemID.BlackLens)
            .AddIngredient(ItemID.LunarTabletFragment, 5)
            .AddIngredient(ItemID.Ectoplasm, 5)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }

    public override void Unload() {
        BloodMoonItem = null;
        GlimmerItem = null;
        EclipseItem = null;
    }

    #region Hooks
    private static void OverrideFishingFlags(On_Projectile.orig_FishingCheck orig, Projectile projectile) {
        EventDeactivatorPlayer.CheckPlayerFlagOverrides(Main.player[projectile.owner]);
        orig(projectile);
        EventDeactivatorPlayer.UndoPlayerFlagOverrides();
    }

    private static void SpawnNPCUndoFlags(On_NPC.orig_SpawnNPC orig) {
        orig();
        EventDeactivatorPlayer.UndoPlayerFlagOverrides();
    }

    private void SpawnNPCOverrideEventsPerPlayer(ILContext il) {
        ILCursor c = new ILCursor(il);

        if (!c.TryGotoNext(MoveType.Before, i => i.MatchLdsfld(typeof(Main), nameof(Main.slimeRain)))) {
            Mod.Logger.Error($"Could not find Main.slimeRain ldsfld code."); return;
        }

        ILLabel slimeRainCheckLabel = c.MarkLabel();

        int loc = -1;
        if (!c.TryGotoPrev(i => i.MatchLdsfld(typeof(Main), nameof(Main.player)) && i.Next.MatchLdloc(out loc)) && loc != -1) {
            Mod.Logger.Error($"Could not find Main.player ldsfld code."); return;
        }

        c.GotoLabel(slimeRainCheckLabel);

        c.Emit(OpCodes.Ldloc, loc);
        c.EmitDelegate((int player) => {
            EventDeactivatorPlayer.UndoPlayerFlagOverrides();
            EventDeactivatorPlayer.CheckPlayerFlagOverrides(Main.player[player]);
        });
    }

    private void OverrideBloodMoonWaterStyle(ILContext il) {
        ILCursor c = new ILCursor(il);
        if (!c.TryGotoNext(MoveType.After, i => i.MatchLdsfld(typeof(Main), nameof(Main.bloodMoon)))) {
            Mod.Logger.Error($"Could not find {nameof(Main)}.{nameof(Main.bloodMoon)} ldsfld-branching code."); return;
        }

        c.EmitDelegate((bool bloodMoon) => bloodMoon && (!Main.LocalPlayer.TryGetModPlayer(out EventDeactivatorPlayer eventDeactivator) || !eventDeactivator.accDisableBloodMoon));
    }

    private void OverrideEventsForMusic(ILContext il) {
        const string SwapMusicField = "swapMusic";
        const BindingFlags SwapMusicBindings = BindingFlags.Static | BindingFlags.NonPublic;

        ILCursor c = new ILCursor(il);

        if (!c.TryGotoNext(MoveType.After, i => i.MatchLdsfld(typeof(Main), SwapMusicField))) {
            Mod.Logger.Error($"Could not find {nameof(Main)}.{SwapMusicField} ldsfld code."); return;
        }

        c.EmitDelegate((bool swapMusic) => EventDeactivatorPlayer.CheckPlayerFlagOverrides(Main.LocalPlayer));
        c.EmitLdsfld(typeof(Main).GetField(SwapMusicField, SwapMusicBindings));

        if (!c.TryGotoNext(MoveType.After, i => i.MatchLdsfld(typeof(Main), nameof(Main.musicBox2)))) {
            Mod.Logger.Error($"Could not find {nameof(Main)}.{nameof(Main.musicBox2)} ldsfld code."); return;
        }

        c.EmitDelegate((int musicBox2) => EventDeactivatorPlayer.UndoPlayerFlagOverrides());
        c.EmitLdsfld(typeof(Main).GetField(nameof(Main.musicBox2), BindingFlags.Static | BindingFlags.Public));
    }
    #endregion

    [AutoloadEquip(EquipType.Face)]
    private class EventDeactivatorItem : InstancedModItem {
        private readonly int _rarity;
        private readonly int _value;

        private readonly Action<Player> _updateFlag;

        public EventDeactivatorItem(string name, string texture, int rarity, int value, Action<Player> updateFlag) : base(name, texture) {
            _rarity = rarity;
            _value = value;
            _updateFlag = updateFlag;
        }

        public override void SetStaticDefaults() {
            ItemSets.WorksInVoidBag[Type] = true;
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory();
            Item.rare = _rarity;
            Item.value = _value;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            _updateFlag.Invoke(player);
        }

        /*
        public EventDeactivatorDisabledItem DisabledVariant { get; private set; }
        
        public override LocalizedText Tooltip => this.GetCategoryText("EventGlasses.Tooltip")
            .WithFormatArgs(base.Tooltip, Language.GetText("Mods.Aequus.Items.CommonTooltips.DisableItem"));

        public int CooldownTime => CDTime;
        bool ICooldownItem.ShowCooldownTip => false;
        string ICooldownItem.TimerId => CD_KEY;

        public override void Load() {
            DisabledVariant = new EventDeactivatorDisabledItem(this);
            Mod.AddContent(DisabledVariant);
        }

        public void Transform(Player player) {
            Item.Transform(DisabledVariant.Type);
            this.SetCooldown(player);
            SoundEngine.PlaySound(SoundID.Unlock);
        }

        public override bool CanUseItem(Player player) {
            return !this.HasCooldown(player);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            if (NPC.downedPlantBoss) {
                tooltips.AddTooltip(new TooltipLine(Mod, "Plantera Lock", );
                tooltips.RemoveAll(t => t.Mod == "Terraria" && t.Name.StartsWith("Tooltip"));
            }
        }

        public override void UpdateInventory(Player player) {
            _updateFlag.Invoke(player);
        }

        public override void UpdateInfoAccessory(Player player) {
            _updateFlag.Invoke(player);
        }*/
    }

    /*private class EventDeactivatorDisabledItem : InstancedModItem, ITransformItem, ICooldownItem {
        public readonly EventDeactivatorItem _parent;

        public EventDeactivatorDisabledItem(EventDeactivatorItem parent) : base(parent.Name + "Disabled", parent.Texture + "Inactive") {
            _parent = parent;
        }

        public int CooldownTime => CDTime;
        bool ICooldownItem.ShowCooldownTip => false;
        string ICooldownItem.TimerId => CD_KEY;

        public void Transform(Player player) {
            Item.Transform(_parent.Type);
            this.SetCooldown(player);
            SoundEngine.PlaySound(SoundID.Unlock);
        }

        public override LocalizedText DisplayName => this.GetCategoryText("EventGlasses.DisabledName")
            .WithFormatArgs(_parent.DisplayName, this.GetCategoryText("EventGlasses.Folded"));
        public override LocalizedText Tooltip => this.GetCategoryText("EventGlasses.Tooltip")
            .WithFormatArgs(_parent.GetLocalization("Tooltip"), Language.GetText("Mods.Aequus.Items.CommonTooltips.EnableItem"));

        public override void SetStaticDefaults() {
            ContentSamples.CreativeResearchItemPersistentIdOverride[Type] = _parent.Type;
            ItemSets.ShimmerCountsAsItem[Type] = _parent.Type;
        }

        public override void SetDefaults() {
            Item.width = 24;
            Item.height = 24;
            Item.rare = _parent.Item.rare;
            Item.value = _parent.Item.value;
        }

        public override bool CanUseItem(Player player) {
            return !this.HasCooldown(player);
        }
    }*/
}
