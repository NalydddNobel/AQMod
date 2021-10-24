using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Common
{
    internal sealed class FargosQOLStuff
    {
        private readonly Mod _fargowiltas;
        private FargosQOLStuff(Mod fargos)
        {
            _fargowiltas = fargos;
        }

        public static bool FargowiltasActive { get; private set; }

        public static void Setup(AQMod aQMod)
        {
            try
            {
                var fargos = ModLoader.GetMod("Fargowiltas");
                if (fargos == null)
                    return;
                FargowiltasActive = true;
                var f = new FargosQOLStuff(fargos);
                f.AddSummon(1.5f, "MushroomClam", () => WorldDefeats.DownedCrabson, Item.buyPrice(gold: 8));
            }
            catch (Exception e)
            {
                aQMod.Logger.Warn("Error occured when setting up fargo boss summons");
                aQMod.Logger.Warn(e.Message);
                aQMod.Logger.Warn(e.StackTrace);
            }
        }

        private void AddSummon(float sort, string itemName, Func<bool> checkFlag, int price)
        {
            _fargowiltas.Call("AddSummon", sort, "AQMod", itemName, checkFlag, price);
        }
    }
}