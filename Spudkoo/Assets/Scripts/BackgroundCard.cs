using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.LowLevelPhysics;
public class BackgroundCard : MonoBehaviour
{
    public Image BackgroundImage;
    public TMP_Text BackgroundText;
    public BackgroundData backgroundData;

    private void Start()
    {
        //In the future this will be called from a background menu manager or something like that
        Initialize(backgroundData);
    }

    public void Initialize(BackgroundData bd)
    {
        backgroundData = bd;
        BackgroundImage.sprite = bd.BackgroundSprite;
        BackgroundText.text = bd.BackgroundName;
    }

    public void LoadBackground()
    {
        BackgroundBrain.Instance.LoadBackground(backgroundData);
    }
}
