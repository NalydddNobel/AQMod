using System.Collections.Generic;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Aequus.Old.Content.Necromancy;

public class NecromancyGlobalItem : GlobalItem {
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.ModItem is ISceptreItem;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        try {
            tooltips.RemoveCritChanceModifier();
        }
        catch {
        }
    }

    public override bool AltFunctionUse(Item item, Player player) {
        return true;
    }

    public override bool CanUseItem(Item item, Player player) {
        item.useStyle = player.altFunctionUse == 2 ? ItemUseStyleID.Swing : ItemUseStyleID.Shoot;
        return true;
    }

    public override bool? UseItem(Item item, Player player) {
        if (player.altFunctionUse == 2) {
            if (player.ItemAnimationJustStarted) {
                bool playSound = false;
                for (int i = 0; i < Main.maxNPCs; i++) {
                    if (Main.npc[i].active && Main.npc[i].friendly && Main.npc[i].TryGetGlobalNPC<NecromancyNPC>(out var zombie) && zombie.isZombie && zombie.zombieOwner == player.whoAmI) {
                        Main.npc[i].Center = player.Center;
                        Main.npc[i].netUpdate = true;
                        playSound = true;
                    }
                }
                if (playSound) {
                    SoundEngine.PlaySound(SoundID.NPCDeath33, player.Center);
                }
            }
            return true;
        }
        return null;
    }

    public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult) {
        if (player.altFunctionUse == 2) {
            mult = 0f;
        }
    }

    public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
        return player.altFunctionUse != 2;
    }
}
