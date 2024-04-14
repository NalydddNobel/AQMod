using Aequus.Common.Buffs;
using Aequus.Common.Items.Components;
using Aequus.Common.Systems;
using Aequus.Core.IO;
using Aequus.Core.UI;
using Aequus.DataSets;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace Aequus.Content.Potions.PotionCanteen;

public abstract class TemplateCanteen : ModItem, IOnShimmer, IHoverSlot {
    public abstract int Rarity { get; }
    public abstract int Value { get; }
    public abstract int PotionsContained { get; }
    public abstract int PotionRecipeRequirement { get; }

    [CloneByReference]
    public Buff[] Buffs { get; private set; }

    public ModItem EmptyCanteenItem { get; private set; }

    [CloneByReference]
    public LocalizedText AltName { get; private set; }

    private double _glowAnimation;
    private int _warningAnimation;

    protected override bool CloneNewInstances => true;

    /// <summary>Determines if this canteen has any buffs at all.</summary>
    public bool HasBuffs() {
        return Buffs != null && Buffs.Any(e => e.BuffId > 0);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        for (int i = 0; i < Buffs.Length; i++) {
            if (Main.myPlayer == player.whoAmI) {
                // Since these buffs are constantly refreshed, prevent right clicking.
                BuffUI.DisableRightClick.Add(Buffs[i].BuffId);
            }

            // Add the buff, and do it quietly since this should run on all clients anyways
            player.AddBuff(Buffs[i].BuffId, 1, quiet: true);
        }
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        foreach (var t in tooltips) {
            if (t.Name != "Tooltip0") {
                continue;
            }

            bool vanillaLine = true;
            bool holdingShift = !Main.keyState.IsKeyDown(Keys.LeftShift) && !Main.keyState.IsKeyDown(Keys.RightShift);
            for (int k = 0; k < Buffs.Length; k++) {
                if (!holdingShift && Buffs[k].ItemId > 0) {
                    ItemTooltip tooltip = LanguageDatabase.GetTooltip(Buffs[k].ItemId);
                    if (tooltip.Lines > 0) {

                        if (vanillaLine) {
                            t.Text = "";
                            vanillaLine = false;
                        }

                        for (int i = 0; i < tooltip.Lines; i++) {
                            string line = tooltip.GetLine(i);
                            if (string.IsNullOrEmpty(line)) {
                                continue;
                            }

                            if (!string.IsNullOrEmpty(t.Text)) {
                                t.Text += '\n';
                            }
                            t.Text += tooltip.GetLine(i);
                        }

                        continue;
                    }
                }

                if (Buffs[k].BuffId > 0) {
                    if (vanillaLine) {
                        t.Text = "";
                        vanillaLine = false;
                    }
                    else if (!string.IsNullOrEmpty(t.Text)) {
                        t.Text += '\n';
                    }

                    t.Text += LanguageDatabase.GetBuffDescription(Buffs[k].BuffId);
                }
            }
        }
    }

    public bool OnShimmer() {
        // Remove the name override,
        // so it is not inherited by the empty canteen
        Item.ClearNameOverride();

        if (!HasBuffs()) {
            return true;
        }

        // Remove buffs
        Buffs = null;
        InitializeBuffs();
        Item.shimmered = true;
        ShimmerSystem.GetShimmeredEffects(Item);
        return false;
    }

    public override void SaveData(TagCompound tag) {
        if (!HasBuffs()) {
            return;
        }

        for (int i = 0; i < Buffs.Length; i++) {
            IDLoader<BuffID>.SaveToTag(tag, $"Buff{i}", Buffs[i].BuffId);
            IDLoader<ItemID>.SaveToTag(tag, $"Item{i}", Buffs[i].ItemId);
        }
    }

    public override void LoadData(TagCompound tag) {
        InitializeBuffs();

        if (tag.ContainsKey("BuffID") && tag.ContainsKey("ItemID")) {
            Buffs[0].BuffId = IDLoader<BuffID>.LoadFromTag(tag, "BuffID");
            Buffs[0].ItemId = IDLoader<ItemID>.LoadFromTag(tag, "ItemID");
        }
        else {
            for (int i = 0; i < Buffs.Length; i++) {
                Buffs[i].BuffId = IDLoader<BuffID>.LoadFromTag(tag, $"Buff{i}", 0);
                Buffs[i].ItemId = IDLoader<ItemID>.LoadFromTag(tag, $"Item{i}", 0);
            }
        }

        SetPotionDefaults();
    }

