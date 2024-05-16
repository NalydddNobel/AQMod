using Aequus.Core.ContentGeneration;
using Aequus.Core.CrossMod;
using Aequus.DataSets;
using Aequus.DataSets.Structures;
using Aequus.DataSets.Structures.DropRulesChest;
using Aequus.DataSets.Structures.Enums;
using System.IO;
using System.Linq;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.ObjectData;
using Terraria.Utilities;
using tModLoaderExtended.Networking;
using tModLoaderExtended.Terraria.GameContent.Creative;

namespace Aequus.Content.Chests.BuriedChests;

// Buried Chests use a fake chest before being unlocked
// This allows them to generate their loot upon being opened
public class UnifiedBuriedChest : UnifiedModChest {
    internal readonly ChestInfo _info;

    internal InstancedLockedBuriedChest Locked { get; private set; }

    internal UnifiedBuriedChest(ChestInfo info) {
        _info = info;
    }

    internal record struct ChestInfo(IContentIdProvider Key, ChestPool LootPool, Color MapEntryColor);

    public override void Load() {
        base.Load();
        Locked = new InstancedLockedBuriedChest(this);
        Mod.AddContent(Locked);
    }

    public override bool CanUnlockChest(int i, int j, int left, int top, Player player) {
        return player.ConsumeItem(_info.Key.GetId());
    }

    public override bool LockChest(int i, int j, ref short frameXAdjustment, ref bool manual) {
        return Main.tile[i, j].TileFrameX < 36;
    }

    public override void SafeSetStaticDefaults() {
        AddMapEntry(_info.MapEntryColor, CreateMapEntryName(), MapChestName);
    }

    internal class InstancedLockedBuriedChest : InstancedModTile {
        internal UnifiedBuriedChest _parent;

        internal InstancedHiddenBuriedChest Hidden { get; private set; }

        public override string Texture => _parent.Texture;

        public InstancedLockedBuriedChest(UnifiedBuriedChest parent) : base(parent.Name + "Locked", parent.Texture + "Locked") {
            _parent = parent;
        }

        public override void Load() {
            Mod.AddContent(new InstancedLockedBuriedChestItem(this));
            Hidden = new InstancedHiddenBuriedChest(this);
            Mod.AddContent(Hidden);
        }

        public override void SetStaticDefaults() {
            Main.tileSpelunker[Type] = true;
            Main.tileContainer[Type] = true;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 1200;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileOreFinderPriority[Type] = 500;

            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.AvoidedByMeteorLanding[Type] = true;
            TileID.Sets.PreventsSandfall[Type] = true;
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.PreventsTileReplaceIfOnTopOfIt[Type] = true;
            TileID.Sets.WallsMergeWith[Type] = true;
            TileDataSet.PreventsSlopesBelow.Add(Type);

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.newTile.AnchorInvalidTiles = new int[] { TileID.MagicalIceBlock };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);

            AddMapEntry(_parent._info.MapEntryColor, CreateMapEntryName());

            DustType = _parent.DustType;
            AdjTiles = new int[] { TileID.Containers };
        }

        public override void PostSetupTileMerge() {
            //for (int i = 0; i < Main.tileMerge.Length; i++) {
            //    Main.tileMerge[i][Type] = true;
            //}
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) {
            return true;
        }

        public override void MouseOver(int i, int j) {
            Player player = Main.LocalPlayer;
            player.cursorItemIconID = _parent._info.Key.GetId();
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
        }

        public override bool RightClick(int i, int j) {
            Player player = Main.LocalPlayer;
            Main.mouseRightRelease = false;
            Tile tile = Main.tile[i, j];

            player.CloseSign();
            player.SetTalkNPC(-1);
            Main.npcChatCornerItem = 0;
            Main.npcChatText = "";
            if (Main.editChest) {
                SoundEngine.PlaySound(SoundID.MenuTick);
                Main.editChest = false;
                Main.npcChatText = string.Empty;
            }

            if (player.editedChestName) {
                NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f);
                player.editedChestName = false;
            }

