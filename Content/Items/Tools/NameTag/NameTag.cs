using Aequus;
using Aequus.Common.Items.Components;
using Aequus.Content.DataSets;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Tools.NameTag;

public class NameTag : ModItem, ICustomNameTagPrice {
    public static int ChestSpawnrate = 8;

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 5;
    }

    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useAnimation = 15;
        Item.useTime = 15;
        Item.consumable = true;
        Item.rare = ItemRarityID.White;
        Item.value = Item.buyPrice(gold: 1);
        Item.maxStack = Item.CommonMaxStack;
    }

    public static bool CanRename(NPC npc) {
        if (NPCSets.NameTagOverride.TryGetValue(npc.netID, out bool canBeRenamedOverride)) {
            return canBeRenamedOverride;
        }

        return npc.townNPC || NPCID.Sets.SpawnsWithCustomName[npc.type] || (!npc.boss && !NPCID.Sets.ShouldBeCountedAsBoss[npc.type] && !npc.immortal && !npc.dontTakeDamage && !npc.SpawnedFromStatue && (npc.realLife == -1 || npc.realLife == npc.whoAmI));
    }

    public override bool? UseItem(Player player) {
        if (Main.myPlayer != player.whoAmI || !player.ItemTimeIsZero || !player.IsInTileInteractionRange(Player.tileTargetX, Player.tileTargetY, TileReachCheckSettings.Simple) || !Item.TryGetGlobalItem<AequusItem>(out var itemNameTag) || !itemNameTag.HasNameTag) {
            return false;
        }

        int screenMouseX = Main.mouseX + (int)Main.screenPosition.X;
        int screenMouseY = Main.mouseY + (int)Main.screenPosition.Y;
        for (int i = 0; i < Main.maxNPCs; i++) {
            var npc = Main.npc[i];
            if (!npc.active || !npc.getRect().Contains(screenMouseX, screenMouseY) || !CanRename(npc) || !npc.TryGetGlobalNPC<NameTagGlobalNPC>(out var npcNameTag) || npcNameTag.Value == itemNameTag.NameTag) {
                continue;
            }

            NametagEffects(i, itemNameTag.NameTag);
            if (Main.netMode != NetmodeID.SinglePlayer) {
                ModContent.GetInstance<NameTagPacket>().Send(i, itemNameTag.NameTag);
            }
            return true;
        }

        return false;
    }

    public static void NametagEffects(int i, string nameTag) {
        if (Main.npc[i].TryGetGlobalNPC<NameTagGlobalNPC>(out var npcNameTag)) {
            npcNameTag.Value = nameTag;
            npcNameTag.nameTagAnimation = 1f;
        }

        SoundEngine.PlaySound(SoundID.Item92, Main.npc[i].Center);

        if (Main.netMode != NetmodeID.Server) {
            for (int k = 0; k < 15; k++) {
                var d = Dust.NewDustDirect(Main.npc[i].position, Main.npc[i].width, Main.npc[i].height, DustID.AncientLight, 0f, 0f, Scale: Main.rand.NextFloat(0.5f, 0.8f));
                d.velocity *= 0.1f;
                d.velocity += Main.npc[i].velocity;
                d.fadeIn = d.scale + 0.25f;
                d.noGravity = true;
            }
        }
    }

    public int GetNameTagPrice(AequusItem aequusItem) {
        return 0;
    }
}