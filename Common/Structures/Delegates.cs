namespace Aequus.Common.Structures;

public delegate ref TResult RefFunc<TResult>();
public delegate ref TResult RefFunc<T1, TResult>(T1 t);
public delegate ref TResult RefFunc<T1, T2, TResult>(T1 t1, T2 t2);
public delegate ref TResult RefFunc<T1, T2, T3, TResult>(T1 t1, T2 t2, T3 t3);
public delegate ref TResult RefFunc<T1, T2, T3, T4, TResult>(T1 t1, T2 t2, T3 t3, T4 t4);
public delegate ref TResult RefFunc<T1, T2, T3, T4, T5, TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
public delegate ref TResult RefFunc<T1, T2, T3, T4, T5, T6, TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);
public delegate ref TResult RefFunc<T1, T2, T3, T4, T5, T6, T7, TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);
public delegate ref TResult RefFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8);
public delegate ref TResult RefFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9);
public delegate ref TResult RefFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10);
