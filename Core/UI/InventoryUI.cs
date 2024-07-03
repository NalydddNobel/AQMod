using Aequu2.Content.Backpacks;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Gamepad;

namespace Aequu2.Core.UI;

public class InventoryUI : ModSystem {
    public static int CoinsAmmoOffsetX { get; internal set; }
    public static int RightsideButtonsOffsetY { get; internal set; }
    public static int BottomInventoryY { get; set; } = 260;
    public static int LeftInventoryPosition { get; set; } = 20;

    /// <summary>Contains Item Slot Contexts which are usually related to normal inventory slots. Check the current slot context to determine drawing item status UI like cooldowns and etc.</summary>
    public static readonly HashSet<int> ContextsInv = new() {
        ItemSlot.Context.EquipArmor,
        ItemSlot.Context.EquipArmorVanity,
        ItemSlot.Context.EquipAccessory,
        ItemSlot.Context.ModdedAccessorySlot,
        ItemSlot.Context.EquipAccessoryVanity,
        ItemSlot.Context.ModdedVanityAccessorySlot,
        ItemSlot.Context.HotbarItem,
        ItemSlot.Context.InventoryItem,
        ItemSlot.Context.BankItem,
        ItemSlot.Context.ChestItem,
        ItemSlot.Context.VoidItem,
    };

    public override void Load() {
        IL_Main.DrawInventory += IL_Main_DrawInventory;
        IL_Main.DrawBestiaryIcon += IL_Main_DrawEmoteAndBestiaryButton;
        IL_Main.DrawEmoteBubblesButton += IL_Main_DrawEmoteAndBestiaryButton;
    }

    public override void UpdateUI(GameTime gameTime) {
        if (!Main.LocalPlayer.TryGetModPlayer(out BackpackPlayer Aequu2Player)) {
            return;
        }
        BackpackLoader.AnimateBackpacks(Aequu2Player.backpacks, out int totalInventorySlots, out int activeBackpacks);

        int coinsAmmoOffsetWantedX = 0;
        if (totalInventorySlots > 0) {
            coinsAmmoOffsetWantedX = (int)(((totalInventorySlots - 1) / 5 + 1) * BackpackSlotsUI.SlotWidth * BackpackSlotsUI.InventoryScale) + BackpackSlotsUI.BackpackPadding * activeBackpacks;
        }

        if (CoinsAmmoOffsetX < coinsAmmoOffsetWantedX) {
            CoinsAmmoOffsetX = (int)MathHelper.Lerp(CoinsAmmoOffsetX, coinsAmmoOffsetWantedX, 0.33f);
            CoinsAmmoOffsetX++;
            if (CoinsAmmoOffsetX > coinsAmmoOffsetWantedX) {
                CoinsAmmoOffsetX = coinsAmmoOffsetWantedX;
            }
        }
        else if (CoinsAmmoOffsetX > coinsAmmoOffsetWantedX) {
            CoinsAmmoOffsetX = (int)MathHelper.Lerp(CoinsAmmoOffsetX, coinsAmmoOffsetWantedX, 0.1f);
            CoinsAmmoOffsetX--;
            if (CoinsAmmoOffsetX < coinsAmmoOffsetWantedX) {
                CoinsAmmoOffsetX = coinsAmmoOffsetWantedX;
            }
        }

        if (CoinsAmmoOffsetX > 0) {
            if (RightsideButtonsOffsetY < 16) {
                RightsideButtonsOffsetY++;
            }
        }
        else if (RightsideButtonsOffsetY > 0) {
            RightsideButtonsOffsetY--;
        }
    }

    #region Hooks
    private void IL_Main_DrawEmoteAndBestiaryButton(ILContext il) {
        //MonoModHooks.DumpIL(ModContent.GetInstance<Aequu2>(), il);
        var cursor = new ILCursor(il);
        if (!cursor.TryGotoNext((i) => i.MatchLdsfld(typeof(Main), nameof(Main.player)))) {
            Mod.Logger.Error($"Could not find {nameof(Main)}.{nameof(Main.player)} loading code."); return;
        }
        cursor.Index--;
        cursor.EmitDelegate<Func<int, int>>((y) => y + (Main.LocalPlayer.chest == -1 && Main.npcShop <= 0 || Main.recBigList ? RightsideButtonsOffsetY : 0));
    }

    private void MoveAchievementAdvisor(ILCursor cursor) {
        if (!cursor.TryGotoNext((i) => i.MatchCallvirt(typeof(AchievementAdvisor), nameof(AchievementAdvisor.DrawOneAchievement)))) {
            Mod.Logger.Error($"Could not find {nameof(AchievementAdvisor)}.{nameof(AchievementAdvisor.DrawOneAchievement)} virtual method."); return;
        }
        if (!cursor.TryGotoPrev((i) => i.MatchLdsfld(typeof(Main), nameof(Main.spriteBatch)))) {
            Mod.Logger.Error($"Could not find {nameof(Main)}.{nameof(Main.spriteBatch)} loading code."); return;
        }
        cursor.Index += 2;
        cursor.EmitDelegate<Func<int, int>>((x) => x + CoinsAmmoOffsetX);
    }

