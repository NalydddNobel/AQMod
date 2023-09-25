using Aequus.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.PermaPowerups.NetherStar;

public class NetherStar : ModItem {
    public const string MiscShaderKey = "Aequus:NetherStar";

    public override void Load() {
        if (!Main.dedServ) {
            GameShaders.Misc[MiscShaderKey] = new MiscShaderData(AequusShaders.FadeToCenter.Ref, "FadeToCenterPass")
                .UseImage1(AequusTextures.VignetteSmall)
                .UseImage2(AequusTextures.EffectNoise);
        }
    }

    public override void SetDefaults() {
        Item.useTime = 45;
        Item.useAnimation = 45;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.width = 24;
        Item.height = 24;
        Item.consumable = true;
        Item.rare = ItemRarityID.LightPurple;
        Item.UseSound = SoundID.Item92;
        Item.maxStack = Item.CommonMaxStack;
        Item.value = Item.sellPrice(gold: 2);
    }

    public override bool? UseItem(Player player) {
        player.GetModPlayer<AequusPlayer>().yinYangBonusSlot = true;
        return true;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
        var texture = TextureAssets.Item[Type].Value;
        var position = Item.Center - Main.screenPosition;
        ItemHelper.GetItemDrawData(Type, out var frame);
        var origin = frame.Size() / 2f;
        lightColor = Color.White * (lightColor.A / 255f);
        spriteBatch.Draw(AequusTextures.BloomStrong, position, null, lightColor * 0.125f, 0f, AequusTextures.BloomStrong.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);
        for (int i = 0; i < 4; i++) {
            spriteBatch.Draw(texture, position + (Main.GlobalTimeWrappedHourly + i * MathHelper.PiOver2).ToRotationVector2() * 2f * scale, frame, lightColor with { A = 0 }, 0f, origin, scale, SpriteEffects.None, 0f);
        }
        spriteBatch.Draw(texture, position, frame, lightColor, 0f, origin, scale, SpriteEffects.None, 0f);
        return false;
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        var texture = TextureAssets.Item[Type].Value;

        spriteBatch.End();
        spriteBatch.Begin_UI(immediate: true);

        float timer = Main.GlobalTimeWrappedHourly * 0.25f;
        float wrappedTimer = timer % 1f;
        var shader = GameShaders.Misc[MiscShaderKey];
        float sautration = MathF.Pow(wrappedTimer, 4f) * 10f;
        shader.UseSaturation(sautration);
        var backgroundTexture = AequusTextures.NetherStarBackground;
        var backgroundOrigin = backgroundTexture.Size() / 2f;
        float backgroundScale = MathF.Sin(wrappedTimer * MathHelper.PiOver2);
        for (int i = 0; i < 7; i++) {
            var backgroundColor = Color.Lerp(Color.HotPink, Color.Blue, Helper.Oscillate(i * 1f + timer * 40f, 1f)) with { A = 0 } * 0.5f * backgroundScale;
            DrawData dd = new(backgroundTexture, position, null, backgroundColor, (int)timer * 1.25f + timer + i * i * 0.4f, backgroundOrigin, Main.inventoryScale * (0.4f + i * 0.02f) * 3f * backgroundScale, SpriteEffects.None, 0f);
            shader.Apply(dd);
            dd.Draw(spriteBatch);
        }

        spriteBatch.End();
        spriteBatch.Begin_UI(immediate: false);

        scale += MathF.Sin(Main.GlobalTimeWrappedHourly * 5f) * 0.05f + 0.05f;

        spriteBatch.Draw(AequusTextures.BloomStrong, position, null, drawColor with { A = 0 } * 0.25f, 0f, AequusTextures.BloomStrong.Size() / 2f, Main.inventoryScale * 0.66f, SpriteEffects.None, 0f);
        spriteBatch.Draw(AequusTextures.BloomStrong, position, null, drawColor with { A = 0 } * 0.15f, 0f, AequusTextures.BloomStrong.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);
        spriteBatch.Draw(texture, position, frame, drawColor with { A = (byte)(drawColor.A / 1.2f) }, 0f, origin, scale, SpriteEffects.None, 0f);
        return false;
    }
}