    public override void NetSend(BinaryWriter writer) {
        InitializeBuffs();

        // PotionsContained is the total amount of buffs, and should be the same across clients.
        // If it isn't going to be consistent, the canteen should be manually synced instead of using this implementation.
        int count = PotionsContained;

        for (int i = 0; i < count; i++) {
            Buff buff = Buffs[i];

            if (buff.BuffId == 0) {
                writer.Write(0); // Signals a quicker end to the array
                break;
            }

            writer.Write(buff.BuffId);
            writer.Write(buff.ItemId);
        }
    }

    public override void NetReceive(BinaryReader reader) {
        InitializeBuffs();

        int count = PotionsContained;

        for (int i = 0; i < count; i++) {
            Buff buff = Buffs[i];

            int type = reader.ReadInt32();
            if (type == 0) {
                // End of array signal.
                break;
            }

            Buffs[i].BuffId = type;
            Buffs[i].ItemId = reader.ReadInt32();
        }
    }

    private bool IsValidPotion(Item item) {
        return item.buffType > 0 && item.buffTime > 0 && item.consumable && item.maxStack >= 30 && !Buffs.Any(b => b.BuffId == item.buffType) && !BuffDataSet.CannotChangeDuration.Contains(item.buffType) && ItemDataSet.Potions.Contains(item.type);
    }

    private static void EatRightClick() {
        Main.mouseRightRelease = false;
        Main.stackSplit = 180;
    }

    public bool HoverSlot(Item[] inventory, int context, int slot) {
        int aContext = Math.Abs(context);
        if (!InventoryUI.ContextsInv.Contains(context)) {
            return false;
        }

        Item heldItem = Main.mouseItem;

        if (_warningAnimation > 0 || !Main.mouseRight || !Main.mouseRightRelease || heldItem == null || heldItem.IsAir) {
            return false;
        }

        InitializeBuffs();

        int consumePotions = PotionRecipeRequirement;

        if (!IsValidPotion(heldItem) || heldItem.stack < consumePotions) {

            // Don't do warning animation on acc slots if right clicking with another accessory
            if ((aContext == ItemSlot.Context.EquipAccessory || aContext == ItemSlot.Context.EquipAccessoryVanity) && heldItem.accessory) {
                return false;
            }

            EatRightClick();

            _warningAnimation = 80;
            SoundEngine.PlaySound(AequusSounds.CanteenBuzzer with { Volume = 0.5f });
            return false;
        }

        EatRightClick();

        int i = 0;
        // Iterate through each buff and check if it has an id of 0
        for (; i < Buffs.Length && Buffs[i].BuffId != 0; i++) { }

        // If there's no empty buffs left
        if (i == Buffs.Length) {
            // We're going to push this new buff into the last index
            i--;
            // Copy everything forward by 1 index
            for (int j = 0; j < Buffs.Length - 1; j++) {
                Buffs[j] = Buffs[j + 1];
            }
        }

        Buffs[i].BuffId = heldItem.buffType;
        Buffs[i].ItemId = heldItem.type;

        heldItem.stack -= consumePotions;
        if (heldItem.stack <= 0) {
            heldItem.TurnToAir();
        }

        _potionTCommonColor = null;
        SetPotionDefaults();
        Item.NetStateChanged();

        SoundEngine.PlaySound(AequusSounds.CanteenUse with { Volume = 0.75f, PitchVariance = 0.2f });

        return true;
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        if (!InventoryUI.ContextsInv.Contains(CurrentSlot.Instance.Context)) {
            return true;
        }

        if (Main.mouseItem != null && !Main.mouseItem.IsAir && IsValidPotion(Main.mouseItem)) {
            if (_glowAnimation < 1.0) {
                _glowAnimation += Main._drawInterfaceGameTime.ElapsedGameTime.TotalSeconds * 9f;
                if (_glowAnimation > 1.0) {
                    _glowAnimation = 1.0;
                }
            }
        }
        else if (_glowAnimation > 0.0) {
            _glowAnimation -= Main._drawInterfaceGameTime.ElapsedGameTime.TotalSeconds * 9f;
            if (_glowAnimation < 0.0) {
                _glowAnimation = 0.0;
            }
        }

        if (_glowAnimation > 0f) {
            Texture2D texture = TextureAssets.InventoryBack15.Value;
            spriteBatch.Draw(texture, position, null, Color.White with { A = 140 } * (float)_glowAnimation * 0.8f, 0f, texture.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);
        }

        if (_warningAnimation > 0) {
            _warningAnimation--;

            if (_warningAnimation % 20 > 10) {
                Texture2D texture = TextureAssets.InventoryBack13.Value;
                spriteBatch.Draw(texture, position, null, Color.Red, 0f, texture.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);
            }
        }

        return true;
    }

    #region TCommonColor
    [CloneByReference]
    protected Color[] _potionTCommonColor;

