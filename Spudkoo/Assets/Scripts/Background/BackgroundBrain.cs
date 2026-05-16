using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class BackgroundBrain : MonoBehaviour
{

    //* --------- Singleton --------- */
    public static BackgroundBrain Instance;


    [Header("UI References")]
    [SerializeField] private Image backgroundImage;
    public List<BackgroundData> backgrounddata=new();
    public BackgroundCard cardprefab;
    public Transform cardContent;
    private void Awake()
    {
        Instance = this;
        InitializeBackgroundCards();
    }

    public void LoadBackground(BackgroundData bgd)
    {
        backgroundImage.sprite = bgd.BackgroundSprite; 
    }

    private void InitializeBackgroundCards()
    {
        foreach(BackgroundData bd in backgrounddata) 
        {
            BackgroundCard bc = Instantiate(cardprefab, cardContent);
            bc.Initialize(bd);
        }
    }
}
