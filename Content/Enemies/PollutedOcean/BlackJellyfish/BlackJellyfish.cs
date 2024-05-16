using Aequus.Common.NPCs;
using Aequus.Common.NPCs.Bestiary;
using Aequus.Content.Biomes.PollutedOcean;
using Aequus.Content.Graphics.Particles;
using Aequus.Core.ContentGeneration;
using Aequus.DataSets;
using System;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Shaders;

namespace Aequus.Content.Enemies.PollutedOcean.BlackJellyfish;

[AutoloadBanner]
[ModBiomes(typeof(PollutedOceanBiomeUnderground))]
public partial class BlackJellyfish : AIJellyfish {
    public static int AttackRange => 60;

    #region Initialization
    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = 4;
        NPCDataSet.PushableByTypeId.Add(Type);
    }

    public override void SetDefaults() {
        NPC.noGravity = true;
        NPC.width = 26;
        NPC.height = 26;
        NPC.aiStyle = -1;
        NPC.damage = 25;
        NPC.defense = 4;
        NPC.lifeMax = 34;
        NPC.HitSound = SoundID.NPCHit25;
        NPC.DeathSound = SoundID.NPCDeath28;
        NPC.value = Item.silver;
        AnimationType = NPCID.BlueJellyfish;
        shockAttackLength = 70;
        dashSpeed = 8f;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        npcLoot.Add(ItemDropRule.Common(ItemID.Glowstick, minimumDropped: 1, maximumDropped: 4));
        npcLoot.Add(ItemDropRule.Common(ItemID.JellyfishNecklace, 100));
    }
    #endregion

    public override void AI() {
        if (Main.netMode != NetmodeID.Server) {
            float lightingIntensity = GetLightMagnitude();
            if (lightingIntensity <= 0.75f) {
                NPC.ShowNameOnHover = false;
                NPC.nameOver = 0f;
            }
            else {
                NPC.ShowNameOnHover = true;
            }
        }
        if (Main.expertMode) {
            if (NPC.ai[2] > 0f || (NPC.wet && NPC.direction != 0 && NPC.HasValidTarget && NPC.Distance(Main.player[NPC.target].Center) < shockRampUpDistance && NPC.velocity.Length() > (dashSpeed / 2f))) {
                ShockAttack();
                return;
            }
            NPC.ai[2] = 0f;
        }
        base.AI();
    }

    private void ShockAttack() {
        // Begin attack sound
        if (NPC.ai[2] == 0) {
            // Play both sounds, it will swap the volumes of the sound if the NPC is or is not in the water.
            SoundEngine.PlaySound(AequusSounds.BlackJellyfishCharge, NPC.Center, sound => {
                sound.Position = NPC.position;
                sound.Volume = NPC.wet ? 0f : 1f;
                return NPC.active && !Main.gameMenu;
            });
            SoundEngine.PlaySound(AequusSounds.BlackJellyfishCharge_Underwater, NPC.Center, sound => {
                sound.Position = NPC.position;
                sound.Volume = NPC.wet ? 1f : 0f;
                return NPC.active && !Main.gameMenu;
            });
        }

        NPC.ai[2]++;
        if (NPC.ai[2] <= shockAttackLength - 16) {
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        }

        NPC.velocity *= 0.99f;
        if (!NPC.wet) {
            NPC.velocity *= 0.9f;
        }
        NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * NPC.velocity.Length(), 0.05f);
        if (NPC.justHit && NPC.ai[2] < shockAttackLength) {
            NPC.ai[2] -= 8f;
            NPC.ai[2] *= 0.75f;
        }

        if ((int)NPC.ai[2] == shockAttackLength) {
            if (Main.netMode != NetmodeID.Server && Cull2D.Rectangle(NPC.getRect(), 240, 240)) {
                ShockEffects();
            }
            NPC.velocity = Vector2.Zero;
            NPC.netUpdate = true;
        }
        else {
            Terraria.Dust d = Terraria.Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.33f, 1f) * Math.Min(NPC.ai[2] / shockAttackLength, 1f) * AttackRange, DustID.MartianSaucerSpark, Alpha: 0, Scale: Main.rand.NextFloat(0.8f, 1.8f));
            d.rotation = 0f;
            d.noGravity = true;
        }

        if (NPC.ai[2] >= shockAttackLength) {
            Vector2 center = NPC.Center;
            NPC.width = AttackRange * 2;
            NPC.height = AttackRange * 2;
            NPC.Center = center;
            NPC.damage *= 4;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
        }

        if ((int)NPC.ai[2] > shockAttackLength + 14f && Main.netMode != NetmodeID.MultiplayerClient) {
            NPC.active = false;
            if (Main.netMode == NetmodeID.Server) {
                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
            }
        }
    }

    private void ShockEffects() {
        // Explosion Sound
        if (NPC.wet) {
            SoundEngine.PlaySound(AequusSounds.BlackJellyfishExplosion_Underwater with { PitchVariance = 0.1f }, NPC.Center);
            SoundEngine.PlaySound(AequusSounds.BlackJellyfishUnderwaterBubbles with { PitchVariance = 0.2f }, NPC.Center);
        }
        else {
            SoundEngine.PlaySound(AequusSounds.BlackJellyfishExplosion with { PitchVariance = 0.1f }, NPC.Center);
        }

        // Dusts
        for (int i = 0; i < 30; i++) {
            Vector2 randomVector = Main.rand.NextVector2Unit();
            Terraria.Dust d = Terraria.Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.MartianSaucerSpark, Alpha: 0, Scale: Main.rand.NextFloat(0.8f, 1.8f));
            d.rotation = 0f;
            d.velocity = randomVector * Main.rand.NextFloat(4f);
            d.fadeIn = d.scale + Main.rand.NextFloat(0.8f);
            d.noGravity = true;
        }
        for (int i = 0; i < 60; i++) {
            Vector2 randomVector = Main.rand.NextVector2Unit();
            Terraria.Dust d = Terraria.Dust.NewDustPerfect(NPC.Center + randomVector * Main.rand.NextFloat(0.8f, 1f) * NPC.ai[2] / shockAttackLength * AttackRange, DustID.MartianSaucerSpark, Alpha: 0, Scale: Main.rand.NextFloat(0.8f, 2.5f));
            d.rotation = 0f;
            d.velocity += randomVector * 4f * Main.rand.NextFloat();
            d.noGravity = true;
        }

        if (Collision.WetCollision(NPC.position, NPC.width, NPC.height)) {
            // Bubble particles if underwater
            UnderwaterBubbleParticles bubbleParticles = ModContent.GetInstance<UnderwaterBubbleParticles>();
            foreach (var particle in bubbleParticles.NewMultipleReduced(32, 8)) {
                particle.Location = NPC.Center;
                particle.Frame = (byte)Main.rand.Next(3);
                particle.Velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.01f, 0.4f);
                particle.UpLift = (1f - particle.Velocity.X) * 0.003f;
                particle.Opacity = Main.rand.NextFloat(0.8f, 1f);
            }
            var bigBubble = bubbleParticles.New();
            bigBubble.Location = NPC.Center;
            bigBubble.Frame = 7;
            bigBubble.Velocity = Vector2.Zero;
            bigBubble.UpLift = 0.002f;

            // Water Ripples
            DrawHelper.AddWaterRipple(NPC.Center, 0.2f, 0.66f, 0f, new Vector2(30f, 30f), RippleShape.Circle, 0f);
            DrawHelper.AddWaterRipple(NPC.Center, 0.5f, 0.8f, 0f, new Vector2(60f, 60f), RippleShape.Circle, 0f);
        }

        // Screen Shake
        ViewHelper.PunchCameraTo(NPC.Center, strength: 6f, frames: 60);
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot) {
        return NPC.ai[2] >= shockAttackLength && NPC.ai[2] < shockAttackLength + 4f;
    }

    public override bool CanShock() {
        return false;
    }

    public override bool? CanFallThroughPlatforms() {
        return true;
    }

    /// <summary>Gets the lighting magnitude for this Jellyfish. Returns 1f if <see cref="NPC.IsABestiaryIconDummy"/> is <see langword="true"></see>.</summary>
    /// <returns><inheritdoc cref="GetLightMagnitude(Color)"/></returns>
    public float GetLightMagnitude() {
        if (NPC.IsABestiaryIconDummy) {
            return 1f;
        }
        return GetLightMagnitude(ExtendLight.Get(NPC.Center));
    }
    /// <param name="lightColor">The Light color</param>
    /// <returns>A value between 0.45 and 1 which represents the light's magnitude.</returns>
    public static float GetLightMagnitude(Color lightColor) {
        return Math.Clamp(MathF.Pow(lightColor.ToVector3().Length(), 4f), 0.45f, 1f);
    }
}