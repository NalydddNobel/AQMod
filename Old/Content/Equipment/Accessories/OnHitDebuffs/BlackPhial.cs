using Aequus.Old.Content.Materials.PossessedShard;
using System.Collections.Generic;

namespace Aequus.Old.Content.Equipment.Accessories.OnHitDebuffs;

[AutoloadEquip(EquipType.Waist)]
public class BlackPhial : ModItem {
    public const string COOLDOWN_KEY = "BlackPhial";

    public static int CooldownDuration { get; set; } = 30;
    public static int DebuffDuration { get; set; } = 120;

    public static readonly List<int> DebuffsAfflicted = new() {
        BuffID.Poisoned,
        BuffID.OnFire3,
        BuffID.Frostburn2,
        BuffID.CursedInferno,
        BuffID.Ichor,
        BuffID.ShadowFlame,
    };

    public override void SetDefaults() {
        Item.DefaultToAccessory(14, 20);
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 10);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<AequusPlayer>().accBlackPhial++;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient<PossessedShard>(3)
            //.AddIngredient<RadonMoss>(16)
            .AddIngredient(ItemID.SoulofNight, 8)
            .Register();
    }
}