    protected Color GetPotionTCommonColor() {
        if (!HasBuffs()) {
            return Color.White;
        }

        // Initialize TCommonColor if they are null.
        if (_potionTCommonColor == null && HasBuffs()) {
            List<Color> TCommonColor = new List<Color>();

            for (int i = 0; i < Buffs.Length; i++) {
                if (Buffs[i].ItemId <= 0) {
                    continue;
                }

                Color[] drinkTCommonColor = ItemSets.DrinkParticleColors[Buffs[i].ItemId];
                if (drinkTCommonColor != null) {
                    TCommonColor.AddRange(drinkTCommonColor);
                }
            }

            _potionTCommonColor = TCommonColor.ToArray();
        }

        Color colorResult = Color.White;
        if (_potionTCommonColor != null && _potionTCommonColor.Length > 0) {
            float time = Main.GlobalTimeWrappedHourly * Buffs.Length;
            if (HasBuffs()) {
                for (int i = 0; i < Buffs.Length; i++) {
                    time += Buffs[i].BuffId;
                }
            }
            colorResult = Color.Lerp(_potionTCommonColor[(int)time % _potionTCommonColor.Length], _potionTCommonColor[(int)(time + 1) % _potionTCommonColor.Length], time % 1f);
        }

        return colorResult * 1.1f;
    }
    #endregion

    #region Initialization
    public override void SetStaticDefaults() {
        AltName = this.GetLocalization("DisplayNameAlt");
    }

    public sealed override void SetDefaults() {
        Item.DefaultToAccessory(20, 20);
        Item.value = Rarity;
        Item.rare = Value;

        SetPotionDefaults();
    }

    public void SetPotionDefaults() {
        InitializeBuffs();

        Item.buffType = Buffs[0].BuffId;
        Item.rare = ItemRarityID.Orange;
        for (int i = 0; i < Buffs.Length; i++) {
            if (Buffs[i].ItemId <= 0) {
                continue;
            }
            Item referenceItem = ContentSamples.ItemsByType[Buffs[i].ItemId];

            if (Item.rare < referenceItem.rare) {
                Item.rare = referenceItem.rare;
            }
        }

        Item.Prefix(Item.prefix);
        Item.ClearNameOverride();
        if (!Main.dedServ && AltName != null) {
            Item.SetNameOverride(GetName(Item.AffixName()));
        }
    }

    public void InitializeBuffs() {
        if (Buffs == null) {
            Buffs = new Buff[PotionsContained];
        }
        else {
            int amt = Math.Min(Buffs.Length, PotionsContained);

            Buff[] entries = new Buff[PotionsContained];
            for (int i = 0; i < amt; i++) {
                entries[i] = Buffs[i];
            }

            Buffs = entries;
        }
    }

    public string GetName(string originalName) {
        if (!HasBuffs()) {
            return originalName;
        }

        string buffText = AltName.Format(string.Join(", ", GetBuffNames()));

        return originalName.Replace(LanguageDatabase.GetItemNameValue(Type), buffText);

        IEnumerable<string> GetBuffNames() {
            for (int i = 0; i < Buffs.Length; i++) {
                if (Buffs[i].BuffId > 0) {
                    yield return LanguageDatabase.GetBuffName(Buffs[i].BuffId);
                }
            }
        }
    }

    /*
    public override void AddRecipes() {
        int[] validPotions = ItemSets.Potions.Where(i => i.ValidEntry && ContentSamples.ItemsByType[i.Id].buffType != BuffID.Lucky).Select(e => e.Id).ToArray();
        int potionCount = PotionsContained;

        // Examples use an array of length 3.
        // Create an array which counts up (0, 1, 2)
        int[] indices = ExtendArray.CreateArray((i) => i, potionCount);

        RecursiveRecipeCreation(0, 0);

        void RecursiveRecipeCreation(int i, int startValue) {
            for (int k = startValue; k < validPotions.Length; ++k) {
                indices[i] = k;

                if (i < indices.Length - 1) {
                    RecursiveRecipeCreation(i + 1, k + 1);
                }
                else {
                    CreateCombinationRecipe();
                }
            }
        }

        void CreateCombinationRecipe() {
            Recipe r = CreateRecipe();

            r.AddIngredient(EmptyCanteenItem.Type);

            TemplateCanteen canteen = r.createItem.ModItem as TemplateCanteen;
            canteen.InitializeBuffs();

            for (int i = 0; i < indices.Length; i++) {
                int potion = validPotions[indices[i]];

                r.AddIngredient(potion, PotionRecipeRequirement);

                canteen.Buffs[i].BuffId = ContentSamples.ItemsByType[potion].buffType;
                canteen.Buffs[i].ItemId = potion;
            }

            canteen.SetPotionDefaults();

            r.Register();
            r.DisableDecraft();
        }
    }
    */
    #endregion

    public record struct Buff(int ItemId, int BuffId);
}
