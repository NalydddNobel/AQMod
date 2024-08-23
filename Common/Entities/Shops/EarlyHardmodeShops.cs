using Aequus.Common.Preferences;
using System;
using System.Collections.Generic;

namespace Aequus.Common.Entities.Shops;

public class EarlyHardmodeShops : LoadedType {
    public readonly HashSet<int> Blacklist = [];

    protected override bool IsLoadingEnabled(Mod mod) {
        return GameplayConfig.Instance.EarlyHardmodeShops;
    }

    protected override void Load() {
        On_Chest.SetupShop_int += On_Chest_SetupShop_int;

        if (!GameplayConfig.Instance.EarlyHallow) {
            // Internal shop index for the Dryad.
            Blacklist.Add(3);
        }
    }

    private static void On_Chest_SetupShop_int(On_Chest.orig_SetupShop_int orig, Chest self, int type) {
        bool oldHardMode = Main.hardMode;
        if (!ModContent.GetInstance<EarlyHardmodeShops>().Blacklist.Contains(type)) {
            Main.hardMode |= Aequus.MediumMode;
        }

        try {
            orig(self, type);
        }
        catch (Exception ex) {
            Aequus.Instance.Logger.Error(ex);
        }

        Main.hardMode = oldHardMode;
    }

}
