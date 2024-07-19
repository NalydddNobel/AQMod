using Aequus.Common.DataSets;
using Aequus.Common.Net;
using Aequus.Common.Particles;
using Aequus.Common.Tiles;
using Aequus.Content.Net;
using Aequus.Items.Equipment.Accessories.Money.FoolsGoldRing;
using Aequus.NPCs;
using Aequus.Particles;
using Aequus.Tiles.Furniture.Gravity;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader.IO;

namespace Aequus.Items;
public partial class AequusItem {
    #region Gravity Blocks
    public static int ReversedGravityCheck;

    public bool reversedGravity;

    public static void CheckItemGravity() {
        ReversedGravityCheck--;
        if (ReversedGravityCheck <= 0) {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i = 0; i < Main.maxItems; i++) {
                var item = Main.item[i];
                if (item.active && !item.IsAir && !ItemID.Sets.ItemNoGravity[item.type] && item.TryGetGlobalItem<AequusItem>(out var aequus)) {
                    aequus.CheckGravityTiles(Main.item[i], i);
                }
            }
            stopWatch.Stop();
            ReversedGravityCheck = Math.Min((int)stopWatch.ElapsedMilliseconds, 30);
        }
    }

    private void CheckGravityTiles(Item item, int i) {
        bool old = reversedGravity;
        reversedGravity = GravityBlockHandler.CheckGravityBlocks(item.position, item.width, item.height) < 0;
        if (reversedGravity != old) {
            item.velocity.Y = -item.velocity.Y;
            if (Main.netMode == NetmodeID.Server) {
                NetMessage.SendData(MessageID.SyncItem, number: i);
            }
        }
    }

    private void Update_ReversedGravity(Item item, ref float gravity, float maxFallSpeed) {
        if (reversedGravity) {
            gravity = -gravity;
            if (item.velocity.Y < -maxFallSpeed) {
                item.velocity.Y = -maxFallSpeed;
            }
        }
    }
    #endregion

    #region Gravity Chests
    public bool CheckItemAbsorber(Item item, int i) {
        var tileCoords = item.Center.ToTileCoordinates();
        int searchSize = 20;
        for (int k = 0; k < suctionChestCheckAmt; k++) {
            int x = tileCoords.X + Main.rand.Next(-searchSize, searchSize);
            int y = tileCoords.Y + Main.rand.Next(-searchSize, searchSize);
            if (!WorldGen.InWorld(x, y) || !Main.tile[x, y].HasTile || !Main.tileContainer[Main.tile[x, y].TileType]) {
                continue;
            }
            if (Main.tile[x, y].TileType >= TileID.Count && TileLoader.GetTile(Main.tile[x, y].TileType) is GravityChestTile gravityChest) {
                int left = x - Main.tile[x, y].TileFrameX / 18;
                int top = y - Main.tile[x, y].TileFrameY / 18;
                int chestID = Chest.FindChest(left, top);
                if (chestID != -1 && gravityChest.PickupItemLogic(x, y, chestID, item, this)) {
                    if (!Helper.TryStackingInto(Main.chest[chestID].item, Chest.maxItems, item, out int chestItemIndex)) {
                        if (Main.netMode != NetmodeID.SinglePlayer)
                            NetMessage.SendData(MessageID.SyncChestItem, number: chestID, number2: chestItemIndex);
                        continue;
                    }
                    var itemPos = item.Center;
                    item.TurnToAir();
                    item.active = false;
                    if (Main.netMode != NetmodeID.SinglePlayer) {
                        NetMessage.SendData(MessageID.SyncItem, number: i);
                        var r = new Rectangle(left - searchSize, top - searchSize, searchSize * 2 + 2, searchSize * 2 + 2).WorldRectangle();
                        for (int m = 0; m < Main.maxPlayers; m++) {
                            if (ScreenCulling.ServerSafeInView(Main.player[m].Center, r)) {
                                var p = Aequus.GetPacket(PacketType.GravityChestPickupEffect);
                                p.Write(itemPos.X);
                                p.Write(itemPos.Y);
                                p.Write(left);
                                p.Write(top);
                                p.Send(toClient: m);
                            }
                        }
                    }
                    else {
                        ScreenCulling.Prepare(padding: 20);
                        if (ScreenCulling.OnScreenWorld(Main.LocalPlayer.Center)) {
                            GravityChestTile.ItemPickupEffect(itemPos, new Vector2(left * 16f + 16f, top * 16f + 16f));
                        }
                    }
                    suctionChestCheckAmt += 3;
                    if (suctionChestCheckAmt > 30)
                        suctionChestCheckAmt = 30;
                    return true;
                }
            }
        }
        return false;
    }

    public static void CheckItemAbsorber() {
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;

        SuctionChestCheck--;
        if (SuctionChestCheck <= 0) {
            if (Main.rand.NextBool(40)) {
                suctionChestCheckAmt--;
                if (suctionChestCheckAmt <= 2)
                    suctionChestCheckAmt = 2;
            }
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i = 0; i < Main.maxItems; i++) {
                var item = Main.item[i];
                if (item.active && !item.IsAir && item.TryGetGlobalItem<AequusItem>(out var aequus)) {
                    if (aequus.CheckItemAbsorber(Main.item[i], i))
                        break;
                }
            }
            stopWatch.Stop();
            SuctionChestCheck = Math.Min((int)stopWatch.ElapsedMilliseconds, 30);
        }
    }
    #endregion

    #region Lucky Drop Effects
    public ushort luckyDrop;

    internal static void LuckyDropSpawnEffect(Vector2 position, int width, int height, int amount) {
        var center = position + new Vector2(width / 2f, height / 2f);
        var hitbox = new Rectangle((int)position.X, (int)position.Y, width, height);
        for (int i = 0; i < amount; i++) {
            float intensity = (float)Math.Pow(0.9f, i + 1);
            ParticleSystem.New<MonoFlashParticle>(ParticleLayer.AboveDust).Setup(Helper.NextFromRect(Main.rand, hitbox), Vector2.Zero, Color.Yellow.UseA(0), Color.White * 0.33f, Main.rand.NextFloat(0.5f, 1f) * intensity, 0.2f, 0f);
        }
        for (int i = 0; i < amount * 2; i++) {
            var d = Dust.NewDustDirect(position, width, height, DustID.SpelunkerGlowstickSparkle);
            d.velocity *= 0.5f;
            d.velocity = Vector2.Normalize(d.position - center) * d.velocity.Length();
        }
    }

    internal void OnSpawn_CheckLuckyDrop(Item item) {
        if (!AequusNPC.doLuckyDropsEffect || item.IsACoin) {
            return;
        }

        luckyDrop = 480;
        int amount = Math.Clamp(item.value / Item.gold, 1, 10);
        if (Main.netMode != NetmodeID.SinglePlayer) {
            PacketSystem.Get<LuckyDropSpawnPacket>().Send(item.position, item.width, item.height, amount);
        }
        else {
            LuckyDropSpawnEffect(item.position, item.width, item.height, amount);
        }
    }

    internal void Update_LuckyDrop(Item item) {
        if (luckyDrop == 0) {
            return;
        }

        luckyDrop--;

        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        if (item.timeSinceItemSpawned % 5 == 0) {
            var texture = TextureAssets.Item[item.type].Value;
            Helper.GetItemDrawData(item, out var frame);
            var d = Dust.NewDustDirect(item.Bottom + new Vector2(frame.Width / -2f, -frame.Height), frame.Width, frame.Height, DustID.SpelunkerGlowstickSparkle);
            d.velocity *= Main.rand.NextFloat(0.2f);
            d.velocity = item.DirectionTo(d.position) * d.velocity.Length();
            d.velocity += item.velocity * 0.5f;
            d.fadeIn = d.scale + 0.3f;
        }
        if (item.velocity.Length() > 2f && Main.GameUpdateCount % 5 == 0) {
            var d = Dust.NewDustDirect(item.position, item.width, item.height, DustID.SpelunkerGlowstickSparkle);
            d.velocity *= 0.1f;
        }
    }
    #endregion

    #region Stack Checks
    public override bool CanStack(Item item1, Item item2) {
        return item1.prefix == item2.prefix;
    }

    public override bool CanStackInWorld(Item item1, Item item2) {
        return item1.prefix == item2.prefix;
    }
    #endregion

    #region On Pickup Effects
    public override bool OnPickup(Item item, Player player) {
        if (naturallyDropped && item.IsACoin) {
            var aequus = player.Aequus();
            FoolsGoldRing.ProcOnPickupCoin(item, player, aequus);
        }
        naturallyDropped = false;
        return true;
    }
    #endregion

    #region Unused Items
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetDefaults_UnusedItemOverride(Item item) {
        if (ItemSets.IsRemovedQuickCheck.Contains(item.type)) {
            item.rare = ItemRarityID.Gray;
        }
    }
    #endregion

    #region Static Helper Methods
    [Obsolete("Remove.")]
    public static Item SetDefaults(int type, bool checkMaterial = true) {
        var i = new Item();
        i.SetDefaults(type, noMatCheck: !checkMaterial);
        return i;
    }
    [Obsolete("Remove.")]
    public static Item SetDefaults<T>(bool checkMaterial = true) where T : ModItem {
        return SetDefaults(ModContent.ItemType<T>(), checkMaterial);
    }

    public static int NewItemCloned(IEntitySource source, Vector2 pos, Item item) {
        int i = Item.NewItem(source, pos, item.type, item.stack);
        Main.item[i] = item.Clone();
        Main.item[i].active = true;
        Main.item[i].whoAmI = i;
        Main.item[i].Center = pos;
        Main.item[i].stack = item.stack;
        return i;
    }

    public static bool IsPotion(Item item) {
        return ItemSets.IsPotion.Contains(item.type);
    }

    public static void SaveItemID(TagCompound tag, string key, int itemID) {
        if (itemID >= ItemID.Count) {
            var modItem = ItemLoader.GetItem(itemID);
            tag[$"{key}Key"] = $"{modItem.Mod.Name}:{modItem.Name}";
            return;
        }
        tag[$"{key}ID"] = itemID;
    }
    public static int LoadItemID(TagCompound tag, string key) {
        if (tag.TryGet($"{key}Key", out string buffKey)) {
            var val = buffKey.Split(":");
            if (ModLoader.TryGetMod(val[0], out var mod)) {
                if (mod.TryFind<ModItem>(val[1], out var modItem)) {
                    return modItem.Type;
                }
            }
        }
        return tag.Get<int>($"{key}ID");
    }
    #endregion
}