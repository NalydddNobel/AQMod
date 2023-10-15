using Aequus.Common.Graphics.Rendering.Tiles;
using Aequus.Common.Net;
using Aequus.Common.Networking;
using Aequus.Common.Tiles.Components;
using Aequus.Content.Tiles.Conductive.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Tiles.Conductive;

public class ConductiveBlockTile : ModTile, INetTileInteraction, ISpecialTileRenderer, ICustomPlaceSound, ITouchEffects, IAddRecipes {
    protected virtual void AddMapEntries() {
        AddMapEntry(new(103, 127, 174), TextHelper.GetDisplayName<ConductiveBlock>());
    }

    public override void SetStaticDefaults() {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        DustType = DustID.Copper;
        HitSound = AequusSounds.Conductive;
        AddMapEntries();
    }

    public void AddRecipes(Aequus aequus) {
        //foreach (var tileId in TileSets.Mechanical) {
        //    Main.tileMerge[Type][tileId] = true;
        //}
    }

    public override bool Slope(int i, int j) {
        return false;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

    public override void HitWire(int i, int j) {
        if (!Wiring.CheckMech(i, j, ConductiveSystem.ActivationDelay)) {
            return;
        }

        if (ConductiveSystem.PoweredLocation == Point.Zero) {
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                Projectile.NewProjectile(new EntitySource_Wiring(i, j), new(i * 16f + 8f, j * 16f + 8f), Vector2.Zero, ModContent.ProjectileType<ConductiveProjectile>(), 20, 1f, Main.myPlayer);
            }

            var oldPoweredLocation = ConductiveSystem.PoweredLocation;
            ConductiveSystem.PoweredLocation = new(i, j);
            ActivateEffect(i, j);
            ConductiveSystem.PoweredLocation = oldPoweredLocation;
            return;
        }

        if (Main.tile[ConductiveSystem.PoweredLocation].TileType != Type) {
            return;
        }

        ActivateEffect(i, j);
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        var tile = Main.tile[i, j];
        var drawCoordinates = (new Vector2(i * 16f, j * 16f) - Main.screenPosition + DrawHelper.TileDrawOffset).Floor();
        var frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
        var color = Lighting.GetColor(i, j);
        if (ConductiveSystem.ActivationPoints.TryGetValue(new(i, j), out var effect)) {
            spriteBatch.Draw(TextureAssets.Tile[Type].Value, drawCoordinates, frame, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            SpecialTileRenderer.AddSolid(i, j, TileRenderLayerID.PostDrawLiquids);
        }
        else {
            spriteBatch.Draw(TextureAssets.Tile[Type].Value, drawCoordinates, frame, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
        return false;
    }

    public void Send(int i, int j, BinaryWriter binaryWriter) {
        binaryWriter.Write(-ConductiveSystem.ActivationEffect.GetDistance(i, j));
    }

    public void Receive(int i, int j, BinaryReader binaryReader, int sender) {
        float distance = binaryReader.ReadSingle();
        ConductiveSystem.ActivationEffect.Activate(i, j, distance);
    }

    private static void ActivateEffect(int i, int j) {
        if (Main.netMode == NetmodeID.Server) {
            PacketSystem.Get<TileInteractionPacket>().Send(i, j);
        }
        else if (Main.netMode == NetmodeID.SinglePlayer) {
            ConductiveSystem.ActivationEffect.Activate(i, j, -ConductiveSystem.ActivationEffect.GetDistance(i, j));
        }
    }

    public void Render(int i, int j, byte layer) {
        if (!ConductiveSystem.ActivationPoints.TryGetValue(new(i, j), out var effect)) {
            return;
        }

        var tile = Main.tile[i, j];
        var drawCoordinates = (new Vector2(i * 16f + 8f, j * 16f + 8f) - Main.screenPosition).Floor();
        var frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
        var electricColor = new Color(255, 210, 120, 0) * effect.electricAnimation;
        var fastRandom = Helper.RandomTileCoordinates(i, j);
        float globalIntensity = effect.intensity;
        for (int k = 0; k < ConductiveSystem.ElectricOffsets.Length; k++) {
            Main.spriteBatch.Draw(TextureAssets.Tile[Type].Value, (drawCoordinates + ConductiveSystem.ElectricOffsets[k] * effect.electricAnimation).Floor(), frame, electricColor * 0.5f * globalIntensity, 0f, new Vector2(8f), 1f, SpriteEffects.None, 0f);
        }

        if (Aequus.GameWorldActive && effect.electricAnimation > 0.1f && Main.rand.NextBool(10)) {
            var d = Dust.NewDustDirect(new Vector2(i * 16f, j * 16f), 16, 16, DustID.MartianSaucerSpark, Alpha: 0, Scale: Main.rand.NextFloat(0.8f, 1.8f));
            d.rotation = 0f;
            //d.fadeIn = d.scale + 0.4f;
            d.noGravity = true;
        }

        if (!Aequus.highQualityEffects) {
            return;
        }

        var shockTexture = AequusTextures.BaseParticleTexture.Value;
        int amount = 12;
        for (int k = 0; k < amount; k++) {
            float time = (Main.GlobalTimeWrappedHourly * fastRandom.NextFloat(1f, 5f)) % 3f;
            float vectorRotationWave = Main.GlobalTimeWrappedHourly * fastRandom.NextFloat(-1f, 1f);
            float rotation = fastRandom.NextFloat(MathHelper.TwoPi) + vectorRotationWave;
            float positionMagnitude = fastRandom.NextFloat(14f, 40f);
            float colorMultiplier = fastRandom.NextFloat(0.5f, 1f);
            int frameY = fastRandom.Next(3);
            float scale = fastRandom.NextFloat(1f, 2f);
            float intensity = Math.Min(MathF.Pow(effect.electricAnimation, k) * 2f, 1f);
            if (time > 1f) {
                continue;
            }

            var shockFrame = shockTexture.Frame(verticalFrames: 3, frameY: frameY);
            float wave = MathF.Sin(time * MathHelper.Pi);
            Main.spriteBatch.Draw(shockTexture,
                (drawCoordinates + new Vector2((1f - MathF.Pow(1f - time, 2f)) * positionMagnitude, 0f).RotatedBy(rotation)).Floor(),
                shockFrame,
                electricColor * wave * 2f * colorMultiplier * intensity * globalIntensity,
                rotation,
                shockFrame.Size() / 2f,
                new Vector2(scale, 1f) * wave * 0.75f * intensity,
                SpriteEffects.None,
                0f);
        }
    }

    public void PlaySound(int i, int j, bool forced, int plr, int style, bool PlaceTile) {
        if (PlaceTile) {
            SoundEngine.PlaySound(AequusSounds.Conductive with { Pitch = 0.6f, PitchVariance = 0.05f }, new Vector2(i * 16f + 8f, j * 16f + 8f));
        }
    }

    public void Touch(int i, int j, Player player, AequusPlayer aequusPlayer) {
        if (!ConductiveSystem.ActivationPoints.TryGetValue(new(i, j), out var effect) || effect.electricAnimation < 0.5f) {
            return;
        }

        player.Hurt(PlayerHelper.CustomDeathReason("Mods.Aequus.Player.DeathMessage.Conductive." + Main.rand.Next(5), player.name), 120, 0, cooldownCounter: ImmunityCooldownID.TileContactDamage, knockback: 0f);
    }
}