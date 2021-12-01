using AQMod.Assets.Graphics.LegacyItemOverlays;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class UltimateSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new UltimateSwordOverlayData(), item.type);
        }

        public override void SetDefaults()
        {
            item.width = 50;
            item.height = 50;
            item.rare = AQItem.Rarities.OmegaStariteRare + 1;
            item.useTime = 18;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.value = AQItem.Prices.OmegaStariteWeaponValue * 2;
            item.damage = 65;
            item.melee = true;
            item.knockBack = 4f;
            item.autoReuse = true;
            item.useTurn = true;
            item.scale = 1.3f;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (player.immuneTime < 5)
            {
                player.immuneTime = 5;
                player.immuneNoBlink = true;
            }
            target.AddBuff(ModContent.BuffType<Buffs.Debuffs.Sparkling>(), 1200);
            if (crit)
            {
                player.AddBuff(ModContent.BuffType<Buffs.Ultima>(), 300);
            }
        }
    }
}