using Terraria.DataStructures;
using Terraria.Localization;

namespace Aequus.Core.Utilities;

public static class PlayerHelper {
    private static readonly Item[] _dummyInventory = ExtendArray.CreateArray(i => new Item(), Main.InventorySlotsTotal);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="pickaxe">The pickaxe.</param>
    /// <param name="x">Tile X coordinate.</param>
    /// <param name="y">Tile Y coordinate.</param>
    /// <returns>Whether or not the player has enough pickaxe power to break this tile.</returns>
    public static System.Boolean CheckPickPower(this Player player, Item pickaxe, System.Int32 x, System.Int32 y) {
        var inv = player.inventory;
        _dummyInventory[0] = pickaxe;
        player.inventory = _dummyInventory;
        System.Boolean value = false;
        try {
            value = player.HasEnoughPickPowerToHurtTile(x, y);
        }
        catch {
        }
        player.inventory = inv;
        return value;
    }

    /// <summary>
    /// Gives the player an item without dropping it onto the floor. (Unless they cannot pick it up)
    /// <para>Automatically syncs the item in multiplayer.</para>
    /// <para>Note: May only work if <see cref="Main.myPlayer"/> is the <paramref name="player"/>.</para>
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="item">The Item.</param>
    /// <param name="source">Item source.</param>
    /// <param name="getItemSettings">The Get Item settings.</param>
    public static void GiveItem(this Player player, Item item, IEntitySource source, GetItemSettings getItemSettings) {
        item.Center = player.Center;
        item = player.GetItem(player.whoAmI, item, getItemSettings);

        if (item != null && !item.IsAir) {
            System.Int32 newItemIndex = Item.NewItem(source, player.getRect(), item);
            Main.item[newItemIndex].newAndShiny = false;
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                NetMessage.SendData(MessageID.SyncItem, number: newItemIndex, number2: 1f);
            }
        }
    }
    /// <summary>
    /// Gives the player an item without dropping it onto the floor. (Unless they cannot pick it up)
    /// <para>Automatically syncs the item in multiplayer.</para>
    /// <para>Note: May only work if <see cref="Main.myPlayer"/> is the <paramref name="player"/>.</para>
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="type">The Item Id.</param>
    /// <param name="stack">The Item's stack.</param>
    /// <param name="prefix">The Item's prefix.</param>
    /// <param name="source">Item source.</param>
    /// <param name="getItemSettings">The Get Item settings.</param>
    public static void GiveItem(this Player player, System.Int32 type, IEntitySource source, GetItemSettings getItemSettings, System.Int32 stack = 1, System.Int32 prefix = 0) {
        player.GiveItem(new Item(type, stack, prefix), source, getItemSettings);
    }

    public static Chest GetCurrentChest(this Player player, System.Boolean ignoreVoidBag = false) {
        if (player.chest > -1) {
            return Main.chest[player.chest];
        }
        else if (player.chest == -2) {
            return player.bank;
        }
        else if (player.chest == -3) {
            return player.bank2;
        }
        else if (player.chest == -4) {
            return player.bank3;
        }
        else if (!ignoreVoidBag && player.chest == -5) {
            return player.bank4;
        }
        return null;
    }

    public static Item HeldItemFixed(this Player player) {
        if (Main.myPlayer == player.whoAmI && player.selectedItem == 58 && Main.mouseItem != null && !Main.mouseItem.IsAir) {
            return Main.mouseItem;
        }
        return player.HeldItem;
    }

    /// <summary>
    /// Gets the "Player Focus". This is by default the player's centre, but when using the Drone, this returns the drone's position.
    /// <para>This position is used to make Radon Fog disappear when approched by the player, or by their controlled drone.</para>
    /// <para>This position also ignores camera panning effects, like screen shakes, scoping, ect.</para>
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static Vector2 GetPlayerFocusPosition(this Player player) {
        if (Main.DroneCameraTracker.TryTracking(out var dronePosition)) {
            return dronePosition;
        }

        return player.Center;
    }

    public static System.Boolean IsFalling(this Player player) {
        return Helper.IsFalling(player.velocity, player.gravDir);
    }

    /// <summary>
    /// Spawns Flask and other "Enchantment" dusts, like the Magma Stone flames.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="velocity"></param>
    /// <param name="player"></param>
    /// <param name="showMagmaStone"></param>
    /// <returns>A potential dust instance, if any spawn. If none spawn, the result will be null.</returns>
    public static Dust SpawnEnchantmentDusts(Vector2 position, Vector2 velocity, Player player, System.Boolean showMagmaStone = true) {
        if (player.magmaStone && showMagmaStone && Main.rand.NextBool(3)) {
            var d = Dust.NewDustPerfect(position, DustID.Torch, velocity * 2f, Alpha: 100, Scale: 2.5f);
            d.noGravity = true;
            return d;
        }
        switch (player.meleeEnchant) {
            case 1: {
                    if (Main.rand.NextBool(3)) {
                        var d = Dust.NewDustPerfect(position, DustID.Venom, velocity * 2f, Alpha: 100);
                        d.noGravity = true;
                        d.fadeIn = 1.5f;
                        d.velocity *= 0.25f;
                        return d;
                    }
                }
                break;

            case 2: {
                    if (Main.rand.NextBool(2)) {
                        var d = Dust.NewDustPerfect(position, DustID.CursedTorch, new Vector2(velocity.X * 0.2f * player.direction * 3f, velocity.Y * 0.2f), Alpha: 100, Scale: 2.5f);
                        d.noGravity = true;
                        d.velocity *= 0.7f;
                        d.velocity.Y -= 0.5f;
                        return d;
                    }
                }
                break;

            case 3: {
                    if (Main.rand.NextBool(2)) {
                        var d = Dust.NewDustPerfect(position, DustID.Torch, new Vector2(velocity.X * 0.2f * player.direction * 3f, velocity.Y * 0.2f), Alpha: 100, Scale: 2.5f);
                        d.noGravity = true;
                        d.velocity *= 0.7f;
                        d.velocity.Y -= 0.5f;
                        return d;
                    }
                }
                break;

            case 4: {
                    if (Main.rand.NextBool(2)) {
                        var d = Dust.NewDustPerfect(position, DustID.Enchanted_Gold, new Vector2(velocity.X * 0.2f * player.direction * 3f, velocity.Y * 0.2f), Alpha: 100, Scale: 2.5f);
                        d.noGravity = true;
                        d.velocity *= 0.7f;
                        d.velocity.Y -= 0.5f;
                        return d;
                    }
                }
                break;

            case 5: {
                    if (Main.rand.NextBool(2)) {
                        var d = Dust.NewDustPerfect(position, DustID.IchorTorch, velocity, Alpha: 100, Scale: 2.5f);
                        d.velocity.X += player.direction;
                        d.velocity.Y -= 0.2f;
                        return d;
                    }
                }
                break;

            case 6: {
                    if (Main.rand.NextBool(2)) {
                        var d = Dust.NewDustPerfect(position, DustID.IceTorch, velocity, Alpha: 100, Scale: 2.5f);
                        d.velocity.X += player.direction;
                        d.velocity.Y -= 0.2f;
                        return d;
                    }
                }
                break;

            case 7: {
                    if (Main.rand.NextBool(40)) {
                        var g = Gore.NewGorePerfect(player.GetSource_ItemUse(player.HeldItem), position, velocity, Main.rand.Next(276, 283));
                        g.velocity.X *= 1f + Main.rand.Next(-50, 51) * 0.01f;
                        g.velocity.Y *= 1f + Main.rand.Next(-50, 51) * 0.01f;
                        g.scale *= 1f + Main.rand.Next(-20, 21) * 0.01f;
                        g.velocity.X += Main.rand.Next(-50, 51) * 0.05f;
                        g.velocity.Y += Main.rand.Next(-50, 51) * 0.05f;
                    }
                    else if (Main.rand.NextBool(20)) {
                        var d = Dust.NewDustPerfect(position, Main.rand.Next(139, 143), velocity, Scale: 1.2f);
                        d.velocity.X *= 1f + Main.rand.Next(-50, 51) * 0.01f;
                        d.velocity.Y *= 1f + Main.rand.Next(-50, 51) * 0.01f;
                        d.velocity.X += Main.rand.Next(-50, 51) * 0.05f;
                        d.velocity.Y += Main.rand.Next(-50, 51) * 0.05f;
                        d.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                        return d;
                    }
                }
                break;

            case 8: {
                    if (Main.rand.NextBool(3)) {
                        var d = Dust.NewDustPerfect(position, DustID.Poisoned, velocity * 2f, Alpha: 100);
                        d.noGravity = true;
                        d.fadeIn = 1.5f;
                        d.velocity *= 0.25f;
                        return d;
                    }
                }
                break;
        }
        return null;
    }

    public static IEntitySource GetSource_HeldItem(this Player player) {
        return player.GetSource_ItemUse(player.HeldItemFixed());
    }

    #region Biomes
    public static System.Boolean ZoneGlimmer(this Player player) {
        return false;
    }
    public static System.Boolean ZoneDemonSiege(this Player player) {
        return false;
    }
    public static System.Boolean ZoneGaleStreams(this Player player) {
        return false;
    }
    #endregion

    #region Damage & Crit
    /// <summary>
    /// Checks weird damage restrictions for NPCs. This literally only checks for Fairies as of right now.
    /// </summary>
    /// <param name="npc"></param>
    /// <returns>Whether the enemy should be immune to this hit (true = Do not hit)</returns>
    public static System.Boolean WeirdNPCHitRestrictions(NPC npc) {
        return npc.aiStyle == NPCAIStyleID.Fairy && !(npc.ai[2] <= 1f);
    }

    public static System.Boolean RollCrit(this Player player, Item item) {
        return !item.DamageType.UseStandardCritCalcs ? false : Main.rand.Next(100) < player.GetWeaponCrit(item);
    }
    public static System.Boolean RollCrit<T>(this Player player) where T : DamageClass {
        return player.RollCrit(ModContent.GetInstance<T>());
    }
    public static System.Boolean RollCrit(this Player player, DamageClass damageClass) {
        return !damageClass.UseStandardCritCalcs ? false : Main.rand.Next(100) < player.GetTotalCritChance(damageClass);
    }
    #endregion

    #region Death Messages
    // Change these if tmodloader eventually adds proper custom death messages
    public static PlayerDeathReason CustomDeathReason(System.String key, params System.Object[] args) {
        return PlayerDeathReason.ByCustomReason(Language.GetTextValue(key, args));
    }

    public static PlayerDeathReason CustomDeathReason(System.String key) {
        return PlayerDeathReason.ByCustomReason(Language.GetTextValue(key));
    }
    #endregion
}