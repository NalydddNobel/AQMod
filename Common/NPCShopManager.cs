using Terraria;
using Terraria.ID;

namespace AQMod.Common
{
    public static class NPCShopManager
    {
        internal static void Setup()
        {
            On.Terraria.Chest.SetupShop += Chest_SetupShop;
        }

        private static void Chest_SetupShop(On.Terraria.Chest.orig_SetupShop orig, Chest self, int type)
        {
            var plr = Main.LocalPlayer;
            bool discount = plr.discount;
            plr.discount = false;
            orig(self, type);
            plr.discount = discount;
            if (discount)
            {
                float discountPercentage = plr.GetModPlayer<AQPlayer>().discountPercentage;
                for (int i = 0; i < Chest.maxItems; i++)
                {
                    if (self.item[i] != null && self.item[i].type != ItemID.None)
                        self.item[i].value = (int)(self.item[i].value * discountPercentage);
                }
            }
        }

        internal static void Unload()
        {

        }
    }
}