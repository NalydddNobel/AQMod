using Aequus.Common.Buffs;
using Aequus.Common.Items.EquipmentBooster;
using Aequus.Core.IO;
using System;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Aequus.Old.Content.Equipment.Accessories.PotionCanteen;

public class PotionCanteen : ModItem {
    public int itemIDLookup;
    public int buffID;

    public bool HasBuff => buffID > 0;

    public static LocalizedText AltName { get; private set; }

    public override void SetStaticDefaults() {
        EquipBoostDatabase.Instance.SetNoEffect(Type);
        AltName = this.GetLocalization("DisplayNameAlt");
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<PotionCanteenEmpty>();
    }

    public void SetPotionDefaults() {
        Item.buffType = buffID;
        Item.rare = ItemRarityID.Orange;
        if (itemIDLookup > 0) {
            Item.rare += ContentSamples.ItemsByType[itemIDLookup].rare;
        }
        Item.rare = Math.Min(Item.rare, ItemRarityID.Purple);
        Item.Prefix(Item.prefix);
        Item.ClearNameOverride();
        if (!Main.dedServ && buffID > 0 && AltName != null) {
            Item.SetNameOverride(GetName(Item.AffixName()));
        }
    }

    public override void SetDefaults() {
        Item.DefaultToAccessory(20, 20);
        Item.value = Item.buyPrice(gold: 10);
        Item.rare = ItemRarityID.Orange;
        SetPotionDefaults();
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        if (buffID > 0) {
            if (Main.myPlayer == player.whoAmI) {
                BuffUI.DisableRightClick.Add(buffID);
            }
            player.AddBuff(buffID, 1, quiet: true);
        }
    }

    public string GetName(string originalName) {
        return originalName.Replace(Lang.GetItemNameValue(Type), AltName.Format(Lang.GetBuffName(buffID)));
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        if (buffID > 0) {
            foreach (var t in tooltips) {
                //if (t.Name == "ItemName" && buffID > 0) {
                //    t.Text = GetName(t.Text);
                //}
                if (t.Name == "Tooltip0") {
                    t.Text = Lang.GetBuffDescription(buffID);
                }
            }
        }
    }

    private Rectangle GetLiquidFrame(Texture2D liquidTexture) {
        return liquidTexture.Frame(verticalFrames: 16, frameY: (int)Main.GameUpdateCount / 7 % 15);
    }

    private Color GetLiquidColor() {
        var clrs = ItemID.Sets.DrinkParticleColors[itemIDLookup];
        Color colorResult = Color.White;
        if (clrs != null && clrs.Length > 0) {
            //int minBrightness = 0;
            //for (int i = 0; i < clrs.Length; i++) {
            //    int colorBrightness = clrs[i].R + clrs[i].G + clrs[i].B;
            //    if (colorBrightness > minBrightness) {
            //        colorResult = clrs[i];
            //        minBrightness = colorBrightness;
            //    }
            //}
            float time = Main.GlobalTimeWrappedHourly;
            colorResult = Color.Lerp(clrs[(int)time % clrs.Length], clrs[(int)(time + 1) % clrs.Length], time % 1f);
        }
        return colorResult * 1.1f;
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        spriteBatch.Draw(AequusTextures.PotionCanteenEmpty, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
        if (!HasBuff) {
            return true;
        }
        var liquidTexture = AequusTextures.PotionCanteen_Liquid.Value;
        var liquidFrame = GetLiquidFrame(liquidTexture);
        var liquidColor = GetLiquidColor();
        float a = drawColor.A > 0 ? drawColor.A / 255f : Main.inventoryBack.A / 255f;
        spriteBatch.Draw(liquidTexture, position, liquidFrame, liquidColor * a, 0f, origin, scale, SpriteEffects.None, 0f);
        spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, liquidColor with { A = 255 }, 0f, origin, scale, SpriteEffects.None, 0f);
        return false;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
        Main.GetItemDrawFrame(Type, out Texture2D texture, out Rectangle frame);
        var position = Item.Center - Main.screenPosition;
        var origin = frame.Size() / 2f;
        spriteBatch.Draw(AequusTextures.PotionCanteenEmpty, position, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0f);
        if (HasBuff) {
            var liquidTexture = AequusTextures.PotionCanteen_Liquid.Value;
            var liquidFrame = GetLiquidFrame(liquidTexture);
            var liquidColor = GetLiquidColor();
            spriteBatch.Draw(liquidTexture, position, liquidFrame, liquidColor, rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, position, frame, ExtendLight.Get(Item.Center).MultiplyRGBA(liquidColor), rotation, origin, scale, SpriteEffects.None, 0f);
        }
        return false;
    }

    public override void SaveData(TagCompound tag) {
        if (!HasBuff) {
            return;
        }

        IDLoader<BuffID>.SaveId(tag, "Buff", buffID);
        IDLoader<ItemID>.SaveId(tag, "Item", itemIDLookup);
    }

    public override void LoadData(TagCompound tag) {
        buffID = IDLoader<BuffID>.LoadId(tag, "Buff");
        itemIDLookup = IDLoader<ItemID>.LoadId(tag, "Item");
        SetPotionDefaults();
    }

    public void OnPickupText(int index, PopupTextContext context, int stack, bool noStack, bool longText) {
        SetPotionDefaults();
        Main.popupText[index].name = GetName(Main.popupText[index].name);
    }
}