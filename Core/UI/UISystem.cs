using Aequus.Common.UI;
using Aequus.Common.UI.EventBars;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Core.UI;

/// <summary>
/// This type does not get loaded/registered when loaded on a Server.
/// </summary>
public partial class UISystem : ModSystem {
    public static int BottomLeftInventoryOffset { get; set; }

    public static byte LinkClickDelay { get; set; }

    public static byte SpecialLeftClickDelay { get; set; }

    public static byte DisableItemSlotLeftClick { get; set; }

    public static ItemSlotContext Slot { get; private set; }

    public static readonly HashSet<int> OnlineLinkedContexts = new() {
        ItemSlot.Context.EquipAccessory,
        ItemSlot.Context.ModdedAccessorySlot,
        ItemSlot.Context.EquipAccessoryVanity,
        ItemSlot.Context.ModdedVanityAccessorySlot,
        ItemSlot.Context.InventoryItem,
        ItemSlot.Context.BankItem,
        ItemSlot.Context.ChestItem,
        ItemSlot.Context.VoidItem,
    };

    public static readonly HashSet<int> TransformSlotContexts = new() {
        ItemSlot.Context.InventoryItem,
        ItemSlot.Context.BankItem,
        ItemSlot.Context.ChestItem,
        ItemSlot.Context.TrashItem,
        ItemSlot.Context.GuideItem,
        ItemSlot.Context.HotbarItem,
        ItemSlot.Context.MouseItem,
        ItemSlot.Context.PrefixItem,
        ItemSlot.Context.VoidItem,
    };

    public static int BottomInventoryY => 260;

    public static byte DisableItemLeftClick { get => DisableItemSlotLeftClick; set => DisableItemSlotLeftClick = Math.Max(DisableItemSlotLeftClick, value); }

    public static bool CanDoLeftClickItemActions => SpecialLeftClickDelay == 0;

    public const float invBackColorMultipler = 0.785f;
    public static readonly Color invBackColor = new Color(63, 65, 151, 255);
    public static Color InventoryBackColor => invBackColor * invBackColorMultipler;

    public static UserInterface TalkInterface { get; private set; }

    public override bool IsLoadingEnabled(Mod mod) {
        return Main.netMode != NetmodeID.Server;
    }

    public override void Load() {
        On_ItemSlot.LeftClick_ItemArray_int_int += Hook_DisableLeftClick;
        On_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += ItemSlot_Draw;
        TalkInterface = new();
    }

    #region Hooks
    private static void Hook_DisableLeftClick(On_ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot) {
        if (DisableItemSlotLeftClick == 0) {
            orig(inv, context, slot);
        }
    }

    private static void ItemSlot_Draw(On_ItemSlot.orig_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color orig, SpriteBatch spriteBatch, Item[] inv, int context, int slot, Vector2 position, Color lightColor) {
        Slot = new(context, slot, inv, position, lightColor);
        orig(spriteBatch, inv, context, slot, position, lightColor);

        //if (inv[slot].IsAir) {
        //    SlotDecals.DrawEmptySlotDecals(spriteBatch, inv, context, slot, position, lightColor);
        //}
    }
    #endregion

    public override void ClearWorld() {
        TalkInterface?.SetState(null);
    }

    public override void UpdateUI(GameTime gameTime) {
        BottomLeftInventoryOffset = 0;
        TalkInterface.Update(gameTime);
        if (Main.mouseItem != null && !Main.mouseItem.IsAir) {
            SpecialLeftClickDelay = Math.Max(SpecialLeftClickDelay, (byte)20);
        }
        else if (SpecialLeftClickDelay > 0) {
            SpecialLeftClickDelay--;
        }
        if (LinkClickDelay > 0) {
            LinkClickDelay--;
        }
        if (DisableItemSlotLeftClick > 0) {
            DisableItemSlotLeftClick--;
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
        BottomLeftInventoryOffset = 0;
        ManageUserInterface(layers, TalkInterface, InterfaceLayers.Inventory_28, "Aequus: NPC Talk Interface", InterfaceScaleType.UI);

        //InsertInterfaceDrawMethod(layers, InterfaceLayers.Ruler_6, "Aequus: Misc World Interface", () => {
        //    return true;
        //}, InterfaceScaleType.Game);

        Insert(layers, InterfaceLayers.Inventory_28, "Aequus: Inventory", () => {
            AequusEventBarLoader.Draw();
            return true;
        });
    }

    internal static void ManageUserInterface(List<GameInterfaceLayer> layers, UserInterface userInterface, string defaultLayer, string layerName, InterfaceScaleType scaleType = InterfaceScaleType.UI) {
        int layer = -1;
        if (userInterface.CurrentState is AequusUIState aequusUIState) {
            if (!aequusUIState.ModifyInterfaceLayers(layers, ref scaleType)) {
                return;
            }
            layer = aequusUIState.GetLayerIndex(layers);
        }
        if (layer == -1) {
            layer = layers.FindIndex((g) => g.Name.Equals(defaultLayer));
        }
        if (layer != -1) {
            layers.Insert(layer, new LegacyGameInterfaceLayer(layerName, () => {
                userInterface.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
                return true;
            }, scaleType));
        }
    }

    internal static void Insert(List<GameInterfaceLayer> layers, string name, string yourName, GameInterfaceDrawMethod method, InterfaceScaleType scaleType = InterfaceScaleType.UI) {
        int index = layers.FindIndex((l) => l.Name.Equals(name));
        if (index != -1) {
            layers.Insert(index + 1, new LegacyGameInterfaceLayer(yourName, method, scaleType));
        }
    }

    public static void CloseAllInventoryRelatedUI() {
        CloseAllInventoryRelatedUI(Main.LocalPlayer);
    }

    public static void CloseAllInventoryRelatedUI(Player player) {
        player.chest = -1;
        player.sign = -1;
        player.SetTalkNPC(-1);
        if (player.whoAmI == Main.myPlayer) {
            Main.playerInventory = false;
            Main.recBigList = false;
            Main.CreativeMenu.CloseMenu();
            if (PlayerInput.GrappleAndInteractAreShared) {
                PlayerInput.Triggers.JustPressed.Grapple = false;
            }
            TalkInterface.SetState(null);
        }
    }

    public static void HoverItem(Item item, int context = -1) {
        Main.hoverItemName = item.Name;
        Main.HoverItem = item.Clone();
        Main.HoverItem.tooltipContext = context;
    }
}