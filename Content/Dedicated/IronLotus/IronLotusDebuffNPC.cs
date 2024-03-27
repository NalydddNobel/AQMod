namespace Aequus.Content.Dedicated.IronLotus;

public class IronLotusDebuffNPC : GlobalNPC {
    public bool hasDebuff;

    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
        return !entity.buffImmune[ModContent.BuffType<IronLotusDebuff>()];
    }

    public override void ResetEffects(NPC npc) {
        hasDebuff = false;
    }

    public override void PostAI(NPC npc) {
        if (!hasDebuff) {
            return;
        }

        int debuff = npc.FindBuffIndex(ModContent.BuffType<IronLotusDebuff>());
        if (debuff == -1) {
            return;
        }

        int plr = Player.FindClosest(npc.position, npc.width, npc.height);
        for (int i = 0; i < Main.maxNPCs; i++) {
            if (Main.npc[i].active && (Main.npc[i].type == NPCID.TargetDummy || Main.npc[i].CanBeChasedBy(Main.player[plr])) && Main.npc[i].Distance(npc.Center) < 100f) {
                Main.npc[i].AddBuff(ModContent.BuffType<IronLotusDebuff>(), npc.buffTime[debuff]);
            }
        }

        if (Main.netMode != NetmodeID.Server) {
            int amt = (int)(npc.Size.Length() / 32f);
            for (int i = 0; i < amt; i++) {
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

    public override void UpdateLifeRegen(NPC npc, ref int damage) {
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
