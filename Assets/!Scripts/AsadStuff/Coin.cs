using UnityEngine;

public class Coin : Collectible //Use of parent script. Which is an abstract class.
{
    public int coinValue = 1;
    public static int totalCoins = 0;

    public override void Collect()
    {
        CollectibleManager.CollectCoin(coinValue);
        Debug.Log("Coins Collected!");
        totalCoins++;
    }
}
