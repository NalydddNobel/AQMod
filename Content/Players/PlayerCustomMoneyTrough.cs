using AQMod.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Content.Players
{
    public sealed class PlayerCustomMoneyTrough : ModPlayer
    {
        public interface ISuperClunkyMoneyTroughTypeThing
        {
            int ChestType { get; }
            int ProjectileType { get; }
            void OnOpen();
            void OnClose();
        }

        private static int _moneyTroughHackProjectileIndex = -1;
        private static ISuperClunkyMoneyTroughTypeThing _moneyTroughHack;

        public override void Initialize()
        {
            _moneyTroughHack = null;
            _moneyTroughHackProjectileIndex = -1;
        }

        public override void UpdateBiomeVisuals()
        {
            if (_moneyTroughHack == null)
                _moneyTroughHackProjectileIndex = -1;
            if (_moneyTroughHackProjectileIndex > -1)
            {
                if (player.flyingPigChest >= 0 || player.chest != -3 || !Main.projectile[_moneyTroughHackProjectileIndex].active || Main.projectile[_moneyTroughHackProjectileIndex].type != ModContent.ProjectileType<ATM>())
                {
                    _moneyTroughHackProjectileIndex = -1;
                    _moneyTroughHack = null;
                }
                else
                {
                    player.chestX = ((int)Main.projectile[_moneyTroughHackProjectileIndex].position.X + Main.projectile[_moneyTroughHackProjectileIndex].width / 2) / 16;
                    player.chestY = ((int)Main.projectile[_moneyTroughHackProjectileIndex].position.Y + Main.projectile[_moneyTroughHackProjectileIndex].height / 2) / 16;
                    if (!player.IsInTileInteractionRange(player.chestX, player.chestY))
                    {
                        if (player.chest != -1)
                            _moneyTroughHack.OnClose();
                        _moneyTroughHack = null;
                        player.flyingPigChest = -1;
                        _moneyTroughHackProjectileIndex = -1;
                        player.chest = -1;
                        Recipe.FindRecipes();
                    }
                    else
                    {
                        player.flyingPigChest = _moneyTroughHackProjectileIndex;
                        player.chest = -2;
                        Main.projectile[_moneyTroughHackProjectileIndex].type = ProjectileID.FlyingPiggyBank;
                    }
                }
            }
        }

        public override void ResetEffects()
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (_moneyTroughHackProjectileIndex > -1)
                {
                    player.flyingPigChest = -1;
                    player.chest = _moneyTroughHack.ChestType;
                    Main.projectile[_moneyTroughHackProjectileIndex].type = _moneyTroughHack.ProjectileType;
                }
            }
        }

        public static void ManageMiscThingsToPreventBugsHopefully()
        {
            if (_moneyTroughHackProjectileIndex == -1)
            {
                _moneyTroughHack = null;
            }
        }

        public static bool CloseMoneyTrough()
        {
            if (_moneyTroughHack != null)
            {
                _moneyTroughHack.OnClose();
                _moneyTroughHack = null;
                Main.LocalPlayer.chest = -1;
                Recipe.FindRecipes();
                return true;
            }
            return false;
        }

        public static bool OpenMoneyTrough(ISuperClunkyMoneyTroughTypeThing moneyTrough, int index)
        {
            if (_moneyTroughHack == null)
            {
                _moneyTroughHack = moneyTrough;
                _moneyTroughHackProjectileIndex = index;
                var plr = Main.LocalPlayer;
                plr.chest = moneyTrough.ChestType;
                plr.chestX = (int)(Main.projectile[index].Center.X / 16f);
                plr.chestY = (int)(Main.projectile[index].Center.Y / 16f);
                plr.talkNPC = -1;
                Main.npcShop = 0;
                Main.playerInventory = true;
                moneyTrough.OnOpen();
                Recipe.FindRecipes();
                return true;
            }
            return false;
        }
    }
}