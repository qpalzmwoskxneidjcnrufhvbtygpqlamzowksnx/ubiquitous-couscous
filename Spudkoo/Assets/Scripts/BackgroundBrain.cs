using UnityEngine;
using UnityEngine.UI;

public class BackgroundBrain : MonoBehaviour
{

    //* --------- Singleton --------- */
    public static BackgroundBrain Instance;


    [Header("UI References")]
    [SerializeField] private Image backgroundImage;

    private void Awake()
    {
        Instance = this;
    }

    public void LoadBackground(BackgroundData bgd)
    {
        backgroundImage.sprite = bgd.BackgroundSprite; 
    }
}
