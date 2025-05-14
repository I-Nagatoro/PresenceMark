using Avalonia.Controls;
using PresenceHTTPClient.Interfaces;

namespace PresenceDesktop;

public partial class MainWindow : Window
{
    private readonly IGroupAPIClient _groupApiClient;
    private readonly IUserAPIClient _userApiClient;
    private readonly IPresenceAPIClient _presenceApiClient;

    // Изменённый конструктор, который принимает зависимости
    public MainWindow(IGroupAPIClient groupApiClient, IUserAPIClient userApiClient, IPresenceAPIClient presenceApiClient)
    {
        InitializeComponent();

        _groupApiClient = groupApiClient;
        _userApiClient = userApiClient;
        _presenceApiClient = presenceApiClient;

        BtnOpenGroups.Click += (_, _) => new GroupWindow(_groupApiClient, _userApiClient).Show();
        BtnOpenAttendance.Click += (_, _) => new AttendanceWindow(_groupApiClient, _presenceApiClient).Show();
    }
}