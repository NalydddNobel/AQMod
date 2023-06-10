using Aequus.Common.Preferences;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters {
    public class AdamantiteMimic : ModNPC, IAddRecipeGroups {
        public override void SetStaticDefaults() {
            Main.npcFrameCount[Type] = 6;
            NPCID.Sets.TrailingMode[Type] = 7;

            if (!GameplayConfig.Instance.AdamantiteMimics) {
                NPCID.Sets.NPCBestiaryDrawOffset[Type] = new(0) {
                    Hide = true,
                };
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
            bestiaryEntry.Info.Add(new FlavorTextBestiaryInfoElement("CommonBestiaryFlavor.Mimic"));
        }

        public override void SetDefaults() {
            NPC.CloneDefaults(NPCID.Mimic);
            AIType = NPCID.Mimic;
            AnimationType = NPCID.Mimic;
        }

        public override void FindFrame(int frameHeight) {
            NPC.frame.Y %= frameHeight * Main.npcFrameCount[Type];
        }

        public void AddRecipeGroups(Aequus aequus) {
            Helper.InheritDropRules(NPCID.Mimic, Type);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            var texture = TextureAssets.Npc[Type].Value;
            var frame = texture.Frame(verticalFrames: 6, frameY: NPC.frame.Y / NPC.frame.Height % 6);
            int trailLength = NPCID.Sets.TrailCacheLength[NPC.type];
            var offset = NPC.Size / 2f + new Vector2(0f, -7f);
            var origin = frame.Size() / 2f;
            var spriteDirection = (-NPC.spriteDirection).ToSpriteEffect();
            for (int i = 0; i < trailLength; i++) {
                if (i < trailLength - 1 && (NPC.oldPos[i] - NPC.oldPos[i + 1]).Length() < 1f) {
                    continue;
                }
                spriteBatch.Draw(texture, (NPC.oldPos[i] - screenPos + offset).Floor(), frame,
                    Helper.GetColor(NPC.oldPos[i] + offset) * Helper.CalcProgress(trailLength, i) * 0.4f, NPC.rotation, origin, NPC.scale, spriteDirection, 0f);
            }
            spriteBatch.Draw(texture, (NPC.position - screenPos + offset).Floor(), frame,
                drawColor, NPC.rotation, origin, NPC.scale, spriteDirection, 0f);
            return false;
        }
    }
}
