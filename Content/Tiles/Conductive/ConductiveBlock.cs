using Aequus.Common.Net;
using Aequus.Common.Networking;
using Aequus.Common.Tiles.Components;
using Aequus.Core.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Tiles.Conductive;
public class ConductiveBlock : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<ConductiveBlockTile>());
        Item.value = Item.buyPrice(silver: 1);
    }

    public override void AddRecipes() {
        CreateRecipe(3)
            .AddIngredient(ItemID.CopperBar)
            .Register();
        CreateRecipe(3)
            .AddIngredient(ItemID.TinBar)
            .Register();

        Recipe.Create(ModContent.ItemType<ConductiveBlockDecor>())
            .AddIngredient(Type)
            .Register();
    }
}

public class ConductiveBlockDecor : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<ConductiveBlockTileDecor>());
        Item.value = Item.buyPrice(silver: 1);
    }

    public override void AddRecipes() {
        Recipe.Create(ModContent.ItemType<ConductiveBlock>())
            .AddIngredient(Type)
            .Register();
    }
}

public class ConductiveBlockTile : ModTile, INetTileInteraction {
    public override void SetStaticDefaults() {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        DustType = DustID.Copper;
        HitSound = SoundID.Tink;
        AddMapEntry(new(103, 127, 174), TextHelper.GetDisplayName<ConductiveBlock>());
    }

    public override bool Slope(int i, int j) {
        return false;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

    public override void HitWire(int i, int j) {
        if (ConductiveProjectile.PoweredLocation == Point.Zero) {
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                Projectile.NewProjectile(new EntitySource_Wiring(i, j), new(i * 16f + 8f, j * 16f + 8f), Vector2.Zero, ModContent.ProjectileType<ConductiveProjectile>(), 20, 1f, Main.myPlayer);
            }
            return;
        }

        if (Main.netMode == NetmodeID.Server) {
            PacketSystem.Get<TileInteractionPacket>().Send(i, j);
        }
        else if (Main.netMode == NetmodeID.SinglePlayer) {
            ActivateEffect(i, j, GetIntensity(i, j));
        }
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        var tile = Main.tile[i, j];
        var drawCoordinates = (new Vector2(i * 16f, j * 16f) - Main.screenPosition + DrawHelper.TileDrawOffset).Floor();
        var frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
        var color = Lighting.GetColor(i, j);
        if (ConductiveSystem.ActivationPoints.TryGetValue(new(i, j), out var effect)) {
            spriteBatch.Draw(TextureAssets.Tile[Type].Value, drawCoordinates + effect.tileDrawOffset, frame, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(AequusTextures.ConductiveBlockTileElectricity, drawCoordinates + effect.tileDrawOffset, frame, Color.Yellow * effect.intensity, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
        else {
            spriteBatch.Draw(TextureAssets.Tile[Type].Value, drawCoordinates, frame, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
        return false;
    }

    public void Send(int i, int j, BinaryWriter binaryWriter) {
        binaryWriter.Write(GetIntensity(i, j));
    }

    public void Receive(int i, int j, BinaryReader binaryReader, int sender) {
        float intensity = binaryReader.ReadSingle();
        ActivateEffect(i, j, intensity);
    }

    public static float GetIntensity(int i, int j) {
        return 1f - Math.Clamp(Vector2.Distance(new Vector2(i, j), ConductiveProjectile.PoweredLocation.ToVector2()) / ConductiveProjectile.WireMax, 0f, 0.66f);
    }

    public static void ActivateEffect(int i, int j, float intensity) {
        var point = new Point(i, j);
        if (ConductiveSystem.ActivationPoints.TryGetValue(point, out var effect)) {
            effect.intensity = Math.Max(effect.intensity, intensity);
        }
        else {
            ConductiveSystem.ActivationPoints[point] = new() { intensity = intensity };
        }
    }
}

public class ConductiveBlockTileDecor : ModTile {
    public override void SetStaticDefaults() {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        AddMapEntry(new(90, 127, 174));
        DustType = DustID.Copper;
        HitSound = SoundID.Tink;
        Main.tileMerge[Type][ModContent.TileType<ConductiveBlockTile>()] = true;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
}