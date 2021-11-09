using OneSignal.RestAPIv3.Client;
using OneSignal.RestAPIv3.Client.Resources;
using OneSignal.RestAPIv3.Client.Resources.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLy.Utilities
{
    public class OneSignalPushNotification
    {

        public static async Task<string> PushNotification(string Title, string Content, List<string> PlayerIds, Guid appId, string restKey, string url)
        {
            try
            {
                OneSignalClient client = new OneSignalClient(restKey);
                var opt = new NotificationCreateOptions()
                {
                    AppId = appId,
                    IncludePlayerIds = PlayerIds,
                    WebPushTopic = Guid.NewGuid().ToString()
                };
                opt.Headings.Add(LanguageCodes.English, Title);
                opt.Contents.Add(LanguageCodes.English, Content);
                if (!string.IsNullOrEmpty(url))
                {
                    opt.Url = url;
                }
                NotificationCreateResult result = await client.Notifications.CreateAsync(opt);
                return result.Id;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        //public static async Task<string> PushNotification2(string Title, string Content, List<string> ExternalId, Guid appId, string restKey, string url)
        //{
        //    try
        //    {
        //        OneSignalClient client = new OneSignalClient(restKey);
        //        var opt = new NotificationCreateOptions()
        //        {
        //            AppId = appId,
        //            IncludePlayerIds = ExternalId,
        //            WebPushTopic = Guid.NewGuid().ToString()
        //        };
        //        opt.Headings.Add(LanguageCodes.English, Title);
        //        opt.Contents.Add(LanguageCodes.English, Content);
        //        if (!string.IsNullOrEmpty(url))
        //        {
        //            opt.Url = url;
        //        }
        //        NotificationCreateResult result = await client.Notifications.CreateAsync(opt);
        //        return result.Id;
        //    }
        //    catch (Exception ex)
        //    {
        //        return "";
        //    }
        //}
    }
}
