using Aequus.Common.NPCs;
using Aequus.Common.Personalities;
using Aequus.Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Profiles;

namespace Aequus.NPCs.Town {
    public abstract class AequusTownNPC : ModNPC, IModifyShoppingSettings, NPCHooks.ITalkNPCUpdate {
        public bool ShowExclamation;
        public float ExclamationOpacity;
        public byte CheckExclamationTimer;

        public override void SetStaticDefaults() {
            Main.npcFrameCount[NPC.type] = 25;
            NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
            NPCID.Sets.AttackFrameCount[NPC.type] = 4;
            NPCID.Sets.DangerDetectRange[NPC.type] = 400;

            NPCID.Sets.ShimmerTownTransform[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[Type] = new(0) {
                Velocity = 1f,
                Direction = -1,
            };
        }

        public override void SetDefaults() {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = 7;
            NPC.damage = 10;
            NPC.defense = 50;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.1f;
            AnimationType = NPCID.Guide;
        }

        protected virtual bool CheckExclamation() {
            return false;
        }

        public override void AI() {
            base.AI();
            if (CheckExclamationTimer > 0) {
                CheckExclamationTimer--;
            }
            else {
                ShowExclamation = CheckExclamation();
            }

            if (ShowExclamation) {
                ExclamationOpacity = MathHelper.Lerp(ExclamationOpacity, 1f, 0.1f);
                Helper.AddClamp(ref ExclamationOpacity, 0.01f);
            }
            else {
                ExclamationOpacity = MathHelper.Lerp(ExclamationOpacity, 0f, 0.1f);
                Helper.AddClamp(ref ExclamationOpacity, -0.01f);
            }
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName) {
            CheckExclamationTimer = 0;
        }

        public virtual void ModifyShoppingSettings(Player player, NPC npc, ref ShoppingSettings settings, ShopHelper shopHelper) {
        }

        public virtual void TalkNPCUpdate(Player player) {
        }

        protected virtual bool PreDrawExclamation(SpriteBatch spriteBatch, Vector2 screenPos, Color npcDrawColor) {
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            if (ExclamationOpacity > 0f && !NPC.IsABestiaryIconDummy && PreDrawExclamation(spriteBatch, screenPos, drawColor)) {
                var texture = AequusTextures.TownNPCExclamation;
                float opacity = ExclamationOpacity;
                float scale = Helper.Wave(Main.GlobalTimeWrappedHourly * 2.5f, 0.9f, 1.1f) * opacity;
                var drawPosition = (NPC.Top + new Vector2(0f, -6f - 20f * MathF.Pow(opacity, 3f)) - screenPos).Floor();
                var origin = texture.Size() / 2f;
                var clr = new Color(150, 150, 255, 222) * opacity;
                MiscWorldInterfaceElements.DrawData.Add(new(
                    AequusTextures.Bloom0,
                    drawPosition,
                    null, Color.Black * opacity * 0.2f, 0f, AequusTextures.Bloom0.Size() / 2f, 0.5f, SpriteEffects.None, 0f
                ));

                if (scale > 1f) {
                    float auraOpacity = (scale - 1f) / 0.1f;
                    var spinningPoint = new Vector2(2f, 0f);
                    for (int i = 0; i < 4; i++) {
                        MiscWorldInterfaceElements.DrawData.Add(new(
                            texture,
                            drawPosition + spinningPoint.RotatedBy(i * MathHelper.PiOver2),
                            null, clr with { A = 0 } * auraOpacity, 0f, origin, scale, SpriteEffects.None, 0f
                        ));
                    }
                }

                MiscWorldInterfaceElements.DrawData.Add(new(
                    texture,
                    drawPosition,
                    null, clr, 0f, origin, scale, SpriteEffects.None, 0f
                ));
            }
            return true;
        }
    }

    public abstract class AequusTownNPC<T> : AequusTownNPC where T : AequusTownNPC<T> {
        public static int ShimmerHeadIndex;
        public static StackedNPCProfile Profile;

        public override void Load() {
            ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, Texture + "_Shimmer_Head");
        }

        public override ITownNPCProfile TownNPCProfile() {
            return Profile;
        }
    }
}