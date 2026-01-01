using VaninChat2.Common;
using VaninChat2.Validators;
using VaninChat2.Workers;
using VaninChat2.Workers.Internet;

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
            EDITOR_PASS.Text = new PassWorker().Generate();
            EDITOR_MESSAGE.IsEnabled = false;
        }

        private async void ConnectAsync(object sender, EventArgs e)
        {
            CONNECT_BTN.IsEnabled = false;

            #region Validation
            var nameValidator = new NameValidator();

            if (!nameValidator.Check(EDITOR_NAME.Text, out var error))
            {
                await DisplayAlert("Некорректное имя", error, "OK");
                CONNECT_BTN.IsEnabled = true;
                return;
            }

            if (!new PassValidator().Check(EDITOR_PASS.Text, out error))
            {
                await DisplayAlert("Некорректный пароль", error, "OK");
                CONNECT_BTN.IsEnabled = true;
                return;
            }

            if (!nameValidator.Check(EDITOR_COMPANION_NAME.Text, out error))
            {
                await DisplayAlert("Некорректное имя собеседника", error, "OK");
                CONNECT_BTN.IsEnabled = true;
                return;
            }
            #endregion

            #region Check internet connection
            if (!await new PingWorker().InternetConnectionCheckAsync())
            {
                var errorMsg = "Проверьте подключение к интернету и разрешения приложения";
                await DisplayAlert("Не удалось достучаться до google.com", errorMsg, "OK");
                CONNECT_BTN.IsEnabled = true;
                return;
            }
            #endregion

            EDITOR_NAME.IsEnabled = false;
            EDITOR_PASS.IsEnabled = false;
            EDITOR_COMPANION_NAME.IsEnabled = false;

            ConnectLabel.IsVisible = true;
            ConnectLabel.Text = "соединение..";

            var singleton = Singleton.Get();
            singleton.Add(new ConnectionWorker(EDITOR_NAME.Text, EDITOR_PASS.Text, EDITOR_COMPANION_NAME.Text));

            var connectionInfo = await singleton.Get<ConnectionWorker>().ExecuteAsync();

            if (connectionInfo == null)
            {
                ConnectLabel.IsVisible = false;
                await DisplayAlert("Ошибка", "Не удалось соединиться", "OK");
                EDITOR_NAME.IsEnabled = true;
                EDITOR_PASS.IsEnabled = true;
                EDITOR_COMPANION_NAME.IsEnabled = true;
                CONNECT_BTN.IsEnabled = true;
                singleton.DisposeAndClear();
                return;
            }

            singleton.DisposeAndClear();

            singleton.Add(connectionInfo);

            ConnectLabel.Text = $"соединение с {EDITOR_COMPANION_NAME.Text} установлено";

            CONNECT_BTN.Text = "отправить";
            CONNECT_BTN.IsEnabled = true;
        }

        protected override void OnDisappearing()
            => Singleton.Get().DisposeAndClear();
    }
}