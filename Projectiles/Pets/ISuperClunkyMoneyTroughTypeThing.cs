namespace AQMod.Projectiles.Pets
{
    public interface ISuperClunkyMoneyTroughTypeThing
    {
        int ChestType { get; }
        int ProjectileType { get; }
        void OnOpen();
        void OnClose();
    }
}