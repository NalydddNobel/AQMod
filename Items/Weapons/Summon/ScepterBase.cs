using Aequus.Content.Necromancy;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon
{
    public abstract class ScepterBase : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;

            Item.ResearchUnlockCount = 1;
        }

        public override bool AllowPrefix(int pre)
        {
            return !AequusItem.CritOnlyModifier.Contains(pre);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            try
            {
                tooltips.RemoveCritChanceModifier();
            }
            catch
            {
            }
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            Item.useStyle = player.altFunctionUse == 2 ? ItemUseStyleID.Swing : ItemUseStyleID.Shoot;
            return true;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                if (player.ItemAnimationJustStarted)
                {
                    bool playSound = false;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].active && Main.npc[i].friendly && Main.npc[i].TryGetGlobalNPC<NecromancyNPC>(out var zombie) && zombie.isZombie && zombie.zombieOwner == player.whoAmI)
                        {
                            Main.npc[i].Center = player.Center;
                            Main.npc[i].netUpdate = true;
                            playSound = true;
                        }
                    }
                    if (playSound)
                    {
                        SoundEngine.PlaySound(SoundID.NPCDeath33, player.Center);
                    }
                }
                return true;
            }
            return null;
        }

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.altFunctionUse == 2)
                mult = 0f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return player.altFunctionUse != 2;
        }
    }
}