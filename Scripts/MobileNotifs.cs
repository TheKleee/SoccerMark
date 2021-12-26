using Unity.Notifications.Android;
using UnityEngine;

public class MobileNotifs : MonoBehaviour
{
    #region Singleton:
    public static MobileNotifs instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    public void Norif(float waitTime)
    {
        //Remove former notifs:
        AndroidNotificationCenter.CancelAllDisplayedNotifications();

        //Create a channel:
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        //Create the notification:

        var notification = new AndroidNotification();
        notification.Title = "Bonus Is Ready!";
        notification.Text = $"Collect your ${SaveData.instance.lvl * 10 + 200} price!";
        notification.SmallIcon = "icon_1";
        notification.LargeIcon = "icon_0";
        notification.FireTime = System.DateTime.Now.AddSeconds(waitTime);

        //Send the notification:
        var id = AndroidNotificationCenter.SendNotification(notification, "channel_id");

        if (AndroidNotificationCenter.CheckScheduledNotificationStatus(id) == NotificationStatus.Scheduled)
        {
            AndroidNotificationCenter.CancelAllNotifications();
            AndroidNotificationCenter.SendNotification(notification, "channel_id");
        }
    }
}
