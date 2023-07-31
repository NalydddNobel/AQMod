using Aequus.Common.UI;
using Aequus.Common.UI.Elements;
using Aequus.Items;
using Aequus.Items.Tools;
using Aequus.NPCs.Town.ExporterNPC;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.Content.UI.GrabBagReroll;

public class RerollUIState : AequusUIState {
    public static HashSet<int> RerollBlacklist = new();
    public static Dictionary<int, Func<Player, bool>> RerollCondition = new();

    public AequusPlaceableItemSlotElement itemSlot;
    public RerollSlot[] slots;
    public UIImageButton openButton;
    public UIImageButton rerollButton;

    public static bool RerollCondition_Blacklist(Player player) {
        return false;
    }

    public static int GetRerollPrice(Item item) {
        long price = item.value / 2;
        if (ItemID.Sets.BossBag[item.type]) {
            price = Math.Max(price, Item.buyPrice(gold: 5));
            for (int i = 0; i < NPCLoader.NPCCount; i++) {
                if (!Helper.DropsItem(i, item.type)) {
                    continue;
                }

                price = Math.Max((long)ContentSamples.NpcsByNetId[i].value * 2, price);
                break;
            }
        }

        price = Math.Max(price, Item.buyPrice(silver: 50));
        return (int)(price / 50 * 50);
    }

    public override void Load(Mod mod) {
    }

    public override void Unload() {
        RerollCondition.Clear();
        RerollBlacklist.Clear();
    }

    public int GetSlotAmount(Player player) {
        return 3;
    }

    private void SetupRollMachine() {
        int amount = GetSlotAmount(Main.LocalPlayer);
        slots = new RerollSlot[amount];
        float padding = 1f / (amount * amount);
        for (int i = 0; i < amount; i++) {
            slots[i] = new RerollSlot();
            slots[i].Width.Set(54, 0f);
            slots[i].Height.Set(120, 0f);
            slots[i].Left.Set(-slots[i].Width.Pixels / (amount * amount), 1 / (float)amount * i + padding);
            slots[i].Top.Set(-140f, 1f);
            Append(slots[i]);
        }
    }

    #region Item Slot and Buttons
    private void SetupItemSlot() {

        itemSlot = new(TextureAssets.InventoryBack.Value);
        itemSlot.Width.Set(54f, 0f);
        itemSlot.Height.Set(54f, 0f);
        itemSlot.Top.Set(10f, 0f);
        itemSlot.HAlign = 0.5f;
        itemSlot.CanPlaceInSlot = ItemSlot_CanPlaceInSlot;
        itemSlot.TakeOutOfSlot = ItemSlot_TakeOutOfSlot;
        itemSlot.OnSlotSwap = ItemSlot_OnSlotSwap;
        itemSlot.StackMustBe1 = true;
        itemSlot.canHover = true;
        itemSlot.showItemTooltipOnHover = true;
        Append(itemSlot);

        openButton = new UIImageButton(AequusTextures.OpenButton.Asset);
        openButton.Left.Set(40f, 0f);
        openButton.Top.Set(14f, 0f);
        openButton.OnUpdate += Open_OnUpdate;
        openButton.OnLeftClick += OpenButton_OnClick;
        Append(openButton);

        rerollButton = new UIImageButton(AequusTextures.RerollButton.Asset);
        rerollButton.Left.Set(openButton.Left.Pixels + 60f, openButton.Left.Percent);
        rerollButton.Top = openButton.Top;
        rerollButton.OnUpdate += Reroll_OnUpdate;
        rerollButton.OnLeftClick += RerollButton_OnClick;
        Append(rerollButton);
    }

    private void ItemSlot_OnSlotSwap(Item slotItem, Item incomingItem) {
        for (int i = 0; i < slots.Length; i++) {
            for (int j = 0; j < slots[i].workingItems.Length; j++) {
                slots[i].workingItems[j] = null;
            }
        }
    }
    private bool ItemSlot_TakeOutOfSlot(Item incomingItem) {

        for (int i = 0; i < slots.Length; i++) {
            if (slots[i].rollSpeed > 0f) {
                return false;
            }
        }

        return true;
    }

    private bool ItemSlot_CanPlaceInSlot(Item incomingItem) {

        if (incomingItem.type == ItemID.LockBox) {
            return Main.LocalPlayer.HasItemInInvOrVoidBag(ItemID.GoldenKey) || Main.LocalPlayer.HasItemInInvOrVoidBag(ModContent.ItemType<SkeletonKey>());
        }
        else if (incomingItem.type == ItemID.ObsidianLockbox) {
            return Main.LocalPlayer.HasItemInInvOrVoidBag(ItemID.ShadowKey);
        }
        else if (incomingItem.type >= ItemID.Count && !ItemLoader.CanRightClick(incomingItem)) {
            return false;
        }
        if (Main.ItemDropsDB.GetRulesForItemID(incomingItem.type).Count < 0) {
            return false;
        }
        for (int i = 0; i < slots.Length; i++) {
            if (slots[i].rollSpeed > 0f) {
                return false;
            }
        }

        return true;
    }

