using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public class SpudkooSettings : MonoBehaviour
{

    //* --------- Singleton --------- *//
    public static SpudkooSettings Instance;

    private void Awake()
    {
        Instance = this;
    }


    //* --------- Private ----------- *//
    private bool _doNotifications;
    private bool _doReminders;
    private bool _doBuddyCollision;
    private bool _doBuddyAnimation;


    [SerializeField] private Sprite activeButtonSprite;
    [SerializeField] private Sprite deactiveButtonSprite;

    [SerializeField] private Image buddyCollisionImage;
    [SerializeField] private Image notificationsImage;
    [SerializeField] private Image remindersImage;
    [SerializeField] private Image buddyAnimationImage;
    //* ------ Public Accessors ------- *//

    // --- Queries ---
    public bool CanSendNotification() => _doNotifications;
    public bool CanSendReminders() => _doReminders;
    public bool CanBuddyCollide() => _doBuddyCollision;
    public bool CanBuddyAnimate() => _doBuddyAnimation;

    // --- Notifications ---
    public void EnableNotifications()
    { _doNotifications = true; }
    public void DisableNotifications()
    { _doNotifications = false; }
    public void ToggleNotifications() 
    { 
        _doNotifications = !_doNotifications;
        if (_doNotifications == true)
        {
            notificationsImage.sprite = activeButtonSprite;

        }
        else
        {
            notificationsImage.sprite = deactiveButtonSprite;
        }
    }

    // --- Reminders ---
    public void EnableReminders()
    { _doReminders = true; }
    public void DisableReminders()
    { _doReminders = false; }
    public void ToggleReminders()  
    { 
        _doReminders = !_doReminders;
        if (_doReminders == true)
        {
            remindersImage.sprite = activeButtonSprite;

        }
        else
        {
            remindersImage.sprite = deactiveButtonSprite;
        }
    }

    // --- Buddy Collision ---
    public void EnableBuddyCollision()
    { _doBuddyCollision = true; }
    public void DisableBuddyCollision()
    { _doBuddyCollision = false; }
    public void ToggleBuddyCollision() 
    { 
        _doBuddyCollision = !_doBuddyCollision;
        if (_doBuddyCollision ==  true) {
            buddyCollisionImage.sprite = activeButtonSprite;
        
        }
        else
        {
            buddyCollisionImage.sprite = deactiveButtonSprite;
        }
    }

    // --- Buddy Animation ---
    public void EnableBuddyAnimation()
    { _doBuddyAnimation = true; }
    public void DisableBuddyAnimation()
    { _doBuddyAnimation = false; }
    public void ToggleBuddyAnimation() 
    { 
        _doBuddyAnimation = !_doBuddyAnimation;
        if (_doBuddyAnimation == true)
        {
            buddyAnimationImage.sprite = activeButtonSprite;

        }
        else
        {
            buddyAnimationImage.sprite = deactiveButtonSprite;
        }
    }
}

