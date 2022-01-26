using AQMod.NPCs.Friendly;
using AQMod.Projectiles.Pets;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace AQMod.Content.Players
{
    public sealed class PlayerStorage : ModPlayer
    {
        public static class Hooks
        {
            internal static void ItemSlot_MouseHover_ItemArray_int_int(On.Terraria.UI.ItemSlot.orig_MouseHover_ItemArray_int_int orig, Item[] inv, int context, int slot)
            {
                orig(inv, context, slot);
                if (context == ItemSlot.Context.ShopItem && NPCs.Friendly.HermitCrab.InUIStorageShop && slot < MaxHermitCrabStorageItems)
                {
                    hoverStorage = Main.player[Main.myPlayer].GetModPlayer<PlayerStorage>().HermitCrab[slot];
                }
            }
        }

        public struct HermitCrabStorage : TagSerializable
        {
            public readonly Item Item;
            public readonly DateTime WhenStored;

            public HermitCrabStorage(Item item)
            {
                Item = item;
                WhenStored = DateTime.Now;
            }

            public HermitCrabStorage(Item item, DateTime time)
            {
                Item = item;
                WhenStored = time;
            }

            public HermitCrabStorage(TagCompound tag)
            {
                Item = tag.Get<Item>("Item");
                WhenStored = new DateTime(tag.GetLong("WhenStored"));
            }

            public bool IsAir => Item == null || Item.IsAir;
            public TimeSpan TimeSinceStored => DateTime.Now - WhenStored;

            public TagCompound SerializeData()
            {
                if (Item != null)
                {
                    return new TagCompound()
                    {
                        ["Item"] = Item,
                        ["WhenStored"] = WhenStored.Ticks,
                    };
                }
                return null;
            }
        }

        public interface ISuperClunkyMoneyTroughTypeThing
        {
            int ChestType { get; }
            int ProjectileType { get; }
            void OnOpen();
            void OnClose();
        }

        private static int _moneyTroughHackProjectileIndex = -1;
        private static ISuperClunkyMoneyTroughTypeThing _moneyTroughHack;
        public HermitCrabStorage[] HermitCrab { get; internal set; }
        public static HermitCrabStorage hoverStorage;
        public const int MaxHermitCrabStorageItems = Chest.maxItems / 2;

        public override void Initialize()
        {
            _moneyTroughHack = null;
            _moneyTroughHackProjectileIndex = -1;
            HermitCrab = new HermitCrabStorage[MaxHermitCrabStorageItems];
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

        public override bool CanSellItem(NPC vendor, Item[] shopInventory, Item item)
        {
            if (vendor.type == ModContent.NPCType<HermitCrab>() && NPCs.Friendly.HermitCrab.InUIStorageShop)
            {
                int itemCount = shopInventory.DoCount((i) => !i.IsAir);
                if (itemCount >= MaxHermitCrabStorageItems)
                {
                    Main.NewText(Language.GetTextValue("Mods.AQMod.HermitCrab.StorageFull"), Colors.RarityYellow);
                    return false;
                }
            }
            else
            {
                NPCs.Friendly.HermitCrab.InUIStorageShop = false;
            }
            return base.CanSellItem(vendor, shopInventory, item);
        }

        public override void PostSellItem(NPC vendor, Item[] shopInventory, Item item)
        {
            if (vendor.type == ModContent.NPCType<HermitCrab>() && NPCs.Friendly.HermitCrab.InUIStorageShop)
            {
                if (HermitCrab == null)
                    HermitCrab = new HermitCrabStorage[MaxHermitCrabStorageItems];
                for (int i = 0; i < HermitCrab.Length; i++)
                {
                    if (HermitCrab[i].IsAir)
                    {
                        HermitCrab[i] = new HermitCrabStorage(item);
                        break;
                    }
                }
            }
        }

        public override void PostBuyItem(NPC vendor, Item[] shopInventory, Item item)
        {
            if (vendor.type == ModContent.NPCType<HermitCrab>() && NPCs.Friendly.HermitCrab.InUIStorageShop)
            {
                if (HermitCrab == null)
                {
                    HermitCrab = new HermitCrabStorage[MaxHermitCrabStorageItems];
                    return;
                }
                for (int i = 0; i < HermitCrab.Length; i++)
                {
                    if (!HermitCrab[i].IsAir)
                    {
                        HermitCrab[i] = new HermitCrabStorage(shopInventory[i], HermitCrab[i].WhenStored);
                    }
                }
            }
        }

        public override TagCompound Save()
        {
            var tag = new TagCompound();
            return new TagCompound()
            {
                ["HermitCrab"] = HermitCrab.Convert((s) => s.SerializeData()).CutNullIndicesToList(),
            };
        }

        public override void Load(TagCompound tag)
        {
            var tagList = tag.GetList<TagCompound>("HermitCrab");
            if (tagList != null && tagList.Count != 0)
            {
                AQMod.GetInstance().Logger.Info("Loading hermit crab data!");
                HermitCrab = new HermitCrabStorage[MaxHermitCrabStorageItems];
                for (int i = 0; i < tagList.Count; i++)
                {
                    AQMod.GetInstance().Logger.Info("i: " + i);
                    try
                    {
                        HermitCrab[i] = new HermitCrabStorage(tagList[i]);
                    }
                    catch
                    {

                    }
                }
            }
            AQMod.GetInstance().Logger.Info("Leaving load method..");
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