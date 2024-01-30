using Aequus.Common.Tiles;
using Aequus.Common.Tiles.Components;
using Aequus.Core.Graphics.Tiles;
using Aequus.Core.Initialization;
using System;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Aequus.Content.Tiles.Conductive;

[LegacyName("ConductiveBlockTile")]
public class ConductiveBlock : ModTile, INetTileInteraction, ISpecialTileRenderer, ICustomPlaceSound, ITouchEffects, IAddRecipes {
    public virtual Int32 BarItem => ItemID.CopperBar;
    public virtual Color MapColor => new(183, 88, 25);

    public override void Load() {
        ModItem item = new InstancedTileItem(this, value: Item.buyPrice(silver: 1));
        Mod.AddContent(item);

        LoadingSteps.EnqueueAddRecipes(() => {
            item.CreateRecipe()
                .AddIngredient(BarItem, 1)
                .AddTile(TileID.Furnaces)
                .Register();
        });
    }

    public override void SetStaticDefaults() {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        DustType = DustID.Copper;
        HitSound = AequusSounds.ConductiveBlock;
        AddMapEntry(MapColor, CreateMapEntryName());
    }

    public void AddRecipes(Aequus aequus) {
        //foreach (var tileId in TileSets.Mechanical) {
        //    Main.tileMerge[Type][tileId] = true;
        //}
    }

    public override Boolean Slope(Int32 i, Int32 j) {
        return false;
    }

    public override void NumDust(Int32 i, Int32 j, Boolean fail, ref Int32 num) => num = fail ? 1 : 3;

    public override void HitWire(Int32 i, Int32 j) {
        ActivateEffect(i, j);
        if (!Wiring.CheckMech(i, j, ConductiveSystem.PoweredLocation == Point.Zero ? ConductiveSystem.ActivationDelay : 0)) {
            return;
        }

        if (ConductiveSystem.PoweredLocation == Point.Zero) {
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                Projectile.NewProjectile(new EntitySource_Wiring(i, j), new(i * 16f + 8f, j * 16f + 8f), Vector2.Zero, ModContent.ProjectileType<ConductiveProjectile>(), 20, 1f, Main.myPlayer);
            }

            var oldPoweredLocation = ConductiveSystem.PoweredLocation;
            ConductiveSystem.PoweredLocation = new(i, j);
            ConductiveSystem.PoweredLocation = oldPoweredLocation;
            return;
        }

        if (Main.tile[ConductiveSystem.PoweredLocation].TileType != Type) {
            return;
        }
    }

    public override Boolean PreDraw(Int32 i, Int32 j, SpriteBatch spriteBatch) {
        var tile = Main.tile[i, j];
        var drawCoordinates = (new Vector2(i * 16f, j * 16f) - Main.screenPosition + TileHelper.DrawOffset).Floor();
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

    public void Send(Int32 i, Int32 j, BinaryWriter binaryWriter) {
        binaryWriter.Write(-ConductiveSystem.ActivationEffect.GetDistance(i, j));
    }

    public void Receive(Int32 i, Int32 j, BinaryReader binaryReader, Int32 sender) {
        Single distance = binaryReader.ReadSingle();
        ConductiveSystem.ActivationEffect.Activate(i, j, distance);
    }

    private static void ActivateEffect(Int32 i, Int32 j) {
        if (Main.netMode == NetmodeID.Server) {
            Aequus.GetPacket<TileInteractionPacket>().Send(i, j);
        }
        else if (Main.netMode == NetmodeID.SinglePlayer) {
            ConductiveSystem.ActivationEffect.Activate(i, j, -ConductiveSystem.ActivationEffect.GetDistance(i, j));
        }
    }

    public void Render(Int32 i, Int32 j, Byte layer) {
        if (!ConductiveSystem.ActivationPoints.TryGetValue(new(i, j), out var effect)) {
            return;
        }

        var tile = Main.tile[i, j];
        var drawCoordinates = (new Vector2(i * 16f + 8f, j * 16f + 8f) - Main.screenPosition).Floor();
        var frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
        var electricColor = new Color(255, 210, 120, 0) * effect.electricAnimation;
        var fastRandom = Helper.RandomTileCoordinates(i, j);
        Single globalIntensity = effect.intensity;
        for (Int32 k = 0; k < ConductiveSystem.ElectricOffsets.Length; k++) {
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
        Int32 amount = 12;
        for (Int32 k = 0; k < amount; k++) {
            Single time = (Main.GlobalTimeWrappedHourly * fastRandom.NextFloat(1f, 5f)) % 3f;
            Single vectorRotationWave = Main.GlobalTimeWrappedHourly * fastRandom.NextFloat(-1f, 1f);
            Single rotation = fastRandom.NextFloat(MathHelper.TwoPi) + vectorRotationWave;
            Single positionMagnitude = fastRandom.NextFloat(14f, 40f);
            Single colorMultiplier = fastRandom.NextFloat(0.5f, 1f);
            Int32 frameY = fastRandom.Next(3);
            Single scale = fastRandom.NextFloat(1f, 2f);
            Single intensity = Math.Min(MathF.Pow(effect.electricAnimation, k) * 2f, 1f);
            if (time > 1f) {
                continue;
            }

            var shockFrame = shockTexture.Frame(verticalFrames: 3, frameY: frameY);
            Single wave = MathF.Sin(time * MathHelper.Pi);
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

    public void PlaySound(Int32 i, Int32 j, Boolean forced, Int32 plr, Int32 style, Boolean PlaceTile) {
        if (PlaceTile) {
            SoundEngine.PlaySound(AequusSounds.ConductiveBlockPlaced, new Vector2(i * 16f + 8f, j * 16f + 8f));
        }
    }

    public void Touch(Int32 i, Int32 j, Player player, AequusPlayer aequusPlayer) {
        if (!ConductiveSystem.ActivationPoints.TryGetValue(new(i, j), out var effect) || effect.electricAnimation < 0.5f) {
            return;
        }

        player.Hurt(PlayerHelper.CustomDeathReason("Mods.Aequus.Player.DeathMessage.Conductive." + Main.rand.Next(5), player.name), 120, 0, cooldownCounter: ImmunityCooldownID.TileContactDamage, knockback: 0f);
    }
}