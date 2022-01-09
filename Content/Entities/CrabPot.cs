using AQMod.Common;
using AQMod.Common.Graphics;
using AQMod.Content.Fishing;
using AQMod.Content.Players;
using AQMod.Items.Fish.BloodMoon;
using AQMod.Items.Fish.Corruption;
using AQMod.Items.Fish.Crimson;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.UI.Chat;

namespace AQMod.Content.Entities
{
    public sealed class CrabPot : Entity, TagSerializable, IAutoloadType // TODO: Tons of netcode!
    {
        public const int maxCrabPots = 150;
        public const int frameCount = 2;
        public static Texture2D Texture { get; private set; }
        public static Texture2D HighlightTexture { get; private set; }
        public static CrabPot[] crabPots { get; private set; }

        public CrabPotDataFlags data;
        public byte invalidLocationKillDelay;
        public bool hasBait;
        public int item;
        public byte killTap;
        private byte killTapDelay;

        public bool LavaProof => data.HasFlag(CrabPotDataFlags.LavaProof);

        public Rectangle getRect()
        {
            return new Rectangle((int)position.X, (int)position.Y, width, height);
        }

        public Item ItemInstance()
        {
            if (item <= 0 || ItemID.Sets.Deprecated[item])
            {
                return null;
            }
            var itemInstance = new Item();
            itemInstance.SetDefaults(item);
            return itemInstance;
        }

        private Rectangle _frame;
        private Vector2 _origin;
        private float _rotation;
        private float _gravity;
        private float _terminalVelocity;

        public void Setup(CrabPotDataFlags data)
        {
            active = true;
            width = 12;
            height = 10;
            this.data = data;
            invalidLocationKillDelay = 120;
            item = 0;
            if (Main.netMode != NetmodeID.Server)
                SetupFrame();
        }

        private void SetupFrame()
        {
            _frame = new Rectangle(0, 0, Texture.Width, Texture.Height / frameCount - 2);
            _origin = _frame.Size() / 2f;
        }

        public void Kill()
        {
            if (item > 0)
            {
                Item.NewItem(getRect(), item);
            }
            Item.NewItem(getRect(), DataToCrabPotItemID(data));
            Clear();
        }

        public void Clear()
        {
            active = false;
            position = default(Vector2);
            velocity = default(Vector2);
            oldPosition = default(Vector2);
            oldVelocity = default(Vector2);
            direction = 1;
            oldDirection = 0;
            wet = false;
            lavaWet = false;
            honeyWet = false;

            hasBait = false;
            data = 0;
            invalidLocationKillDelay = 120;
            item = 0;
            killTap = 0;
            killTapDelay = 0;
        }

        public void Update()
        {
            if (!active)
            {
                return;
            }

            if (killTapDelay == 0)
            {
                killTap = 0;
            }
            else
            {
                killTapDelay--;
            }
            _gravity = 0.1f;
            _terminalVelocity = 7f;
            if (velocity.Length() > 0.01f)
            {
                CollideWithOtherPots();
            }
            int x = (int)(position.X + width / 2f) / 16;
            int y = (int)(position.Y + height / 2f) / 16;
            if (wet)
            {
                invalidLocationKillDelay = 120;
                FloatOnWater(x, y);
                if (item <= 0 && velocity.Length() < 0.01f && hasBait)
                {
                    FishingCheck();
                }
            }
            else if ((invalidLocationKillDelay == 0 && velocity.Length() < 0.1f) || !WorldGen.InWorld(x, y, 24))
            {
                Kill();
            }
            if (invalidLocationKillDelay != 0)
                invalidLocationKillDelay++;

            oldVelocity = velocity;
            velocity.Y += _gravity;
            if (velocity.Y > _terminalVelocity)
            {
                velocity.Y = _terminalVelocity;
            }
            if (wet)
            {
                velocity.X *= 0.995f;
            }
            else
            {
                velocity.X *= 0.98f;
            }
            if (velocity.X < 0.1 && velocity.X > -0.1)
            {
                velocity.X = 0f;
            }

            if (Collision.LavaCollision(position, width, height))
            {
                lavaWet = true;
            }
            bool isWet = Collision.WetCollision(position, width, height);
            if (Collision.honey)
            {
                honeyWet = true;
            }
            if (isWet)
            {
                if (!wet)
                {
                    if (wetCount == 0)
                    {
                        WaterParticles();
                    }
                    wet = true;
                }
                wetCount++;
            }
            else
            {
                wet = false;
            }
            if (!wet)
            {
                lavaWet = false;
                honeyWet = false;
            }

            if (lavaWet && !LavaProof)
            {
                Kill();
                return;
            }

            var bump = velocity;
            velocity = Collision.TileCollision(position, velocity, width, height);
            if (velocity.X != bump.X && bump.X.Abs() > 1f)
            {
                velocity.X = -bump.X * 0.8f;
            }
            Vector4 collisionVector = Collision.SlopeCollision(position, velocity, width, height, _gravity);
            position.X = collisionVector.X;
            position.Y = collisionVector.Y;
            velocity.X = collisionVector.Z;
            velocity.Y = collisionVector.W;

            Collision.StepConveyorBelt(this, 1f);

            if (lavaWet && LavaProof)
            {
                Kill();
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    // sync death here
                    //NetMessage.SendData(21, -1, -1, null, whoAmI);
                }
            }

