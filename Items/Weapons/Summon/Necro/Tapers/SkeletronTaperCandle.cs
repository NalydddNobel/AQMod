using Aequus.Content.Necromancy;
using Aequus.Projectiles.Summon.Necro;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.Necro.Tapers
{
    public class SkeletronTaperCandle : SoulTaperBase
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            DefaultToTaper(0, 3);
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 3);
            Item.flame = true;
            Item.UseSound = SoundID.Item83;
        }

        public override void HoldStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X += -4f * player.direction;
            player.itemLocation.Y += 8f;

            Lighting.AddLight(player.itemLocation, TorchID.Bone);
        }

        public override bool CanApplyTaper(NPC npc, Player player)
        {
            return base.CanApplyTaper(npc, player) && !NecromancyProj.AnyOwnedByNPC(npc, ModContent.ProjectileType<SkeletronHandProj>());
        }

        public override void ApplySupportEffects(Player player, NPC npc, int soulGemDamage)
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