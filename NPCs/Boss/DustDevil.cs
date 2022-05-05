using Aequus.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Boss
{
    [AutoloadBossHead]
    public class DustDevil : AequusBoss // test, not finished in any shape or form
    {
        public bool PhaseTwo => NPC.life * (Main.expertMode ? 2f : 4f) <= NPC.lifeMax;

        public float _torandoMovement;

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

            FrozenNPC.Catalouge.NPCBlacklist.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 300;
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
            _torandoMovement += 1f / 60f;
            NPC.TargetClosest(faceTarget: false);
            NPC.velocity.X += Math.Sign(Main.player[NPC.target].Center.X - NPC.Center.X) * 0.2f;
            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -10f, 10f);
            NPC.rotation = -NPC.velocity.X * 0.05f;
            var tornadoBottom = new Vector2(NPC.position.X + NPC.width / 2f, NPC.position.Y + NPC.height);
            Dust.NewDustPerfect(tornadoBottom, DustID.Cloud, NPC.velocity * 0.085f + Main.rand.NextVector2Unit() * 0.2f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var texture = TextureAssets.Npc[Type].Value;
            var frame = NPC.frame;
            var origin = texture.Size() / 2f;
            var tornadoBottom = new Vector2(NPC.position.X + NPC.width / 2f, NPC.position.Y + NPC.height);

            int tornadoX = texture.Width / 3;
            float scale;
            int i = 0;

            for (int height = NPC.height; height > 0; height -= Math.Max((int)(frame.Height * scale) - 4, 2))
            {
                scale = NPC.scale * 1.2f;
                if (height < 240)
                {
                    scale *= 1f / 240f * height;
                    scale *= scale;
                }
                float wave = (float)Math.Sin(i * 0.35f + _torandoMovement * 5f);
                var drawCoords = tornadoBottom + new Vector2(wave * tornadoX * scale, -height);
                var segmentColor = NPC.GetNPCColorTintedByBuffs(Lighting.GetColor(drawCoords.ToTileCoordinates()));
                Main.spriteBatch.Draw(texture, drawCoords - screenPos, frame, segmentColor, wave * 0.1f, origin, scale, SpriteEffects.None, 0f);
                segmentColor = NPC.GetNPCColorTintedByBuffs(Color.White);
                Main.spriteBatch.Draw(texture, drawCoords - screenPos + new Vector2(wave * 8f * scale, 0f), frame, segmentColor * 0.3f, wave * 0.1f, origin, scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, drawCoords - screenPos - new Vector2(wave * 8f * scale, 0f), frame, segmentColor * 0.3f, wave * 0.1f, origin, scale, SpriteEffects.None, 0f);
                i++;
            }

            tornadoX = texture.Width * 2;
            for (int height = NPC.height; height > 0; height -= Math.Max((int)(frame.Height * scale) - 4, 2))
            {
                scale = NPC.scale * 1.2f;
                if (height < 240)
                {
                    scale *= 1f / 240f * height;
                    scale *= scale;
                }
                scale *= 0.5f;
                float wave = (float)Math.Sin(i * 0.35f + _torandoMovement * 5f + MathHelper.TwoPi / 3f);
                var drawCoords = tornadoBottom + new Vector2(wave * tornadoX * scale, -height);
                var segmentColor = NPC.GetNPCColorTintedByBuffs(Color.White);
                Main.spriteBatch.Draw(texture, drawCoords - screenPos, frame, segmentColor * 0.2f, wave * 0.1f, origin, scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, drawCoords - screenPos + new Vector2(wave * 8f * scale, 0f), frame, segmentColor * 0.1f, wave * 0.1f, origin, scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, drawCoords - screenPos - new Vector2(wave * 8f * scale, 0f), frame, segmentColor * 0.1f, wave * 0.1f, origin, scale, SpriteEffects.None, 0f);
                i++;
            }

            for (int height = NPC.height; height > 0; height -= Math.Max((int)(frame.Height * scale) - 4, 2))
            {
                scale = NPC.scale * 1.2f;
                if (height < 240)
                {
                    scale *= 1f / 240f * height;
                    scale *= scale;
                }
                scale *= 0.5f;
                float wave = (float)Math.Sin(i * 0.35f + _torandoMovement * 5f + MathHelper.TwoPi / 3f * 2f);
                var drawCoords = tornadoBottom + new Vector2(wave * tornadoX * scale, -height);
                var segmentColor = NPC.GetNPCColorTintedByBuffs(Color.White);
                Main.spriteBatch.Draw(texture, drawCoords - screenPos, frame, segmentColor * 0.2f, wave * 0.1f, origin, scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, drawCoords - screenPos + new Vector2(wave * 8f * scale, 0f), frame, segmentColor * 0.1f, wave * 0.1f, origin, scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, drawCoords - screenPos - new Vector2(wave * 8f * scale, 0f), frame, segmentColor * 0.1f, wave * 0.1f, origin, scale, SpriteEffects.None, 0f);
                i++;
            }

            return false;
        }
    }
}