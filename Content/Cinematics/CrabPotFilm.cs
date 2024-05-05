using Aequus.Content.Fishing.CrabPots;
using Terraria.Cinematics;
using Terraria.DataStructures;

namespace Aequus.Content.Cinematics;

internal class CrabPotFilm : AequusFilm {
    private readonly Point[] _crabPots;

    public CrabPotFilm() {
        _crabPots = new Point[4];

        BuildFullSequence();
    }

    protected override void Begin() {
        Main.dayTime = true;
        Main.time = 27000.0;
        Main.windSpeedCurrent = Main.windSpeedTarget = 0.22f;
        Main.hideUI = false;
        _startPoint = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY - 32f);
        TECrabPot.ChancePerTick = 100000000;
    }
    protected override void End() {
        TECrabPot.ChancePerTick = 100000;
        Main.LocalPlayer.isControlledByFilm = false;
        Main.hideUI = false;

        for (int k = 0; k < _crabPots.Length; k++) {
            if (_crabPots[k] != Point.Zero) {
                WorldGen.KillTile(_crabPots[k].X, _crabPots[k].Y, noItem: true);
            }
        }
        for (int k = 0; k < Main.maxItems; k++) {
            Main.item[k].TurnToAir();
        }
        for (int i = 0; i < Main.InventoryItemSlotsCount; i++) {
            if (!Main.LocalPlayer.inventory[i].IsAir && Main.LocalPlayer.inventory[i].ModItem is CutsceneItem) {
                Main.LocalPlayer.selectedItem = i;
                break;
            }
        }
    }

    protected override void BuildSequence() {
        AppendKeyFrames(InitPlayer, WalkPlayerRight);
        AppendEmptySequence(107);
        AppendKeyFrames(StopPlayerMovement);
        AppendEmptySequence(14);

        // Leftmost crab pot
        AppendKeyFrames(PlaceCrabPot);
        AppendEmptySequence(6);
        AppendKeyFrames(StopPlayerUseItem, WalkPlayerRight);
        AppendEmptySequence(36);

        // Left middle crab pot
        AppendKeyFrames(StopPlayerMovement, PlaceCrabPot);
        AppendEmptySequence(6);
        AppendKeyFrames(StopPlayerUseItem, WalkPlayerRight);
        AppendEmptySequence(25);

        // Right middle crab pot
        AppendKeyFrames(StopPlayerMovement, PlaceCrabPot);
        AppendEmptySequence(6);
        AppendKeyFrames(StopPlayerUseItem, WalkPlayerRight);
        AppendEmptySequence(14);

        // Rightmost crab pot
        AppendKeyFrames(StopPlayerMovement, PlaceCrabPot);
        AppendEmptySequence(6);

        AppendKeyFrames(StopPlayerUseItem, WalkPlayerRight);
        AppendEmptySequence(14);
        AppendKeyFrames(WalkPlayerLeft);
        AppendEmptySequence(11);
        AppendKeyFrames(StopPlayerMovement);
        AppendEmptySequence(120);
        AppendKeyFrames(WalkPlayerLeft);
        AppendEmptySequence(35);

        AppendKeyFrames(StopPlayerMovement);
        AppendEmptySequence(8);
        AppendKeyFrames(SetHeldItem(ItemID.ApprenticeBait), RclickCrabPot);
        AppendEmptySequence(1);
        AppendKeyFrames(WalkPlayerLeft);
        AppendEmptySequence(14);

        AppendKeyFrames(StopPlayerMovement);
        AppendEmptySequence(8);
        AppendKeyFrames(SetHeldItem(ItemID.JourneymanBait), RclickCrabPot);
        AppendEmptySequence(1);
        AppendKeyFrames(WalkPlayerLeft);
        AppendEmptySequence(28);

        AppendKeyFrames(StopPlayerMovement);
        AppendEmptySequence(8);
        AppendKeyFrames(SetHeldItem(ItemID.MasterBait), RclickCrabPot);
        AppendEmptySequence(1);
        AppendKeyFrames(WalkPlayerLeft);
        AppendEmptySequence(18);

        AppendKeyFrames(StopPlayerMovement);
        AppendEmptySequence(8);
        AppendKeyFrames(SetHeldItem(ItemID.MasterBait), RclickCrabPot);
        AppendEmptySequence(1);
        AppendKeyFrames(WalkPlayerLeft);
        AppendEmptySequence(180);


        AppendKeyFrames(StopPlayerMovement);
        AppendSequence(3000, SpeedUpTime);


        AppendKeyFrames(InitPlayer, WalkPlayerRight, UnSpeedUpTime);
        AppendEmptySequence(115);

        // Leftmost crab pot
        AppendKeyFrames(RclickCrabPot);
        AppendEmptySequence(6);
        AppendKeyFrames(StopPlayerUseItem, WalkPlayerRight);
        AppendEmptySequence(16);

        // Left middle crab pot
        AppendKeyFrames(StopPlayerMovement, RclickCrabPot);
        AppendEmptySequence(6);
        AppendKeyFrames(StopPlayerUseItem, WalkPlayerRight);
        AppendEmptySequence(23);

        // Right middle crab pot
        AppendKeyFrames(StopPlayerMovement, RclickCrabPot);
        AppendEmptySequence(6);
        AppendKeyFrames(StopPlayerUseItem, WalkPlayerRight);
        AppendEmptySequence(14);

        // Rightmost crab pot
        AppendKeyFrames(StopPlayerMovement, RclickCrabPot);
        AppendEmptySequence(6);
        AppendKeyFrames(StopPlayerUseItem, WalkPlayerRight);
        AppendEmptySequence(120);
    }

    private void UnSpeedUpTime(FrameEventData evt) {
        Main.fastForwardTimeToDawn = false;
        Main.fastForwardTimeToDusk = false;
        TECrabPot.ChancePerTick = 100000000;
    }

    private void SpeedUpTime(FrameEventData evt) {
        Main.fastForwardTimeToDawn = true;
        Main.fastForwardTimeToDusk = true;
        TECrabPot.ChancePerTick = 10000;
    }

    private void RclickCrabPot(FrameEventData evt) {
        Player player = Main.LocalPlayer;

        Point crabPotSpot = player.Center.ToTileCoordinates();
        crabPotSpot.Y += 5;

        if (TileLoader.GetTile(Main.tile[crabPotSpot].TileType) is not UnifiedCrabPot crabPot) {
            Terraria.Dust d = Terraria.Dust.NewDustPerfect(crabPotSpot.ToWorldCoordinates(), DustID.Torch);
            d.noGravity = true;
            d.fadeIn = d.scale + 2f;
            return;
        }

        crabPot.RightClick(crabPotSpot.X, crabPotSpot.Y);
    }

    private void PlaceCrabPot(FrameEventData evt) {
        Player player = Main.LocalPlayer;
        player.selectedItem = 0;
        player.controlUseItem = true;
        Point crabPotSpot = player.Center.ToTileCoordinates();
        crabPotSpot.Y += 4;
        WorldGen.PlaceTile(crabPotSpot.X, crabPotSpot.Y, ModContent.TileType<CrabPot>(), style: player.HeldItem.placeStyle);

        CrabPotBiomeData biomeData = new CrabPotBiomeData(TECrabPot.GetWaterStyle(crabPotSpot.X, crabPotSpot.Y));
        int id = ModContent.GetInstance<TECrabPot>().Place(crabPotSpot.X, crabPotSpot.Y);
        ((TECrabPot)TileEntity.ByID[id]).biomeData = biomeData;
        TECrabPot.PlacementEffects(crabPotSpot.X, crabPotSpot.Y);

        for (int k = 0; k < _crabPots.Length; k++) {
            if (_crabPots[k] == Point.Zero) {
                _crabPots[k] = crabPotSpot;
                break;
            }
        }
    }

    private void InitPlayer(FrameEventData evt) {
        Player player = Main.LocalPlayer;
        FindFloorAt(_startPoint, out var x, out var y);
        player.BottomLeft = new Vector2(x, y);
        player.velocity.X = 6f;
        player.isControlledByFilm = true;
        player.inventory[0].SetDefaults(Aequus.Instance.Find<ModItem>("CrabPotCopper").Type);
    }
}
