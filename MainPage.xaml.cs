using Dropbox.Api;
using Dropbox.Api.Files;
using System.Text;
using System.Text.RegularExpressions;
using VaninChat2.Common;
using VaninChat2.Helpers;
using VaninChat2.Helpers.Dropbox;
using VaninChat2.Helpers.Internet;
using VaninChat2.Validators;
using VaninChat2.Workers;
using static Dropbox.Api.Files.SearchMatchType;

namespace VaninChat2
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            EDITOR_PASS.Text = PassHelper.Generate();
        }

        private async void ConnectAsync(object sender, EventArgs e)
        {
            CONNECT_BTN.IsEnabled = false;

            #region Validation
            if (!NameValidator.Check(EDITOR_NAME.Text, out var error))
            {
                await DisplayAlert("Некорректное имя", error, "OK");
                CONNECT_BTN.IsEnabled = true;
                return;
            }

            if (!PassValidator.Check(EDITOR_PASS.Text, out error))
            {
                await DisplayAlert("Некорректный пароль", error, "OK");
                CONNECT_BTN.IsEnabled = true;
                return;
            }

            if (!NameValidator.Check(EDITOR_COMPANION_NAME.Text, out error))
            {
                await DisplayAlert("Некорректное имя собеседника", error, "OK");
                CONNECT_BTN.IsEnabled = true;
                return;
            }
            #endregion

            EDITOR_NAME.IsEnabled = false;
            EDITOR_PASS.IsEnabled = false;
            EDITOR_COMPANION_NAME.IsEnabled = false;

            ConnectLabel.IsVisible = true;
            ConnectLabel.Text = "проверка доступа в интернет..";

            #region Check internet connection
            if (!await PingHelper.InternetConnectionCheckAsync())
            {
                ConnectLabel.IsVisible = false;
                var errorMsg = "Проверьте подключение к интернету и разрешения приложения";
                await DisplayAlert("Не удалось достучаться до google.com", errorMsg, "OK");

                EDITOR_NAME.IsEnabled = true;
                EDITOR_PASS.IsEnabled = true;
                EDITOR_COMPANION_NAME.IsEnabled = true;
                CONNECT_BTN.IsEnabled = true;

                return;
            }
            #endregion

            ConnectLabel.Text = "проверка best-proxies ключа..";

            #region Check best-proxies api key
            if (!await ProxyHelper.CheckApiKey(AppSettings.BEST_PROXIES_KEY))
            {
                ConnectLabel.IsVisible = false;
                var errorMsg = "Вероятно, ключ для прокси недействителен";
                await DisplayAlert("Обратитесь к разработчику приложения", errorMsg, "OK");

                EDITOR_NAME.IsEnabled = true;
                EDITOR_PASS.IsEnabled = true;
                EDITOR_COMPANION_NAME.IsEnabled = true;
                CONNECT_BTN.IsEnabled = true;

                return;
            }
            #endregion

            ConnectLabel.Text = "проверка dropbox токена..";

            #region Check dropbox access token
            var dropboxAccessTokenInfo = await DropboxHelper.CheckAccessToken(AppSettings.DROPBOX_ACCESS_TOKEN);
            if (!dropboxAccessTokenInfo.IsSuccess)
            {
                ConnectLabel.IsVisible = false;
                await DisplayAlert("Обратитесь к разработчику приложения", dropboxAccessTokenInfo.Error, "OK");
                
                EDITOR_NAME.IsEnabled = true;
                EDITOR_PASS.IsEnabled = true;
                EDITOR_COMPANION_NAME.IsEnabled = true;
                CONNECT_BTN.IsEnabled = true;

                return;
            }
            #endregion

            ConnectLabel.Text = "соединение..";

            var connectionWorker = new ConnectionWorker(
                EDITOR_NAME.Text, EDITOR_PASS.Text, EDITOR_COMPANION_NAME.Text);

            var connectionInfo = await connectionWorker.ExecuteAsync();

            if (connectionInfo == null)
            {
                ConnectLabel.IsVisible = false;
                await DisplayAlert("Ошибка", "Не удалось соединиться", "OK");

                EDITOR_NAME.IsEnabled = true;
                EDITOR_PASS.IsEnabled = true;
                EDITOR_COMPANION_NAME.IsEnabled = true;
                CONNECT_BTN.IsEnabled = true;

                return;
            }

            ConnectLabel.Text = $"активное соединение с {EDITOR_COMPANION_NAME.Text}";
            CONNECT_BTN.IsVisible = false;

            GO_TO_DISCUSSION_BTN.IsEnabled = true;
            GO_TO_DISCUSSION_BTN.IsVisible = true;

            DISCONNECT_BTN.IsEnabled = true;
            DISCONNECT_BTN.IsVisible = true;

            Singleton.Instance.Add(new DiscussionPage(connectionInfo));
            await Navigation.PushAsync(Singleton.Instance.Get<DiscussionPage>());
        }

        private async void GoToDiscussionAsync(object sender, EventArgs e)
            => await Navigation.PushAsync(Singleton.Instance.Get<DiscussionPage>());

        private async void DisconnectAsync(object sender, EventArgs e)
        {
            DISCONNECT_BTN.IsEnabled = false;
            ConnectLabel.Text = "разъединение..";

            GO_TO_DISCUSSION_BTN.IsEnabled = false;

            var discussionPage = Singleton.Instance.Get<DiscussionPage>();
            var connectionInfo = discussionPage.ConnectionInfo;

            if (!await new DisconnectionWorker(connectionInfo).ExecuteAsync())
            {
                await DisplayAlert("Ошибка разъединения", "Не удалось разъединиться", "OK");

                GO_TO_DISCUSSION_BTN.IsEnabled = true;

                DISCONNECT_BTN.IsEnabled = true;
                ConnectLabel.Text = $"активное соединение с {EDITOR_COMPANION_NAME.Text}";
                return;
            }

            EDITOR_NAME.IsEnabled = true;
            EDITOR_PASS.IsEnabled = true;
            EDITOR_COMPANION_NAME.IsEnabled = true;

            GO_TO_DISCUSSION_BTN.IsVisible = false;
            DISCONNECT_BTN.IsVisible = false;

            ConnectLabel.Text = "соединение..";
            ConnectLabel.IsVisible = false;

            CONNECT_BTN.IsVisible = true;
            CONNECT_BTN.IsEnabled = true;

            Singleton.Instance.Remove<DiscussionPage>();
        }
    }
}