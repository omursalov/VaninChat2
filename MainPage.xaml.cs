using VaninChat2.Common;
using VaninChat2.Workers;

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
            EDITOR_MESSAGE.IsEnabled = false;
        }

        private async void ConnectAsync(object sender, EventArgs e)
        {
            CONNECT_BTN.IsEnabled = false;

            #region Validation
            if (string.IsNullOrWhiteSpace(EDITOR_NAME.Text))
            {
                await DisplayAlert("Не заполнены обязательные поля", "Укажите имя", "OK");
                CONNECT_BTN.IsEnabled = true;
                return;
            }

            if (EDITOR_NAME.Text.Any(x => Char.IsWhiteSpace(x)))
            {
                await DisplayAlert("Имя указано не верно", "Уберите пробелы", "OK");
                CONNECT_BTN.IsEnabled = true;
                return;
            }

            if (EDITOR_NAME.Text.Length < 4)
            {
                await DisplayAlert("Имя указано не верно", "Минимум 4 символа", "OK");
                CONNECT_BTN.IsEnabled = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(EDITOR_PASS.Text))
            {
                await DisplayAlert("Не заполнены обязательные поля", "Укажите пароль", "OK");
                CONNECT_BTN.IsEnabled = true;
                return;
            }

            if (EDITOR_PASS.Text.Any(x => Char.IsWhiteSpace(x)))
            {
                await DisplayAlert("Пароль указан не верно", "Уберите пробелы", "OK");
                CONNECT_BTN.IsEnabled = true;
                return;
            }

            if (EDITOR_PASS.Text.Length < 8)
            {
                await DisplayAlert("Пароль указан не верно", "Минимум 8 символов", "OK");
                CONNECT_BTN.IsEnabled = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(EDITOR_COMPANION_NAME.Text))
            {
                await DisplayAlert("Не заполнены обязательные поля", "Укажите имя собеседника", "OK");
                CONNECT_BTN.IsEnabled = true;
                return;
            }

            if (EDITOR_COMPANION_NAME.Text.Any(x => Char.IsWhiteSpace(x)))
            {
                await DisplayAlert("Имя собеседника указано не верно", "Уберите пробелы", "OK");
                CONNECT_BTN.IsEnabled = true;
                return;
            }

            if (EDITOR_COMPANION_NAME.Text.Length < 4)
            {
                await DisplayAlert("Имя собеседника указано не верно", "Минимум 4 символа", "OK");
                CONNECT_BTN.IsEnabled = true;
                return;
            }
            #endregion

            EDITOR_NAME.IsEnabled = false;
            EDITOR_PASS.IsEnabled = false;
            EDITOR_COMPANION_NAME.IsEnabled = false;

            ConnectLabel.IsVisible = true;
            ConnectLabel.Text = "соединение..";

            var passWorker = new ConnectionWorker(
                EDITOR_NAME.Text, EDITOR_PASS.Text, EDITOR_COMPANION_NAME.Text);
            if (!await passWorker.ExecuteAsync())
            {
                ConnectLabel.IsVisible = false;
                await DisplayAlert("Ошибка", "Не удалось соединиться", "OK");
                CONNECT_BTN.IsEnabled = true;
                return;
            }

            ConnectLabel.Text = "соединение установлено";

            CONNECT_BTN.Text = "отправить";
            CONNECT_BTN.IsEnabled = true;
        }

        protected override void OnDisappearing()
        {
            var singleton = Singleton.Get();
            // singleton.TryGet<TcpWorker>("tcpWorker")?.Dispose();
        }
    }
}
