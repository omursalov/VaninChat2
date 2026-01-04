using VaninChat2.Models;
using VaninChat2.Validators;
using VaninChat2.Workers;

namespace VaninChat2;

public partial class DiscussionPage : ContentPage
{
    public ConnectionInfo ConnectionInfo { get; }

    public DiscussionPage(ConnectionInfo connectionInfo)
    {
        InitializeComponent();
        ConnectionInfo = connectionInfo;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    private async void SendAsync(object sender, EventArgs e)
    {
        SEND_BTN.IsEnabled = false;

        if (MessageValidator.Check(EDITOR_MESSAGE.Text, out var error))
        {
            await DisplayAlert("Некорректное сообщение", error, "OK");
            SEND_BTN.IsEnabled = true;
            return;
        }

        await new DiscussionWorker(ConnectionInfo).SendAsync(EDITOR_MESSAGE.Text);
    }
}