using Aequu2.Core.CodeGeneration;
using Aequu2.Core.ContentGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.GameContent;

namespace Aequu2.Old.Content.Items.Weapons.Summon.Minions.ScribbleMinion;

[Gen.Aequu2Player_Field<bool>("minionScribble")]
public class ScribbleNotebookMinion : UnifiedModMinion {
    public const int HorizontalFrames = 12;

    public class EmojiID {
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
        public const int Thirsty = 14;
        public const int Gravestone = 15;
    }

    public interface IMinionTextureProvider {
        void GetDrawData(out Texture2D Texture, out Rectangle Frame);
    }
    public readonly record struct ItemTextureProvider(int ItemID) : IMinionTextureProvider {
        public void GetDrawData(out Texture2D Texture, out Rectangle Frame) {
            Main.GetItemDrawFrame(ItemID, out Texture, out Frame);
        }
    }

    public static readonly List<IMinionTextureProvider> Textures = [
            new ItemTextureProvider(ItemID.CookingPot),
            new ItemTextureProvider(ItemID.Chest),
            new ItemTextureProvider(ItemID.GoldChest),
            new ItemTextureProvider(ItemID.Bottle),
            new ItemTextureProvider(ItemID.EmptyBucket),
            new ItemTextureProvider(ItemID.ClayPot),
            new ItemTextureProvider(ItemID.Barrel),
            new ItemTextureProvider(ItemID.Safe),
            new ItemTextureProvider(ItemID.TrashCan),
            new ItemTextureProvider(ItemID.Mug),
            new ItemTextureProvider(ItemID.Mushroom),
            new ItemTextureProvider(ItemID.GlowingMushroom),
            new ItemTextureProvider(ItemID.Toilet),
            new ItemTextureProvider(ItemID.WorkBench),
            new ItemTextureProvider(ItemID.Compass),
            new ItemTextureProvider(ItemID.Toolbelt),
            new ItemTextureProvider(ItemID.SlimeStatue),
            new ItemTextureProvider(ItemID.IronAnvil),
            new ItemTextureProvider(ItemID.LeadAnvil),
            new ItemTextureProvider(ItemID.AnvilStatue),
            new ItemTextureProvider(ItemID.AngelStatue),
            new ItemTextureProvider(ItemID.ArmorStatue),
            new ItemTextureProvider(ItemID.Mannequin),
            new ItemTextureProvider(ItemID.Womannquin),
            new ItemTextureProvider(ItemID.TitanGlove),
            new ItemTextureProvider(ItemID.PhilosophersStone),
            new ItemTextureProvider(ItemID.DartTrap),
            new ItemTextureProvider(ItemID.Boulder),
            new ItemTextureProvider(ItemID.Book),
            new ItemTextureProvider(ItemID.SpellTome),
            new ItemTextureProvider(ItemID.SnowGlobe),
            new ItemTextureProvider(ItemID.Tombstone),
            new ItemTextureProvider(ItemID.GraveMarker),
            new ItemTextureProvider(ItemID.CrossGraveMarker),
            new ItemTextureProvider(ItemID.Headstone),
            new ItemTextureProvider(ItemID.Gravestone),
            new ItemTextureProvider(ItemID.Obelisk),
            new ItemTextureProvider(ItemID.MusicBox),
            new ItemTextureProvider(ItemID.PumpkinPie),
            new ItemTextureProvider(ItemID.JellyfishDivingGear),
            new ItemTextureProvider(ItemID.Present),
            new ItemTextureProvider(ItemID.GreenPresent),
            new ItemTextureProvider(ItemID.WoodenCrate),
            new ItemTextureProvider(ItemID.WoodenCrateHard),
            new ItemTextureProvider(ItemID.IronCrate),
            new ItemTextureProvider(ItemID.Piano),
            new ItemTextureProvider(ItemID.ShipsWheel),
            new ItemTextureProvider(ItemID.TartarSauce),
            new ItemTextureProvider(ItemID.LargeAmethyst),
            new ItemTextureProvider(ItemID.LargeTopaz),
            new ItemTextureProvider(ItemID.LargeSapphire),
            new ItemTextureProvider(ItemID.LargeEmerald),
            new ItemTextureProvider(ItemID.LargeRuby),
            new ItemTextureProvider(ItemID.LargeDiamond),
            new ItemTextureProvider(ItemID.LargeAmber),
            new ItemTextureProvider(ItemID.DD2ElderCrystal),
            new ItemTextureProvider(ItemID.DD2PetGato),
            new ItemTextureProvider(ItemID.Apple),
            new ItemTextureProvider(ItemID.Lemon),
            new ItemTextureProvider(ItemID.Coconut),
            new ItemTextureProvider(ItemID.Burger),
            new ItemTextureProvider(ItemID.ApplePie),
            new ItemTextureProvider(ItemID.ChickenNugget),
            new ItemTextureProvider(ItemID.RoastedDuck),
            new ItemTextureProvider(ItemID.RollingCactus),
            new ItemTextureProvider(ItemID.Football),
            new ItemTextureProvider(ItemID.SuperAbsorbantSponge),
        ];