    private void MoveSlots(ILCursor cursor, string weirdHardcodedNameLeftover, float hardcodedTextXValue, int hardcodedSlotXValue) {
        // Find the "Coins" text
        if (!cursor.TryGotoNext((i) => i.MatchLdstr(weirdHardcodedNameLeftover))) {
            Mod.Logger.Error($"Could not find '{weirdHardcodedNameLeftover}' loaded string."); return;
        }

        // Find the number 496, a hardcoded value for rendering the Coins text
        if (!cursor.TryGotoNext((i) => i.MatchLdcR4(hardcodedTextXValue))) {
            Mod.Logger.Error($"Could not find '{weirdHardcodedNameLeftover}' text X value."); return;
        }
        cursor.Index++;
        cursor.EmitDelegate<Func<float, float>>((i) => i + CoinsAmmoOffsetX);

        // Find the number 497, another hardcoded value but this time for rendering the Coins slot
        if (!cursor.TryGotoNext((i) => i.MatchLdcI4(hardcodedSlotXValue))) {
            Mod.Logger.Error($"Could not find '{weirdHardcodedNameLeftover}' slot X value."); return;
        }
        cursor.Index++;
        cursor.EmitDelegate<Func<int, int>>((x) => x + CoinsAmmoOffsetX);
    }

    private void MoveChestIcons(ILCursor cursor) {
        if (!cursor.TryGotoNext((i) => i.MatchCall(typeof(ChestUI), nameof(ChestUI.Draw)))) {
            Mod.Logger.Error($"Could not find {nameof(Main)}.{nameof(ChestUI.Draw)} method."); return;
        }
        if (!cursor.TryGotoNext((i) => i.MatchCall(typeof(UILinkPointNavigator), nameof(UILinkPointNavigator.SetPosition)))) {
            Mod.Logger.Error($"Could not find {nameof(UILinkPointNavigator)}.{nameof(UILinkPointNavigator.SetPosition)} method."); return;
        }
        if (!cursor.TryGotoNext((i) => i.MatchLdsfld(typeof(TextureAssets), nameof(TextureAssets.ChestStack)))) {
            Mod.Logger.Error($"Could not find {nameof(TextureAssets)}.{nameof(TextureAssets.ChestStack)} loading code."); return;
        }
        if (!cursor.TryGotoNext((i) => i.MatchLdelemRef())) {
            Mod.Logger.Error($"Could not find ldelem.ref."); return;
        }
        // X
        if (!cursor.TryGotoNext((i) => i.MatchLdloc(out _))) {
            Mod.Logger.Error($"Could not find x value for chest stack drawing."); return;
        }
        // Y
        if (!cursor.TryGotoNext((i) => i.MatchLdloc(out _))) {
            Mod.Logger.Error($"Could not find y value for chest stack drawing."); return;
        }

        cursor.Index++;
        cursor.EmitDelegate<Func<int, int>>((y) => y + RightsideButtonsOffsetY);

        if (!cursor.TryGotoNext((i) => i.MatchCall(typeof(UILinkPointNavigator), nameof(UILinkPointNavigator.SetPosition)))) {
            Mod.Logger.Error($"Could not find {nameof(UILinkPointNavigator)}.{nameof(UILinkPointNavigator.SetPosition)} method."); return;
        }
        if (!cursor.TryGotoNext((i) => i.MatchLdsfld(typeof(TextureAssets), nameof(TextureAssets.InventorySort)))) {
            Mod.Logger.Error($"Could not find {nameof(TextureAssets)}.{nameof(TextureAssets.InventorySort)} loading code."); return;
        }
        if (!cursor.TryGotoNext((i) => i.MatchLdelemRef())) {
            Mod.Logger.Error($"Could not find ldelem.ref."); return;
        }
        if (!cursor.TryGotoNext((i) => i.MatchLdsfld(typeof(Main), nameof(Main.spriteBatch)))) {
            Mod.Logger.Error($"Could not find {nameof(Main)}.{nameof(Main.spriteBatch)} loading code."); return;
        }
        // Texture2D
        if (!cursor.TryGotoNext((i) => i.MatchLdloc(out _))) {
            Mod.Logger.Error($"Could not find x value for chest sort drawing."); return;
        }
        // X
        if (!cursor.TryGotoNext((i) => i.MatchLdloc(out _))) {
            Mod.Logger.Error($"Could not find y value for chest sort drawing."); return;
        }
        // Y
        if (!cursor.TryGotoNext((i) => i.MatchLdloc(out _))) {
            Mod.Logger.Error($"Could not find y value for chest sort drawing."); return;
        }
        cursor.Index++;
        cursor.EmitDelegate<Func<int, int>>((y) => y + RightsideButtonsOffsetY);
    }

    private void IL_Main_DrawInventory(ILContext il) {
        var cursor = new ILCursor(il);
        try {
            MoveAchievementAdvisor(cursor);
            MoveSlots(cursor, "Coins", 496f, 497);
            MoveSlots(cursor, "Ammo", 532f, 534);
            MoveChestIcons(cursor);
        }
        catch {
            MonoModHooks.DumpIL(ModContent.GetInstance<Aequu2>(), il);
        }
    }
    #endregion
}