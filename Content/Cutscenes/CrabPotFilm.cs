using Aequus.Content.Cutscenes;
using Aequus.Content.Fishing.CrabPots;
using Terraria.Cinematics;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;

namespace Aequus.Content.Cinematics;

/// <summary><see cref="DD2Film"/> <see cref="DSTFilm"/></summary>
internal class CrabPotFilm : Film {
    private Vector2 _startPoint;
    private Point[] _crabPots;

    public CrabPotFilm() {
        _crabPots = new Point[4];

        BuildSequence();
    }

    public override void OnBegin() {
        Main.NewText("CrabPotFilm: Begin");
        Main.dayTime = true;
        Main.time = 27000.0;
        Main.windSpeedCurrent = Main.windSpeedTarget = 0.22f;
        Main.hideUI = false;
        _startPoint = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY - 32f);
        TECrabPot.ChancePerTick = 100000000;
        base.OnBegin();
    }

    public override void OnEnd() {
        TECrabPot.ChancePerTick = 100000;
        Main.LocalPlayer.isControlledByFilm = false;
        Main.hideUI = false;
        Main.NewText("CrabPotFilm: End");

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

        base.OnEnd();
    }

    private void BuildSequence() {
        AppendKeyFrames(InitPlayer, ControlPlayer);
        AppendEmptySequence(107);
        AppendKeyFrames(StopPlayer);
        AppendEmptySequence(14);

        // Leftmost crab pot
        AppendKeyFrames(PlaceCrabPot);
        AppendEmptySequence(6);
        AppendKeyFrames(StopPlayerItemAction, ControlPlayer);
        AppendEmptySequence(36);

        // Left middle crab pot
        AppendKeyFrames(StopPlayer, PlaceCrabPot);
        AppendEmptySequence(6);
        AppendKeyFrames(StopPlayerItemAction, ControlPlayer);
        AppendEmptySequence(25);

        // Right middle crab pot
        AppendKeyFrames(StopPlayer, PlaceCrabPot);
        AppendEmptySequence(6);
        AppendKeyFrames(StopPlayerItemAction, ControlPlayer);
        AppendEmptySequence(14);

        // Rightmost crab pot
        AppendKeyFrames(StopPlayer, PlaceCrabPot);
        AppendEmptySequence(6);

        AppendKeyFrames(StopPlayerItemAction, ControlPlayer);
        AppendEmptySequence(14);
        AppendKeyFrames(FlipPlayerLeft);
        AppendEmptySequence(11);
        AppendKeyFrames(StopPlayer);
        AppendEmptySequence(120);
        AppendKeyFrames(FlipPlayerLeft);
        AppendEmptySequence(35);

        AppendKeyFrames(StopPlayer);
        AppendEmptySequence(8);
        AppendKeyFrames(GiveApprenticeBait, FillCrabPot);
        AppendEmptySequence(1);
        AppendKeyFrames(FlipPlayerLeft);
        AppendEmptySequence(14);

        AppendKeyFrames(StopPlayer);
        AppendEmptySequence(8);
        AppendKeyFrames(GiveJourneymanBait, FillCrabPot);
        AppendEmptySequence(1);
        AppendKeyFrames(FlipPlayerLeft);
        AppendEmptySequence(28);

        AppendKeyFrames(StopPlayer);
        AppendEmptySequence(8);
        AppendKeyFrames(GiveMasterBait, FillCrabPot);
        AppendEmptySequence(1);
        AppendKeyFrames(FlipPlayerLeft);
        AppendEmptySequence(18);

        AppendKeyFrames(StopPlayer);
        AppendEmptySequence(8);
        AppendKeyFrames(GiveMasterBait, FillCrabPot);
        AppendEmptySequence(1);
        AppendKeyFrames(FlipPlayerLeft);
        AppendEmptySequence(180);


        AppendKeyFrames(StopPlayer);
        AppendSequence(3000, SpeedUpTime);


        AppendKeyFrames(InitPlayer, ControlPlayer, UnSpeedUpTime);
        AppendEmptySequence(115);

        // Leftmost crab pot
        AppendKeyFrames(FillCrabPot);
        AppendEmptySequence(6);
        AppendKeyFrames(StopPlayerItemAction, ControlPlayer);
        AppendEmptySequence(16);

        // Left middle crab pot
        AppendKeyFrames(StopPlayer, FillCrabPot);
        AppendEmptySequence(6);
        AppendKeyFrames(StopPlayerItemAction, ControlPlayer);
        AppendEmptySequence(23);

        // Right middle crab pot
        AppendKeyFrames(StopPlayer, FillCrabPot);
        AppendEmptySequence(6);
        AppendKeyFrames(StopPlayerItemAction, ControlPlayer);
        AppendEmptySequence(14);

        // Rightmost crab pot
        AppendKeyFrames(StopPlayer, FillCrabPot);
        AppendEmptySequence(6);
        AppendKeyFrames(StopPlayerItemAction, ControlPlayer);
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

    private void GiveMasterBait(FrameEventData evt) {
        Main.LocalPlayer.inventory[0].SetDefaults(ItemID.MasterBait);
    }

    private void GiveJourneymanBait(FrameEventData evt) {
        Main.LocalPlayer.inventory[0].SetDefaults(ItemID.JourneymanBait);
    }

    private void GiveApprenticeBait(FrameEventData evt) {
        Main.LocalPlayer.inventory[0].SetDefaults(ItemID.ApprenticeBait);
    }

    private void FillCrabPot(FrameEventData evt) {
        Player player = Main.LocalPlayer;

        Point crabPotSpot = player.Center.ToTileCoordinates();
        crabPotSpot.Y += 5;

        if (TileLoader.GetTile(Main.tile[crabPotSpot].TileType) is not BaseCrabPot crabPot) {
            Dust d = Dust.NewDustPerfect(crabPotSpot.ToWorldCoordinates(), DustID.Torch);
            d.noGravity = true;
            d.fadeIn = d.scale + 2f;
            return;
        }

        crabPot.RightClick(crabPotSpot.X, crabPotSpot.Y);
    }

    private void FlipPlayerLeft(FrameEventData evt) {
        Main.LocalPlayer.controlLeft = true;
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

    private void StopPlayerItemAction(FrameEventData evt) {
        Main.LocalPlayer.controlUseItem = false;
    }

    private void StopPlayer(FrameEventData evt) {
        Player player = Main.LocalPlayer;
        player.controlRight = false;
        player.controlLeft = false;
    }

    private void ControlPlayer(FrameEventData evt) {
        Player player = Main.LocalPlayer;
        player.controlRight = true;
    }

    private void InitPlayer(FrameEventData evt) {
        Player player = Main.LocalPlayer;
        FindFloorAt(_startPoint, out var x, out var y);
        player.BottomLeft = new Vector2(x, y);
        player.velocity.X = 6f;
        player.isControlledByFilm = true;
        player.inventory[0].SetDefaults(Aequus.Instance.Find<ModItem>("CrabPotCopper").Type);
    }

    private void ResetPlayerSelectedItem(FrameEventData evt) {
    }

    private void EndSlot() {
    }

    private static void FindFloorAt(Vector2 position, out int x, out int y) {
        x = (int)position.X;
        y = (int)position.Y;
        int i = x / 16;
        int j;
        for (j = y / 16; !WorldGen.SolidTile(i, j); j++) {
        }

        y = j * 16;
    }
}
