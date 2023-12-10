using Aequus.Common.Items;
using Aequus.Common.Items.Components;
using Aequus.Common.Items.Tooltips;
using Aequus.Core;
using Aequus.Core.UI;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace Aequus.Content.DedicatedContent.EtOmniaVanitas;

[Autoload(false)]
[WorkInProgress]
internal partial class EtOmniaVanitas : InstancedModItem, IDedicatedItem, ITransformItem, ICooldownItem, IAddKeywords {
    public readonly record struct ScaledStats(int Damage, int UseTime, float ShootSpeed, float AmmoConsumptionReduction, int Rarity, int CooldownTime, float ChargeShotDamageIncrease, int ChargeShotDefenseReduction, int ChargeShotDefenseReductionDuration, int FrostburnDebuff = BuffID.Frostburn);

    public static int DropChance { get; set; } = 100;

    public static int MaxChargeProgress { get; set; } = 300;

    public readonly GameProgression TierLock;
    public readonly ScaledStats Stats;

    public string DedicateeName => "Yuki-Miyako";
    public Color TextColor => new Color(60, 60, 120);

    public int CooldownTime => Stats.CooldownTime;
    bool ICooldownItem.AddTooltip => false;

    public override LocalizedText DisplayName => this.GetCategoryText("EtOmniaVanitas.DisplayName", () => "Et Omnia Vanitas");
    public override LocalizedText Tooltip {
        get {
            var tt = this.GetCategoryText("EtOmniaVanitas.Tooltip");
            object consumeLine = "";
            if (Stats.AmmoConsumptionReduction > 0f) {
                consumeLine = Language.GetText("CommonItemTooltip.PercentChanceToSaveAmmo").WithFormatArgs(TextHelper.Percent(Stats.AmmoConsumptionReduction));
            }
            return tt.WithFormatArgs(consumeLine);
        }
    }

    public EtOmniaVanitas(GameProgression tierLock, ScaledStats stats) : base("EtOmniaVanitas" + tierLock.ToString(), AequusTextures.EtOmniaVanitas.Path) {
        TierLock = tierLock;
        Stats = stats;
    }

    public override void SetStaticDefaults() {
        ItemID.Sets.gunProj[Type] = true;
        ContentSamples.CreativeResearchItemPersistentIdOverride[Type] = EtOmniaVanitasLoader.Tier1.Type;
        ItemID.Sets.ShimmerCountsAsItem[Type] = EtOmniaVanitasLoader.Tier1.Type;
        EtOmniaVanitasLoader.ProgressionToItem.Add(TierLock, this);
    }

    public override void SetDefaults() {
        Item.SetWeaponValues(Stats.Damage, 4f);
        Item.DefaultToRangedWeapon(ModContent.ProjectileType<EtOmniaVanitasProj>(), AmmoID.Bullet, Stats.UseTime, Stats.ShootSpeed, hasAutoReuse: true);
        Item.rare = Stats.Rarity;
        Item.value = Item.sellPrice(gold: 1);
        Item.channel = true;
        Item.noUseGraphic = true;
        if (!Main.gameMenu) {
            _checkAutoUpgrade = EtOmniaVanitasLoader.GetGameProgress();
        }
    }

    public override bool CanConsumeAmmo(Item ammo, Player player) {
        return Main.rand.NextFloat() > Stats.AmmoConsumptionReduction;
    }

    public void AddSpecialTooltips() {
        var keyword = new Keyword(this.GetCategoryTextValue("EtOmniaVanitas.ChargedShotKeyword"), new Color(140, 180, 255));
        keyword.AddLine(this.GetCategoryTextValue("EtOmniaVanitas.ChargedShotKeyTip").FormatWith(TextHelper.Percent(Stats.ChargeShotDamageIncrease), Stats.ChargeShotDefenseReduction, TextHelper.Seconds(Stats.ChargeShotDefenseReductionDuration), TextHelper.Seconds(this.GetCooldownTime(Item.prefix))));
        KeywordSystem.Tooltips.Add(keyword);

        keyword = new Keyword(this.GetCategoryTextValue("EtOmniaVanitas.ScalingKeyword"), new Color(120, 240, 150));
        keyword.AddLine(this.GetCategoryTextValue("EtOmniaVanitas.ScalingKeyTip"));
        KeywordSystem.Tooltips.Add(keyword);
    }

    #region Drawing
    public override Vector2? HoldoutOffset() {
        return new Vector2(-12f, 0f);
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        CheckAutoUpgradeWhenDrawing();
        return true;
    }

    public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        if (UISystem.Slot.Context == ItemSlot.Context.CreativeInfinite) {
            return;
        }
        var iconPosition = position + new Vector2(TextureAssets.InventoryBack.Value.Width - 30f, TextureAssets.InventoryBack.Value.Height - 30f) / 2f * Main.inventoryScale;
        int rare = Item.OriginalRarity;
        var color = rare <= ItemRarityID.Purple ? ItemRarity.GetColor(rare) : RarityLoader.GetRarity(rare).RarityColor;
        spriteBatch.Draw(AequusTextures.EtOmniaVanitasTierIndicator, iconPosition, null, color, 0f, AequusTextures.EtOmniaVanitasTierIndicator.Size() / 2f, 1f, SpriteEffects.None, 0f);
    }
    #endregion

    #region Save & Load
    public override void SaveData(TagCompound tag) {
        tag["GameProgression"] = _checkAutoUpgrade.ToString();
    }

    public override void LoadData(TagCompound tag) {
        if (tag.TryGet("GameProgression", out string gameProgressionString) && Enum.TryParse<GameProgression>(gameProgressionString, out var gameProgression)) {
            _checkAutoUpgrade = gameProgression;
            CheckAutoUpgrade(playSound: false);
        }
    }
    #endregion

    public override bool CanShoot(Player player) {
        return player.ownedProjectileCounts[Item.shoot] < 1;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
        Projectile.NewProjectile(source, player.Center, Vector2.Normalize(velocity), Item.shoot, damage, knockback, player.whoAmI);
        return false;
    }
}