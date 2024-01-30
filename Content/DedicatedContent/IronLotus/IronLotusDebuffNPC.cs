namespace Aequus.Content.DedicatedContent.IronLotus;

public class IronLotusDebuffNPC : GlobalNPC {
    public System.Boolean hasDebuff;

    public override System.Boolean InstancePerEntity => true;

    public override System.Boolean AppliesToEntity(NPC entity, System.Boolean lateInstantiation) {
        return !entity.buffImmune[ModContent.BuffType<IronLotusDebuff>()];
    }

    public override void ResetEffects(NPC npc) {
        hasDebuff = false;
    }

    public override void PostAI(NPC npc) {
        if (!hasDebuff) {
            return;
        }

        System.Int32 debuff = npc.FindBuffIndex(ModContent.BuffType<IronLotusDebuff>());
        if (debuff == -1) {
            return;
        }

        System.Int32 plr = Player.FindClosest(npc.position, npc.width, npc.height);
        for (System.Int32 i = 0; i < Main.maxNPCs; i++) {
            if (Main.npc[i].active && (Main.npc[i].type == NPCID.TargetDummy || Main.npc[i].CanBeChasedBy(Main.player[plr])) && Main.npc[i].Distance(npc.Center) < 100f) {
                Main.npc[i].AddBuff(ModContent.BuffType<IronLotusDebuff>(), npc.buffTime[debuff]);
            }
        }

        if (Main.netMode != NetmodeID.Server) {
            System.Int32 amt = (System.Int32)(npc.Size.Length() / 32f);
            for (System.Int32 i = 0; i < amt; i++) {
                Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.RedTorch, npc.velocity.X, npc.velocity.Y, 100, Color.Red with { A = 0 }, 3f);
                dust.noGravity = true;
                //ParticleSystem.New<MonoBloomParticle>(ParticleLayer.BehindPlayers).Setup(
                //    Main.rand.NextFromRect(npc.Hitbox),
                //    -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-2f, 2f), -Main.rand.NextFloat(-2f, 2f)),
                //    new Color(180, 90, 40, 60) * 0.5f, new Color(6, 0, 3, 0),
                //    Main.rand.NextFloat(2f, 3f), 0.3f,
                //    Main.rand.NextFloat(MathHelper.TwoPi)
                //);
            }
        }
    }

    public override void UpdateLifeRegen(NPC npc, ref System.Int32 damage) {
        if (!hasDebuff) {
            return;
        }

        if (npc.lifeRegen > 0) {
            npc.lifeRegen = 0;
        }

        npc.lifeRegen -= IronLotusDebuff.Damage;
        damage = IronLotusDebuff.DamageNumber;

        if (npc.oiled) {
            npc.lifeRegen -= 50;
        }
    }
}
