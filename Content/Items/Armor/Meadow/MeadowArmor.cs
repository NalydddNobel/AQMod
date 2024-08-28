using Aequus.Common.ContentTemplates;
using Aequus.Common.ContentTemplates.Armor;
using Aequus.Common.Utilities;
using Aequus.Content.Biomes.Meadows.Tiles;
using Aequus.Projectiles;
using Terraria.Localization;

namespace Aequus.Content.Items.Armor.Meadow;

[Gen.AequusPlayer_ResetField<bool>("setMeadow")]
[Gen.AequusPlayer_ResetField<bool>("setMeadowMagicTag")]
public class MeadowArmor : UnifiedArmorSet, IAddRecipes, ISetbonusProvider {
    #region Stats
    public static readonly float HelmetManaReduction = 0.12f;
    public static readonly int HelmetMaxMana = 20;
    public static readonly int BreastplateMagicAndSummonDamageFlat = 1;
    public static readonly int BreastplateMaxMana = 20;
    public static readonly float GreavesMovementSpeedIncrease = 0.1f;
    public static readonly int GreavesMaxMana = 20;
    public static readonly int SetbonusMinionSlot = 1;

    public static readonly int HeadDefense = 2;
    public static readonly int BodyDefense = 3;
    public static readonly int LegsDefense = 2;
    #endregion

    public ModItem? Helmet { get; private set; }
    public ModItem? Breastplate { get; private set; }
    public ModItem? Greaves { get; private set; }

    public LocalizedText? SetbonusText { get; set; }

    public override void Load() {
        Helmet = this.AddContent(new InstancedArmorPiece(this, nameof(Helmet), new(
            EquipTypes: [EquipType.Head],
            Defense: HeadDefense,
            UpdateEquip: UpdateHelmet,
            TooltipFormatArgs: [ALanguage.Percent(HelmetManaReduction), HelmetMaxMana],
            Setbonus: this
        )));
        Breastplate = this.AddContent(new InstancedArmorPiece(this, nameof(Breastplate), new(
            EquipTypes: [EquipType.Body],
            Defense: BodyDefense,
            UpdateEquip: UpdateBreastplate,
            TooltipFormatArgs: [BreastplateMagicAndSummonDamageFlat, BreastplateMaxMana]
        )));
        Greaves = this.AddContent(new InstancedArmorPiece(this, nameof(Greaves), new(
            EquipTypes: [EquipType.Legs],
            Defense: LegsDefense,
            UpdateEquip: UpdateGreaves,
            TooltipFormatArgs: [ALanguage.Percent(GreavesMovementSpeedIncrease), GreavesMaxMana]
        )));
        SetbonusText = this.GetLocalization("Setbonus").WithFormatArgs(SetbonusMinionSlot);

        On_Main.DrawInterface_1_2_DrawEntityMarkersInWorld += On_Main_DrawInterface_1_2_DrawEntityMarkersInWorld;
    }

    void UpdateHelmet(Item Item, Player Player) {
        Player.manaCost *= 1f - HelmetManaReduction;
        Player.statManaMax2 += HelmetMaxMana;
    }

    void UpdateBreastplate(Item Item, Player Player) {
        Player.GetDamage(DamageClass.Magic).Flat += BreastplateMagicAndSummonDamageFlat;
        Player.GetDamage(DamageClass.Summon).Flat += BreastplateMagicAndSummonDamageFlat;
        Player.statManaMax2 += BreastplateMaxMana;
    }

    void UpdateGreaves(Item Item, Player Player) {
        Player.moveSpeed += GreavesMovementSpeedIncrease;
        Player.statManaMax2 += BreastplateMaxMana;
    }

    bool ISetbonusProvider.IsArmorSet(Item Head, Item Body, Item Legs) {
        return Body.type == Breastplate!.Type && Legs.type == Greaves!.Type;
    }

    void ISetbonusProvider.UpdateArmorSet(Item Item, Player Player) {
        Player.maxMinions += SetbonusMinionSlot;
        AequusPlayer aequus = Player.GetModPlayer<AequusPlayer>();
        aequus.setMeadow = true;
        aequus.setMeadowMagicTag = true;
    }

    // Dumb hack to only allow projectiles from the player's held item to change the minion target.
    // This is to prevent weapons like the Crimson Rod from changing the minion target.
    [Gen.AequusPlayer_ModifyHitNPCWithProj]
    public static void ModifyHitNPCWithProj(Player player, AequusPlayer aequus, Projectile proj, NPC target, ref NPC.HitModifiers modifiers) {
        // Check if the projectile source from the player's current held item.
        if (aequus.setMeadowMagicTag
            && proj.TryGetGlobalProjectile(out AequusProjectile aequusProj)
            && aequusProj.sourceItemUsed != player.HeldItem.type) {
            // If not, set this flag to false to disable the code in OnHitNPC.
            aequus.setMeadowMagicTag = false;
        }
    }

    [Gen.AequusPlayer_OnHitNPC]
    public static void OnHitNPC(Player player, AequusPlayer aequus, NPC target, NPC.HitInfo hit) {
        if (!aequus.setMeadowMagicTag || !hit.DamageType.CountsAsClass(DamageClass.Magic)) {
            // If this condition doesnt pass, set the flag to match aequus.setMeadow, since it might have been set to false previously by ModifyHitNPCWithProj.
            aequus.setMeadowMagicTag = aequus.setMeadow;
            return;
        }

        player.MinionAttackTargetNPC = target.whoAmI;
    }

    void IAddRecipes.AddRecipes(Aequus aequus) {
        Helmet!.CreateRecipe()
            .AddIngredient(Instance<MeadowWood>().Item.Type, 20)
            .AddTile(TileID.WorkBenches)
            .Register();

        Breastplate!.CreateRecipe()
            .AddIngredient(Instance<MeadowWood>().Item.Type, 30)
            .AddTile(TileID.WorkBenches)
            .Register();

        Greaves!.CreateRecipe()
            .AddIngredient(Instance<MeadowWood>().Item.Type, 30)
            .AddTile(TileID.WorkBenches)
            .Register();
    }

    #region Hooks
    // Use a detour and simple hack instead of an IL edit.
    private static void On_Main_DrawInterface_1_2_DrawEntityMarkersInWorld(On_Main.orig_DrawInterface_1_2_DrawEntityMarkersInWorld orig) {
        DamageClass? overrideDamageClass = null;
        Item heldItem = Main.LocalPlayer.HeldItem;

        // We temporarily mark the held item as a summon weapon to trick the game into rendering the selected minion target.
        if (Main.LocalPlayer.TryGetModPlayer(out AequusPlayer aequus) && aequus.setMeadowMagicTag && (heldItem.DamageType?.CountsAsClass(DamageClass.Magic) ?? false)) {
            overrideDamageClass = Main.LocalPlayer.HeldItem.DamageType;
            Main.LocalPlayer.HeldItem.DamageType = DamageClass.Summon;
        }

        try {
            // Run the original code.
            orig();
        }
        finally {
            // Reset the damage class back to its original instance.
            if (overrideDamageClass != null) {
                Main.LocalPlayer.HeldItem.DamageType = overrideDamageClass;
            }
        }
    }
    #endregion
}
