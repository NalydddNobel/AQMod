using Aequus.Projectiles.Summon.Misc;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.Passive
{
    [GlowMask]
    [AutoloadEquip(EquipType.Head)]
    public class MoonlunaHat : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            int head = Item.headSlot;
            Item.CloneDefaults(ItemID.WizardHat);
            Item.headSlot = head;
            Item.DamageType = DamageClass.Summon;
        }

        public override void UpdateEquip(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                var aequus = player.Aequus();
                aequus.wearingSummonHelmet = true;
                if (aequus.summonHelmetTimer != 0)
                {
                    aequus.summonHelmetTimer -= (int)player.velocity.Length() / 2;
                }
                if (aequus.summonHelmetTimer <= 0)
                {
                    if (aequus.summonHelmetTimer != -1)
                    {
                        Item.damage = 20;
                        int damage = player.GetWeaponDamage(Item);
                        Item.damage = 0;
                        int p = Projectile.NewProjectile(player.GetSource_Accessory(Item, "Helmet"), player.Center + new Vector2(0f, Main.rand.NextFloat(-player.height / 2f, player.height / 2f)), Vector2.Zero,
                            ModContent.ProjectileType<MoonlunaHatProj>(), damage, 0f, player.whoAmI);
                        Main.projectile[p].ArmorPenetration = Item.ArmorPenetration;
                    }
                    aequus.summonHelmetTimer = 30;
                }
                player.GetDamage(DamageClass.Summon) += 0.10f;
            }
        }
    }
}