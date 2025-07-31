
using System;

public static class GameEvents
{
    public static event Action OnNextDay;
    public static void CallNextDay() => OnNextDay?.Invoke();

    public static event Action OnSaleItemImmediately;
    public static void CallSaleItemImmediately() => OnSaleItemImmediately?.Invoke();
}
