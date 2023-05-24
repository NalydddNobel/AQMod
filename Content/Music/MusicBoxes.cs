using Aequus.Items;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Content.Music {
    public abstract class MusicBoxItem<T> : OnlineLinkedItem where T : ModTile {
        public abstract string MusicFile { get; }

        public override Asset<Texture2D> ButtonTexture => AequusTextures.MusicButton;

        public override void SetStaticDefaults() {
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;

            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Assets/Music/" + MusicFile), Type, Item.createTile);
        }

        public override void SetDefaults() {
            base.SetDefaults();
            Item.DefaultToMusicBox(ModContent.TileType<T>(), 0);
        }
    }
    public abstract class MusicBoxTile<T> : ModTile where T : ModItem {
        public override void SetStaticDefaults() {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);

            AddMapEntry(new(200, 200, 200), TextHelper.GetItemName<T>());
        }

        public override void MouseOver(int i, int j) {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<T>();
        }
    }

    #region Extra
    public class CrabsonMusicBox : MusicBoxItem<CrabsonMusicBoxTile> {
        public override string MusicFile => "Extra_Crabson";
        public override string Link => null;
    }
    public class CrabsonMusicBoxTile : MusicBoxTile<CrabsonMusicBox> {
    }

    public class Glimmer2MusicBox : MusicBoxItem<Glimmer2MusicBoxTile> {
        public override string MusicFile => "Extra_Glimmer";
        public override string Link => null;
    }
    public class Glimmer2MusicBoxTile : MusicBoxTile<Glimmer2MusicBox> {
    }

    public class TitleScreenMusicBox : MusicBoxItem<TitleScreenMusicBoxTile> {
        public override string MusicFile => "Extra_Title";
        public override string Link => "https://youtu.be/-K715Nlqyfk";

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<CrabCreviceMusicBox>()
                .AddIngredient<GlimmerMusicBox>()
                .AddIngredient<GaleStreamsMusicBox>()
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
    public class TitleScreenMusicBoxTile : MusicBoxTile<TitleScreenMusicBox> {
    }
    #endregion

    #region Spectra
    public class CrabCreviceMusicBox : MusicBoxItem<CrabCreviceMusicBoxTile> {
        public override string MusicFile => "Spectra_CrabCrevice";
        public override string Link => "https://youtu.be/7u4GsyOp5MI";
    }
    public class CrabCreviceMusicBoxTile : MusicBoxTile<CrabCreviceMusicBox> {
    }
    public class GlimmerMusicBox : MusicBoxItem<GlimmerMusicBoxTile> {
        public override string MusicFile => "Spectra_Glimmer";
        public override string Link => "https://youtu.be/BI_lwgljMAE";
    }
    public class GlimmerMusicBoxTile : MusicBoxTile<GlimmerMusicBox> {
    }
    public class OmegaStariteMusicBox : MusicBoxItem<OmegaStariteMusicBoxTile> {
        public override string MusicFile => "Spectra_OmegaStarite";
        public override string Link => "https://youtu.be/tZgA0bVov_g";
    }
    public class OmegaStariteMusicBoxTile : MusicBoxTile<OmegaStariteMusicBox> {
    }
    public class GaleStreamsMusicBox : MusicBoxItem<GaleStreamsMusicBoxTile> {
        public override string MusicFile => "Spectra_GaleStreams";
        public override string Link => "https://youtu.be/l-Adrs6kKIU";
    }
    public class GaleStreamsMusicBoxTile : MusicBoxTile<GaleStreamsMusicBox> {
    }
    #endregion
}