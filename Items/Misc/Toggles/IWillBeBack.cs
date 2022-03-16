using AQMod.Common.ID;
using AQMod.Content.World;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Misc.Toggles
{
    public class IWillBeBack : ModItem
    {
        public static Color TextColor => new Color(255, 200, 50, 255);

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[item.type] = ItemSortingID.BossSummon_Abeemination;
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.LightRed;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (item.lavaWet)
            {
                gravity = 0f;
                if (item.velocity.Y > 0f)
                    item.velocity.Y *= 0.9f;
                int x = (int)(item.position.X + item.width / 2f) / 16;
                int y = (int)(item.position.Y + item.height / 2f) / 16;
                if (Main.tile[x, y] == null)
                {
                    Main.tile[x, y] = new Tile();
                }
                if (Main.tile[x, y].liquid < 255)
                {
                    item.velocity.Y *= 0.5f;
                }
                else
                {
                    item.velocity.Y -= 0.2f;
                    if (item.velocity.Y < -4f)
                        item.velocity.Y = -4f;
                }
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(mod, "Activity", "(" + AQMod.GetText(MiscWorldInfo.villagerLavaImmunity ? "Active" : "Inactive") + ")") { overrideColor = TextColor });
        }

        public override void GrabRange(Player player, ref int grabRange)
        {
            if (item.lavaWet)
                grabRange *= 2;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
                return true;
            return player.ZoneUnderworldHeight && !NPC.AnyNPCs(NPCID.WallofFlesh);
        }

        public override bool UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                MiscWorldInfo.villagerLavaImmunity = !MiscWorldInfo.villagerLavaImmunity;
                if (MiscWorldInfo.villagerLavaImmunity)
                {
                    AQMod.BroadcastMessage("Mods.AQMod.IWillBeBackToggle.True", TextColor);
                }
                else
                {
                    AQMod.BroadcastMessage("Mods.AQMod.IWillBeBackToggle.False", TextColor);
                }
            }
            else
            {
                NPC.SpawnWOF(player.Center);
            }
            return true;
        }
    }
}