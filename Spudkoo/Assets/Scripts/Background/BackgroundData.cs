using System.IO.Enumeration;
using UnityEngine;
[CreateAssetMenu(fileName="NewBackgroundData", menuName="Background/BackgroundData")]
public class BackgroundData : ScriptableObject
{
    public string BackgroundName;
    public Sprite BackgroundSprite;
    public int Cost = 0;
    public bool Unlocked;
    public int ID;
    
}
