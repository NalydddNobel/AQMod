namespace Aequus {
    public delegate ref T RefFunc<T>();
    public delegate void RefAction<T>(ref T value);
}