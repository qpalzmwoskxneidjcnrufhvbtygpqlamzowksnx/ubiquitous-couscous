using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    //* ----- Singleton Reference ------- *//
    public static MoneyManager Singleton;
    
    [Header("References")]
    public AIController aiController;


    [Header("Settings")]
    [SerializeField] private float budsChance;
    [SerializeField] private int budsAmount;

    [Header("Player")]
    public int Buds = 0;




    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        aiController.OnAIConversed += HandleAIConversed;
    }


    public bool AddBuds(int amount)
    {
        Buds += amount;
        Debug.Log($"Added {amount} buds, total={Buds}");
        return true;

        //Returns bool incase we have some bud max
    }

    public bool RemoveBuds(int amount)
    {
        if(Buds-amount<0) return false;
        else Buds -= amount;
        return true;
    }

    private void HandleAIConversed()
    {
        TryAddBuds(budsChance);
    }

    private void TryAddBuds(float chance)
    {
        if(Random.value > chance)
        {
            AddBuds(budsAmount);
        }
    }


}
