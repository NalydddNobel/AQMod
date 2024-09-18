﻿using Aequus.Common.Items;
using Aequus.NPCs.Town.PhysicistNPC.Analysis;
using Aequus.Projectiles.Misc;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Aequus.Tiles.Blocks;
public class EmancipationGrill : ModItem, ItemHooks.ICustomCanPlace {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 100;
        AnalysisSystem.IgnoreItem.Add(Type);
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<EmancipationGrillTile>());
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.buyPrice(copper: 50);
    }

    public override Color? GetAlpha(Color lightColor) {
        return new Color(255, 255, 255, 100);
    }

    public bool? CheckCanPlace(Player player, int i, int j) {
        return ItemHooks.ICustomCanPlace.BubbleTilePlacement(i, j) ? true : null;
    }
}

public class EmancipationGrillTile : ModTile {
    public override void SetStaticDefaults() {
        Main.tileLighted[Type] = true;
        AddMapEntry(Color.CornflowerBlue * 1.1f, TextHelper.GetItemName<EmancipationGrill>());
        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);
        DustType = DustID.Electric;
        MineResist = 0.1f;
        PhysicsGunProj.TileBlocksLaser.Add(Type);
    }

    public override bool Slope(int i, int j) {
        return false;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = fail ? 1 : 3;
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        r = 0.2f;
        g = 0.5f;
        b = 0.75f;
    }

    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
    }

    public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch) {
    }

    public override void HitWire(int i, int j) {
        Main.tile[i, j].Actuated(!Main.tile[i, j].IsActuated);
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        var texture = TextureAssets.Tile[Type].Value;
        var drawPosition = (new Vector2(i * 16f, j * 16f) + Helper.TileDrawOffset - Main.screenPosition).Floor();
        var frame = new Rectangle(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY, 16, 16);

        float multiplier = 1f;
        if (Main.tile[i, j].IsActuated) {
            multiplier = 0.1f;
        }
        else {
            CheckForPlayers(i, j);
        }

        spriteBatch.Draw(texture, drawPosition, frame, new Color(255, 255, 255, 100) * multiplier, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

        var array = new Vector2[]
        {
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(1f, -1f),
            new Vector2(0f, -1f),
            new Vector2(-1f, -1f),
            new Vector2(-1f, 0f),
            new Vector2(-1f, 1f),
        };

        for (int k = 0; k < array.Length; k++) {
            var offset = new Vector2(1f * Math.Sign(array[k].X), 1f * Math.Sign(array[k].Y));
            spriteBatch.Draw(texture, drawPosition + offset * 2f, frame, new Color(100, 100, 255, 100) * multiplier * Helper.Wave(Main.GlobalTimeWrappedHourly * 5f + k / 4f * MathHelper.TwoPi + i + j, 0.5f, 1f), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
        for (int k = 0; k < array.Length; k++) {
            var offset = new Vector2(2f * Math.Sign(array[k].X), 2f * Math.Sign(array[k].Y));
            spriteBatch.Draw(texture, drawPosition + offset * 2f, frame, new Color(100, 100, 255, 100) * multiplier * Helper.Wave(Main.GlobalTimeWrappedHourly * 15f + k / 4f * MathHelper.TwoPi + i + j + MathHelper.Pi, 0.05f, 0.25f), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
        return false;
    }

    public void CheckForPlayers(int i, int j) {
        bool playSound = false;
        if (Main.LocalPlayer.getRect().Intersects(new Rectangle(i * 16, j * 16, 16, 16))) {
            for (int k = 0; k < Main.maxProjectiles; k++) {
                if (Main.projectile[k].active && Main.projectile[k].owner == Main.myPlayer && (Main.projectile[k].type == ProjectileID.PortalGunBolt || Main.projectile[k].type == ProjectileID.PortalGunGate)) {
                    Main.projectile[k].Kill();
                    playSound = true;
                }
            }
        }
        if (playSound) {
            SoundEngine.PlaySound(SoundID.Item8, new Vector2(i, j).ToWorldCoordinates());
        }
    }
}