using Aequus.Common.Items;
using Aequus.Common.Items.EquipmentBooster;
using Aequus.Common.Recipes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Money.FaultyCoin;

public class FaultyCoin : ModItem {
    /// <summary>
    /// Default Value: <see cref="Item.platinum"/> (1000000) (1 Platinum)
    /// </summary>
    public static long MoneyAmount = Item.platinum;
    public static float IncreasedMoney = 0.1f;

    public float removeFailAnimation;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(TextHelper.Create.Percent(IncreasedMoney));

    public override void SetStaticDefaults() {
        EquipBoostDatabase.Instance.SetEntry(this, EquipBoostDatabase.ModItemTooltip(this).WithFormatArgs(TextHelper.Create.Percent(IncreasedMoney * 2f)));
    }

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Green;
        Item.value = Item.buyPrice(gold: 10);
        Item.hasVanityEffects = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        var aequus = player.Aequus();
        aequus.increasedEnemyMoney += 0.05f;
        aequus.accFaultyCoinLoan = Math.Max(MoneyAmount, aequus.accFaultyCoinLoan);
        aequus.accFaultyCoinItem = Item;
    }

    public override bool CanEquipAccessory(Player player, int slot, bool modded) {
        return player.Aequus().accFaultyCoinDebt <= 0;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        var player = Main.LocalPlayer;
        var aequus = player.Aequus();
        if (Main.mouseRight && Main.mouseRightRelease && player.CanAfford((int)aequus.accFaultyCoinDebt)) {
            OnRemoveAccessory(player);
        }

        long tooltipDebt = Math.Max(aequus.accFaultyCoinDebt, MoneyAmount);
        string colorText = TextHelper.ColorCommandStart(Colors.CoinPlatinum, alphaPulse: true);

        foreach (var t in tooltips) {
            if (t.Mod != "Terraria") {
                continue;
            }

            t.Text = t.Text.Replace("[[", colorText);
            t.Text = t.Text.Replace("]]", "]");
        }

        tooltips.Insert(
            tooltips.GetIndex("Material", 1),
            new(Mod, "FaultyCoinDebt", TextHelper.GetTextValueWith("Items.FaultyCoin.Debt", new { Debt = TextHelper.PriceTextColored(tooltipDebt, alphaPulse: true) }))
        );
    }

    public override void AddRecipes() {
        AequusRecipes.AddShimmerCraft(Type, ModContent.ItemType<FoolsGoldRing.FoolsGoldRing>());
    }

    public void OnRemoveAccessory(Player player) {
        player.BuyItem((int)player.Aequus().accFaultyCoinDebt);
        player.Aequus().accFaultyCoinDebt = 0;
        SoundEngine.PlaySound(SoundID.Coins);
    }

    public void OnUnsuccessfulRemove(Player player) {
        SoundEngine.PlaySound(AequusSounds.coinHit);
        removeFailAnimation = 30f;
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        float rotation = MathF.Sin(removeFailAnimation / 2f) * removeFailAnimation / 60f;

        if (removeFailAnimation > 0f) {
            float progress = removeFailAnimation / 30f;

            spriteBatch.Draw(
                AequusTextures.Bloom0,
                position,
                null,
                Color.Black * progress,
                0f,
                AequusTextures.Bloom0.Size() / 2f,
                Main.inventoryScale * 0.6f * progress, SpriteEffects.None, 0f
            );

            Main.instance.LoadProjectile(ProjectileID.RainbowCrystalExplosion);
            var texture = TextureAssets.Projectile[ProjectileID.RainbowCrystalExplosion].Value;
            var bulletOrigin = texture.Size() / 2f;
            ulong seed = (ulong)Main.LocalPlayer.name.GetHashCode();
            float powProgress = MathF.Pow(progress, 2f);
            float bulletOpacity = 1f;
            if (removeFailAnimation > 15f) {
                bulletOpacity = 1f - (removeFailAnimation - 15f) / 15f;
            }
            foreach (var r in Helper.Circular(10)) {

                float randomFloat = Utils.RandomFloat(ref seed);

                float bulletRotation = r + (randomFloat * 0.2f - 0.1f);
                float bulletProgress = powProgress * Helper.Wave(Main.GlobalTimeWrappedHourly * Math.Max(randomFloat, 0.5f) * 3f + randomFloat * 10f, 1f, 1.2f);
                var v = bulletRotation.ToRotationVector2();
                spriteBatch.Draw(
                    texture,
                    position + v * bulletProgress * 40f * Main.inventoryScale,
                    null,
                    Color.Black * bulletProgress * bulletOpacity,
                    bulletRotation + MathHelper.PiOver2,
                    bulletOrigin,
                    new Vector2(1f, 2f) * Main.inventoryScale * 0.2f, SpriteEffects.None, 0f
                );
            }
        }

        spriteBatch.Draw(
            TextureAssets.Item[Type].Value,
            position,
            frame,
            drawColor,
            rotation,
            frame.Size() / 2f,
            scale, SpriteEffects.None, 0f
        );

        removeFailAnimation *= 0.9f;
        if (removeFailAnimation < 0.01f) {
            removeFailAnimation = 0f;
        }
        return false;
    }
}