using Aequus.Content.Necromancy;
using Aequus.Projectiles.Summon.Necro;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.Necro
{
    public class SkeletronCandle : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.holdStyle = ItemHoldStyleID.HoldFront;
            Item.DamageType = NecromancyDamageClass.Instance;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.useAmmo = ItemDefaults.AmmoBloodyTearstone;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 15);
            Item.flame = true;
            Item.UseSound = SoundID.Item83;
        }

        public override void HoldStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X += -4f * player.direction;
            player.itemLocation.Y += 8f;

            Lighting.AddLight(player.itemLocation, TorchID.Bone);
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.SummonWeapon;
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer != player.whoAmI)
                return null;

            int chosenNPC = -1;
            float distance = 64f;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].IsZombieAndInteractible(player.whoAmI) && CanApply(Main.npc[i], player))
                {
                    float d = Main.npc[i].Distance(Main.MouseWorld);
                    if (d < distance)
                    {
                        chosenNPC = i;
                        distance = d;
                    }
                }
            }
            if (chosenNPC != -1)
            {
                ApplySupportEffects(player, Main.npc[chosenNPC], 0);
            }
            return true;
        }

        public virtual bool CanApply(NPC npc, Player player)
        {
            return !NecromancyProj.AnyOwnedByNPC(npc, ModContent.ProjectileType<SkeletronHandProj>());
        }

        public virtual void ApplySupportEffects(Player player, NPC npc, int soulGemDamage)
        {
            for (int i = -1; i <= 1; i += 2)
            {
                var p = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), npc.Center, Vector2.UnitX * 4f * i, ModContent.ProjectileType<SkeletronHandProj>(), npc.damage + soulGemDamage, 1f, player.whoAmI);
                p.spriteDirection = i;
                p.direction = i;
                p.ai[0] = npc.whoAmI;
                var zombie = p.GetGlobalProjectile<NecromancyProj>();
                zombie.renderLayer = npc.GetGlobalNPC<NecromancyNPC>().renderLayer;
                zombie.zombieDebuffTier = npc.GetGlobalNPC<NecromancyNPC>().zombieDebuffTier;
                zombie.zombieNPCOwner = npc.whoAmI;
                zombie.isZombie = true;
            }
        }
    }
}