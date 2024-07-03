namespace Aequu2.Old.Content.Tiles.GravityBlocks;

public class GravityBlocksGlobalItem : GlobalItem {
    public bool HasReversedGravity;

    public override bool InstancePerEntity => true;
    protected override bool CloneNewInstances => true;

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return !ItemSets.ItemNoGravity[entity.type];
    }

    public override void Update(Item item, ref float gravity, ref float maxFallSpeed) {
        if (HasReversedGravity) {
            gravity = -gravity;
            if (item.velocity.Y < -maxFallSpeed) {
                item.velocity.Y = -maxFallSpeed;
            }
        }
    }

    public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
        if (HasReversedGravity) {
            rotation = MathHelper.Pi - rotation;
        }

        return true;
    }
}
