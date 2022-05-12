using Aequus.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Boss
{
    [AutoloadBossHead]
    public class DustDevil : AequusBoss
    {
        public const float PHASE_INTRO = 0f;

        public bool PhaseTwo => NPC.life * (Main.expertMode ? 2f : 4f) <= NPC.lifeMax;

        public float timer;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData()
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Wet,
                    BuffID.Confused,
                    BuffID.Suffocation,
                    BuffID.Lovestruck,
                }
            });

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers(0) { PortraitPositionYOverride = 48f, });

            FrozenNPCEffect.Blacklist.NPCTypes.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 100;
            NPC.lifeMax = 14500;
            NPC.damage = 50;
            NPC.defense = 12;
            NPC.aiStyle = -1;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.boss = true;
            NPC.value = Item.buyPrice(gold: 10);
            NPC.lavaImmune = true;
            NPC.trapImmune = true;
        }

        public override void AI()
        {
            timer += 1f / 60f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                return true;
            }

            var rand = EffectsSystem.EffectRand;

            var bottom = NPC.position + new Vector2(NPC.width / 2f, NPC.height / 2f + 150f);
            for (int i = 0; i < 300; i++)
            {
                rand.SetRand(i * 3);
                float cloudProgress = (rand.Rand(0f, 2f) + timer * rand.Rand(0.1f, 0.3f)) % 2f;
                if (cloudProgress > 1f)
                {
                    continue;
                }
                var cloudTexture = TextureAssets.Cloud[(int)rand.Rand(0f, Main.maxCloudTypes)].Value;
                float wave = AequusHelpers.Wave(timer * rand.Rand(0.6f, 10f) + rand.Rand(MathHelper.TwoPi), -NPC.width / 2f, NPC.width / 2f);
                float opacity = 1f;
                if (cloudProgress < 0.8f)
                {
                    wave *= cloudProgress / 0.8f;
                }
                if (cloudProgress < 0.3f)
                {
                    opacity *= cloudProgress / 0.3f;
                }
                else if (cloudProgress > 0.7f)
                {
                    opacity *= (1f - cloudProgress) / 0.3f;
                }
                wave *= rand.Rand(0.5f, 1.2f);
                var drawCoordinates = bottom - screenPos + new Vector2(wave, cloudProgress * -300f);
                var rotation = AequusHelpers.Wave(timer * rand.Rand(0.1f, 2f), -0.05f, 0.05f);
                float scale = (rand.Rand(0.06f, 0.2f) + AequusHelpers.Wave(timer * rand.Rand(0.1f, 5.5f), -0.05f, 0.1f)) * 0.35f + cloudProgress * 0.1f;
                
                foreach (var v in AequusHelpers.CircularVector(4))
                {
                    spriteBatch.Draw(cloudTexture, drawCoordinates + v * 2f * scale,
                        null, Color.Blue * opacity * 0.25f, rotation, cloudTexture.Size() / 2f,
                        scale, SpriteEffects.None, 0f);
                }

                spriteBatch.Draw(cloudTexture, drawCoordinates,
                    null, Color.White * opacity, rotation, cloudTexture.Size() / 2f,
                    scale, SpriteEffects.None, 0f);
            }

            var texture = TextureAssets.Npc[Type].Value;
            foreach (var v in AequusHelpers.CircularVector(4))
            {
                spriteBatch.Draw(texture, NPC.Center - screenPos + v * 2f * NPC.scale,
                    null, Color.Blue * 0.3f, 0f, texture.Size() / 2f,
                    NPC.scale, SpriteEffects.None, 0f);
            }

            var ray = Images.LightRay.Value;
            for (int i = 0; i < 50; i++)
            {
                rand.SetRand(i * 12);
                float rayIntensity = AequusHelpers.Wave(timer * rand.Rand(0.1f, 0.5f) + rand.Rand(MathHelper.TwoPi), 0f, 1f);
                float rotation = rand.Rand(MathHelper.TwoPi) + timer * rand.Rand(0.1f, 0.5f);
                spriteBatch.Draw(ray, NPC.Center - screenPos + rotation.ToRotationVector2() * 24f,
                    null, new Color(222, 222, 255, 100) * rayIntensity, rotation + MathHelper.PiOver2, ray.Size() / 2f,
                    rand.Rand(0.4f, 0.75f) * rayIntensity, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(texture, NPC.Center - screenPos,
                null, Color.White, NPC.rotation, texture.Size() / 2f,
                NPC.scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}