namespace Aequu2.Core.Entities.Tiles.Components;

public interface IPlaceChestHook {
    /// <summary>
    /// Return -2 to allow vanilla function to occur.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="style"></param>
    /// <param name="notNearOtherChests"></param>
    /// <returns></returns>
    int PlaceChest(int x, int y, int style, bool notNearOtherChests);

    /// <summary>
    /// Return true to allow vanilla function to occur.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="style"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    bool PlaceChestDirect(int x, int y, int style, int id);
}