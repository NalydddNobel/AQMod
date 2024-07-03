using Aequu2.Core.Assets;
using Aequu2.Core.CodeGeneration;
using System;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace Aequu2.Content.Items.PermaPowerups.NetherStar;

[WorkInProgress]
[Gen.AequusPlayer_SavedField<bool>("usedConvergentHeart")]
public class NetherStar : ModItem {
    public const string MiscShaderKey = "Aequu2:NetherStar";

    public override void Load() {
        if (!Main.dedServ) {
            GameShaders.Misc[MiscShaderKey] = new MiscShaderData(AequusShaders.FadeToCenter, "FadeToCenterPass")
                .UseImage1(AequusTextures.NetherStarMask)
                .UseImage2(AequusTextures.EffectNoise);
        }
    }

    public override void SetStaticDefaults() {
        ItemSets.ItemNoGravity[Type] = true;
    }

    public override void SetDefaults() {
        Item.useTime = 45;
        Item.useAnimation = 45;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.width = 24;
        Item.height = 24;
        Item.consumable = true;
        Item.UseSound = SoundID.Item92;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ItemRarityID.Purple;
        Item.value = Item.sellPrice(gold: 2);
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.White;
    }

    public override bool? UseItem(Player player) {
        var Aequu2Player = player.GetModPlayer<AequusPlayer>();
        if (Aequu2Player.usedConvergentHeart) {
            return false;
        }
        Aequu2Player.usedConvergentHeart = true;
        return true;
    }

    private void DrawBackground(SpriteBatch spriteBatch, Vector2 drawCoordinates, float scale) {
        var backgroundTexture = AequusTextures.NetherStarBackground;
        var backgroundOrigin = backgroundTexture.Size() / 2f;
        var shader = GameShaders.Misc[MiscShaderKey];
        float timer = Main.GlobalTimeWrappedHourly * 4f;
        float backgroundScale = Helper.Oscillate(timer, 1f);
        float sautration = (1f - MathF.Pow(Helper.Oscillate(timer, 1f), 2f)) * 10f;
        shader.UseSaturation(sautration);
        for (int i = 0; i < 2; i++) {
            var backgroundColor = Color.Lerp(Color.Violet, Color.Blue, Helper.Oscillate(timer + i * MathHelper.PiOver2, 1f)) with { A = 0 } * backgroundScale * 0.35f;
            DrawData dd = new(backgroundTexture, drawCoordinates, null, backgroundColor, (int)(timer / MathHelper.TwoPi) * (1 + i), backgroundOrigin, scale * (0.4f + backgroundScale * 0.66f), SpriteEffects.None, 0f);
            shader.Apply(dd);
            dd.Draw(spriteBatch);
        }
    }

    private void DrawGem(SpriteBatch spriteBatch, Vector2 drawCoordinates, Rectangle itemFrame, Color drawColor, float rotation, Vector2 origin, float itemScale, float bloomScale, float flareScale) {
        var texture = TextureAssets.Item[Type].Value;
        float beatTime = Main.GlobalTimeWrappedHourly * 4f;
        float colorSwapTime = MathF.Sin(beatTime / 2f);
        Color bloomColor = Color.Lerp(Color.Violet, Color.Blue, Helper.Oscillate(beatTime, 1f)) with { A = 0 };
        Color flareColor = colorSwapTime > 0f ? drawColor with { A = 0 } * 0.6f : Color.Black with { A = drawColor.A } * 0.6f;
        float pulse = 1f + (MathF.Sin(Main.GlobalTimeWrappedHourly * 5f) * 0.05f + 0.05f);
        itemScale *= pulse;
        bloomScale *= pulse;
        float shakeIntensity = MathF.Pow(Helper.Oscillate(beatTime, 1f), 10f) * 2f * itemScale;
        drawCoordinates += Main.rand.NextVector2Square(-shakeIntensity, shakeIntensity);

        spriteBatch.Draw(AequusTextures.BloomStrong, drawCoordinates, null, bloomColor * 0.08f, 0f, AequusTextures.BloomStrong.Size() / 2f, bloomScale * 1.5f, SpriteEffects.None, 0f);
        spriteBatch.Draw(AequusTextures.BloomStrong, drawCoordinates, null, bloomColor * 0.1f, 0f, AequusTextures.BloomStrong.Size() / 2f, bloomScale * 2f, SpriteEffects.None, 0f);

        spriteBatch.Draw(texture, drawCoordinates, itemFrame, drawColor, rotation, origin, itemScale, SpriteEffects.None, 0f);
        if (colorSwapTime > 0f) {
            spriteBatch.Draw(AequusTextures.NetherStar_Glow, drawCoordinates, itemFrame, drawColor * colorSwapTime, rotation, origin, itemScale, SpriteEffects.None, 0f);
        }

        float beatScale = Math.Max(MathF.Pow(Helper.Oscillate(beatTime, 1f), 4f) * 1.05f, MathF.Pow(Helper.Oscillate(beatTime - 1.4f, 1f), 4f) * 0.8f);
        spriteBatch.Draw(texture, drawCoordinates, itemFrame, drawColor with { A = 0 } * beatScale * 0.66f, rotation, origin, itemScale * Math.Max(beatScale * 1.2f, 1f), SpriteEffects.None, 0f);

        var flareCoords = drawCoordinates + new Vector2(1f, 2f) * itemScale;
        float whiteBallScale = itemScale * flareScale * MathF.Min(beatScale, 1f) * 1.5f;
        spriteBatch.Draw(AequusTextures.BloomStrong, flareCoords, null, flareColor * beatScale, 0f, AequusTextures.BloomStrong.Size() / 2f, whiteBallScale * 0.125f, SpriteEffects.None, 0f);
        for (int i = -1; i < 2; i += 2) {
            spriteBatch.Draw(AequusTextures.Flare, flareCoords, null, flareColor * beatScale, MathHelper.PiOver2 + 0.5f * i, AequusTextures.Flare.Size() / 2f, new Vector2(0.6f, 1.66f) * whiteBallScale * 0.5f, SpriteEffects.None, 0f);
        }
        spriteBatch.Draw(AequusTextures.Flare, flareCoords, null, flareColor * beatScale, MathHelper.PiOver2, AequusTextures.Flare.Size() / 2f, new Vector2(0.4f, 2f) * whiteBallScale, SpriteEffects.None, 0f);
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
        var position = Item.Center - Main.screenPosition;
        Main.GetItemDrawFrame(Type, out _, out var frame);
        var origin = frame.Size() / 2f;
        lightColor = Color.White * (lightColor.A / 255f);

        spriteBatch.End();
        spriteBatch.BeginWorld(shader: true);

        DrawBackground(spriteBatch, position, 1f);

        spriteBatch.End();
        spriteBatch.BeginWorld(shader: false);

        DrawGem(spriteBatch, position, frame, lightColor, rotation, origin, scale, 1f, 1f);
        return false;
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        spriteBatch.End();
        spriteBatch.BeginUI(immediate: true, useScissorRectangle: true);

        DrawBackground(spriteBatch, position, Main.inventoryScale);

        spriteBatch.End();
        spriteBatch.BeginUI(immediate: false, useScissorRectangle: true);

        DrawGem(spriteBatch, position, frame, drawColor, 0f, origin, scale, Main.inventoryScale * 0.5f, Main.inventoryScale * 0.55f);
        return false;
    }
}