    private List<Item>[] CreateItemRollLists(int amt, int item, List<IItemDropRule> rules) {
        var lists = new List<Item>[amt];

        var tests = new List<AequusItem.NewItem>[50];
        DropAttemptInfo dropAttemptInfo = new() {
            item = item,
            player = Main.LocalPlayer,
            IsExpertMode = Main.expertMode,
            IsMasterMode = Main.masterMode,
            rng = Main.rand,
            IsInSimulation = true,
        };
        AequusItem.EnablePreventItemDrops = true;
        AequusItem.EnableCacheItemDrops = true;
        for (int i = 0; i < tests.Length; i++) {
            AequusItem.CachedItemDrops.Clear();
            try {
                Main.ItemDropSolver.TryDropping(dropAttemptInfo);
                tests[i] = new(AequusItem.CachedItemDrops);
            }
            catch (Exception ex) {
                Aequus.Instance.Logger.Error($"{ex.Message}\n{ex.StackTrace}");
            }
        }
        AequusItem.CachedItemDrops.Clear();
        AequusItem.EnableCacheItemDrops = false;
        AequusItem.EnablePreventItemDrops = false;

        Dictionary<int, int> itemCounts = new();
        for (int i = 0; i < tests.Length; i++) {
            foreach (var d in tests[i]) {
                if (ContentSamples.ItemsByType[d.Type].maxStack != 1) {
                    continue;
                }
                if (itemCounts.ContainsKey(d.Type)) {
                    itemCounts[d.Type]++;
                }
                else {
                    itemCounts[d.Type] = 1;
                }
            }
        }

        if (itemCounts.Count == 0) {
            return lists;
        }

        List<Item> itemsToRoll = new();
        foreach (var data in itemCounts) {
            if (data.Value == tests.Length) {
                continue;
            }
            itemsToRoll.Add(new(data.Key));
        }
        int amount = Math.Min(itemsToRoll.Count, amt);
        for (int i = 0; i < amount; i++) {
            lists[i] = new(itemsToRoll);
        }
        return lists;
    }

    private void RerollButton_OnClick(UIMouseEvent evt, UIElement listeningElement) {

        SoundEngine.PlaySound(SoundID.MenuTick);
        for (int i = 0; i < slots.Length; i++) {
            if (slots[i].playRollAnimation) {
                slots[i].EndRoll();
                return;
            }
        }

        if (!itemSlot.HasItem) {
            return;
        }

        if (!Main.LocalPlayer.BuyItem(GetRerollPrice(itemSlot.item))) {
            SoundEngine.PlaySound(SoundID.MenuClose);
            return;
        }

        SoundEngine.PlaySound(SoundID.Coins);

        var lists = CreateItemRollLists(slots.Length, itemSlot.item.type, Main.ItemDropsDB.GetRulesForItemID(itemSlot.item.type));
        if (lists.Length <= 0) {
            return;
        }

        openButton.SetVisibility(0.5f, 0.4f);
        for (int i = 0; i < slots.Length; i++) {
            if (lists[i] != null) {
                slots[i].BeginRoll(lists[i]);
            }
            else {
                slots[i].disable = true;
            }
        }
    }
    private void Reroll_OnUpdate(UIElement affectedElement) {
        if (affectedElement.IsMouseHovering) {

            for (int i = 0; i < slots.Length; i++) {
                if (slots[i].playRollAnimation) {
                    Main.instance.MouseText($"Stop slot #{i + 1}");
                    return;
                }
            }


            if (itemSlot.HasItem) {
                Main.instance.MouseText($"Roll items");
            }
            else {
                Main.instance.MouseText("Place an item in the slot to roll items");
            }
        }
    }

    private bool CanOpen(Item item) {
        if (item.type == ItemID.LockBox && !Main.LocalPlayer.Aequus().HasSkeletonKey) {
            return Main.LocalPlayer.ConsumeItemInInvOrVoidBag(ItemID.GoldenKey);
        }

        return true;
    }