            if (player.ConsumeItem(_parent._info.Key.GetId())) {
                if (Main.netMode == NetmodeID.MultiplayerClient) {
                    ExtendedMod.GetPacket<UnlockBuriedChestPacket>().Send(i, j, Main.myPlayer);
                }
                else {
                    UnlockBuriedChest(i, j);
                }
            }

            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) {
            num = fail ? 1 : 3;
        }

        internal void UnlockBuriedChest(int i, int j) {
            Tile tile = Main.tile[i, j];
            int left = i - tile.TileFrameX % 36 / 18;
            int top = j - tile.TileFrameY % 36 / 18;

            //WorldGen.KillTile(i, j, noItem: true);
            SoundEngine.PlaySound(SoundID.Unlock, new Vector2(i, j).ToWorldCoordinates());
            for (int x = left; x < left + 2; x++) {
                for (int y = top; y < top + 2; y++) {
                    Vector2 dustCoordinates = new Vector2(i * 16, j * 16);
                    for (int k = 0; k < 4; k++) {
                        Terraria.Dust.NewDust(dustCoordinates, 16, 16, _parent.DustType);
                    }

                    Tile chestTile = Framing.GetTileSafely(x, y);
                    if (chestTile.TileType == Type && Main.netMode != NetmodeID.MultiplayerClient) {
                        chestTile.HasTile = false;
                    }
                }
            }

            if (Main.netMode != NetmodeID.MultiplayerClient) {
                ushort chestType = _parent.Type;
                int chest = WorldGen.PlaceChest(left, top + 1, chestType);

                if (Main.chest.IndexInRange(chest)) {
                    // Create an RNG seed using the world's seed and the number of buried chests opened.
                    int seed = Main.ActiveWorldFileData.Seed + WorldState.BuriedChestsLooted;

                    WorldState.BuriedChestsLooted++;

                    UnifiedRandom chestSpecificRNG = new UnifiedRandom(seed);

                    // Roll chest loot using special seed.
                    ChestLootDatabase.Instance.SolveRules(_parent._info.LootPool, new ChestLootInfo(chest, rng: chestSpecificRNG));
                }

                if (Main.netMode == NetmodeID.Server) {
                    NetMessage.SendTileSquare(-1, left - 1, top - 1, 4, 4);
                    NetMessage.SendData(MessageID.ChestUpdates,
                        number: 100,
                        number2: Main.chest[chest].x, number3: Main.chest[chest].y + 1,
                        number4: 0f, number5: 0,
                        number6: chestType);
                }
            }
        }
    }

    internal class InstancedHiddenBuriedChest : InstancedModTile {
        internal InstancedLockedBuriedChest _parent;

        public InstancedHiddenBuriedChest(InstancedLockedBuriedChest parent) : base(parent.Name + "Hidden", AequusTextures.None.Path) {
            _parent = parent;
        }

        public override void Load() {
        }

        public override void SetStaticDefaults() {
            Main.tileSolid[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.AvoidedByMeteorLanding[Type] = true;
            TileID.Sets.PreventsSandfall[Type] = true;
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.PreventsTileReplaceIfOnTopOfIt[Type] = true;
            TileID.Sets.WallsMergeWith[Type] = true;
            TileDataSet.PreventsSlopesBelow.Add(Type);

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.newTile.AnchorInvalidTiles = new int[] { TileID.MagicalIceBlock };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);

            AddMapEntry(Color.Lerp(Color.LightGray, Color.Gray, 0.5f), Language.GetOrRegister("Mods.Aequus.Tiles.HiddenChest.MapEntry"));

            DustType = _parent.DustType;
            AdjTiles = new int[] { TileID.Containers };
        }

        public override void PostSetupTileMerge() {
            for (int i = 0; i < Main.tileMerge.Length; i++) {
                Main.tileMerge[i][Type] = true;
            }
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
            if (IsRevealed(i, j)) {
                SetRevealedState(i, j);
            }
            return true;
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged) {
            return false;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem) {
            if (fail && !noItem) {
                SetRevealedState(i, j);
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
            if (!TileDrawing.IsVisible(Main.tile[i, j])) {
                return false;
            }

            int tileClone = TileID.Stone;
            Main.instance.LoadTiles(tileClone);
            Texture2D tileTexture = TextureAssets.Tile[tileClone].Value;
            Vector2 drawCoordinates = new Vector2(i, j) * 16f + TileHelper.DrawOffset - Main.screenPosition;
            Rectangle frame = new Rectangle(18, 18, 16, 16);
            Color lightColor = Lighting.GetColor(i, j);

            spriteBatch.Draw(tileTexture, drawCoordinates, frame, lightColor);

            return false;
        }

        private bool IsRevealed(int i, int j) {
            Tile tile = Main.tile[i, j];
            int left = i - tile.TileFrameX % 36 / 18;
            int top = j - tile.TileFrameY % 36 / 18;

            // 4 Wide
            for (int x = left - 1; x < left + 3; x++) {
                // Check top and bottom strips

                // # = checked
                // * = unchecked
                // C = chest

                // # # # #
                // * C C *
                // * C C *
                // # # # #

                for (int y = top - 1; y < top + 4; y += 3) {
                    if (IsTileRevealed(x, y)) {
                        return true;
                    }
                }
            }

            for (int x = left - 1; x < left + 3; x += 3) {
                // Check left and right strips

                // # = checked
                // * = unchecked
                // C = chest

                // * * * *
                // # C C #
                // # C C #
                // * * * *

                for (int y = top; y < top + 2; y++) {
                    if (IsTileRevealed(x, y)) {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsTileRevealed(int i, int j) {
            Tile tile = Framing.GetTileSafely(i, j);

            //Dust.NewDustPerfect(new Vector2(i, j).ToWorldCoordinates(), DustID.FlameBurst);
            return !tile.IsFullySolid();
        }

        internal void SetRevealedState(int i, int j) {
            Tile tile = Main.tile[i, j];
            int left = i - tile.TileFrameX % 36 / 18;
            int top = j - tile.TileFrameY % 36 / 18;

            for (int x = left; x < left + 2; x++) {
                for (int y = top; y < top + 2; y++) {
                    Tile chestTile = Framing.GetTileSafely(x, y);
                    if (chestTile.TileType == Type) {
                        chestTile.TileFrameX += 36;
                        chestTile.TileType = _parent.Type;
                    }
                }
            }

            SoundEngine.PlaySound(AequusSounds.BuriedChestReveal with { PitchVariance = 0.2f, Volume = 0.75f }, new Vector2(i + 1, j + 1) * 16f);

            for (int x = left; x < left + 2; x++) {
                for (int y = top; y < top + 2; y++) {
                    WorldGen.SquareTileFrame(x, y);
                    Vector2 dustCoordinates = new Vector2(x * 16, y * 16);
                    for (int k = 0; k < 12; k++) {
                        Terraria.Dust.NewDust(dustCoordinates, 16, 16, DustID.Stone);
                    }
                }
            }
        }
    }

    private class InstancedLockedBuriedChestItem : InstancedTileItem {
        private readonly InstancedLockedBuriedChest _parent;
        public InstancedLockedBuriedChestItem(InstancedLockedBuriedChest parent) : base(parent, style: 1, journeyOverride: new JourneySortByTileId(TileID.Containers)) {
            _parent = parent;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ItemLoader.GetItem(_parent._parent._info.Key.GetId()).DisplayName);

        public override bool CanRightClick() {
            return Main.LocalPlayer.ConsumeItem(_parent._parent._info.Key.GetId());
        }

        public override void RightClick(Player player) {
            SoundEngine.PlaySound(SoundID.Unlock);
        }

        public override void ModifyItemLoot(ItemLoot itemLoot) {
            foreach (IItemDropRule itemDropRule in ChestLootDatabase.Instance
                .GetRulesForType(_parent._parent._info.LootPool)
                .SelectWhereOfType<IConvertDropRules>()
                .Select(convert => convert.ToItemDropRule())) {
                itemLoot.Add(itemDropRule);
            }
        }
    }

    private class UnlockBuriedChestPacket : PacketHandler {
        public void Send(int i, int j, int plr, ushort? TileTypeOverride = null) {
            ModPacket packet = GetPacket();
            packet.Write((ushort)i);
            packet.Write((ushort)j);
            packet.Write(TileTypeOverride ?? Main.tile[i, j].TileType);
            packet.Send();
        }

        public override void Receive(BinaryReader reader, int sender) {
            int i = reader.ReadUInt16();
            int j = reader.ReadUInt16();
            ushort type = reader.ReadUInt16();

            if (TileLoader.GetTile(type) is not InstancedLockedBuriedChest buriedChest) {
                return;
            }

            if (Main.netMode == NetmodeID.Server) {
                Send(i, j, Main.myPlayer, TileTypeOverride: type);
            }

            buriedChest.UnlockBuriedChest(i, j);
        }
    }
}
