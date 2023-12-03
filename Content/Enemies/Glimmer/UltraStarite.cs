using Aequus.Common.NPCs;
using Aequus.Core.Assets;
using Aequus.Core.Graphics.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Content.Enemies.Glimmer;

public class UltraStarite : ModNPC {
    public const string MiscShaderKey = "Aequus:UltraStarite";

    private UltraStariteRenderTarget.DrawInfo _body;

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
        if (_body == null) {
            var meshGenerator = new CircleCrystalGenMesh(Main.rand.Next(int.MaxValue), 0);
            meshGenerator.Generate();
            _body = new(meshGenerator, NPC.Size.Length() / 2f);
        }

        UltraStariteRenderTarget.Instance.RenderTodo.Add(_body);

        if (_body.Target == null || _body.Target.IsDisposed) {
            return false;
        }

        var drawCoordinates = NPC.Center + new Vector2(0f, NPC.gfxOffY) - screenPos;
        spriteBatch.End();
        if (NPC.IsABestiaryIconDummy) {
            spriteBatch.BeginUI(immediate: true, useScissorRectangle: true);
        }
        else {
            spriteBatch.BeginWorld(shader: true);
        }

        var random = new FastRandom(NPC.whoAmI);
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
            var bloomDD = new DrawData(bloomTexture, drawCoordinates + dustOffset, bloomFrame, Color.Blue * pulse * random.NextFloat(0.01f, 0.2f), dustOffset.ToRotation(), bloomFrame.Size() / 2f, new Vector2(5f, 0.5f) * pulse * random.NextFloat(0.5f, 1f), SpriteEffects.None, 0f);
            bloomDD.Draw(spriteBatch);
        }

        var coreShader = GameShaders.Misc[MiscShaderKey];
        coreShader.UseOpacity(0.3f);

        var dd = new DrawData(_body.Target, drawCoordinates, null, Color.White, NPC.rotation, _body.Target.Size() / 2f, NPC.scale * 2f, SpriteEffects.None, 0f);
        dd.Draw(spriteBatch);

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
            var dustDrawData = new DrawData(dustTexture, drawCoordinates + dustOffset, dustFrame, Color.White with { A = 80 } * pulse * 0.6f, 0f, dustFrame.Size() / 2f, NPC.scale + 0.5f * pulse, SpriteEffects.None, 0f);
            GameShaders.Armor.GetShaderFromItemId(ItemID.TwilightDye).Apply(NPC, dustDrawData);
            dustDrawData.Draw(spriteBatch);
        }

        coreShader.Apply(dd);
        dd.Draw(spriteBatch);
        float corePulse = Helper.Oscillate(Main.GlobalTimeWrappedHourly * 2.5f, 1f);
        for (int i = 0; i < 4; i++) {
            (dd with { position = dd.position + (i * MathHelper.PiOver2 + Main.GlobalTimeWrappedHourly).ToRotationVector2() * (int)(corePulse * 8f), color = dd.color with { A = 0 } * 0.3f * corePulse }).Draw(spriteBatch);
        }

        spriteBatch.End();
        if (NPC.IsABestiaryIconDummy) {
            spriteBatch.BeginUI(immediate: false, useScissorRectangle: true);
        }
        else {
            spriteBatch.BeginWorld(shader: false);
        }

        //dd.Draw(spriteBatch);

        return false;
    }
}