using Aequus.Common.Items.DropRules;
using Aequus.Core.CodeGeneration;
using System;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Content.Items.PermaPowerups.NoHit;

[LegacyName("VictorsReward")]
[SavedPlayerField("usedMaxHPRespawnReward", "bool")]
public class NoHitReward : ModItem {
    private uint _gameTick;
    private uint _nextAmbientTick;
    private int _direction;
    internal NoHitRewardParticles.ParticleSlot _slot;

    protected override bool CloneNewInstances => true;

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
        Item.rare = ItemRarityID.LightPurple;
        Item.UseSound = SoundID.Item92;
        Item.maxStack = Item.CommonMaxStack;
        Item.value = Item.sellPrice(gold: 2);
    }

    public override bool? UseItem(Player player) {
        AequusPlayer aequusPlayer = player.GetModPlayer<AequusPlayer>();
        if (aequusPlayer.usedMaxHPRespawnReward) {
            return false;
        }
        aequusPlayer.usedMaxHPRespawnReward = true;
        return true;
    }

    public override void PostUpdate() {
        for (int i = -8; i < 2; i++) {
            Lighting.AddLight(Item.Center + new Vector2(0f, i * 16f), new Vector3(0.5f));
        }

        CheckFloor();
        if (Main.netMode == NetmodeID.Server) {
            return;
        }
        CheckDirection();
        CheckSound();
        ModContent.GetInstance<NoHitRewardParticles>().New(this);
    }

    private void CheckDirection() {
        if (_gameTick < Main.GameUpdateCount - 10) {
            _direction = 0;
        }

        if (_direction == 0) {
            if (Item.Center.X + Item.velocity.X < Main.LocalPlayer.Center.X) {
                _direction = 1;
            }
            else {
                _direction = -1;
            }
        }
    }

    private void CheckFloor() {
        if (ExtendCollision.GetFloor(Item.Center, 80, out Vector2 floor)) {
            if (Item.velocity.Y > 0f) {
                Item.velocity.Y *= 0.6f;
            }
            if (floor.Y - Item.Bottom.Y < 48f) {
                Item.velocity.Y -= 0.1f;
            }
        }
        else {
            Item.velocity.Y += 0.22f;
        }
    }

    private void CheckSound() {
        // Keep track of the game tick on in-world updates
        // When this item is not in the world, _gameTick is not updated, but Main.GameUpdateCount will keep counting up
        // So, we check if _gameTick and Main.GameUpdateCount are too far apart, and play the sound.
        // Other methods for "play a sound upon the item spawning" seem to be less reliable.
        if (_gameTick < Main.GameUpdateCount - 10) {
            SoundEngine.PlaySound(AequusSounds.NoHitRewardSpawn with { Volume = 0.8f }, Item.Center);
            SoundEngine.PlaySound(AequusSounds.NoHitRewardAmbient with { Volume = 0.1f, MaxInstances = 3 }, Item.Center);
            SetNextAmbientTick();
        }
        if (_nextAmbientTick < Main.GameUpdateCount) {
            SoundEngine.PlaySound(AequusSounds.NoHitRewardAmbient with { Volume = 0.05f, PitchVariance = 0.33f, MaxInstances = 3 }, Item.Center);
            SetNextAmbientTick();
        }
        _gameTick = Main.GameUpdateCount;
    }

    private void SetNextAmbientTick() {
        _nextAmbientTick = Main.GameUpdateCount + (uint)Main.rand.Next(240, 360);
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
        float playerDistance = Main.LocalPlayer.Distance(Item.Center);
        if (playerDistance < 1000f && Main.musicFade.IndexInRange(Main.curMusic)) {
            for (int i = 0; i < Main.musicFade.Length; i++) {
                if (Main.musicFade[i] > 0f && i != Main.curMusic) {
                    Main.musicFade[i] -= 0.05f;
                }
            }

            if (Main.musicFade[Main.curMusic] > 0.015f) {
                Main.musicFade[Main.curMusic] -= 0.01f;
            }
        }

        Vector2 drawCoordinates = Item.Center - Main.screenPosition;
        Vector2 itemCoordinates = drawCoordinates + new Vector2(-1f, Helper.Oscillate(Main.GlobalTimeWrappedHourly, -4f, 4f));
        Main.GetItemDrawFrame(Type, out Texture2D itemTexture, out Rectangle itemFrame);
        Vector2 origin = itemFrame.Size() / 2f;
        Color drawColor = Color.Lerp(lightColor, Color.White, 0.75f);

        Texture2D bloom = AequusTextures.Bloom;
        Rectangle bloomFrame = bloom.Frame(verticalFrames: 2, frameY: 0);
        Vector2 bloomOrigin = bloom.Size() / 2f;
        Color bloomColor = Color.White with { A = 0 } * 0.15f;
        SpriteEffects spriteEffects = _direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        spriteBatch.Draw(bloom, itemCoordinates, null, bloomColor * 0.4f, 0f, bloomOrigin, scale * 1.5f, SpriteEffects.None, 0f);
        spriteBatch.Draw(itemTexture, itemCoordinates + new Vector2(4f, 4f), itemFrame, Color.Black * 0.2f, rotation, origin, scale, spriteEffects, 0f);
        spriteBatch.Draw(itemTexture, itemCoordinates, itemFrame, drawColor, rotation, origin, scale, spriteEffects, 0f);

        Texture2D flareTexture = AequusTextures.Flare;
        Rectangle flareFrame = flareTexture.Bounds;
        Vector2 flareOrigin = flareTexture.Size() / 2f;
        float flareRotation = MathHelper.PiOver2;
        Texture2D softFlareTexture = AequusTextures.FlareSoft;
        Rectangle softFlareFrame = softFlareTexture.Bounds;
        Vector2 softFlareOrigin = softFlareFrame.Size() / 2f;
        float softFlareRotation = 0f;

        float flareScaleMultiplier = 0.9f;
        ulong seed = (ulong)(whoAmI * 5);
        int flareCount = 50;
        Color flareColor = Color.White with { A = 0 } * 0.5f;
        for (int i = 0; i < flareCount; i++) {
            float flareScale = (0.33f + Utils.RandomFloat(ref seed)) * flareScaleMultiplier;
            float timer = i / (float)flareCount * 2f + Main.GlobalTimeWrappedHourly * 0.35f + Utils.RandomFloat(ref seed) * 0.05f;
            float timerWrapped = timer % 2f;
            Vector2 sparkleCoordinates = drawCoordinates + new Vector2(Utils.RandomInt(ref seed, -10, 10), Utils.RandomInt(ref seed, -12, 12) + 4f);

            if (timerWrapped > 1f) {
                continue;
            }

            timerWrapped = MathF.Pow(timerWrapped, 5f);

            float wave = MathF.Sin(timerWrapped * MathHelper.Pi);
            Color finalColor = flareColor * MathF.Pow(wave, 30f);
            Vector2 finalScale = new Vector2(wave * wave, wave * 2f) * 0.3f * flareScale;
            Main.spriteBatch.Draw(softFlareTexture, sparkleCoordinates, softFlareFrame, finalColor, softFlareRotation, softFlareOrigin, finalScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(softFlareTexture, sparkleCoordinates, softFlareFrame, finalColor, softFlareRotation + MathHelper.PiOver2, softFlareOrigin, finalScale * 0.6f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(flareTexture, sparkleCoordinates, flareFrame, finalColor, flareRotation, flareOrigin, finalScale, SpriteEffects.None, 0f);
        }

        return false;
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
    }

    private bool GetFloor(int blockHeight, int padding, out Vector2 floor) {
        floor = Item.Center;
        int checks = blockHeight * (16 / padding);

        for (int i = 0; i < checks; i++) {
            if (Collision.SolidCollision(floor, 1, 1)) {
                return true;
            }

            floor.Y += padding;
        }

        return false;
    }

    internal static IItemDropRule GetGlobalLoot() {
        return ItemDropRule.ByCondition(new ConditionBossNoDamageTaken(), ModContent.ItemType<NoHitReward>());
    }
}