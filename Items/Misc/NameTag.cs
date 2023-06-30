using Aequus;
using Aequus.Common.NPCs.Global;
using Aequus.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc {
    public class NameTag : ModItem {
        public static Dictionary<int, bool> CanBeRenamedOverride { get; private set; }

        public override void Load() {
            CanBeRenamedOverride = new Dictionary<int, bool>() {
                [NPCID.EaterofWorldsBody] = false,
                [NPCID.EaterofWorldsTail] = false,
            };
        }

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void Unload() {
            CanBeRenamedOverride?.Clear();
            CanBeRenamedOverride = null;
        }

        public override void SetDefaults() {
            Item.width = 24;
            Item.height = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.consumable = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 5);
            Item.maxStack = Item.CommonMaxStack;
        }

        public override bool? UseItem(Player player) {
            if (Main.myPlayer == player.whoAmI && player.ItemTimeIsZero && player.IsInTileInteractionRange(Player.tileTargetX, Player.tileTargetY) && Item.TryGetGlobalItem<AequusItem>(out var nameTag) && nameTag.HasNameTag) {
                int screenMouseX = Main.mouseX + (int)Main.screenPosition.X;
                int screenMouseY = Main.mouseY + (int)Main.screenPosition.Y;
                for (int i = 0; i < Main.maxNPCs; i++) {
                    var npc = Main.npc[i];
                    if (!CanBeRenamedOverride.TryGetValue(npc.type, out bool canBeRenamedOverride)) {
                        canBeRenamedOverride = false;
                    }
                    else if (!canBeRenamedOverride) {
                        continue;
                    }
                    if (npc.active &&
                        (npc.townNPC || canBeRenamedOverride ||
                        !npc.boss && !NPCID.Sets.ShouldBeCountedAsBoss[npc.type]
                        && !npc.immortal && !npc.dontTakeDamage
                        && !npc.SpawnedFromStatue
                        && (Main.npc[i].realLife == -1 || Main.npc[i].realLife == i))
                        && Main.npc[i].TryGetGlobalNPC<NPCNameTag>(out var npcNameTag) && npcNameTag.NameTag != nameTag.NameTag) {
                        if (Main.npc[i].getRect().Contains(screenMouseX, screenMouseY)) {
                            if (Main.netMode != NetmodeID.SinglePlayer) {
                                var p = Aequus.GetPacket(PacketType.ApplyNameTagToNPC);
                                p.Write(i);
                                p.Write(nameTag.NameTag);
                                p.Send();
                            }
                            ApplyNametagToNPC(i, nameTag.NameTag);
                            for (int k = 0; k < 15; k++) {
                                var d = Dust.NewDustDirect(Main.npc[i].position, Main.npc[i].width, Main.npc[i].height, DustID.AncientLight, 0f, Main.rand.NextFloat(-6f, -2f), Scale: Main.rand.NextFloat(0.5f, 0.8f));
                                d.velocity.X *= 0.75f;
                                d.velocity.Y *= 1.2f;
                                d.velocity += Main.npc[i].velocity;
                                d.fadeIn = d.scale + 0.25f;
                                d.position.Y += 16f;
                                d.noGravity = true;
                            }
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static void ApplyNametagToNPC(int i, string nameTag) {
            if (Main.npc[i].TryGetGlobalNPC<NPCNameTag>(out var npcNameTag))
                npcNameTag.NameTag = nameTag;
            SoundEngine.PlaySound(SoundID.Item92, Main.npc[i].Center);
        }
    }
}