﻿using Aequus.Common.NPCs;
using Aequus.Core.Assets;
using Aequus.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Content.Enemies.Glimmer;

public class UltraStarite : ModNPC {
    public const string MiscShaderKey = "Aequus:UltraStarite";

    public override void Load() {
        if (!Main.dedServ) {
            GameShaders.Misc[MiscShaderKey] = new MiscShaderData(AequusShaders.StariteCore.Ref, "StariteCorePass")
                .UseOpacity(1f)
                .UseImage1(AequusTextures.UltraStariteGradient9)
                .UseImage2(AequusTextures.UltraStariteSampler);
        }
    }

    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = 1;
        ItemID.Sets.KillsToBanner[BannerItem] = 10;
        NPCID.Sets.DontDoHardmodeScaling[Type] = true;
        NPCID.Sets.CantTakeLunchMoney[Type] = true;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry)
            .QuickUnlock();
    }

    public override void SetDefaults() {
        NPC.width = 120;
        NPC.height = 120;
        NPC.lifeMax = 1500;
        NPC.damage = 50;
        NPC.defense = 20;
        NPC.HitSound = SoundID.NPCHit5;
        NPC.DeathSound = SoundID.NPCDeath55;
        NPC.aiStyle = -1;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.knockBackResist = 0f;
        NPC.value = Item.buyPrice(silver: 50);
        NPC.npcSlots = 3f;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment) {
        NPC.lifeMax = (int)(NPC.lifeMax * 0.85f * numPlayers);
        NPC.damage = (int)(NPC.damage * 0.9f);
    }

    public override void AI() {
        NPC.velocity *= 0.95f;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        var drawCoordinates = NPC.Center + new Vector2(0f, NPC.gfxOffY) - screenPos;
        spriteBatch.End();
        if (NPC.IsABestiaryIconDummy) {
            spriteBatch.BeginUI(immediate: true, useScissorRectangle: true);
        }
        else {
            spriteBatch.BeginWorld(shader: true);
        }

        //var ropeTexture = TextureAssets.MagicPixel.Value;
        //var _backCoordinates = new Vector2[40];
        //var _frontCoordinates = new Vector2[40];
        //var _backRotations = new float[40];
        //var _frontRotations = new float[40];
        //CircleDrawer.GetVertices(_frontCoordinates, _frontRotations, _backCoordinates, _backRotations, new(8f, 8f), Helper.Oscillate(Main.GlobalTimeWrappedHourly, 2f, 3f), Helper.Oscillate(Main.GlobalTimeWrappedHourly * 0.1f, 0.5f, 1f));

        var coreShader = GameShaders.Misc[MiscShaderKey];
        coreShader.UseOpacity(0.3f);

        var dd = new DrawData(AequusTextures.UltraStarite, drawCoordinates, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);
        coreShader.Apply(dd);
        //for (int i = _backCoordinates.Length - 1; i >= 0; i--) {
        //    DrawHelper.DrawBasicVertexLine(ropeTexture, _backCoordinates, _backRotations,
        //        p => Color.Lerp(Color.Yellow * 0.33f, Color.Orange * 0.1f, MathF.Sin(p * MathHelper.Pi)),
        //        p => 4f
        //    , NPC.Center - Main.screenPosition);
        //}
        //for (int i = 0; i < _frontCoordinates.Length; i++) {
        //    MiscWorldUI.Drawer.DrawBasicVertexLine(ropeTexture, _frontCoordinates, _frontRotations,
        //        p => Color.Lerp(Color.Yellow * 0.33f, Color.LightYellow, MathF.Sin(p * MathHelper.Pi)),
        //        p => 4f
        //    , NPC.Center - Main.screenPosition);
        //}

        //_backCoordinates = new Vector2[40];
        //_frontCoordinates = new Vector2[40];
        //_backRotations = new float[40];
        //_frontRotations = new float[40];
        //CircleDrawer.GetVertices(_frontCoordinates, _frontRotations, _backCoordinates, _backRotations, new(10f, 10f), Helper.Oscillate(Main.GlobalTimeWrappedHourly, 2f, 3f), Helper.Oscillate(Main.GlobalTimeWrappedHourly * 0.5f, 2.5f, 4f));

        //for (int i = _backCoordinates.Length - 1; i >= 0; i--) {
        //    DrawHelper.DrawBasicVertexLine(ropeTexture, _backCoordinates, _backRotations,
        //        p => Color.Lerp(Color.Yellow * 0.33f, Color.Orange * 0.1f, MathF.Sin(p * MathHelper.Pi)),
        //        p => 4f
        //    , NPC.Center - Main.screenPosition);
        //}
        //for (int i = 0; i < _frontCoordinates.Length; i++) {
        //    MiscWorldUI.Drawer.DrawBasicVertexLine(ropeTexture, _frontCoordinates, _frontRotations,
        //        p => Color.Lerp(Color.Yellow * 0.33f, Color.LightYellow, MathF.Sin(p * MathHelper.Pi)),
        //        p => 4f
        //    , NPC.Center - Main.screenPosition);
        //}
        dd.Draw(spriteBatch);

        var random = new FastRandom(NPC.whoAmI);
        var dustTexture = AequusTextures.BloomStrong.Value;
        for (int i = 0; i < 20; i++) {
            float time = random.NextFloat(10f) + i + Main.GlobalTimeWrappedHourly;
            time %= 1f;
            if (time > 1f) {
                continue;
            }
            var dustFrame = dustTexture.Frame(verticalFrames: 1, frameY: random.Next(1));
            var dustOffset = NPC.Size.RotatedBy(random.NextFloat(MathHelper.TwoPi) + Main.GlobalTimeWrappedHourly * random.NextFloat(0.5f, 1f)) * time * 0.33f * NPC.scale;
            float pulse = MathF.Sin(time * MathHelper.Pi);
            var dustDrawData = new DrawData(dustTexture, drawCoordinates + dustOffset, dustFrame, Color.White * pulse * 0.6f, Main.GlobalTimeWrappedHourly * random.NextFloat(0.5f, 1f), dustFrame.Size() / 2f, NPC.scale + 0.5f * pulse, SpriteEffects.None, 0f);
            GameShaders.Armor.GetShaderFromItemId(ItemID.TwilightDye).Apply(NPC, dustDrawData);
            dustDrawData.Draw(spriteBatch);
        }

        var bloomTexture = AequusTextures.BloomStrong.Value;
        for (int i = 0; i < 20; i++) {
            float time = random.NextFloat(10f) + i + Main.GlobalTimeWrappedHourly * random.NextFloat(0.2f, 0.5f);
            time %= 1f;
            if (time > 1f) {
                continue;
            }
            var bloomFrame = bloomTexture.Frame(verticalFrames: 1, frameY: random.Next(1));
            var dustOffset = NPC.Size.RotatedBy(random.NextFloat(MathHelper.TwoPi) + Main.GlobalTimeWrappedHourly * random.NextFloat(0.1f, 0.2f)) * time * 0.1f * NPC.scale;
            float pulse = MathF.Sin(time * MathHelper.Pi);
            var bloomDD = new DrawData(bloomTexture, drawCoordinates + dustOffset, bloomFrame, Color.White * pulse * random.NextFloat(0.01f, 0.1f), dustOffset.ToRotation(), bloomFrame.Size() / 2f, new Vector2(5f, 0.5f) * pulse * random.NextFloat(0.5f, 1f), SpriteEffects.None, 0f);
            coreShader.Apply(bloomDD);
            bloomDD.Draw(spriteBatch);
        }

        coreShader.Apply(dd);
        dd.Draw(spriteBatch);
        float corePulse = Helper.Oscillate(Main.GlobalTimeWrappedHourly * 2.5f, 1f);
        for (int i = 0; i < 4; i++) {
            (dd with { position = dd.position + (i * MathHelper.PiOver2 + Main.GlobalTimeWrappedHourly * 5f).ToRotationVector2() * (int)(corePulse * 4f), color = dd.color with { A = 0 } * 0.15f * corePulse }).Draw(spriteBatch);
        }

        spriteBatch.End();
        if (NPC.IsABestiaryIconDummy) {
            spriteBatch.BeginUI(immediate: false, useScissorRectangle: true);
        }
        else {
            spriteBatch.BeginWorld(shader: false);
        }
        return false;
    }
}