    public float rotation;
    public int emoji;
    public int emojiTime;
    public int textureSlot;
    public int movingFast;

    public override void SetStaticDefaults() {
        Main.projFrames[Type] = 3;
        Main.projPet[Projectile.type] = true;

        ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
        ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        this.SetTrail(12);
    }

    public override void SetDefaults() {
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

    public override bool MinionContactDamage() {
        return true;
    }

    public override bool PreAI() {
        Main.player[Projectile.owner].slime = false;
        return true;
    }

    public override void AI() {
        base.AI();
        if (textureSlot == 0) {
            textureSlot = Main.rand.Next(Textures.Count);
            Projectile.netUpdate = true;
        }
        if (Projectile.tileCollide) {
            rotation += Projectile.velocity.X / 32f;
        }
        else {
            rotation += Projectile.velocity.Length() / 40f * Projectile.direction;
        }
        if (Projectile.velocity.Length() > 6f) {
            movingFast++;
        }
        else {
            if (movingFast > 0) {
                movingFast--;
            }
        }
        Projectile.rotation = rotation;
        Projectile.CollideWithOthers();
        UpdateEmoji();
    }
    public void UpdateEmoji() {
        if (Projectile.numUpdates == -1) {
            if (emoji != -1) {
                emojiTime++;
                if (emojiTime > 180) {
                    emoji = -1;
                    emojiTime = 0;
                    Projectile.netUpdate = true;
                }
            }
            else if (Main.myPlayer == Projectile.owner) {
                int chance = 400;
                int target = Projectile.FindTargetWithLineOfSight();
                if (target != -1) {
                    chance /= 5;
                }
                else {
                    if (Projectile.velocity.Length() > 4f)
                        chance *= 5;
                    if (Main.player[Projectile.owner].HeldItem.buffType > 0 && BuffSets.IsWellFed[Main.player[Projectile.owner].HeldItem.buffType])
                        chance /= 4;
                }
                if (Main.rand.NextBool(chance)) {
                    var l = PickEmojis(target != -1);
                    if (l.Count > 0) {
                        emoji = Main.rand.Next(l);
                        //SoundEngine.PlaySound(SoundID.Chat, Projectile.Center);
                        Projectile.netUpdate = true;
                    }
                }
            }
        }
    }
    public List<int> PickEmojis(bool enemiesNearby) {
        var l = new List<int>();
        if (enemiesNearby) {
            l.Add(EmojiID.Attack);
            l.Add(EmojiID.Bomb);
            l.Add(EmojiID.Flame);
            return l;
        }

        var player = Main.player[Projectile.owner];
        var heldItem = Main.player[Projectile.owner].HeldItem;
        if (Main.dontStarveWorld || heldItem.buffType > 0 && BuffSets.IsFedState[heldItem.buffType]) {
            l.Add(EmojiID.Food);
        }
        bool nearNurse = Main.player[Projectile.owner].isNearNPC(NPCID.Nurse, 800f);
        if (nearNurse || Main.player[Projectile.owner].statLife / (float)Main.player[Projectile.owner].statLifeMax2 < 0.5f) {
            l.Add(EmojiID.Medical);
        }
        if (Main.player[Projectile.owner].adjWater || Main.player[Projectile.owner].isNearNPC(NPCID.Angler, 800f)
            || Main.player[Projectile.owner].isNearNPC(NPCID.Goldfish, 800f) || Main.player[Projectile.owner].isNearNPC(NPCID.Pupfish, 800f)) {
            l.Add(EmojiID.Fish);
            l.Add(EmojiID.Thirsty);
        }
        bool nearMechanic = player.isNearNPC(NPCID.Mechanic, 800f);
        if (heldItem.mech || player.head == ArmorIDs.Head.EngineeringHelmet || nearMechanic) {
            l.Add(EmojiID.Wrench);
        }
        if (player.loveStruck || player.blackCat
            || nearMechanic && player.isNearNPC(NPCID.GoblinTinkerer, 800f)
            || nearNurse && player.isNearNPC(NPCID.ArmsDealer, 800f)) {
            l.Add(EmojiID.Heart);
        }
        if (player.companionCube) {
            l.Add(EmojiID.Heart);
            l.Add(EmojiID.Flame);
        }
        if (heldItem.type > ItemID.None && ItemSets.ItemsThatCountAsBombsForDemolitionistToSpawn[heldItem.type] || player.isNearNPC(NPCID.Demolitionist, 800f)) {
            l.Add(EmojiID.Bomb);
            l.Add(EmojiID.Flame);
        }
        if (player.isNearNPC(ModContent.NPCType<TownNPCs.OccultistNPC.Occultist>(), 800f)) {
            l.Add(EmojiID.Flame);
        }

        if (l.Count == 0 || Main.rand.NextBool(3)) {
            l.Add(EmojiID.Following);
            l.Add(EmojiID.Money);
            if (movingFast > 60) {
                l.Add(EmojiID.Energetic);
                l.Add(EmojiID.Thirsty);
            }
            else {
                l.Add(EmojiID.QuestionMark);
                l.Add(EmojiID.Wrench);
                l.Add(EmojiID.Smile);
            }
            if (Main.GraveyardVisualIntensity > 0.2f) {
                l.Add(EmojiID.Gravestone);
            }
        }
        return l;
    }

    public override bool PreDraw(ref Color lightColor) {
        Textures[this.textureSlot].GetDrawData(out Texture2D texture, out Rectangle frame);
        var off = new Vector2(Projectile.width / 2f, Projectile.height - frame.Height / 2f);
        int trailLength = ProjectileID.Sets.TrailCacheLength[Type];
        var origin = frame.Size() / 2f;
        for (int i = 0; i < trailLength; i++) {
            float p = Helper.CalcProgress(trailLength, i);
            Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + off - Main.screenPosition, frame, lightColor * p * p * 0.5f, Projectile.oldRot[i], origin, Projectile.scale * p, SpriteEffects.None, 0);
        }
        Main.EntitySpriteDraw(texture, Projectile.position + off - Main.screenPosition, frame, lightColor, rotation, origin, Projectile.scale, SpriteEffects.None, 0);

        if (emoji > -1) {
            DrawEmoji(frame, Projectile.position + off - Main.screenPosition);
        }

        return false;
    }
    public void DrawEmoji(Rectangle projectileFrame, Vector2 drawPosition) {
        float scale = 1f;
        if (emojiTime < 6) {
            scale = emojiTime / 7f;
        }
        if (emojiTime > 180 - 6) {
            scale = (180 - emojiTime) / 7f;
        }
        int direction = Projectile.Center.X < Main.player[Projectile.owner].Center.X ? -1 : 1;
        var frame = TextureAssets.Projectile[Type].Value.Frame(HorizontalFrames, Main.projFrames[Type],
            emoji * 2 % HorizontalFrames + (direction == -1 ? 1 : 0), emoji * 2 / HorizontalFrames);
        var origin = new Vector2(direction == -1 ? frame.Width : 0, frame.Height);
        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, drawPosition + new Vector2(projectileFrame.Width / 2f * Projectile.scale * direction, -projectileFrame.Height / 2f),
            frame, Color.White, 0f, origin, Projectile.scale * scale, SpriteEffects.None, 0);
    }

    public override void SendExtraAI(BinaryWriter writer) {
        writer.Write(emoji);
        writer.Write(emojiTime);
        writer.Write(textureSlot);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        emoji = reader.ReadInt32();
        emojiTime = reader.ReadInt32();
        textureSlot = Math.Clamp(reader.ReadInt32(), 0, Textures.Count);
    }

    internal override InstancedMinionBuff CreateMinionBuff() {
        return new InstancedMinionBuff(this, (p) => ref p.GetModPlayer<Aequu2Player>().minionScribble);
    }
}