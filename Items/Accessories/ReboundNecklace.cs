using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class ReboundNecklace : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 10);
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ReboundNecklacePlayer>().reboundNecklace = true;
        }
    }

    public class ReboundNecklacePlayer : ModPlayer
    {
        public bool reboundNecklace;
        public bool forceJumpInput;
        public int reboundDelay;
        public int lastRefreshedFall;

        public override void ResetEffects()
        {
            if (!reboundNecklace)
                return;
            reboundNecklace = false;
            AequusHelpers.TickDown(ref reboundDelay);
            int y = (int)(Player.position.Y + Player.height) / 16;
            int fallAmt = y - Player.fallStart;
            if (fallAmt < lastRefreshedFall)
            {
                lastRefreshedFall = fallAmt;
            }
        }

        public override void SetControls()
        {
            if (forceJumpInput)
            {
                Player.controlJump = true;
            }
            forceJumpInput = false;
        }

        public override void PostUpdateEquips()
        {
            int y = (int)(Player.position.Y + Player.height) / 16;
            int fallAmt = y - Player.fallStart;
            if (fallAmt - lastRefreshedFall > 20)
            {
                lastRefreshedFall = fallAmt;
                RefreshADoubleJump();
            }
            if (Player.noFallDmg || Player.slowFall || Player.wingTime > 0 || fallAmt < 10)
            {
                return;
            }
            int x = (int)(Player.position.X + Player.width / 2f) / 16;
            int tileScanAmt = Math.Min((int)(Player.velocity.Y / 2f), 30);
            for (int i = 0; i < tileScanAmt; i++)
            {
                if (y + i > Main.maxTilesY - 10)
                {
                    return;
                }
                if (Main.tile[x, y + i].IsSolid())
                {
                    Rebound();
                    break;
                }
            }
        }


        public void Rebound()
        {
            if (reboundDelay > 0)
            {
                return;
            }
            RefreshADoubleJump();
            reboundDelay = 60;
            forceJumpInput = true;
        }

        public void RefreshADoubleJump()
        {
            if (Player.hasJumpOption_Cloud && !Player.isPerformingJump_Cloud && !Player.canJumpAgain_Cloud)
            {
                Player.canJumpAgain_Cloud = true;
            }
            else if (Player.hasJumpOption_Blizzard && !Player.isPerformingJump_Blizzard && !Player.canJumpAgain_Blizzard)
            {
                Player.canJumpAgain_Blizzard = true;
            }
            else if (Player.hasJumpOption_Sandstorm && !Player.isPerformingJump_Sandstorm && !Player.canJumpAgain_Sandstorm)
            {
                Player.canJumpAgain_Sandstorm = true;
            }
            else if (Player.hasJumpOption_Fart && !Player.isPerformingJump_Fart && !Player.canJumpAgain_Fart)
            {
                Player.canJumpAgain_Fart = true;
            }
            else if (Player.hasJumpOption_Sail && !Player.isPerformingJump_Sail && !Player.canJumpAgain_Sail)
            {
                Player.canJumpAgain_Sail = true;
            }
            else if (Player.hasJumpOption_Basilisk && !Player.isPerformingJump_Basilisk && !Player.canJumpAgain_Basilisk)
            {
                Player.canJumpAgain_Basilisk = true;
            }
            else if (Player.hasJumpOption_Unicorn && !Player.isPerformingJump_Unicorn && !Player.canJumpAgain_Unicorn)
            {
                Player.canJumpAgain_Unicorn = true;
            }
            else if (Player.hasJumpOption_WallOfFleshGoat && !Player.isPerformingJump_WallOfFleshGoat && !Player.canJumpAgain_WallOfFleshGoat)
            {
                Player.canJumpAgain_WallOfFleshGoat = true;
            }
        }
    }
}