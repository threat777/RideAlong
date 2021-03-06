using System;

using Android.App;

using RideAlong;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Auth;
using Newtonsoft.Json.Linq;
using RideAlong.Entities;

[assembly: ExportRenderer(typeof(Login), typeof(RideAlong.Droid.LoginRenderer))]

namespace RideAlong.Droid
{
    class LoginRenderer : PageRenderer
    {
        public LoginRenderer()
        {
            var activity = this.Context as Activity;

            var auth = new OAuth2Authenticator(
                clientId: "1876955965864698", // your OAuth2 client id
                scope: "", // the scopes for the particular API you're accessing, delimited by "+" symbols
                authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
                redirectUrl: new Uri("http://www.facebook.com/connect/login_success.html"));

            auth.Completed += async (sender, eventArgs) => {
                if (eventArgs.IsAuthenticated)
                {
                    var accessToken = eventArgs.Account.Properties["access_token"].ToString();
                    var expiresIn = Convert.ToDouble(eventArgs.Account.Properties["expires_in"]);
                    var expiryDate = DateTime.Now + TimeSpan.FromSeconds(expiresIn);

                    var request = new OAuth2Request("GET", new Uri("https://graph.facebook.com/me"), null, eventArgs.Account);
                    var response = await request.GetResponseAsync();
                    var obj = JObject.Parse(response.GetResponseText());

                    User user = new User();
                    user.id = long.Parse(obj["id"].ToString().Replace("\"", ""));
                    user.name = obj["name"].ToString().Replace("\"", "");
                    user.rating = 9000;

                    await App.NavigateToHome(user);
                }
                else
                {
                    App.HideLoginView();
                    
                }
            };

            activity.StartActivity(auth.GetUI(activity));
        }
    }
}