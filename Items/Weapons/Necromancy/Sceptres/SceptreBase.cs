using Aequus.Common.Effects.RenderBatches;
using Aequus.Content.ItemPrefixes.Necromancy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Necromancy.Sceptres {
    public abstract class SceptreBase : ModItem, ItemHooks.ISelectNPC, ILayerRenderer {
        public class EnemyRender {
            public NPC NPC;
            public float Opacity;
            public Vector2 Location;
            public Vector2 Size;

            public EnemyRender(NPC npc) {
                NPC = npc;
                Location = npc.Center;
                Size = npc.Size;
            }
        }

        public int HealAmount;

        public bool IsReady => true;

        public float SelectionRange => 64f;

        protected List<EnemyRender> npcRenders = new();

        public override void SetStaticDefaults() {
            Item.staff[Type] = true;
        }

        public override void SetDefaults() {
            Item.DamageType = Aequus.NecromancyMagicClass;
        }

        public override bool AllowPrefix(int pre) {
            return PrefixLoader.GetPrefix(pre) is NecromancyPrefixBase;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            return player.altFunctionUse != 2;
        }

        public virtual void SetupBatchLayers() {
            ModContent.GetInstance<BehindAllNPCsBatch>().renderers.Add(this);
        }

        // TODO: Set to abstract
        public virtual void DrawToLayer(RenderLayerBatch layer, SpriteBatch spriteBatch) {
        }

        public virtual void OnClearWorld() {
            npcRenders.Clear();
        }

        public virtual void OnUpdate() {
            bool selectedNPC = false;
            var selectedNPCIndex = Main.LocalPlayer.Aequus().heldItemSelectedNPC;
            for (int i = 0; i < npcRenders.Count; i++) {
                EnemyRender n = npcRenders[i];
                if (n.NPC == null || n.NPC.whoAmI != selectedNPCIndex) {
                    n.Opacity -= 0.04f;
                    if (n.Opacity <= 0f) {
                        npcRenders.RemoveAt(i);
                        i--;
                    }
                }
                else {
                    Helper.AddClamp(ref n.Opacity, 0.07f, 0f, 1f);
                }

                if (n.NPC == null || !n.NPC.active) {
                    n.NPC = null;
                    continue;
                }

                selectedNPC |= n.NPC.whoAmI == selectedNPCIndex;
                n.Location = n.NPC.Center;
                n.Location.Y += n.NPC.gfxOffY;
                n.Size = n.NPC.Size;
            }

            if (!selectedNPC && selectedNPCIndex >= 0) {
                npcRenders.Add(new(Main.npc[selectedNPCIndex]));
            }
        }

        public bool IsSelectable(Player player, AequusPlayer aequusPlayer, NPC npc) {
            return npc.CanBeChasedBy(player);
        }

        public bool UpdateSelection(Player player, AequusPlayer aequusPlayer, NPC npc) {
            return player.itemAnimation <= 0;
        }
    }
}