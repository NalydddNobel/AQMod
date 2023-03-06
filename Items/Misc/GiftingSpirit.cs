using Aequus.Items.Materials.Festive;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc
{
    public class GiftingSpirit : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToHoldUpItem();
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item92;
            Item.value = Item.sellPrice(gold: 1);
            Item.maxStack = 9999;
        }

        public override bool? UseItem(Player player)
        {
            AequusWorld.xmasHats = !AequusWorld.xmasHats;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GoldCrown)
                .AddIngredient(ItemID.SnowBlock, 150)
                .AddIngredient<FrolicEnergy>(10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class GiftingSpiritGlobalNPC : GlobalNPC
    {
        public override void Load()
        {
            Terraria.On_NPC.UsesPartyHat += NPC_UsesPartyHat;
        }

        private static bool NPC_UsesPartyHat(Terraria.On_NPC.orig_UsesPartyHat orig, NPC self)
        {
            return !AequusWorld.xmasHats && orig(self);
        }

        public override void PostAI(NPC npc)
        {
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (WearsXmasHat(npc) && npc.altTexture == 0)
            {
                npc.altTexture = 1;
            }
            return true;
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!WearsXmasHat(npc))
                return;

            int num = npc.frame.Y / npc.frame.Height;
            int[] framingGroup = NPCID.Sets.TownNPCsFramingGroups[NPCID.Sets.NPCFramingGroup[npc.type]];
            if (num >= framingGroup.Length)
            {
                num = 0;
            }
            Texture2D value = TextureAssets.Extra[ExtrasID.TownNPCHats].Value;
            Rectangle frame = value.Frame(20, 1, 8 % 20);
            frame.Width -= 2;
            frame.Height -= 2;
            int xOffset1 = 0;
            if (npc.type == NPCID.Princess)
            {
                xOffset1 = 1;
            }
            if (npc.type == NPCID.TownCat)
            {
                xOffset1 = 6;
                switch (num)
                {
                    case 19:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                    case 26:
                    case 27:
                        xOffset1 -= 2;
                        break;
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                        xOffset1 += 2;
                        break;
                }
            }
            if (npc.type == NPCID.TownDog)
            {
                xOffset1 = 12;
            }
            if (npc.type == NPCID.TownBunny)
            {
                xOffset1 = 6;
                switch (num)
                {
                    case 1:
                    case 2:
                    case 3:
                        xOffset1 -= 2;
                        break;
                    case 18:
                    case 19:
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                        xOffset1 -= 4;
                        break;
                    case 8:
                        xOffset1 -= 2;
                        break;
                }
            }
            Vector2 vector = npc.Top + new Vector2(-2 * npc.spriteDirection, npc.gfxOffY);
            vector.X += xOffset1 * npc.spriteDirection;
            vector.Y += framingGroup[num];
            vector.Y += NPCID.Sets.HatOffsetY[npc.type];
            int yOffset = 0;
            if (npc.ai[0] == 5f)
            {
                yOffset = -4;
                if (npc.type == NPCID.Demolitionist)
                {
                    yOffset = -8;
                }
                if (npc.type == NPCID.Mechanic)
                {
                    yOffset = -2;
                }
                if (npc.type == NPCID.DD2Bartender)
                {
                    yOffset = -4;
                }
                if (npc.type == NPCID.Golfer)
                {
                    yOffset = -4;
                }
                if (npc.type == NPCID.Wizard || npc.type == NPCID.Steampunker)
                {
                    yOffset = -6;
                }
                if (npc.type == NPCID.TownCat)
                {
                    yOffset = -12;
                }
                if (npc.type == NPCID.Princess)
                {
                    yOffset = -8;
                }
            }
            vector.Y += yOffset;
            if (npc.type == NPCID.Pirate && npc.ai[0] == 12f)
            {
                vector.X -= npc.spriteDirection * 4;
            }
            if (npc.type == NPCID.DD2Bartender && npc.ai[0] == 5f)
            {
                vector.X += npc.spriteDirection * 7;
            }
            Vector2 origin = frame.Size() - new Vector2(frame.Width / 2, 12f);
            int xOffset2 = 0;
            switch (npc.type)
            {
                case 550:
                    xOffset2 = -4;
                    break;
                case 588:
                    xOffset2 = 0;
                    break;
                case 227:
                    xOffset2 = -4;
                    break;
                case 228:
                    xOffset2 = -2;
                    break;
                case 17:
                case 18:
                case 19:
                case 20:
                case 22:
                case 124:
                case 229:
                case 353:
                case 633:
                case 637:
                case 638:
                case 656:
                    xOffset2 = -1;
                    break;
                case 37:
                case 38:
                case 54:
                case 107:
                case 108:
                case 160:
                case 207:
                case 209:
                    xOffset2 = -3;
                    break;
                case 178:
                case 208:
                case 369:
                    xOffset2 = 1;
                    break;
            }
            vector.X += xOffset2 * npc.spriteDirection;
            vector.X += 4 * npc.spriteDirection;
            spriteBatch.Draw(value, new Vector2(vector.X - screenPos.X, vector.Y - screenPos.Y), (Rectangle?)frame, npc.GetNPCColorTintedByBuffs(drawColor) * npc.Opacity,
                0f, origin, npc.scale, (-npc.spriteDirection).ToSpriteEffect(), 0f);
            if (npc.altTexture == 1)
                npc.altTexture = 0;
        }

        public bool WearsXmasHat(NPC npc)
        {
            return AequusWorld.xmasWorld || AequusWorld.xmasHats && (npc.townNPC || NPCID.Sets.ActsLikeTownNPC[npc.type]);
        }
    }
}