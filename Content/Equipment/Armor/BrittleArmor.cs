using Aequus.Common.Items;
using Aequus.Common.Items.Components;
using Aequus.Common.Items.Tooltips;
using Aequus.Common.Players.Drawing;
using Aequus.Core.ContentGeneration;
using Aequus.Core.UI;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace Aequus.Content.Equipment.Armor;

public abstract class BrittleArmor : ModTexturedType, ILocalizedModType {
    public readonly int MaxHits;
    public readonly EquipType EquipType;

    internal readonly BrittleArmorItem[] Items;

    public int NumTiers => Items.Length;

    public string LocalizationCategory => "Items.Armor";

    public BrittleArmor(int numTiers, int maxHits, EquipType equipType) {
        MaxHits = maxHits;
        EquipType = equipType;
        Items = new BrittleArmorItem[numTiers];
    }

    internal virtual LocalizedText GetDisplayName(BrittleArmorItem armor) => this.GetLocalization(TierText("DisplayName", armor.Tier));
    internal virtual LocalizedText GetTooltip(BrittleArmorItem armor) => this.GetLocalization(TierText("Tooltip", armor.Tier));

    internal abstract void SetItemDefaults(Item Item, BrittleArmorItem armor);

    internal virtual void SetItemStaticDefaults(Item Item, BrittleArmorItem armor) { }

    internal virtual void UpdateEquip(Player player, Item Item, BrittleArmorItem armor) { }

    internal virtual void OnBreak(Player player, BrittleArmorItem armor) {
        armor.Item.Transform(Items[armor.Tier + 1].Type);
    }

    protected sealed override void Register() {
        for (int i = 0; i < NumTiers; i++) {
            Items[i] = new BrittleArmorItem(this, i);
            Mod.AddContent(Items[i]);
        }
    }

    public sealed override void SetupContent() {
        SetStaticDefaults();
    }

    internal void FullUpdateEquip(Player player, Item item, BrittleArmorItem armor) {
        UpdateEquip(player, item, armor);
        if (armor.HitsLeft > 0) {
            player.GetModPlayer<BrittleArmorPlayer>().brittlePieces.Add(armor);
        }
    }

    public int HitsNeededForTier(int tier) {
        if (tier >= NumTiers - 1) {
            return 0;
        }

        return MaxHits / Math.Max(tier + 1, 1);
    }

