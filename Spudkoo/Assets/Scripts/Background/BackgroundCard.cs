using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.LowLevelPhysics;
using Utilities.Extensions;
public class BackgroundCard : MonoBehaviour
{
    public Image BackgroundImage;
    public Image lockedImage;
    public TMP_Text BackgroundText;
    public TMP_Text CostText;
    public BackgroundData backgroundData;

    private void Start()
    {
        //In the future this will be called from a background menu manager or something like that
        //Initialize(backgroundData);
    }

    public void Initialize(BackgroundData bd)
    {
        backgroundData = bd;
        lockedImage.gameObject.SetActive(true);
        BackgroundImage.sprite = bd.BackgroundSprite;
        BackgroundText.text = bd.BackgroundName;
        CostText.text = bd.Cost + " Buds";
    }

    public void LoadBackground()
    {
        BackgroundBrain.Instance.LoadBackground(backgroundData);
    }

    public void OnBackgroundCardClick()
    {
        if(backgroundData.Unlocked)
        {
            LoadBackground();
        }
        else
        {
            if(MoneyManager.Singleton.Buds >= backgroundData.Cost)
            {
                MoneyManager.Singleton.RemoveBuds(backgroundData.Cost);
                backgroundData.Unlocked = true;
                lockedImage.gameObject.SetActive(false);
                Debug.Log("Background Purchased!");
            }
        }
    }
}
