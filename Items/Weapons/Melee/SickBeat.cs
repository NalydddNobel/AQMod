using Aequus.Projectiles.Melee;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Items.Weapons.Melee
{
    public class SickBeat : ModItem
    {
        public string musicSource;
        public bool HasModdedMusicSource => !string.IsNullOrEmpty(musicSource);
        public string MusicSource { get => HasModdedMusicSource ? "Terraria" : musicSource; }

        public string GetMusicProviderName()
        {
            if (!HasModdedMusicSource)
            {
                return TextHelper.GetTextValue("Terraria");
            }
            if (TextHelper.TryGetText($"ModName.{musicSource}", out string name))
            {
                return name;
            }
            if (ModLoader.TryGetMod(name, out var mod))
            {
                return mod.DisplayName;
            }
            return musicSource;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 44;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.rare = ItemRarityID.LightRed;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.value = ItemDefaults.EarlyHardmode;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 2f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<SickBeatProj>();
            Item.shootSpeed = 10.5f;
            Item.autoReuse = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var obj = new { MusicSource = GetMusicProviderName(), };
            foreach (var l in tooltips)
            {
                if (l.Name.StartsWith("Tooltip"))
                {
                    l.Text = AequusHelpers.FormatWith(l.Text, obj);
                }
            }
        }

        public override void SaveData(TagCompound tag)
        {
            if (HasModdedMusicSource)
                tag["MusicSource"] = musicSource;
        }
        public override void LoadData(TagCompound tag)
        {
            musicSource = tag.Get<string>("MusicSource");
        }

        public override void NetSend(BinaryWriter writer)
        {
            if (HasModdedMusicSource)
            {
                writer.Write(true);
                writer.Write(musicSource);
            }
            else
            {
                writer.Write(false);
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            if (reader.ReadBoolean())
            {
                musicSource = reader.ReadString();
            }
        }
    }
}