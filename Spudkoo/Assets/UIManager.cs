using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public List<GameObject> ScreenList ;
    public static UIManager Instance;
    void Awake()
    {
        Instance = this;
        Debug.Log("Awake");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Start");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SwitchPage(string PageName)
    {
        foreach(GameObject gob in ScreenList)
        {
            if (gob.name == PageName)
            {
                gob.SetActive (true);
            }
            else
            {
                gob.SetActive (false);
            }
        }
    }
}
