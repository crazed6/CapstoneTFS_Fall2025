using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance;

    public int CoinsCollected { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //GameManager.Instance.CollectibleManager = this;
        //Going to have to double check on usage of this.

    }

    public void CollectCoin(int amount) //called within the Coin script to add with each additional coin
    {
        CoinsCollected += amount;
        Debug.Log("Total Coins:" + Coin.totalCoins);
    }

    #region Save and Load

    public void Save()
    {

    }

    #endregion
}
