
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

    public static event Action OnTimeAngleUnit18;
    public static void CallTimeAngleUnit18() => OnTimeAngleUnit18?.Invoke();

    public static event Action OnBuyWeapon;
    public static void CallBuyWeapon() => OnBuyWeapon?.Invoke();
}
