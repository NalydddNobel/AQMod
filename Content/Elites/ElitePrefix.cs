using Aequus.Common.Effects;
using Aequus.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Elites
{
    public abstract class ElitePrefix : ModType<NPC, ElitePrefix>
    {
        public abstract string EliteName { get; }

        public MiscShaderWrap Shader { get; private set; }
        public abstract Vector3 ShaderColor { get; }

        public int Type { get; private set; }

        public virtual bool PreAI(NPC npc)
        {
            return true;
        }
        public virtual void AI(NPC npc)
        {
        }
        public virtual void PostAI(NPC npc)
        {
        }

        public virtual bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin_World(shader: true);
            Shader.Apply(null);
            return true;
        }

        public virtual void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin_World(shader: false);
        }

        protected sealed override void Register()
        {
        }

        public sealed override void SetupContent()
        {
            if (!Main.dedServ)
            {
                Shader = new MiscShaderWrap($"{this.NamespacePath()}/MossEnemyShader", $"{Name}", "MossEnemyShaderPass", loadStatics: true);
                Shader.UseColor(ShaderColor);
                Shader.UseImage1(ModContent.Request<Texture2D>($"{Aequus.VanillaTexture}Misc/noise"));
            }
            Type = AequusNPC.Elites.Count;
            AequusNPC.Elites.Add(this);
            SetStaticDefaults();
        }

        protected virtual void SetupClone(ElitePrefix clone) {

        }

        public ElitePrefix ProperClone(NPC npc) {
            var clone = Clone(npc);
            SetupClone(clone);
            return clone;
        }

        protected override NPC CreateTemplateEntity() {
            return null;
        }

        public virtual void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {

        }
        public virtual void RecieveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {

        }
    }

    public class ArgonElite : ElitePrefix
    {
        public override string EliteName => TextHelper.GetTextValue("EliteName.ArgonElite");
        public override Vector3 ShaderColor => new Vector3(1f, 0f, 0.5f);

        public float scaleIncrease = 0f;

        private void Init(NPC npc) {
            scaleIncrease = npc.scale + 0.5f;

            int oldWidth = npc.width;
            int oldHeight = npc.height;
            int oldLifeMax = npc.lifeMax;
            npc.lifeMax = (int)(npc.lifeMax * 1.66f);
            npc.life += npc.lifeMax - oldLifeMax;
            npc.width = (int)(npc.width * scaleIncrease);
            npc.height = (int)(npc.height * scaleIncrease);
            npc.position.X -= (npc.width - oldWidth) / 2f;
            npc.position.Y -= (npc.height - oldHeight) / 2f;
            npc.knockBackResist = 0f;
        }

        public override void PostAI(NPC npc)
        {
            npc.StatSpeed() *= 0.8f;
            if (scaleIncrease == 0f) {
                Init(npc);
            }

            if (npc.scale < scaleIncrease) {
                npc.scale += 0.01f;
                if (npc.scale > scaleIncrease) {
                    npc.scale = scaleIncrease;
                }
            }
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {
            binaryWriter.Write(scaleIncrease);
            binaryWriter.Write(npc.lifeMax);
        }

        public override void RecieveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {
            scaleIncrease = binaryReader.ReadSingle();
            npc.lifeMax = binaryReader.ReadInt32();
        }
    }

    public class KryptonElite : ElitePrefix
    {
        public override string EliteName => TextHelper.GetTextValue("EliteName.KryptonElite");
        public override Vector3 ShaderColor => new Vector3(0.5f, 1f, 0f);

        public byte checkTimer;

        private void CheckShield(NPC npc) {

            if (Main.netMode == NetmodeID.MultiplayerClient) {
                return;
            }

            for (int i = 0; i < Main.maxNPCs; i++) {
                if (!Main.npc[i].active || Main.npc[i].ModNPC is not KryptonShield shield) {
                    continue;
                }

                if (shield.NPCOwner == npc.whoAmI) {
                    return;
                }
            }

            NPC.NewNPC(npc.GetSource_FromThis(), (int)npc.position.X, (int)npc.position.Y,
                ModContent.NPCType<KryptonShield>(), npc.whoAmI, ai1: npc.whoAmI + 1);
        }

        public override void PostAI(NPC npc)
        {
            if (checkTimer == 0) {
                // TODO: Replace with on-hit defense instead of modifying defense stat?
                npc.defDefense += 20;
                npc.defense += 20;
                npc.knockBackResist *= 0.25f;
                if (npc.knockBackResist < 0.1f) {
                    npc.knockBackResist = 0f;
                }
            }
            npc.StatSpeed() += 0.1f;
            checkTimer++;
            if (checkTimer > 60) {
                checkTimer = 1;
                CheckShield(npc);
            }
        }
    }

    public class NeonElite : ElitePrefix
    {
        public override string EliteName => TextHelper.GetTextValue("EliteName.NeonElite");
        public override Vector3 ShaderColor => new Vector3(0.5f, 0f, 1f);

        public ushort attackTimer;

        public override void PostAI(NPC npc)
        {
            if (!npc.HasValidTarget || !npc.HasPlayerTarget) {
                return;
            }

            var target = Main.player[npc.target];

            if (attackTimer > 280) {
                attackTimer = 1;
                npc.netUpdate = true;
                return;
            }

            if (attackTimer < 120) {
                if (npc.Distance(target.Center) > 400f || !Collision.CanHitLine(npc.position, npc.width, npc.height, target.position, target.width, target.height)) {

                    if (attackTimer > 100) {
                        attackTimer--;
                    }
                    return;
                }
            }

            attackTimer++;

            if (attackTimer == 120 || attackTimer == 180) {
                npc.netUpdate = true;
            }
            if (attackTimer == 120) {
                SoundEngine.PlaySound(AequusSounds.neonCharge, npc.Center);
            }

            if (attackTimer > 120 && attackTimer < 180) {
                npc.StatSpeed() *= Math.Max(1f - (attackTimer - 120) / 60f, 0.33f);
                var d = Dust.NewDustDirect(npc.Center - new Vector2(20f), 40, 40, DustID.CrystalPulse);
                d.velocity = (npc.Center - d.position) / 10f;
                d.noGravity = true;
            }
            if (attackTimer == 180) {
                if (Main.netMode != NetmodeID.MultiplayerClient) {

                    float speed = Main.expertMode ? 3f : 1.5f;
                    Projectile.NewProjectile(
                        npc.GetSource_FromAI(),
                        npc.Center,
                        npc.DirectionTo(Main.player[npc.target].Center) * speed,
                        ModContent.ProjectileType<NeonAttack>(),
                        14,
                        0f,
                        Main.myPlayer
                    );
                }
            }
            if (attackTimer > 180 && attackTimer < 190) {
                npc.velocity = npc.DirectionFrom(Main.player[npc.target].Center) * 4.5f;
            }
            if (attackTimer > 180 && attackTimer < 220 && Main.rand.NextBool(4)) {
                var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.CrystalPulse);
                d.velocity = -npc.velocity * 0.5f;
                d.noGravity = true;
            }
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {
            binaryWriter.Write(attackTimer);
        }

        public override void RecieveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {
            attackTimer = binaryReader.ReadUInt16();
        }
    }

    public class XenonElite : ElitePrefix
    {
        public override string EliteName => TextHelper.GetTextValue("EliteName.XenonElite");
        public override Vector3 ShaderColor => new Vector3(0f, 0.5f, 1f);

        public ushort attackTimer;

        public override void PostAI(NPC npc)
        {
            npc.StatSpeed() += 0.33f;
            attackTimer++;
            if (attackTimer > 40) {
                attackTimer = 0;
                if (Main.netMode != NetmodeID.MultiplayerClient) {

                    int size = 16;
                    var center = npc.Center - new Vector2(size / 2f);
                    for (int i = 0; i < 30; i++) {
                        var pos = center + new Vector2(Main.rand.NextFloat(-48f, 48f), Main.rand.NextFloat(-48f, 48f));
                        if (!Collision.SolidCollision(pos, size, size)) {
                            Projectile.NewProjectile(
                                npc.GetSource_FromAI(), 
                                pos, 
                                Vector2.Zero, 
                                ModContent.ProjectileType<XenonSpore>(),
                                10,
                                0f,
                                Main.myPlayer
                            );
                            break;
                        }
                    }
                }
            }
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {
            binaryWriter.Write(attackTimer);
        }

        public override void RecieveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {
            attackTimer = binaryReader.ReadUInt16();
        }
    }
}