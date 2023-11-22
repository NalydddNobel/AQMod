using Aequus.Content.CrossMod;
using Aequus.Content.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Aequus.Content.DedicatedContent.Familiar;

public class FamiliarPet : ModPet {
    public override string Texture => AequusTextures.NPC(NPCID.Guide);

    private Player dummyPlayer;

    public override void SetStaticDefaults() {
        Main.projFrames[Projectile.type] = 20;
        Main.projPet[Projectile.type] = true;
        ProjectileID.Sets.CharacterPreviewAnimations[Type] = new() { Offset = new(10f, 10f), };
    }

    public override void SetDefaults() {
        Projectile.width = 20;
        Projectile.height = 42;
        Projectile.friendly = true;
        Projectile.aiStyle = ProjAIStyleID.Pet;
        AIType = ProjectileID.BlackCat;
        Projectile.scale = 0.5f;
    }

    public void CopyPlayerAttributes(Player parent) {
        // copies player attributes
        dummyPlayer.eyeColor = parent.eyeColor;
        dummyPlayer.hairColor = parent.hairColor;
        dummyPlayer.hairDyeColor = parent.hairDyeColor;
        dummyPlayer.pantsColor = parent.pantsColor;
        dummyPlayer.shirtColor = parent.shirtColor;
        dummyPlayer.shoeColor = parent.shoeColor;
        dummyPlayer.skinColor = parent.skinColor;
        dummyPlayer.underShirtColor = parent.underShirtColor;
        dummyPlayer.Male = parent.Male;
        dummyPlayer.skinVariant = parent.skinVariant;
        dummyPlayer.hairDye = parent.hairDye;
        dummyPlayer.hairDyeVar = parent.hairDyeVar;
        dummyPlayer.hair = parent.hair;

        //dummyPlayer.selectedItem = 0;
        //dummyPlayer.inventory[dummyPlayer.selectedItem] = parent.HeldItem.Clone();

        // copies proj attributes
        dummyPlayer.width = Projectile.width;
        dummyPlayer.height = Projectile.height;
        dummyPlayer.oldVelocity = Projectile.oldVelocity;
        dummyPlayer.velocity = Projectile.velocity;
        dummyPlayer.oldDirection = Projectile.oldDirection;
        dummyPlayer.wet = Projectile.wet;
        dummyPlayer.lavaWet = Projectile.lavaWet;
        dummyPlayer.honeyWet = Projectile.honeyWet;
        dummyPlayer.wetCount = Projectile.wetCount;
        if (Projectile.velocity != Vector2.Zero || Projectile.direction == 0) {
            dummyPlayer.direction = Projectile.velocity.X < 0f ? -1 : 1;
        }
        dummyPlayer.oldPosition = Projectile.oldPosition;
        dummyPlayer.position = Projectile.position;
        dummyPlayer.position.Y -= 42 * (1f - Projectile.scale);
        dummyPlayer.whoAmI = Projectile.owner;

        dummyPlayer.PlayerFrame();
    }
    public void TryCopyingMrPlagueRaceAttributes(Player parent) {
        if (MrPlagueRaces.TryGetMrPlagueRacePlayer(dummyPlayer, out var racePlayer) && MrPlagueRaces.TryGetMrPlagueRacePlayer(parent, out var parentRacePlayer)) {
            foreach (var f in MrPlagueRaces.RacePlayerFieldInfo) {
                f.SetValue(racePlayer, f.GetValue(parentRacePlayer));
            }
            return;
        }
    }

    private void UpdateTick() {
        var parent = Main.player[Projectile.owner];
        dummyPlayer ??= new Player();

        CopyPlayerAttributes(parent);
        if (MrPlagueRaces.Instance != null && MrPlagueRaces.RacePlayerFieldInfo != null && MrPlagueRaces.MrPlagueRacesPlayer != null) {
            TryCopyingMrPlagueRaceAttributes(parent);
        }
    }

    public override bool PreAI() {
        UpdateTick();
        return base.PreAI();
    }

    public override bool PreDraw(ref Color lightColor) {
        if (dummyPlayer == null) {
            return false;
        }
        if (Projectile.isAPreviewDummy) {
            UpdateTick();
            dummyPlayer.headFrame = Main.player[Projectile.owner].headFrame;
            dummyPlayer.bodyFrame = Main.player[Projectile.owner].bodyFrame;
            dummyPlayer.legFrame = Main.player[Projectile.owner].legFrame;
        }

        dummyPlayer.GetModPlayer<AequusPlayer>().DrawScale = Projectile.scale;
        dummyPlayer.GetModPlayer<AequusPlayer>().DrawForceDye = Main.CurrentDrawnEntityShader;

        if (!Projectile.isAPreviewDummy) {
            dummyPlayer.Bottom = Projectile.Bottom;
        }

        Main.PlayerRenderer.DrawPlayer(Main.Camera, dummyPlayer, Projectile.position, 0f, Vector2.Zero, 0f, Projectile.scale);
        return false;
    }

    internal override InstancedPetBuff CreatePetBuff() {
        return new(this, (p) => ref p.GetModPlayer<AequusPlayer>().petFamiliar, lightPet: false);
    }

    internal override InstancedPetItem CreatePetItem() {
        return new DedicatedPetItem(this, "Crabs", new Color(200, 65, 70), nameHidden: true);
    }
}