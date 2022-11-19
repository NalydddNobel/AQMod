using Aequus.Buffs.Minion;
using Aequus.Graphics;
using Aequus.NPCs.Friendly.Town;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon
{
    public class ScribbleNotebookMinion : ModProjectile
    {
        public const int HorizontalFrames = 12;

        public class EmojiID
        {
            public const int Money = 0;
            public const int Medical = 1;
            public const int Wrench = 2;
            public const int Heart = 3;
            public const int QuestionMark = 4;
            public const int Attack = 5;
            public const int Smile = 6;
            public const int Food = 7;
            public const int Following = 8;
            public const int Fish = 9;
            public const int Flame = 10;
            public const int Bomb = 11;
            public const int Exclamation = 12;
            public const int Energetic = 13;
            /// <summary>
            /// 🥵🥵🥵🥵🥵🥵🥵🥵🥵🥵🥵🥵🥵🥵🥵🥵🥵 so thirsty uuuuuuuuooooooooooooooooooh~
            /// </summary>
            public const int Thirsty = 14;
            public const int Gravestone = 15;
        }

        public struct MinionTexture
        {
            public string Texture;
            public int ReferenceItem;
            public Rectangle? Frame;

            public MinionTexture(string texture, Rectangle? frame = null)
            {
                Texture = texture;
                Frame = frame;
                ReferenceItem = 0;
            }

            public static MinionTexture Vanilla(string texture, Rectangle? frame = null)
            {
                return new MinionTexture(Aequus.VanillaTexture + texture, frame);
            }
            public static MinionTexture VanillaItem(int itemID, Rectangle? frame = null)
            {
                var i = Vanilla("Item_" + itemID, frame);
                i.ReferenceItem = itemID;
                return i;
            }
        }

        public static List<MinionTexture> Textures { get; private set; }

        public float rotation;
        public int emoji;
        public int emojiTime;
        public int texture;
        public int movingFast;

        public override void Load()
        {
            Textures = new List<MinionTexture>()
            {
                MinionTexture.VanillaItem(ItemID.CookingPot),
                MinionTexture.VanillaItem(ItemID.Chest),
                MinionTexture.VanillaItem(ItemID.GoldChest),
                MinionTexture.VanillaItem(ItemID.Bottle),
                MinionTexture.VanillaItem(ItemID.EmptyBucket),
                MinionTexture.VanillaItem(ItemID.ClayPot),
                MinionTexture.VanillaItem(ItemID.Barrel),
                MinionTexture.VanillaItem(ItemID.Safe),
                MinionTexture.VanillaItem(ItemID.TrashCan),
                MinionTexture.VanillaItem(ItemID.Mug),
                MinionTexture.VanillaItem(ItemID.Mushroom),
                MinionTexture.VanillaItem(ItemID.GlowingMushroom),
                MinionTexture.VanillaItem(ItemID.Toilet),
                MinionTexture.VanillaItem(ItemID.WorkBench),
                MinionTexture.VanillaItem(ItemID.Compass),
                MinionTexture.VanillaItem(ItemID.Toolbelt),
                MinionTexture.VanillaItem(ItemID.SlimeStatue),
                MinionTexture.VanillaItem(ItemID.IronAnvil),
                MinionTexture.VanillaItem(ItemID.LeadAnvil),
                MinionTexture.VanillaItem(ItemID.AnvilStatue),
                MinionTexture.VanillaItem(ItemID.AngelStatue),
                MinionTexture.VanillaItem(ItemID.ArmorStatue),
                MinionTexture.VanillaItem(ItemID.Mannequin),
                MinionTexture.VanillaItem(ItemID.Womannquin),
                MinionTexture.VanillaItem(ItemID.TitanGlove),
                MinionTexture.VanillaItem(ItemID.PhilosophersStone),
                MinionTexture.VanillaItem(ItemID.DartTrap),
                MinionTexture.VanillaItem(ItemID.Boulder),
                MinionTexture.VanillaItem(ItemID.Book),
                MinionTexture.VanillaItem(ItemID.SpellTome),
                MinionTexture.VanillaItem(ItemID.SnowGlobe),
                MinionTexture.VanillaItem(ItemID.Tombstone),
                MinionTexture.VanillaItem(ItemID.GraveMarker),
                MinionTexture.VanillaItem(ItemID.CrossGraveMarker),
                MinionTexture.VanillaItem(ItemID.Headstone),
                MinionTexture.VanillaItem(ItemID.Gravestone),
                MinionTexture.VanillaItem(ItemID.Obelisk),
                MinionTexture.VanillaItem(ItemID.MusicBox),
                MinionTexture.VanillaItem(ItemID.PumpkinPie),
                MinionTexture.VanillaItem(ItemID.JellyfishDivingGear),
                MinionTexture.VanillaItem(ItemID.Present),
                MinionTexture.VanillaItem(ItemID.BluePresent),
                MinionTexture.VanillaItem(ItemID.GreenPresent),
                MinionTexture.VanillaItem(ItemID.WoodenCrate),
                MinionTexture.VanillaItem(ItemID.WoodenCrateHard),
                MinionTexture.VanillaItem(ItemID.IronCrate),
                MinionTexture.VanillaItem(ItemID.Piano),
                MinionTexture.VanillaItem(ItemID.ShipsWheel),
                MinionTexture.VanillaItem(ItemID.TartarSauce),
                MinionTexture.VanillaItem(ItemID.LargeAmethyst),
                MinionTexture.VanillaItem(ItemID.LargeTopaz),
                MinionTexture.VanillaItem(ItemID.LargeSapphire),
                MinionTexture.VanillaItem(ItemID.LargeEmerald),
                MinionTexture.VanillaItem(ItemID.LargeRuby),
                MinionTexture.VanillaItem(ItemID.LargeDiamond),
                MinionTexture.VanillaItem(ItemID.LargeAmber),
                MinionTexture.VanillaItem(ItemID.DD2ElderCrystal),
                MinionTexture.VanillaItem(ItemID.DD2PetGato),
                MinionTexture.VanillaItem(ItemID.Apple),
                MinionTexture.VanillaItem(ItemID.Lemon),
                MinionTexture.VanillaItem(ItemID.Coconut),
                MinionTexture.VanillaItem(ItemID.Burger),
                MinionTexture.VanillaItem(ItemID.ApplePie),
                MinionTexture.VanillaItem(ItemID.ChickenNugget),
                MinionTexture.VanillaItem(ItemID.RoastedDuck),
                MinionTexture.VanillaItem(ItemID.RollingCactus),
                MinionTexture.VanillaItem(ItemID.Football),
                MinionTexture.VanillaItem(ItemID.SuperAbsorbantSponge),
            };
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
            Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            this.SetTrail(12);
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BabySlime);
            AIType = ProjectileID.BabySlime;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.extraUpdates = 1;
            Projectile.localNPCHitCooldown = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.usesIDStaticNPCImmunity = false;
            emoji = -1;
        }

        public override bool MinionContactDamage()
        {
            return true;
        }

        public override bool PreAI()
        {
            Main.player[Projectile.owner].slime = false;
            return true;
        }

        public override void AI()
        {
            if (!AequusHelpers.UpdateProjActive<ScribbleNotebookBuff>(Projectile))
            {
                return;
            }
            if (texture == 0)
            {
                texture = Main.rand.Next(Textures.Count);
                Projectile.netUpdate = true;
            }
            if (Projectile.tileCollide)
            {
                rotation += Projectile.velocity.X / 32f;
            }
            else
            {
                rotation += Projectile.velocity.Length() / 40f * Projectile.direction;
            }
            if (Projectile.velocity.Length() > 6f)
            {
                movingFast++;
            }
            else
            {
                if (movingFast > 0)
                    movingFast--;
            }
            Projectile.rotation = rotation;
            Projectile.CollideWithOthers();
            UpdateEmoji();
        }
        public void UpdateEmoji()
        {
            if (Projectile.numUpdates == -1)
            {
                if (emoji != -1)
                {
                    emojiTime++;
                    if (emojiTime > 180)
                    {
                        emoji = -1;
                        emojiTime = 0;
                        Projectile.netUpdate = true;
                    }
                }
                else
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        int chance = 400;
                        int target = Projectile.FindTargetWithLineOfSight();
                        if (target != -1)
                        {
                            chance /= 5;
                        }
                        else
                        {
                            if (Projectile.velocity.Length() > 4f)
                                chance *= 5;
                            if (Main.player[Projectile.owner].HeldItem.buffType > 0 && BuffID.Sets.IsWellFed[Main.player[Projectile.owner].HeldItem.buffType])
                                chance /= 4;
                        }
                        if (Main.rand.NextBool(chance))
                        {
                            var l = PickEmojis(target != -1);
                            if (l.Count > 0)
                            {
                                emoji = Main.rand.Next(l);
                                SoundEngine.PlaySound(SoundID.Chat, Projectile.Center);
                                Projectile.netUpdate = true;
                            }
                        }
                    }
                }
            }
        }
        public List<int> PickEmojis(bool enemiesNearby)
        {
            var l = new List<int>();
            if (enemiesNearby)
            {
                l.Add(EmojiID.Attack);
                l.Add(EmojiID.Bomb);
                l.Add(EmojiID.Flame);
                return l;
            }

            var player = Main.player[Projectile.owner];
            var heldItem = Main.player[Projectile.owner].HeldItem;
            if (Main.dontStarveWorld || (heldItem.buffType > 0 && BuffID.Sets.IsWellFed[heldItem.buffType]))
            {
                l.Add(EmojiID.Food);
            }
            bool nearNurse = Main.player[Projectile.owner].isNearNPC(NPCID.Nurse, 800f);
            if (nearNurse || Main.player[Projectile.owner].statLife / (float)Main.player[Projectile.owner].statLifeMax2 < 0.5f)
            {
                l.Add(EmojiID.Medical);
            }
            if (Main.player[Projectile.owner].adjWater || Main.player[Projectile.owner].isNearNPC(NPCID.Angler, 800f)
                || Main.player[Projectile.owner].isNearNPC(NPCID.Goldfish, 800f) || Main.player[Projectile.owner].isNearNPC(NPCID.Pupfish, 800f))
            {
                l.Add(EmojiID.Fish);
                l.Add(EmojiID.Thirsty);
            }
            bool nearMechanic = player.isNearNPC(NPCID.Mechanic, 800f);
            if (heldItem.mech || player.head == ArmorIDs.Head.EngineeringHelmet || nearMechanic)
            {
                l.Add(EmojiID.Wrench);
            }
            if (player.loveStruck || player.blackCat
                || (nearMechanic && player.isNearNPC(NPCID.GoblinTinkerer, 800f))
                || (nearNurse && player.isNearNPC(NPCID.ArmsDealer, 800f)))
            {
                l.Add(EmojiID.Heart);
            }
            if (player.companionCube)
            {
                l.Add(EmojiID.Heart);
                l.Add(EmojiID.Flame);
            }
            if ((heldItem.type > ItemID.None && ItemID.Sets.ItemsThatCountAsBombsForDemolitionistToSpawn[heldItem.type]) || player.isNearNPC(NPCID.Demolitionist, 800f))
            {
                l.Add(EmojiID.Bomb);
                l.Add(EmojiID.Flame);
            }
            if (player.isNearNPC(ModContent.NPCType<Occultist>(), 800f))
            {
                l.Add(EmojiID.Flame);
            }

            if (l.Count == 0 || Main.rand.NextBool(3))
            {
                l.Add(EmojiID.Following);
                l.Add(EmojiID.Money);
                if (movingFast > 60)
                {
                    l.Add(EmojiID.Energetic);
                    l.Add(EmojiID.Thirsty);
                }
                else
                {
                    l.Add(EmojiID.QuestionMark);
                    l.Add(EmojiID.Wrench);
                    l.Add(EmojiID.Smile);
                }
                if (Main.GraveyardVisualIntensity > 0.2f)
                {
                    l.Add(EmojiID.Gravestone);
                }
            }
            return l;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            EffectsSystem.ProjsBehindProjs.Add(Projectile.whoAmI);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            GetDrawingInfo(out var image, out var frame);
            var off = new Vector2(Projectile.width / 2f, Projectile.height - frame.Height / 2f);
            if (EffectsSystem.ProjsBehindProjs.RenderingNow)
            {
                int trailLength = ProjectileID.Sets.TrailCacheLength[Type];
                var origin = frame.Size() / 2f;
                for (int i = 0; i < trailLength; i++)
                {
                    float p = AequusHelpers.CalcProgress(trailLength, i);
                    Main.EntitySpriteDraw(image, Projectile.oldPos[i] + off - Main.screenPosition, frame, lightColor * p * p * 0.5f, Projectile.oldRot[i], origin, Projectile.scale * p, SpriteEffects.None, 0);
                }
                Main.EntitySpriteDraw(image, Projectile.position + off - Main.screenPosition, frame, lightColor, rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }
            else
            {
                if (emoji > -1)
                    DrawEmoji(frame, Projectile.position + off - Main.screenPosition);
            }
            return false;
        }
        public void DrawEmoji(Rectangle projectileFrame, Vector2 drawPosition)
        {
            float scale = 1f;
            if (emojiTime < 6)
            {
                scale = emojiTime / 7f;
            }
            if (emojiTime > 180 - 6)
            {
                scale = (180 - emojiTime) / 7f;
            }
            int direction = (Projectile.Center.X < Main.player[Projectile.owner].Center.X) ? -1 : 1;
            var frame = TextureAssets.Projectile[Type].Value.Frame(HorizontalFrames, Main.projFrames[Type],
                emoji * 2 % HorizontalFrames + (direction == -1 ? 1 : 0), emoji * 2 / HorizontalFrames);
            var origin = new Vector2(direction == -1 ? frame.Width : 0, frame.Height);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, drawPosition + new Vector2((projectileFrame.Width / 2f) * Projectile.scale * direction, -projectileFrame.Height / 2f),
                frame, Color.White, 0f, origin, Projectile.scale * scale, SpriteEffects.None, 0);
        }

        public void GetDrawingInfo(out Texture2D image, out Rectangle frame)
        {
            var textureData = Textures[texture];
            var assetImage = ModContent.Request<Texture2D>(textureData.Texture, AssetRequestMode.ImmediateLoad);
            frame = Rectangle.Empty;
            if (textureData.Frame == null)
            {
                if (assetImage.IsLoaded)
                {
                    if (textureData.ReferenceItem != 0 && Main.itemAnimations[textureData.ReferenceItem] != null)
                    {
                        frame = Main.itemAnimations[textureData.ReferenceItem].GetFrame(assetImage.Value);
                    }
                    else
                    {
                        frame = assetImage.Value.Bounds;
                    }

                }
            }
            else
            {
                frame = textureData.Frame.Value;
            }
            image = assetImage.Value;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(emoji);
            writer.Write(emojiTime);
            writer.Write(texture);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            emoji = reader.ReadInt32();
            emojiTime = reader.ReadInt32();
            texture = reader.ReadInt32();
        }
    }
}