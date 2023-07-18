using Aequus.Items.Weapons.Melee.Swords.BattleAxe;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Ranged.Misc.BlockGlove {
    public struct BlockGloveProjectileInfo {
        public delegate bool OnAI(Projectile projectile);
        public delegate void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers);
        public delegate void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone);

        public readonly string Key;
        public readonly OnAI MethodOnAI;
        public readonly ModifyHitNPC MethodModifyHitNPC;
        public readonly OnHitNPC MethodOnHitNPC;

        public BlockGloveProjectileInfo(string key = null, OnAI onAI = null, ModifyHitNPC modifyHitNPC = null, OnHitNPC onHitNPC = null) {
            Key = key;
            MethodOnAI = onAI;
            MethodModifyHitNPC = modifyHitNPC;
            MethodOnHitNPC = onHitNPC;
        }

        internal static (int tileId, BlockGloveProjectileInfo info) FromTileID(int tileId, OnAI onAI = null, ModifyHitNPC modifyHitNPC = null, OnHitNPC onHitNPC = null) {
            string key = $"Mods.Aequus.Items.BlockGloveTooltips.{TileID.Search.GetName(tileId).Replace("Aequus/", "")}";
            Language.GetOrRegister(key);
            return (tileId, new(key, onAI, modifyHitNPC, onHitNPC));
        }

        public static bool OnAI_FireBlock(Projectile projectile) {
            if (Main.rand.NextBool(projectile.MaxUpdates)) {
                var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Torch, projectile.velocity.X, projectile.velocity.Y);
                d.noGravity = true;
                d.velocity *= 0.5f;
            }
            return true;
        }

        public static void OnHitNPC_FireBlock(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone) {
            target.AddBuff(BuffID.OnFire3, 60);
        }

        public static void ModifyHitNPC_Spikes(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers) {
            modifiers.SourceDamage += 0.5f;
        }

        public static void OnHitNPC_Spikes(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone) {
            target.AddBuff(ModContent.BuffType<BattleAxeBleeding>(), 60);
        }

        public static void OnHitNPC_Poo(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone) {
            target.AddBuff(BuffID.Stinky, 60);
        }
    }
}