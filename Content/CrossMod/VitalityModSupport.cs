using Aequus.Items.Accessories.Vanity.Cursors;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    public class VitalityModSupport : ModSystem
    {
        public static Mod VitalityMod { get; private set; }

        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModLoader.HasMod("VitalityMod");
        }

        public override void AddRecipes()
        {
            VitalityMod = null;
            if (ModLoader.TryGetMod("VitalityMod", out var cerebralMod))
            {
                VitalityMod = cerebralMod;
            }
        }

        public override void Unload()
        {
            VitalityMod = null;
        }
    }

    public class VitalityModSupportGlobalNPC : GlobalNPC
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModLoader.HasMod("VitalityMod");
        }

        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            if (VitalityModSupport.VitalityMod == null) 
                return;
            if (VitalityModSupport.VitalityMod.TryFind<ModNPC>("Miner", out var miner) && type == miner.Type)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SwordCursor>());
            }
        }
    }
}