            _rotation = velocity.X * 0.2f;

            oldPosition = position;
            position += velocity;
        }

        private void CollideWithOtherPots()
        {
            var center = Center;
            var rectangle = getRect();
            for (int i = 0; i < maxCrabPots; i++)
            {
                if (i == whoAmI)
                    continue;
                if (rectangle.Intersects(crabPots[i].getRect()))
                {
                    var normal = Vector2.Normalize(center - crabPots[i].Center);
                    velocity += normal * 0.1f;
                    crabPots[i].velocity += normal * -0.1f;
                }
            }
        }

        private bool FishingCheckRNG()
        {
            //return Main.rand.NextBool(60);
            return Main.rand.NextBool(5500);
        }

        private void FishingCheck()
        {
            if (!FishingCheckRNG())
            {
                return;
            }

            item = FishingCheckGetItem();
            if (item != 0)
            {
                hasBait = false;
                wetCount = 0;
                velocity.X += Main.rand.NextFloat(-0.1f, 0.1f);
                velocity.Y += 1f;
            }
        }

        private int FishingCheckGetItem()
        {
            if (honeyWet)
            {
                return 0;
            }
            var point = Center.ToTileCoordinates();
            int worldLayer = (!(point.Y < Main.worldSurface * 0.5)) ? ((point.Y < Main.worldSurface) ? 1 : ((point.Y < Main.rockLayer) ? 2 : ((point.Y >= Main.maxTilesY - 300) ? 4 : 3))) : 0;

            if (LavaProof && lavaWet)
            {
                return Main.rand.NextBool() ? ItemID.FlarefinKoi : ItemID.Obsidifish;
            }
            else if (worldLayer == FishLoader.WorldLayers.HellLayer)
            {
                return 0;
            }

            bool ocean = position.X < 3200f || position.X > Main.maxTilesX * 16f - 3200f;
            bool crimson = false;
            bool corruption = false;
            bool desert = false;
            bool jungle = false;
            bool hallow = false;
            bool snow = false;

            for (int j = 0; j < 30; j++)
            {
                if (point.Y + j > Main.maxTilesY - 10)
                {
                    continue;
                }
                var tile = Main.tile[point.X, point.Y + j];
                if (tile == null)
                {
                    continue;
                }
                if (!ocean)
                {
                    switch (tile.type)
                    {
                        case TileID.Sand:
                        case TileID.Ebonsand:
                        case TileID.Crimsand:
                        case TileID.Pearlsand:
                        case TileID.HardenedSand:
                        case TileID.CorruptHardenedSand:
                        case TileID.CrimsonHardenedSand:
                        case TileID.HallowHardenedSand:
                        case TileID.Sandstone:
                        case TileID.CorruptSandstone:
                        case TileID.CrimsonSandstone:
                        case TileID.HallowSandstone:
                            {
                                desert = true;
                            }
                            break;
                    }
                }
                if (tile.wall == WallID.Sandstone)
                {
                    desert = true;
                }
                if (TileID.Sets.Snow[tile.type] || TileID.Sets.Conversion.Ice[tile.type])
                {
                    snow = true;
                }
                if (TileID.Sets.Hallow[tile.type])
                {
                    hallow = true;
                    break;
                }
                if (TileID.Sets.Corrupt[tile.type])
                {
                    corruption = true;
                    break;
                }
                if (TileID.Sets.Crimson[tile.type])
                {
                    crimson = true;
                    break;
                }
                if (tile.type == TileID.JungleGrass || tile.type == TileID.LihzahrdBrick)
                {
                    jungle = true;
                    break;
                }
            }

            bool spawnGenericFish = !crimson && !corruption && !desert && !ocean && !jungle && !snow;

            List<int> selectableFish = new List<int>();
            // TODO: Add custom items instead of these sudo regular fishing tables!
            // TODO 2: Aadd custom crab pot item support through mod calls?
            if (worldLayer <= FishLoader.WorldLayers.Overworld)
            {
                if (Main.bloodMoon)
                {
                    selectableFish.Add(ModContent.ItemType<PalePufferfish>());
                    if (Main.hardMode)
                        selectableFish.Add(ModContent.ItemType<VampireSquid>());
                }
                if (spawnGenericFish)
                {

                }
            }
            if (crimson)
            {
                selectableFish.Add(ItemID.Hemopiranha);
                selectableFish.Add(ItemID.CrimsonTigerfish);
                if (NPC.downedBoss2)
                {
                    selectableFish.Add(ModContent.ItemType<Fleshscale>());
                }
            }
            else if (corruption)
            {
                selectableFish.Add(ItemID.Ebonkoi);
                if (!Main.dayTime)
                {
                    selectableFish.Add(ModContent.ItemType<Fizzler>());
                }
                if (NPC.downedBoss2)
                {
                    selectableFish.Add(ModContent.ItemType<Depthscale>());
                }
            }
            else if (jungle)
            {
                selectableFish.Add(ItemID.DoubleCod);
                selectableFish.Add(ItemID.VariegatedLardfish);
            }
            else if (hallow)
            {
                selectableFish.Add(ItemID.PrincessFish);
                selectableFish.Add(ItemID.Prismite);
                if (worldLayer == FishLoader.WorldLayers.UndergroundLayer || worldLayer == FishLoader.WorldLayers.CavernLayer)
                    selectableFish.Add(ItemID.ChaosFish);
            }
            if (ocean && worldLayer == FishLoader.WorldLayers.Overworld)
            {
                selectableFish.Add(ItemID.Shrimp);
                selectableFish.Add(ItemID.BlueJellyfish);
                selectableFish.Add(ItemID.GreenJellyfish);
                selectableFish.Add(ItemID.PinkJellyfish);
            }
            else if (snow)
            {
                selectableFish.Add(ItemID.AtlanticCod);
                selectableFish.Add(ItemID.FrostMinnow);
            }

            if (spawnGenericFish)
            {
                if (worldLayer == FishLoader.WorldLayers.Space)
                {
                    selectableFish.Add(ItemID.Damselfish);
                }
                else if (worldLayer == FishLoader.WorldLayers.UndergroundLayer || worldLayer == FishLoader.WorldLayers.CavernLayer)
                {
                    selectableFish.Add(ItemID.Stinkfish);
                    selectableFish.Add(ItemID.SpecularFish);
                    selectableFish.Add(ItemID.ArmoredCavefish);
                }
                if (worldLayer <= FishLoader.WorldLayers.Overworld)
                {
                    selectableFish.Add(ItemID.Bass);
                }
            }

            if (selectableFish.Count == 1)
                return selectableFish[0];
            if (selectableFish.Count > 1)
                return selectableFish[Main.rand.Next(selectableFish.Count)];
            return 0;
        }

