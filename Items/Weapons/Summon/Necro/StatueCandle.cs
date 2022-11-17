using Aequus.Projectiles.Summon.Necro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Items.Weapons.Summon.Necro
{
    public class StatueCandle : ModItem
    {
        public static Dictionary<TileKey, int> StatueFrameToEnemy { get; private set; }

        public override void Load()
        {
            StatueFrameToEnemy = new Dictionary<TileKey, int>()
            {
                [new TileKey(TileID.Statues, 4)] = NPCID.BlueSlime,
                [new TileKey(TileID.Statues, 5)] = NPCID.GoblinScout,
                [new TileKey(TileID.Statues, 7)] = NPCID.CaveBat,
                [new TileKey(TileID.Statues, 8)] = NPCID.Goldfish,
                [new TileKey(TileID.Statues, 9)] = NPCID.Bunny,
                [new TileKey(TileID.Statues, 10)] = NPCID.Skeleton,
                [new TileKey(TileID.Statues, 13)] = NPCID.FireImp,
                [new TileKey(TileID.Statues, 16)] = NPCID.Hornet,
                [new TileKey(TileID.Statues, 18)] = NPCID.Crab,
                [new TileKey(TileID.Statues, 23)] = NPCID.PinkJellyfish,
                [new TileKey(TileID.Statues, 28)] = NPCID.Bird,
                [new TileKey(TileID.Statues, 30)] = NPCID.EaterofSouls,
                [new TileKey(TileID.Statues, 42)] = NPCID.Piranha,
                [new TileKey(TileID.Statues, 50)] = NPCID.Shark,
                [new TileKey(TileID.Statues, 51)] = NPCID.Squirrel,
                [new TileKey(TileID.Statues, 52)] = NPCID.Butterfly,
                [new TileKey(TileID.Statues, 53)] = NPCID.Worm,
                [new TileKey(TileID.Statues, 54)] = NPCID.Firefly,
                [new TileKey(TileID.Statues, 55)] = NPCID.Scorpion,
                [new TileKey(TileID.Statues, 56)] = NPCID.Snail,
                [new TileKey(TileID.Statues, 57)] = NPCID.Grasshopper,
                [new TileKey(TileID.Statues, 58)] = NPCID.Mouse,
                [new TileKey(TileID.Statues, 59)] = NPCID.Seagull,
                [new TileKey(TileID.Statues, 60)] = NPCID.Penguin,
                [new TileKey(TileID.Statues, 61)] = NPCID.Frog,
                [new TileKey(TileID.Statues, 62)] = NPCID.Buggy,
                [new TileKey(TileID.Statues, 63)] = NPCID.WallCreeper,
                [new TileKey(TileID.Statues, 64)] = NPCID.Unicorn,
                [new TileKey(TileID.Statues, 65)] = NPCID.Drippler,
                [new TileKey(TileID.Statues, 66)] = NPCID.Wraith,
                [new TileKey(TileID.Statues, 67)] = NPCID.MisassembledSkeleton,
                [new TileKey(TileID.Statues, 68)] = NPCID.UndeadViking,
                [new TileKey(TileID.Statues, 69)] = NPCID.Medusa,
                [new TileKey(TileID.Statues, 70)] = NPCID.Harpy,
                [new TileKey(TileID.Statues, 71)] = NPCID.PigronHallow,
                [new TileKey(TileID.Statues, 72)] = NPCID.GreekSkeleton,
                [new TileKey(TileID.Statues, 73)] = NPCID.GraniteGolem,
                [new TileKey(TileID.Statues, 74)] = NPCID.ArmedZombie,
                [new TileKey(TileID.Statues, 75)] = NPCID.BloodZombie,
                [new TileKey(TileID.Statues, 76)] = NPCID.Owl,
                [new TileKey(TileID.Statues, 77)] = NPCID.Seagull,
                [new TileKey(TileID.Statues, 78)] = NPCID.RedDragonfly,
                [new TileKey(TileID.Statues, 79)] = NPCID.Turtle,
            };
        }

        public override void Unload()
        {
            StatueFrameToEnemy?.Clear();
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 16;
            Item.holdStyle = ItemHoldStyleID.HoldFront;
            Item.DamageType = NecromancyDamageClass.Instance;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.useAmmo = ItemDefaults.AmmoBloodyTearstone;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 15);
            Item.flame = true;
            Item.UseSound = SoundID.Item83;
        }

        public override void HoldStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X += -4f * player.direction;
            player.itemLocation.Y += 8f;

            Lighting.AddLight(player.itemLocation, TorchID.Bone);
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.SummonWeapon;
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer != player.whoAmI)
                return null;
            if (AequusHelpers.IsInCustomTileInteractionRange(player, Player.tileTargetX, Player.tileTargetY, 3, 3))
            {
                if (Main.tileFrameImportant[Main.tile[Player.tileTargetX, Player.tileTargetY].TileType])
                {
                    int left = Player.tileTargetX - Main.tile[Player.tileTargetX, Player.tileTargetY].TileFrameX % 36 / 18;
                    int top = Player.tileTargetY - Main.tile[Player.tileTargetX, Player.tileTargetY].TileFrameY % 54 / 18;
                    var objData = TileObjectData.GetTileData(Main.tile[left, top]);
                    if (objData != null)
                    {
                        var style = TileObjectData.GetTileStyle(Main.tile[left, top]);
                        //Main.NewText(Main.tile[left, top].TileType);
                        //Main.NewText(objData.StyleWrapLimit, Color.Yellow);
                        if (Main.tile[left, top].TileType == TileID.Statues)
                        {
                            if (style >= objData.StyleWrapLimit * 2)
                            {
                                style -= objData.StyleWrapLimit;
                            }
                            style %= objData.StyleWrapLimit * 2;
                        }
                        //Main.NewText(style, Color.Lime);
                        if (StatueFrameToEnemy.TryGetValue(new TileKey(Main.tile[left, top].TileType, style), out int val))
                        {
                            Projectile.NewProjectileDirect(player.GetSource_ItemUse_WithPotentialAmmo(Item, 0), new Vector2(left * 16f + 16f, top * 16f + 16f), Vector2.Zero, ModContent.ProjectileType<GhostSpawner>(),
                                Item.damage, 0f, player.whoAmI, val, 0.2f);
                        }
                    }
                }
            }
            return true;
        }
    }
}