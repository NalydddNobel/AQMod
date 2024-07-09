namespace AequusRemake.Systems.Fishing;

public interface IFishDropRule {
    bool CanCatch(in FishDropInfo dropInfo);
    void TryCatching(ref FishDropInfo dropInfo);
}