        private void FloatOnWater(int x, int y)
        {
            if (y > 0)
            {
                _gravity = 0f;
                if (velocity.Y > 0f)
                    velocity.Y *= 0.9f;
                if (Main.tile[x, y] == null)
                {
                    Main.tile[x, y] = new Tile();
                }
                if (Main.tile[x, y].liquid > 0 && Main.tile[x, y - 1].liquid == 0)
                {
                    //Main.NewText(0.06274509f * Main.tile[x, y].liquid);
                    var toVelocity = y * 16f + 16f - (0.06274509f * Main.tile[x, y].liquid) - position.Y;
                    //Main.NewText(toVelocity);
                    //Dust.NewDust(new Vector2(position.X + width / 2f, position.Y + toVelocity), 2, 2, DustID.Fire);
                    velocity.Y = MathHelper.Lerp(velocity.Y, toVelocity / 16f, 0.05f);
                }
                else
                {
                    velocity.Y -= 0.2f;
                    if (velocity.Y < -4f)
                        velocity.Y = -4f;
                }
            }
        }

        private void WaterParticles()
        {
            if (!lavaWet)
            {
                if (honeyWet)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        int d = Dust.NewDust(new Vector2(position.X - 6f, position.Y + (float)(height / 2) - 8f), width + 12, 24, 152);
                        Main.dust[d].velocity.Y -= 1f;
                        Main.dust[d].velocity.X *= 2.5f;
                        Main.dust[d].scale = 1.3f;
                        Main.dust[d].alpha = 100;
                        Main.dust[d].noGravity = true;
                    }
                    Main.PlaySound(SoundID.Splash, (int)position.X, (int)position.Y);
                }
                else
                {
                    for (int i = 0; i < 10; i++)
                    {
                        int d = Dust.NewDust(new Vector2(position.X - 6f, position.Y + (float)(height / 2) - 8f), width + 12, 24, Dust.dustWater());
                        Main.dust[d].velocity.Y -= 4f;
                        Main.dust[d].velocity.X *= 2.5f;
                        Main.dust[d].scale *= 0.8f;
                        Main.dust[d].alpha = 100;
                        Main.dust[d].noGravity = true;
                    }
                    Main.PlaySound(SoundID.Splash, (int)position.X, (int)position.Y);
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    int d = Dust.NewDust(new Vector2(position.X - 6f, position.Y + (float)(height / 2) - 8f), width + 12, 24, 35);
                    Main.dust[d].velocity.Y -= 1.5f;
                    Main.dust[d].velocity.X *= 2.5f;
                    Main.dust[d].scale = 1.3f;
                    Main.dust[d].alpha = 100;
                    Main.dust[d].noGravity = true;
                }
                Main.PlaySound(SoundID.Splash, (int)position.X, (int)position.Y);
            }
        }

        private void RenderItemBlip(Item item, Vector2 position)
        {
            var texture = Main.itemTexture[item.type];
            var frame = Main.itemAnimations[item.type] == null ? texture.Frame() : Main.itemAnimations[item.type].GetFrame(texture);
            var scale2 = 1f;
            var color = Color.White;
            ItemSlot.GetItemLight(ref color, ref scale2, item.type);
            var drawColor = item.GetAlpha(color);
            float scale = frame.Width <= frame.Height ? (float)30f / frame.Height : (float)30f / frame.Width;
            Vector2 backSize = Main.inventoryBack3Texture.Size();
            var origin = frame.Size() / 2f;
            var drawPos = position + backSize / 2f - frame.Size() * scale / 2f;

            if (ItemLoader.PreDrawInInventory(item, Main.spriteBatch, position, frame, drawColor, item.GetColor(Main.inventoryBack), origin, scale * scale2))
            {
                Main.spriteBatch.Draw(texture, position, frame, drawColor, 0f, origin, scale * scale2, SpriteEffects.None, 0f);
                if (item.color != Color.Transparent)
                    Main.spriteBatch.Draw(texture, position, frame, item.GetColor(Main.inventoryBack), 0f, origin, scale * scale2, SpriteEffects.None, 0f);
            }
            ItemLoader.PostDrawInInventory(item, Main.spriteBatch, position, frame, drawColor, item.GetColor(color), origin, scale * scale2);
            if (ItemID.Sets.TrapSigned[item.type])
                Main.spriteBatch.Draw(Main.wireTexture, position + new Vector2(40f, 40f), new Rectangle(4, 58, 8, 8), Main.inventoryBack, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);
            if (item.stack > 1)
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontItemStack, item.stack.ToString(), position + new Vector2(10f, 26f), Main.inventoryBack, 0f, Vector2.Zero, new Vector2(1f), -1f, 1f);
        }

        private void HandleInteractions(Vector2 drawCoordinates, Color lightColor)
        {
            var plr = Main.LocalPlayer;
            if (getRect().Contains(Main.MouseWorld.ToPoint()) && plr.IsInTileInteractionRange((int)position.X / 16, (int)position.Y / 16))
            {
                var outlineTexture = HighlightTexture;
                plr.noThrow = 2;
                plr.showItemIcon = true;
                Item bait = null;
                if (item > 0 && !ItemID.Sets.Deprecated[item])
                {
                    plr.showItemIcon2 = item;
                }
                else
                {
                    if (!hasBait)
                    {
                        bait = findBaitItem(plr);
                    }
                    if (bait != null)
                    {
                        plr.showItemIcon2 = bait.type;
                    }
                    else
                    {
                        plr.showItemIcon2 = DataToCrabPotItemID(data);
                    }
                }
                if (PlayerInput.UsingGamepad)
                    plr.GamepadEnableGrappleCooldown();
                Main.spriteBatch.Draw(outlineTexture, drawCoordinates, _frame, Color.Lerp(lightColor, Main.OurFavoriteColor, 0.5f), _rotation, _frame.Size() / 2f, 1f, SpriteEffects.None, 0f);
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    if (plr.HeldItem.pick > 0)
                    {
                        killTap = 3;
                    }
                    killTap++;
                    killTapDelay = 12;
                    Main.PlaySound(SoundID.Tink, -1, -1, 1, 0.8f, 0.5f);
                    if (killTap > 3)
                    {
                        Kill();
                    }
                }
                if (Main.mouseRight && Main.mouseRightRelease)
                {
                    if (item > 0)
                    {
                        Main.mouseRightRelease = false;
                        Item.NewItem(getRect(), item);
                        Main.PlaySound(SoundID.Grab);
                        item = 0;
                    }
                    else if (!hasBait)
                    {
                        if (bait == null)
                            bait = findBaitItem(plr);
                        if (bait != null)
                        {
                            hasBait = true;
                            Main.PlaySound(SoundID.Grab);
                            if (bait.consumable)
                            {
                                bait.stack--;
                                if (bait.stack <= 0)
                                {
                                    bait.TurnToAir();
                                }
                            }
                        }
                    }
                }
            }
        }

        private Item findBaitItem(Player plr)
        {
            Item bait = null;
            if (plr.HeldItem.bait > 0)
            {
                bait = plr.HeldItem;
            }
            else
            {
                for (int i = 0; i < Main.maxInventory; i++)
                {
                    if (plr.inventory[i].type > 0 && plr.inventory[i].stack > 0 && plr.inventory[i].bait > 0)
                    {
                        bait = plr.inventory[i];
                        break;
                    }
                }
            }
            return bait;
        }

        public void Render()
        {
            if (!active || !AQGraphics.Cull(new Rectangle((int)(position.X - Main.screenPosition.X - 10), (int)(position.Y - Main.screenPosition.Y - 40), width + 20, height + 40)))
            {
                return;
            }
            var drawCoordinates = Center - Main.screenPosition;
            if (item > 0 && !ItemID.Sets.Deprecated[item])
            {
                RenderItemBlip(ItemInstance(), new Vector2(drawCoordinates.X, drawCoordinates.Y - 40f));
            }
            if (wet)
            {
                drawCoordinates.Y += (float)Math.Sin(Main.GlobalTime + position.X * 0.01f) * 2f;
            }
            var lightColor = Lighting.GetColor(((int)position.X + width / 2) / 16, ((int)position.Y + height / 2) / 16);
            if (killTapDelay > 4)
            {
                drawCoordinates += new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
            }
            HandleInteractions(drawCoordinates, lightColor);

            Main.spriteBatch.Draw(Texture, drawCoordinates, _frame, lightColor, _rotation, _origin, 1f, SpriteEffects.None, 0f);
        }

        public static void Initialize()
        {
            crabPots = new CrabPot[maxCrabPots];
            for (int i = 0; i < maxCrabPots; i++)
            {
                crabPots[i] = new CrabPot();
                crabPots[i].whoAmI = i;
            }

            if (!Main.dedServ)
            {
                Texture = ModContent.GetTexture("AQMod/Assets/Entities/CrabPot");
                HighlightTexture = ModContent.GetTexture("AQMod/Assets/Entities/CrabPot_Highlight");
            }
        }

        /// <summary>
        /// Creates a new crab pot and returns its index in the crab pot pool. Returns -1 if a new crab pot was not created.
        /// </summary>
        /// <param name="position">The position of the crab pot in the world.</param>
        /// <param name="velocity">The velocity of the crab pot</param>
        /// <returns></returns>
        public static int NewCrabPot(Vector2 position, Vector2 velocity, CrabPotDataFlags data)
        {
            for (int i = 0; i < maxCrabPots; i++)
            {
                if (!crabPots[i].active)
                {
                    crabPots[i].Setup(data);
                    crabPots[i].Center = position;
                    crabPots[i].velocity = velocity;
                    crabPots[i].whoAmI = i;
                    return i;
                }
            }
            return -1;
        }

        public static void ClearCrabPots()
        {
            for (int i = 0; i < maxCrabPots; i++)
            {
                crabPots[i].Clear();
                crabPots[i].whoAmI = i;
            }
        }

        public static int DataToCrabPotItemID(CrabPotDataFlags data)
        {
            return ModContent.ItemType<Items.Tools.Fishing.CrabPots.CrabPot>();
        }

        void IAutoloadType.OnLoad()
        {
        }
        void IAutoloadType.Unload()
        {
            crabPots = null;
        }

        public TagCompound SerializeData()
        {
            return new TagCompound()
            {
                ["X"] = position.X,
                ["Y"] = position.Y,
                ["data"] = (byte)data,
                ["item"] = item,
                ["hasBait"] = hasBait,
            };
        }

        public void Deserialize(TagCompound tag, Version version)
        {
            switch (version.ToString())
            {
                default:
                    active = true;
                    Setup((CrabPotDataFlags)tag.GetByte("data"));
                    hasBait = tag.GetBool("hasBait");
                    item = tag.GetInt("item");
                    position.X = tag.GetFloat("X");
                    position.Y = tag.GetFloat("Y");
                    break;
            }
        }

        public static void SignSaveData(TagCompound tag)
        {
            var pots = new List<CrabPot>();
            for (int i = 0; i < maxCrabPots; i++)
            {
                if (crabPots[i].active)
                {
                    pots.Add(crabPots[i]);
                }
            }
            var crabbyTaggies = new List<TagCompound>();
            //AQMod.GetInstance().Logger.Debug(pots.Count);
            tag["CrabPotCount"] = pots.Count;

            for (int i = 0; i < pots.Count; i++)
            {
                crabbyTaggies.Add(pots[i].SerializeData());
            }
            tag["crabbyTaggies"] = crabbyTaggies;
        }

        public static void LoadData(TagCompound tag, Version version)
        {
            int crabPotCount = tag.GetInt("CrabPotCount");
            if (crabPotCount <= 0)
                return;
            var arr = tag.GetList<TagCompound>("crabbyTaggies");
            for (int i = 0; i < crabPotCount; i++)
            {
                crabPots[i].Deserialize(arr[i], version);
            }
        }
    }
}