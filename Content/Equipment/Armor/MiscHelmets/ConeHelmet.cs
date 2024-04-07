using Aequus.Common.Items;
using Aequus.Common.Items.Components;
using Aequus.Common.Items.Tooltips;
using Aequus.Common.Players.Drawing;
using Aequus.Core.UI;
using System;
using System.ComponentModel;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace Aequus.Content.Equipment.Armor.MiscHelmets;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public abstract class ConeHelmetBase : ModItem, IPickItemMovementAction {
    internal class ConeHelmetEquipTexture : EquipTexture, IEquipTextureDraw {
        public void Draw(ref PlayerDrawSet drawInfo, Vector2 position, Rectangle frame, Color color, float rotation, Vector2 origin, SpriteEffects effects, int shader) {
            Texture2D texture = AequusTextures.ConeHelmet_Head;

            int frameY = frame.Y / frame.Height;

            Rectangle realFrame = texture.Frame(verticalFrames: 20, frameY: frameY);
            int difference = realFrame.Height - frame.Height;

            position += drawInfo.drawPlayer.GetModPlayer<ConeHelmetPlayer>()._coneHelmetShake;

            drawInfo.Draw(texture, position - new Vector2(0f, difference - 2f), realFrame, color, rotation, origin, 1f, effects, shader);
        }
    }

    public void OverrideItemMovementAction(ref int result, Item[] inventory, int context, int slot, Item checkItem) {
        if (context == ItemSlot.Context.PrefixItem) {
            result = IPickItemMovementAction.RESULT_SIMPLE_SWAP;
        }
    }

    public override void Load() {
        Item.headSlot = EquipLoader.AddEquipTexture(Mod, AequusTextures.None.Path, EquipType.Head, this, null, new ConeHelmetEquipTexture());
    }

    public override void SetStaticDefaults() {
        HelmetEquipSets.DrawHatHair[Item.headSlot] = true;
    }

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
        Item.value = ItemCommons.Price.PollutedOceanLoot;
        Item.headSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
    }

    #region Drawing
    public virtual float GetDamagePercentage => 0f;

    public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        float percentage = GetDamagePercentage;

        if (CurrentSlot.Instance.Context == ItemSlot.Context.MouseItem || percentage >= 1f) {
            return;
        }

        const int DurabilityUIFrames = 21;

        Texture2D durabilityTexture = AequusTextures.Durability;
        int durabilityFrameY = Math.Clamp((int)(DurabilityUIFrames * (1f - percentage)), 0, DurabilityUIFrames - 1);

        Rectangle durabilityFrame = durabilityTexture.Frame(verticalFrames: DurabilityUIFrames, frameY: durabilityFrameY);

        spriteBatch.Draw(durabilityTexture, position, durabilityFrame, Color.White, 0f, durabilityFrame.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f); ;
    }
    #endregion
}

public class ConeHelmet : ConeHelmetBase, IAddKeywords {
    public static int HitsRequiredToBreak { get; set; } = 30;

    public int hitsLeft;

    public override float GetDamagePercentage => hitsLeft / (float)HitsRequiredToBreak;

    public static float BonusEndurance { get; set; } = 0.25f;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ExtendLanguage.Percent(BonusEndurance));

    public override void SetDefaults() {
        base.SetDefaults();
        Item.rare = ItemCommons.Rarity.PollutedOceanLoot;
        Item.defense = 15;
        RefreshHitsRequired();
    }

    public override void UpdateEquip(Player player) {
        player.GetModPlayer<ConeHelmetPlayer>().armorConeHelmet = this;
        player.SafelyAddDamageReduction(BonusEndurance);
    }

    public override void SaveData(TagCompound tag) {
        tag["HitsLeft"] = hitsLeft;
    }

    public override void LoadData(TagCompound tag) {
        if (tag.TryGet("HitsLeft", out int hitsLeft)) {
            this.hitsLeft = hitsLeft;
        }
    }

    private void RefreshHitsRequired() {
        hitsLeft = HitsRequiredToBreak;
    }

    public void OnBreak(Player player) {
        Item.Transform(ModContent.ItemType<BrokenConeHelmet>());
        SoundEngine.PlaySound(AequusSounds.ItemBreak, player.Center);
    }

    #region Reforging 
    public override bool CanReforge() {
        return hitsLeft < HitsRequiredToBreak;
    }

    public override bool ReforgePrice(ref int reforgePrice, ref bool canApplyDiscount) {
        reforgePrice = Math.Clamp((int)((1f - hitsLeft / (float)HitsRequiredToBreak) * reforgePrice), reforgePrice / HitsRequiredToBreak, reforgePrice);
        return true;
    }

    public override void PostReforge() {
        RefreshHitsRequired();
    }
    #endregion

    #region Keywords
    public void AddSpecialTooltips() {
        Keyword tooltip = new Keyword(Language.GetTextValue("Mods.Aequus.Items.Keywords.Brittle.DisplayName"), Color.Lerp(Color.Blue, Color.White, 0.75f), ModContent.ItemType<BrokenConeHelmet>());
        tooltip.AddLine(Language.GetTextValue("Mods.Aequus.Items.Keywords.Brittle.Description", hitsLeft));
        KeywordSystem.Tooltips.Add(tooltip);
    }
    #endregion
}

public class BrokenConeHelmet : ConeHelmetBase {
    public override string Texture => AequusTextures.ConeHelmet.Path;

    public override LocalizedText DisplayName => ModContent.GetInstance<ConeHelmet>().GetLocalization("DisplayName_Broken");
    public override LocalizedText Tooltip => ModContent.GetInstance<ConeHelmet>().GetLocalization("Tooltip_Broken");

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        ContentSamples.CreativeResearchItemPersistentIdOverride[Type] = ModContent.ItemType<ConeHelmet>();
    }

    public override void SetDefaults() {
        base.SetDefaults();
        Item.defense = 1;
        Item.rare = ItemRarityID.Gray;
        Item.headSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
    }

    public override bool ReforgePrice(ref int reforgePrice, ref bool canApplyDiscount) {
        reforgePrice *= 2;
        return true;
    }

    public override void PostReforge() {
        Item.Transform(ModContent.ItemType<ConeHelmet>());
    }
}
