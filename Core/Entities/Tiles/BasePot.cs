using AequusRemake.DataSets;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ObjectData;
using Terraria.Utilities;

namespace AequusRemake.Core.Entities.Tiles;

public abstract class BasePot : ModTile {
    public virtual Color MapColor => new Color(81, 84, 101);

    private bool _multiTile = false;

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;
        Main.tileCut[Type] = true;

        TileID.Sets.DisableSmartCursor[Type] = true;

        TileDataSet.IsSmashablePot.Add(Type);

        SetupTileObjectData();
        _multiTile = TileObjectData.newTile.Width > 1 || TileObjectData.newTile.Height > 1;
        TileObjectData.addTile(Type);

        HitSound = SoundID.Shatter;

        AddMapEntry(MapColor, Language.GetText("MapObject.Pot"));
    }

    protected virtual void SetupTileObjectData() {
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.StyleWrapLimit = 3;
    }

    protected virtual bool PreDropEffects(int i, int j) {
        return true;
    }

    protected virtual bool DoSpecialBiomeTorch(ref int itemID) {
        return false;
    }

    protected virtual int ChooseGlowstick(int i, int j) {
        return ItemID.Glowstick;
    }

    protected virtual string GoreName(int i, int j, int frameX, int frameY) {
        return $"{Name}_";
    }

    protected virtual void DropGores(int i, int j) {
        string path = GoreName(i, j, Main.tile[i, j].TileFrameX / 36, Main.tile[i, j].TileFrameY / 36);
        try {
            Gore.NewGore(new EntitySource_TileBreak(i, j), new Vector2(i * 16, j * 16), default(Vector2), Mod.Find<ModGore>($"{path}0").Type);
            Gore.NewGore(new EntitySource_TileBreak(i, j), new Vector2(i * 16, j * 16), default(Vector2), Mod.Find<ModGore>($"{path}1").Type);
            Gore.NewGore(new EntitySource_TileBreak(i, j), new Vector2(i * 16, j * 16), default(Vector2), Mod.Find<ModGore>($"{path}2").Type);
        }
        catch {
        }
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY) {
        if (_multiTile) {
            Drop(i, j);
        }
    }

    public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem) {
        if (!_multiTile && !noItem) {
            Drop(i, j);
        }
    }

    private void Drop(int i, int j) {
        if (Main.netMode != NetmodeID.Server) {
            DropGores(i, j);
        }
    }

    public override IEnumerable<Item> GetItemDrops(int i, int j) {
        //bool flag = false;
        //int num = 0;
        //int num9 = j;
        //for (num += Main.tile[i, j].TileFrameX / 18; num > 1; num -= 2) {
        //}
        //num *= -1;
        //num += i;
        //int num15 = Main.tile[i, j].TileFrameY / 18;
        //int num16 = 0;
        //while (num15 > 1) {
        //    num15 -= 2;
        //    num16++;
        //}
        //num9 -= num15;
        //for (int k = num; k < num + 2; k++) {
        //    for (int l = num9; l < num9 + 2; l++) {
        //        int num17;
        //        for (num17 = Main.tile[k, l].TileFrameX / 18; num17 > 1; num17 -= 2) {
        //        }
        //        if (!Main.tile[k, l].HasTile || Main.tile[k, l].TileType != Type || num17 != k - num || Main.tile[k, l].TileFrameY != (l - num9) * 18 + num16 * 36) {
        //            flag = true;
        //        }
        //    }
        //    if (!WorldGen.SolidTile2(k, num9 + 2)) {
        //        flag = true;
        //    }
        //}
        //if (!flag) {
        //    return;
        //}
        var tileObjectData = TileObjectData.GetTileData(Main.tile[i, j]);
        i -= Main.tile[i, j].TileFrameX % tileObjectData.CoordinateFullWidth / tileObjectData.CoordinateWidth;
        j -= Main.tile[i, j].TileFrameX % tileObjectData.CoordinateFullHeight / tileObjectData.CoordinateHeights[0];

        var mainRand = Main.rand;
        var genRand = WorldGen._genRand;
        var randomizer = new UnifiedRandom((int)Helper.TileSeed(i, j));
        Main.rand = randomizer;
        WorldGen._genRand = randomizer;

        if (PreDropEffects(i, j)) {
            float valueMultiplier = 1f;
            bool undergroundDesert = false;

            valueMultiplier = (valueMultiplier * 2f + 1f) / 3f;
            int coinPortalChance = (int)(500f / ((valueMultiplier + 1f) / 2f));
            if (!WorldGen.gen) {
                var source = new EntitySource_TileBreak(i, j);
                if (Player.GetClosestRollLuck(i, j, coinPortalChance) == 0f) {
                    if (Main.netMode != NetmodeID.MultiplayerClient) {
                        Projectile.NewProjectile(source, i * 16 + 16, j * 16 + 16, 0f, -12f, ProjectileID.CoinPortal, 0, 0f, Main.myPlayer);
                    }
                }
                else if (WorldGen.genRand.NextBool(35) && Main.wallDungeon[Main.tile[i, j].WallType] && j > Main.worldSurface) {
                    yield return new Item(ItemID.GoldenKey);
                }
                else if (Main.getGoodWorld && WorldGen.genRand.NextBool(4)) {
                    Projectile.NewProjectile(source, i * 16 + 16, j * 16 + 8, Main.rand.Next(-100, 101) * 0.002f, 0f, ProjectileID.Bomb, 0, 0f, Player.FindClosest(new Vector2(i * 16, j * 16), 16, 16));
                }
                else if (WorldGen.genRand.NextBool(45) || WorldGen.genRand.NextBool(45) && Main.expertMode) {
                    if (j < Main.worldSurface) {
                        int pot;
                        int stack = 1;
                        switch (WorldGen.genRand.Next(10)) {
                            case 0:
                                pot = ItemID.IronskinPotion;
                                break;
                            case 1:
                                pot = ItemID.ShinePotion;
                                break;
                            case 2:
                                pot = ItemID.NightOwlPotion;
                                break;
                            case 3:
                                pot = ItemID.BattlePotion;
                                break;
                            case 4:
                                pot = ItemID.MiningPotion;
                                break;
                            case 5:
                                pot = ItemID.MiningPotion;
                                break;
                            case 6:
                                pot = ItemID.CalmingPotion;
                                break;
                            case 7:
                                pot = ItemID.HeartreachPotion;
                                break;
                            default:
                                pot = ItemID.RecallPotion;
                                stack = WorldGen.genRand.Next(3) + 1;
                                break;
                        }
                        yield return new Item(pot, stack);
                    }
                    else if (j < Main.rockLayer) {
                        int random = WorldGen.genRand.Next(11);
                        int potItem = 0;
                        switch (random) {
                            case 0:
                                potItem = ItemID.RegenerationPotion;
                                break;
                            case 1:
                                potItem = ItemID.ShinePotion;
                                break;
                            case 2:
                                potItem = ItemID.NightOwlPotion;
                                break;
                            case 3:
                                potItem = ItemID.SwiftnessPotion;
                                break;
                            case 4:
                                potItem = ItemID.ArcheryPotion;
                                break;
                            case 5:
                                potItem = ItemID.GillsPotion;
                                break;
                            case 6:
                                potItem = ItemID.HunterPotion;
                                break;
                            case 7:
                                potItem = ItemID.MiningPotion;
                                break;
                            case 8:
                                potItem = ItemID.TrapsightPotion;
                                break;
                        }

                        if (potItem != 0) {
                            yield return new Item(potItem);
                        }

                        if (random >= 7) {
                            yield return new Item(2350, WorldGen.genRand.Next(1, 3));
                        }
                    }
                    else if (j < Main.UnderworldLayer) {
                        int randomItemChoice = WorldGen.genRand.Next(15);
                        if (randomItemChoice == 0) {
                            yield return new Item(296);
                        }
                        if (randomItemChoice == 1) {
                            yield return new Item(295);
                        }
                        if (randomItemChoice == 2) {
                            yield return new Item(299);
                        }
                        if (randomItemChoice == 3) {
                            yield return new Item(302);
                        }
                        if (randomItemChoice == 4) {
                            yield return new Item(303);
                        }
                        if (randomItemChoice == 5) {
                            yield return new Item(305);
                        }
                        if (randomItemChoice == 6) {
                            yield return new Item(301);
                        }
                        if (randomItemChoice == 7) {
                            yield return new Item(302);
                        }
                        if (randomItemChoice == 8) {
                            yield return new Item(297);
                        }
                        if (randomItemChoice == 9) {
                            yield return new Item(304);
                        }
                        if (randomItemChoice == 10) {
                            yield return new Item(2322);
                        }
                        if (randomItemChoice == 11) {
                            yield return new Item(2323);
                        }
                        if (randomItemChoice == 12) {
                            yield return new Item(2327);
                        }
                        if (randomItemChoice == 13) {
                            yield return new Item(2329);
                        }
                        if (randomItemChoice >= 7) {
                            yield return new Item(2350, WorldGen.genRand.Next(1, 3));
                        }
                    }
                    else {
                        int randomItemChoice = WorldGen.genRand.Next(14);
                        if (randomItemChoice == 0) {
                            yield return new Item(296);
                        }
                        if (randomItemChoice == 1) {
                            yield return new Item(295);
                        }
                        if (randomItemChoice == 2) {
                            yield return new Item(293);
                        }
                        if (randomItemChoice == 3) {
                            yield return new Item(288);
                        }
                        if (randomItemChoice == 4) {
                            yield return new Item(294);
                        }
                        if (randomItemChoice == 5) {
                            yield return new Item(297);
                        }
                        if (randomItemChoice == 6) {
                            yield return new Item(304);
                        }
                        if (randomItemChoice == 7) {
                            yield return new Item(305);
                        }
                        if (randomItemChoice == 8) {
                            yield return new Item(301);
                        }
                        if (randomItemChoice == 9) {
                            yield return new Item(302);
                        }
                        if (randomItemChoice == 10) {
                            yield return new Item(288);
                        }
                        if (randomItemChoice == 11) {
                            yield return new Item(300);
                        }
                        if (randomItemChoice == 12) {
                            yield return new Item(2323);
                        }
                        if (randomItemChoice == 13) {
                            yield return new Item(2326);
                        }
                        if (WorldGen.genRand.NextBool(5)) {
                            yield return new Item(4870);
                        }
                    }
                }
                else if (Main.netMode == NetmodeID.Server && Main.rand.NextBool(30)) {
                    yield return new Item(ItemID.WormholePotion);
                }
                else {
                    int randomOption = Main.rand.Next(7);
                    if (Main.expertMode) {
                        randomOption--;
                    }
                    var player = Main.player[Player.FindClosest(new Vector2(i * 16, j * 16), 16, 16)];
                    int carriedTorches = 0;
                    int torchThreshold = 20;
                    for (int num5 = 0; num5 < 50; num5++) {
                        Item item = player.inventory[num5];
                        if (!item.IsAir && item.createTile > -1 && TileID.Sets.Torch[item.createTile]) {
                            carriedTorches += item.stack;
                            if (carriedTorches >= torchThreshold) {
                                break;
                            }
                        }
                    }
                    bool hasPlentyOfTorches = carriedTorches < torchThreshold;
                    if (randomOption == 0 && player.statLife < player.statLifeMax2) {
                        yield return new Item(ItemID.Heart);
                        if (Main.rand.NextBool(2)) {
                            yield return new Item(ItemID.Heart);
                        }
                        if (Main.expertMode) {
                            if (Main.rand.NextBool(2)) {
                                yield return new Item(ItemID.Heart);
                            }
                            if (Main.rand.NextBool(2)) {
                                yield return new Item(ItemID.Heart);
                            }
                        }
                    }
                    else if (randomOption == 1 || randomOption == 0 && hasPlentyOfTorches) {
                        int stack = Main.rand.Next(2, 7);
                        if (Main.expertMode) {
                            stack += Main.rand.Next(1, 7);
                        }
                        int torchType = 8;
                        int glowstickType = ChooseGlowstick(i, j);

                        if (player.ZoneHallow) {
                            stack += Main.rand.Next(2, 7);
                            torchType = ItemID.HallowedTorch;
                        }
                        else if (player.ZoneCrimson) {
                            stack += Main.rand.Next(2, 7);
                            torchType = ItemID.CrimsonTorch;
                        }
                        else if (player.ZoneCorrupt) {
                            stack += Main.rand.Next(2, 7);
                            torchType = ItemID.CorruptTorch;
                        }
                        else if (DoSpecialBiomeTorch(ref torchType)) {
                            stack += Main.rand.Next(2, 7);
                        }

                        if (Main.tile[i, j].LiquidAmount > 0) {
                            yield return new Item(glowstickType, stack);
                        }
                        else {
                            yield return new Item(torchType, stack);
                        }
                    }
                    else if (randomOption == 2) {
                        int stack = Main.rand.Next(10, 21);
                        int arrowType = 40;
                        if (j < Main.rockLayer && WorldGen.genRand.NextBool(2)) {
                            arrowType = !Main.hardMode ? ItemID.Shuriken : ItemID.Grenade;
                        }
                        if (j > Main.UnderworldLayer) {
                            arrowType = ItemID.HellfireArrow;
                        }
                        else if (Main.hardMode) {
                            arrowType = !Main.rand.NextBool(2) ? ItemID.UnholyArrow : WorldGen.SavedOreTiers.Silver != TileID.Tungsten ? ItemID.SilverBullet : ItemID.TungstenBullet;
                        }

                        yield return new Item(arrowType, stack);
                    }
                    else if (randomOption == 3) {
                        int healingPotion = ItemID.LesserHealingPotion;
                        if (j > Main.UnderworldLayer || Main.hardMode) {
                            healingPotion = ItemID.HealingPotion;
                        }
                        int stack = 1;
                        if (Main.expertMode && !Main.rand.NextBool(3)) {
                            stack++;
                        }
                        yield return new Item(healingPotion, stack);
                    }
                    else if (randomOption == 4 && (undergroundDesert || j > Main.rockLayer)) {
                        int bomb = ItemID.Bomb;
                        if (undergroundDesert) {
                            bomb = ItemID.ScarabBomb;
                        }
                        int stack = Main.rand.Next(4) + 1;
                        if (Main.expertMode) {
                            stack += Main.rand.Next(4);
                        }
                        yield return new Item(bomb, stack);
                    }
                    else if ((randomOption == 4 || randomOption == 5) && j < Main.UnderworldLayer && !Main.hardMode) {
                        int stack = Main.rand.Next(20, 41);
                        yield return new Item(ItemID.Rope, stack);
                    }
                    else {
                        float valueDropped = 200 + WorldGen.genRand.Next(-100, 101);
                        if (j < Main.worldSurface) {
                            valueDropped *= 0.5f;
                        }
                        else if (j < Main.rockLayer) {
                            valueDropped *= 0.75f;
                        }
                        else if (j > Main.maxTilesY - 250) {
                            valueDropped *= 1.25f;
                        }
                        valueDropped *= 1f + Main.rand.Next(-20, 21) * 0.01f;
                        if (Main.rand.NextBool(4)) {
                            valueDropped *= 1f + Main.rand.Next(5, 11) * 0.01f;
                        }
                        if (Main.rand.NextBool(8)) {
                            valueDropped *= 1f + Main.rand.Next(10, 21) * 0.01f;
                        }
                        if (Main.rand.NextBool(12)) {
                            valueDropped *= 1f + Main.rand.Next(20, 41) * 0.01f;
                        }
                        if (Main.rand.NextBool(16)) {
                            valueDropped *= 1f + Main.rand.Next(40, 81) * 0.01f;
                        }
                        if (Main.rand.NextBool(20)) {
                            valueDropped *= 1f + Main.rand.Next(50, 101) * 0.01f;
                        }
                        if (Main.expertMode) {
                            valueDropped *= 2.5f;
                        }
                        if (Main.expertMode && Main.rand.NextBool(2)) {
                            valueDropped *= 1.25f;
                        }
                        if (Main.expertMode && Main.rand.NextBool(3)) {
                            valueDropped *= 1.5f;
                        }
                        if (Main.expertMode && Main.rand.NextBool(4)) {
                            valueDropped *= 1.75f;
                        }
                        valueDropped *= valueMultiplier;
                        if (NPC.downedBoss1) {
                            valueDropped *= 1.1f;
                        }
                        if (NPC.downedBoss2) {
                            valueDropped *= 1.1f;
                        }
                        if (NPC.downedBoss3) {
                            valueDropped *= 1.1f;
                        }
                        if (NPC.downedMechBoss1) {
                            valueDropped *= 1.1f;
                        }
                        if (NPC.downedMechBoss2) {
                            valueDropped *= 1.1f;
                        }
                        if (NPC.downedMechBoss3) {
                            valueDropped *= 1.1f;
                        }
                        if (NPC.downedPlantBoss) {
                            valueDropped *= 1.1f;
                        }
                        if (NPC.downedQueenBee) {
                            valueDropped *= 1.1f;
                        }
                        if (NPC.downedGolemBoss) {
                            valueDropped *= 1.1f;
                        }
                        if (NPC.downedPirates) {
                            valueDropped *= 1.1f;
                        }
                        if (NPC.downedGoblins) {
                            valueDropped *= 1.1f;
                        }
                        if (NPC.downedFrost) {
                            valueDropped *= 1.1f;
                        }
                        while ((int)valueDropped > 0) {
                            if (valueDropped > Item.platinum) {
                                int num11 = (int)(valueDropped / 1000000f);
                                if (num11 > 50 && Main.rand.NextBool(2)) {
                                    num11 /= Main.rand.Next(3) + 1;
                                }
                                if (Main.rand.NextBool(2)) {
                                    num11 /= Main.rand.Next(3) + 1;
                                }
                                valueDropped -= 1000000 * num11;
                                yield return new Item(ItemID.PlatinumCoin, num11);
                                continue;
                            }
                            if (valueDropped > Item.gold) {
                                int num12 = (int)(valueDropped / 10000f);
                                if (num12 > 50 && Main.rand.NextBool(2)) {
                                    num12 /= Main.rand.Next(3) + 1;
                                }
                                if (Main.rand.NextBool(2)) {
                                    num12 /= Main.rand.Next(3) + 1;
                                }
                                valueDropped -= 10000 * num12;
                                yield return new Item(ItemID.GoldCoin, num12);
                                continue;
                            }
                            if (valueDropped > Item.silver) {
                                int num13 = (int)(valueDropped / 100f);
                                if (num13 > 50 && Main.rand.NextBool(2)) {
                                    num13 /= Main.rand.Next(3) + 1;
                                }
                                if (Main.rand.NextBool(2)) {
                                    num13 /= Main.rand.Next(3) + 1;
                                }
                                valueDropped -= 100 * num13;
                                yield return new Item(ItemID.SilverCoin, num13);
                                continue;
                            }
                            int coinsDropped = (int)valueDropped;
                            if (coinsDropped > 50 && Main.rand.NextBool(2)) {
                                coinsDropped /= Main.rand.Next(3) + 1;
                            }
                            if (Main.rand.NextBool(2)) {
                                coinsDropped /= Main.rand.Next(4) + 1;
                            }
                            if (coinsDropped < 1) {
                                coinsDropped = 1;
                            }
                            valueDropped -= coinsDropped;
                            yield return new Item(ItemID.CopperCoin, coinsDropped);
                        }
                    }
                }
            }
        }

        Main.rand = mainRand;
        WorldGen._genRand = genRand;
    }
}