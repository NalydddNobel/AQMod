using Aequus.Assets;
using Aequus.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
            NPC.TargetClosest(faceTarget: false);
            NPC.velocity.X += Math.Sign(Main.player[NPC.target].Center.X - NPC.Center.X) * 0.2f;
            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -10f, 10f);
            NPC.rotation = -NPC.velocity.X * 0.05f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var texture = TextureCache.Bloom[3].Value;
            var origin = texture.Size() / 2f;
            int segments = 30;
            float y = NPC.height;
            var bottom = NPC.position.Y + NPC.height / 2f + y / 2f;
            float halfWidth = NPC.width / 2f;
            float middle = NPC.position.X + halfWidth;
            List<(Vector2, Color, float)> tornadoSegments = new List<(Vector2, Color, float)>();
            for (int i = -segments; i < segments; i++)
            {
                float progress = AequusHelpers.CalcProgress(segments * 2, i + segments);
                float x = AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 7.5f + i * 0.35f, -halfWidth, halfWidth);
                if (progress < 0.6f)
                {
                    x *= progress / 0.6f;
                }
                var color = Color.White;
                if (i % 2 == 0)
                {
                    x = -x;
                    color = Color.Lerp(color, Color.Blue, progress * 0.8f);
                }
                else
                {
                    color = Color.Lerp(color, Color.OrangeRed, progress * 0.8f);
                }
                tornadoSegments.Add((new Vector2(middle + x, bottom) + new Vector2(0f, -y * progress).RotatedBy(NPC.rotation * progress) - Main.screenPosition, color, NPC.scale * progress * 0.6f));
            }
            foreach (var tuple in tornadoSegments)
            {
                Main.spriteBatch.Draw(texture, tuple.Item1, null, Color.White, 0f, origin, tuple.Item3, SpriteEffects.None, 0f);
            }
            foreach (var tuple in tornadoSegments)
            {
                Main.spriteBatch.Draw(texture, tuple.Item1, null, tuple.Item2, 0f, origin, tuple.Item3 * 0.4f, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}