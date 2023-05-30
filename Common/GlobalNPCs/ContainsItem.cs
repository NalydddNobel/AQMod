using Aequus.Items.Weapons.Melee.Thrown;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Common.GlobalNPCs {
    public class ContainsItem : GlobalNPC {
        public int item;

        public override bool InstancePerEntity => true;
        protected override bool CloneNewInstances => true;

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
            return entity.type == NPCID.ToxicSludge;
        }

        public override void OnSpawn(NPC npc, IEntitySource source) {
            if (npc.type == NPCID.ToxicSludge) {
                if (Main.rand.NextBool(5)) {
                    item = ModContent.ItemType<SickBeat>();
                    npc.netUpdate = true;
                }
            }
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            if (item > 0 && !npc.IsABestiaryIconDummy) {
                npc.Opacity = 0.5f;
                Main.instance.LoadItem(item);
                var texture = TextureAssets.Item[item].Value;
                Helper.GetItemDrawData(item, out var frame);
                float scale = 1f;
                if (texture.Width > npc.frame.Width / 2) {
                    scale = npc.frame.Width / 2 / (float)texture.Width;
                }
                spriteBatch.Draw(texture, npc.Center - screenPos, frame, drawColor, npc.rotation, frame.Size() / 2f, scale * npc.scale, SpriteEffects.None, 0f);
            }
            return true;
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {
            bitWriter.WriteBit(item > 0);
            if (item > 0)
                binaryWriter.Write(item);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {
            if (bitReader.ReadBit())
                item = binaryReader.ReadInt32();
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
            if (npc.type == NPCID.ToxicSludge) {
                npcLoot.Add(new LeadingConditionRule(new Conditions.NeverTrue())).OnSuccess(ItemDropRule.Common(ModContent.ItemType<SickBeat>(), 5));
            }
        }

        public override void OnKill(NPC npc) {
            if (item > 0) {
                CommonCode.DropItem(npc.getRect(), npc.GetSource_Loot(), item, 1, scattered: false);
            }
        }
    }
}