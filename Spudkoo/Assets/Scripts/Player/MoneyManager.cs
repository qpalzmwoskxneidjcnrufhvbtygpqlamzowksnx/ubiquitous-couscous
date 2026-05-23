using UnityEngine;
using TMPro;
public class MoneyManager : MonoBehaviour
{
    //* ----- Singleton Reference ------- *//
    public static MoneyManager Singleton;
    
    [Header("References")]
    public AIController aiController;
    public TMP_Text buddstext;
    

    [Header("Settings")]
    [SerializeField] private float buddsChance;
    [SerializeField] private int buddsAmount;

    [Header("Player")]
    public int Budds = 0;




    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        aiController.OnAIConversed += HandleAIConversed;
        UpdateText();
    }


    public bool AddBudds(int amount)
    {
        Budds += amount;
        Debug.Log($"Added {amount} budds, total={Budds}");
        UpdateText();
        return true;

        //Returns bool incase we have some bud max
    }

    public bool RemoveBudds(int amount)
    {
        if(Budds-amount<0) return false;
        else Budds -= amount;
        UpdateText();
        return true;
    }

    private void HandleAIConversed()
    {
        TryAddBudds(buddsChance);
    }

    private void UpdateText()
    {
        buddstext.text= "Budds:" + Budds.ToString();
    }
    private void TryAddBudds(float chance)
    {
        if(Random.value > chance)
        {
            AddBudds(buddsAmount);
        }
    }


}
