using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.HookEquips
{
    public class CrabBarb : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
            GrapplingHookModules.RegisterHookBarb(Type, new GrapplingHookModules.DebuffDamageBarbData(15, BuffID.Poisoned, 120) { ModuleTypes = new System.Collections.Generic.List<int>(), });
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 40);
        }
    }
}