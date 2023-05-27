using Aequus.Buffs;
using Aequus.Buffs.Debuffs;
using Aequus.Common.Net.Sounds;
using Aequus.Content.Events.DemonSiege;
using Aequus.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee {
    [AutoloadGlowMask]
    public class HellsBoon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DemonSiegeSystem.RegisterSacrifice(new SacrificeData(ItemID.LightsBane, Type, UpgradeProgressionType.PreHardmode));
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 46;
            Item.useTime = 38;
            Item.useAnimation = 19;
            Item.autoReuse = true;
            Item.rare = ItemDefaults.RarityDemonSiege;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.value = ItemDefaults.ValueDemonSiege;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 3f;
            Item.shootSpeed = 35f;
            Item.shoot = ModContent.ProjectileType<HellsBoonSpawner>();
            Item.scale = 1f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor.MaxRGBA(50);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Helper.AddBuffs(target, 240, 1, CorruptionHellfire.Debuffs);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            AequusBuff.ApplyBuff<CorruptionHellfire>(player, 240, out bool canPlaySound);
            if (canPlaySound)
            {
                ModContent.GetInstance<BlueFireDebuffSound>().Play(target.Center, pitchOverride: -0.2f);
            }
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            damage = (int)(damage * 0.5f);
            position = Main.MouseWorld;
            player.LimitPointToPlayerReachableArea(ref position);
        }
    }
}