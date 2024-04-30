﻿using Terraria.DataStructures;
using Terraria.GameContent.Golf;
using Terraria.GameContent.ItemDropRules;
using Terraria.UI;

namespace Aequus.Common.Hooks;

public partial class TerrariaHooks : ILoad {
    private Mod Mod { get; set; }

    public void Load(Mod mod) {
        Mod = mod;

        IL_Chest.SetupTravelShop += IL_Chest_SetupTravelShop;

        On_ChestUI.QuickStack += On_ChestUI_QuickStack;

        On_GolfHelper.StepGolfBall += On_GolfHelper_StepGolfBall;

        On_ItemDropResolver.ResolveRule += ItemDropResolver_ResolveRule;

        On_ItemSlot.PickItemMovementAction += On_ItemSlot_PickItemMovementAction;
        On_ItemSlot.RightClick_ItemArray_int_int += ItemSlot_RightClick;

        On_Main.DrawNPC += On_Main_DrawNPC;
        On_Main.DrawNPCs += On_Main_DrawNPCs;
        On_Main.DrawItems += On_Main_DrawItems;
        On_Main.UpdateTime_StartDay += On_Main_UpdateTime_StartDay;
        On_Main.UpdateTime_StartNight += On_Main_UpdateTime_StartNight;

        On_NPC.NPCLoot_DropMoney += NPC_NPCLoot_DropMoney;

        IL_Player.PlaceThing_ValidTileForReplacement += IL_Player_PlaceThing_ValidTileForReplacement;
        IL_Player.UpdateManaRegen += IL_Player_UpdateManaRegen;
        On_Player.GetPreferredGolfBallToUse += On_Player_GetPreferredGolfBallToUse;
        On_Player.RollLuck += On_Player_RollLuck;
        On_Player.PlaceThing_Tiles_CheckLavaBlocking += On_Player_PlaceThing_Tiles_CheckLavaBlocking;
        On_Player.UpdateVisibleAccessories += On_Player_UpdateVisibleAccessories;
        On_Player.QuickStackAllChests += On_Player_QuickStackAllChests;
        On_Player.ConsumeItem += On_Player_ConsumeItem;
        On_Player.QuickMount_GetItemToUse += On_Player_QuickMount_GetItemToUse;
        On_Player.QuickHeal_GetItemToUse += On_Player_QuickHeal_GetItemToUse;
        On_Player.QuickMana_GetItemToUse += On_Player_QuickMana_GetItemToUse;
        On_Player.HasUnityPotion += Player_HasUnityPotion;
        On_Player.TakeUnityPotion += On_Player_TakeUnityPotion;
        On_Player.GetRespawnTime += On_Player_GetRespawnTime;
        On_Player.DashMovement += On_Player_DashMovement;
        On_Player.PlaceThing_PaintScrapper_LongMoss += On_Player_PlaceThing_PaintScrapper_LongMoss;

        On_PlayerDeathReason.GetDeathText += On_PlayerDeathReason_GetDeathText;

        On_PlayerDrawLayers.DrawPlayer_RenderAllLayers += PlayerDrawLayers_DrawPlayer_RenderAllLayers;

        On_WorldGen.PlaceTile += On_WorldGen_PlaceTile;
        On_WorldGen.PlaceChest += On_WorldGen_PlaceChest;
        On_WorldGen.PlaceChestDirect += On_WorldGen_PlaceChestDirect;
        On_WorldGen.UpdateWorld_OvergroundTile += WorldGen_UpdateWorld_OvergroundTile;
        On_WorldGen.UpdateWorld_UndergroundTile += WorldGen_UpdateWorld_UndergroundTile;
    }

    public void Unload() { }
}