    private void OpenButton_OnClick(UIMouseEvent evt, UIElement listeningElement) {
        SoundEngine.PlaySound(SoundID.MenuTick);

        for (int i = 0; i < slots.Length; i++) {
            if (slots[i].rollSpeed > 0f) {
                return;
            }
        }

        if (!itemSlot.HasItem || !CanOpen(itemSlot.item)) {
            return;
        }

        AequusItem.EnableCacheItemDrops = true;
        var player = Main.LocalPlayer;
        try {
            DropAttemptInfo dropAttemptInfo = new() {
                player = player,
                item = itemSlot.item.type,
                IsExpertMode = true,
                IsMasterMode = true,
                rng = Main.rand,
            };
            Main.ItemDropSolver.TryDropping(dropAttemptInfo);
            itemSlot.item.TurnToAir();
        }
        catch {
        }
        AequusItem.EnableCacheItemDrops = false;

        HashSet<int> quickLookup = new();
        foreach (var cache in AequusItem.CachedItemDrops) {
            quickLookup.Add(cache.Type);
        }

        for (int i = 0; i < slots.Length; i++) {

            if (slots[i].workingItems[1] == null) {
                continue;
            }

            if (quickLookup.Contains(slots[i].workingItems[1].type)) {
                continue;
            }

            player.QuickSpawnItem(new EntitySource_Misc("Aequus: Exporter Reroll Reward"), slots[i].workingItems[1].type);
            quickLookup.Add(slots[i].workingItems[1].type);
        }
        AequusItem.CachedItemDrops.Clear();
    }
    private void Open_OnUpdate(UIElement affectedElement) {
        if (affectedElement.IsMouseHovering) {

            for (int i = 0; i < slots.Length; i++) {
                if (slots[i].playRollAnimation) {
                    openButton.SetVisibility(0.5f, 0.4f);
                    return;
                }
            }

            openButton.SetVisibility(1f, 0.4f);

            if (itemSlot.HasItem) {
                Main.instance.MouseText("Open with extra guaranteed drops");
            }
            else {
                Main.instance.MouseText("Place an item in the slot to open it with extra guaranteed drops");
            }
        }
    }
    #endregion

    public override void OnInitialize() {
        OverrideSamplerState = SamplerState.LinearClamp;

        Width.Set(300, 0.05f);
        Height.Set(200, 0.05f);
        Top.Set(100, 0f);
        HAlign = 0.5f;

        var uiPanel = new UIPanel {
            BackgroundColor = new Color(68, 99, 164) * 0.825f
        };
        uiPanel.Width.Set(0, 1f);
        uiPanel.Height.Set(0, 1f);
        Append(uiPanel);

        SetupRollMachine();
        SetupItemSlot();

        var uiText = new UIText($"* {TextHelper.GetText("Chat.Exporter.SlotMachineHint")} *") {
            HAlign = 0.5f,
            TextColor = Color.LightBlue * 1.25f,
        };
        uiText.Top.Set(80f, 0f);
        Append(uiText);
    }

    public override void OnDeactivate() {

        if (itemSlot.HasItem) {
            Main.LocalPlayer.QuickSpawnItem(new EntitySource_Misc("Aequus: Exporter Reroll UI"), itemSlot.item, itemSlot.item.stack);
        }
    }

    public override void Update(GameTime gameTime) {
        if (NotTalkingTo<Exporter>()) {
            Aequus.UserInterface.SetState(null);
            return;
        }
        if (IsMouseHovering) {
            Main.LocalPlayer.mouseInterface = true;
        }
        if (!itemSlot.HasItem) {
            for (int i = 0; i < slots.Length; i++) {
                for (int j = 0; j < slots[i].workingItems.Length; j++) {
                    slots[i].workingItems[j] = null;
                }
            }
        }
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch) {
        base.Draw(spriteBatch);

        var dimensions = GetDimensions();
        var font = FontAssets.MouseText.Value;

        ItemSlot.DrawSavings(spriteBatch, dimensions.X + dimensions.Width + 10f, dimensions.Y - 40f);
        if (itemSlot.HasItem) {
            long cost = GetRerollPrice(itemSlot.item);
            string costText = "Cost: " + TextHelper.PriceTextColored(cost, "");

            ChatManager.DrawColorCodedStringWithShadow(
                spriteBatch,
                font,
                costText,
                new Vector2(dimensions.X + dimensions.Width / 2f + 35f, dimensions.Y + 10f),
                Color.White,
                Color.Black,
                0f,
                Vector2.Zero,
                Vector2.One * 0.9f
            );
        }
    }

    public override void ConsumePlayerControls(Player player) {
        if (player.controlInv) {
            SoundEngine.PlaySound(SoundID.MenuClose);
            player.releaseInventory = false;
            player.SetTalkNPC(-1);
            Aequus.UserInterface.SetState(null);
        }
    }

    public override int GetLayerIndex(List<GameInterfaceLayer> layers) {
        int index = layers.FindIndex((l) => l.Name.Equals(AequusUI.InterfaceLayers.Inventory_28));
        if (index == -1)
            return -1;
        return index + 1;
    }

    public override bool ModifyInterfaceLayers(List<GameInterfaceLayer> layers, ref InterfaceScaleType scaleType) {
        DisableAnnoyingInventoryLayeringStuff(layers);
        return true;
    }
}