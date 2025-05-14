using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using data.RemoteData.RemoteDataBase;
using data.RemoteData.RemoteDatabase.DAO;
using PresenceHTTPClient.Interfaces;

namespace PresenceDesktop;

public partial class AttendanceWindow : Window
    {
        private readonly IGroupAPIClient _groupApiClient;
        private readonly IPresenceAPIClient _presenceApiClient;
        private List<GroupDAO> _groups = new();
        
        public AttendanceWindow()
        {
            InitializeComponent();
        }

        public AttendanceWindow(IGroupAPIClient groupApiClient, IPresenceAPIClient presenceApiClient)
        {
            InitializeComponent();
            _groupApiClient = groupApiClient;
            _presenceApiClient = presenceApiClient;

            GroupComboBox.SelectionChanged += (_, _) => LoadAttendanceAsync();
            DateCalendar.SelectedDatesChanged += (_, _) => LoadAttendanceAsync();

            Loaded += async (_, _) => await InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await LoadGroupsAsync();
            await LoadAttendanceAsync();
        }

        private async Task LoadGroupsAsync()
        {
            _groups = await _groupApiClient.GetGroupsAsync();
            GroupComboBox.ItemsSource = _groups;

            if (GroupComboBox.SelectedIndex == -1 && _groups.Count > 0)
                GroupComboBox.SelectedIndex = 0;

            if (DateCalendar.SelectedDate == null)
                DateCalendar.SelectedDate = DateTime.Today;
        }

        private async Task LoadAttendanceAsync()
        {
            if (GroupComboBox.SelectedItem is not GroupDAO selectedGroup
                || DateCalendar.SelectedDate == null)
                return;

            var date = DateCalendar.SelectedDate.Value.Date;

            try
            {
                var response = await _presenceApiClient.GetPresenceAsync(
                    selectedGroup.Id,
                    date.ToString("dd.MM.yyyy"),
                    date.ToString("dd.MM.yyyy")
                );

                if (response?.Users == null || response.Users.Count == 0)
                {
                    AttendanceList.ItemsSource = new List<string> { "Нет данных о посещаемости." };
                    return;
                }

                var list = response.Users.Select(u =>
                {
                    var parsedDate = u.Date;
                    return $"{u.FIO} - {(u.IsAttendance ? "Присутствовал" : "Отсутствовал")} - {parsedDate:dd.MM.yyyy}";
                }).ToList();

                AttendanceList.ItemsSource = list;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке посещаемости: {ex.Message}");
                AttendanceList.ItemsSource = new List<string> { "Ошибка при загрузке данных." };
            }
        }

        private async void BtnRefreshAttendance_Click(object? sender, RoutedEventArgs? e)
        {
            await LoadGroupsAsync();
            await LoadAttendanceAsync();
        }
    }