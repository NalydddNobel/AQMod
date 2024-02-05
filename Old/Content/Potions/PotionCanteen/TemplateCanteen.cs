using Aequus.Common.Buffs;
using Aequus.Common.Items.Components;
using Aequus.Content.DataSets;
using Aequus.Core.ContentGeneration;
using Aequus.Core.DataSets;
using Aequus.Core.IO;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace Aequus.Old.Content.Potions.PotionCanteen;

public abstract class TemplateCanteen : ModItem, IOnShimmer {
    public abstract int Rarity { get; }
    public abstract int Value { get; }
    public abstract int PotionsContained { get; }
    public abstract int PotionRecipeRequirement { get; }

    [CloneByReference]
    public Buff[] Buffs { get; private set; }

    public ModItem EmptyCanteenItem { get; private set; }

    [CloneByReference]
    public LocalizedText AltName { get; private set; }

    /// <summary>Determines if this canteen has any buffs at all.</summary>
    public bool HasBuffs => Buffs != null && Buffs.Any(e => e.BuffId > 0);

    protected override bool CloneNewInstances => true;

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
                    ItemTooltip tooltip = Lang.GetTooltip(Buffs[k].ItemId);
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

                if (vanillaLine) {
                    t.Text = "";
                    vanillaLine = false;
                }
                else if (!string.IsNullOrEmpty(t.Text)) {
                    t.Text += '\n';
                }

                t.Text += Lang.GetBuffDescription(Buffs[k].BuffId);
            }
        }
    }

    public void OnShimmer() {
        // Remove the name override,
        // so it is not inherited by the empty canteen
        Item.ClearNameOverride();
    }

    public override void SaveData(TagCompound tag) {
        if (!HasBuffs) {
            return;
        }

        for (int i = 0; i < Buffs.Length; i++) {
            IDLoader<BuffID>.SaveId(tag, $"Buff{i}", Buffs[i].BuffId);
            IDLoader<ItemID>.SaveId(tag, $"Item{i}", Buffs[i].ItemId);
        }
    }

    public override void LoadData(TagCompound tag) {
        InitializeBuffs();

        if (tag.ContainsKey("BuffID") && tag.ContainsKey("ItemID")) {
            Buffs[0].BuffId = IDLoader<BuffID>.LoadId(tag, "BuffID");
            Buffs[0].ItemId = IDLoader<ItemID>.LoadId(tag, "ItemID");
        }
        else {
            for (int i = 0; i < Buffs.Length; i++) {
                Buffs[i].BuffId = IDLoader<BuffID>.LoadId(tag, $"Buff{i}", 0);
                Buffs[i].ItemId = IDLoader<ItemID>.LoadId(tag, $"Item{i}", 0);
            }
        }

        SetPotionDefaults();
    }

    #region Colors
    [CloneByReference]
    protected Color[] _potionColors;

    protected Color GetPotionColors() {
        if (!HasBuffs) {
            return Color.White;
        }

        // Initialize colors if they are null.
        if (_potionColors == null && HasBuffs) {
            List<Color> colors = new List<Color>();

            for (int i = 0; i < Buffs.Length; i++) {
                if (Buffs[i].ItemId <= 0) {
                    continue;
                }

                Color[] drinkColors = ItemID.Sets.DrinkParticleColors[Buffs[i].ItemId];
                if (drinkColors != null) {
                    colors.AddRange(drinkColors);
                }
            }

            _potionColors = colors.ToArray();
        }

        Color colorResult = Color.White;
        if (_potionColors != null && _potionColors.Length > 0) {
            float time = Main.GlobalTimeWrappedHourly * Buffs.Length;
            if (HasBuffs) {
                for (int i = 0; i < Buffs.Length; i++) {
                    time += Buffs[i].BuffId;
                }
            }
            colorResult = Color.Lerp(_potionColors[(int)time % _potionColors.Length], _potionColors[(int)(time + 1) % _potionColors.Length], time % 1f);
        }

        return colorResult * 1.1f;
    }
    #endregion

    #region Initialization
    public sealed override void Load() {
        EmptyCanteenItem = new EmptyCanteen(this);

        Mod.AddContent(EmptyCanteenItem);
    }

    public override void SetStaticDefaults() {
        AltName = this.GetLocalization("DisplayNameAlt");
        ItemID.Sets.ShimmerTransformToItem[Type] = EmptyCanteenItem.Type;
        ContentSamples.CreativeResearchItemPersistentIdOverride[EmptyCanteenItem.Type] = Type;
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
        if (!HasBuffs) {
            return originalName;
        }

        string buffText = AltName.Format(string.Join(", ", GetBuffNames()));

        return originalName.Replace(Lang.GetItemNameValue(Type), buffText);

        IEnumerable<string> GetBuffNames() {
            for (int i = 0; i < Buffs.Length; i++) {
                if (Buffs[i].BuffId > 0) {
                    yield return Lang.GetBuffName(Buffs[i].BuffId);
                }
            }
        }
    }

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
    #endregion

    public record struct Buff(int ItemId, int BuffId);

    private class EmptyCanteen : InstancedModItem {
        private readonly TemplateCanteen _parent;

        public EmptyCanteen(TemplateCanteen canteen) : base(canteen.Name + "Empty", canteen.Texture + "Empty") {
            _parent = canteen;
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory();
            Item.rare = _parent.Rarity;
            Item.value = _parent.Value;
        }
    }
}
