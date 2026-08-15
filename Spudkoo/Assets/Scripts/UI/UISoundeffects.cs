using UnityEngine;

public class UISoundeffects : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private AudioClip UIButtonClickSFX;
    public void playUIClick()
    {
        AudioManager.Instance.PlaySFX(UIButtonClickSFX);
    }
  
    
}
