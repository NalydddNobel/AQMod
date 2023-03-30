using Aequus.Content.Necromancy;
using Aequus.Projectiles.Summon.Necro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Offense.Necro
{
    public class Malediction : ModItem, INecromancySupportAcc
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(6, 5));
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.mana = 150;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 15);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aequus = player.Aequus();
            aequus.selectGhostNPC = -2;
            aequus.accGhostSupport = Item;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (var t in tooltips)
            {
                if (t.Name.StartsWith("Tooltip"))
                {
                    t.Text = string.Format(t.Text, TextHelper.ArmorSetBonusKey);
                }
            }
        }

        public void ApplySupportEffects(Player player, AequusPlayer aequus, NPC npc, NecromancyNPC zombie)
        {
            SoundEngine.PlaySound(SoundID.Item4, player.Center);
            player.CheckMana(Item.mana, pay: true);
            zombie.hasSupportEffects = true;
            for (int i = -1; i <= 1; i += 2)
            {
                var p = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), npc.Center, Vector2.UnitX * 4f * i, ModContent.ProjectileType<SkeletronHandProj>(), npc.damage * Item.EquipmentStacks(1), 1f, player.whoAmI);
                p.spriteDirection = i;
                p.direction = i;
                p.ai[0] = npc.whoAmI;
                var zombieProj = p.GetGlobalProjectile<NecromancyProj>();
                zombieProj.renderLayer = npc.GetGlobalNPC<NecromancyNPC>().renderLayer;
                zombieProj.zombieDebuffTier = npc.GetGlobalNPC<NecromancyNPC>().zombieDebuffTier;
                zombieProj.zombieNPCOwner = npc.whoAmI;
                zombieProj.isZombie = true;
            }
        }
    }
}