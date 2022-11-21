﻿using Aequus.Items.Prefixes.Potions;
using Terraria;
using Terraria.ID;

namespace Aequus.Buffs.Misc.Empowered
{
    public class EmpoweredShine : EmpoweredBuffBase
    {
        public override int OriginalBuffType => BuffID.Shine;

        public override void SetStaticDefaults()
        {
            EmpoweredPrefix.ItemToEmpoweredBuff.Add(ItemID.ShinePotion, Type);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            Lighting.AddLight((int)(player.position.X + (float)(player.width / 2)) / 16, (int)(player.position.Y + (float)(player.height / 2)) / 16, 1.6f, 1.9f, 2f);
        }
    }
}