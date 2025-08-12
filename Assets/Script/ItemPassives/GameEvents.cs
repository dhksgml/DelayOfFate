
using System;

public static class GameEvents
{
    public static event Action OnNextDay;
    public static void CallNextDay() => OnNextDay?.Invoke();

    public static event Action OnSaleItemImmediately;
    public static void CallSaleItemImmediately() => OnSaleItemImmediately?.Invoke();

    public static event Action OnPickupItem;
    public static void CallPickupItem() => OnPickupItem?.Invoke();

    public static event Action OnDropItem;
    public static void CallDropItem() => OnDropItem?.Invoke();
}