    public static string TierText(string inputText, int tier) {
        return inputText + (tier > 0 ? $"_{tier}" : string.Empty);
    }
}

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class BrittleArmorItem : InstancedModItem, IPickItemMovementAction, IAddKeywords {
    [CloneByReference]
    internal readonly BrittleArmor _parent;
    public readonly int Tier;

    public int HitsLeft;

    public float DamagePercent => HitsLeft / (float)_parent.MaxHits;

    public override LocalizedText DisplayName => _parent.GetDisplayName(this);
    public override LocalizedText Tooltip => _parent.GetTooltip(this);

    public BrittleArmorItem(BrittleArmor parent, int tier) : base(BrittleArmor.TierText(parent.Name, tier), BrittleArmor.TierText(parent.Texture, tier)) {
        _parent = parent;
        Tier = tier;
    }

    public void OverrideItemMovementAction(ref int result, Item[] inventory, int context, int slot, Item checkItem) {
        if (context == ItemSlot.Context.PrefixItem) {
            result = IPickItemMovementAction.RESULT_SIMPLE_SWAP;
        }
    }

    public override void Load() {
        Item.headSlot = EquipLoader.AddEquipTexture(Mod, AequusTextures.None.Path, _parent.EquipType, this, null, new BrittleEquipTexture(Texture + $"_{_parent.EquipType}"));
    }

    public override void SetStaticDefaults() {
        _parent.SetItemStaticDefaults(Item, this);
        if (Tier != 0) {
            ContentSamples.CreativeResearchItemPersistentIdOverride[Type] = _parent.Items[0].Type;
        }
    }

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
        Item.value = ItemCommons.Price.PollutedOceanLoot;
        Item.headSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
        HitsLeft = _parent.HitsNeededForTier(Tier);
        _parent.SetItemDefaults(Item, this);
    }

    public override void UpdateEquip(Player player) {
        _parent.FullUpdateEquip(player, Item, this);
    }

    #region Reforging
    public override bool CanReforge() {
        return HitsLeft < _parent.MaxHits;
    }

    public override bool ReforgePrice(ref int reforgePrice, ref bool canApplyDiscount) {
        // Increased price when the hat is fully damaged
        if (HitsLeft == 0) {
            reforgePrice *= 2;
        }
        return true;
    }

    public override void PostReforge() {
        // Transform into the base cone hat upon reforging.
        Item.Transform(_parent.Items[0].Type);
    }
    #endregion

    #region Keywords
    public void AddSpecialTooltips() {
        Keyword tooltip = new Keyword(Language.GetTextValue("Mods.Aequus.Items.Keywords.Brittle.DisplayName"), Color.Lerp(Color.Blue, Color.White, 0.75f), _parent.Items[^1].Type);
        
        if (HitsLeft > 0) {
            tooltip.AddLine(Language.GetTextValue("Mods.Aequus.Items.Keywords.Brittle.Description", HitsLeft));
        }
        tooltip.AddLine(Language.GetTextValue("Mods.Aequus.Items.CommonTooltips.GoblinTinkererRepair", HitsLeft));

        KeywordSystem.Tooltips.Add(tooltip);
    }
    #endregion

    #region Drawing
    // Draw the durability bar.

    public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        float percentage = DamagePercent;

        // Don't draw if the item is not in a proper Inventory Slot (inventory, equip menu, chest, etc) or if the percentage bar is 100%
        if (!InventoryUI.ContextsInv.Contains(CurrentSlot.Instance.Context) || percentage >= 1f) {
            return;
        }

        const int DurabilityUIFrames = 21;

        Texture2D durabilityTexture = AequusTextures.Durability;
        int durabilityFrameY = Math.Clamp((int)(DurabilityUIFrames * (1f - percentage)), 0, DurabilityUIFrames - 1);

        Rectangle durabilityFrame = durabilityTexture.Frame(verticalFrames: DurabilityUIFrames, frameY: durabilityFrameY);

        spriteBatch.Draw(durabilityTexture, position, durabilityFrame, Color.White, 0f, durabilityFrame.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f); ;
    }
    #endregion

    #region Save & Load
    public override void SaveData(TagCompound tag) {
        // Don't save if the hits are equal to the default amount of hits for this item
        if (HitsLeft != _parent.HitsNeededForTier(Tier)) {
            tag["HitsLeft"] = HitsLeft;
        }
    }

    public override void LoadData(TagCompound tag) {
        if (tag.TryGet("HitsLeft", out int hitsLeft)) {
            HitsLeft = hitsLeft;
        }
    }
    #endregion
}

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class BrittleEquipTexture : EquipTexture, IEquipTextureDraw {
    private readonly string _texturePath;
    private Asset<Texture2D> _texture;

    public BrittleEquipTexture(string texturePath) {
        _texturePath = texturePath;
    }

    // Custom rendering which shakes the equip sprite and allows for tall sprite rendering aswell.
    public void Draw(ref PlayerDrawSet drawInfo, Vector2 position, Rectangle frame, Color color, float rotation, Vector2 origin, SpriteEffects effects, int shader) {
        Texture2D texture = (_texture ??= ModContent.Request<Texture2D>(_texturePath)).Value;

        int frameY = frame.Y / frame.Height;

        Rectangle realFrame = texture.Frame(verticalFrames: 20, frameY: frameY);
        int difference = realFrame.Height - frame.Height;

        position += drawInfo.drawPlayer.GetModPlayer<BrittleArmorPlayer>().shake;

        drawInfo.Draw(texture, position - new Vector2(0f, difference - 2f), realFrame, color, rotation, origin, 1f, effects, shader);
    }
}

[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class BrittleArmorPlayer : ModPlayer {
    internal List<BrittleArmorItem> brittlePieces = new();
    public Vector2 shake;
    public int shakeIntensity;

    public override void ResetEffects() {
        brittlePieces?.Clear();
    }

    public override void PreUpdate() {
        if (shakeIntensity > 0) {
            shakeIntensity--;
            shake = Main.rand.NextVector2Square(-shakeIntensity, shakeIntensity) * 0.4f;
        }
    }

    public override void OnHurt(Player.HurtInfo info) {
        for (int i = 0; i < brittlePieces.Count; i++) {
            BrittleArmorItem piece = brittlePieces[i];
            piece.HitsLeft--;
            shakeIntensity = 12;

            // Break item once it reaches the amount of hits for the next tier.
            if (piece.HitsLeft <= piece._parent.HitsNeededForTier(piece.Tier + 1)) {
                // Remove the Mod Item's reference immediately.
                brittlePieces.RemoveAt(i);
                i--;

                // Run break method (transforms the item, does effects, etc.)
                piece._parent.OnBreak(Player, piece);
            }
        }
    }
}
