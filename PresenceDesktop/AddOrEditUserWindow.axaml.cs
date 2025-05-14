using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using data.RemoteData.RemoteDataBase;
using data.RemoteData.RemoteDatabase.DAO;
using PresenceHTTPClient.Interfaces;

namespace PresenceDesktop;

public partial class AddOrEditUserWindow : Window
    {
        private readonly IUserAPIClient _userApiClient;
        private readonly IGroupAPIClient _groupApiClient;
        private readonly UserDAO _editingUser;
        private List<GroupDAO> _groups = new();

        public AddOrEditUserWindow()
        {
            InitializeComponent();
        }

        public AddOrEditUserWindow(
            IUserAPIClient userApiClient,
            IGroupAPIClient groupApiClient,
            UserDAO? editingUser = null)
            : this()
        {
            _userApiClient = userApiClient;
            _groupApiClient = groupApiClient;
            _editingUser = editingUser;

            Loaded += AddOrEditUserWindow_Loaded;
        }

        private async void AddOrEditUserWindow_Loaded(object? sender, System.EventArgs e)
        {
            _groups = await _groupApiClient.GetGroupsAsync();
            GroupComboBox.ItemsSource = _groups;
            GroupComboBox.SelectedIndex = 0;

            if (_editingUser != null)
            {
                FIOTextBox.Text = _editingUser.FIO;
                GroupComboBox.SelectedItem = _groups.FirstOrDefault(g => g.Id == _editingUser.GroupId);
            }
        }

        private async void SaveButton_Click(object? sender, RoutedEventArgs e)
        {
            if (GroupComboBox.SelectedItem is not GroupDAO selectedGroup)
                return;

            var fio = FIOTextBox.Text?.Trim();
            if (string.IsNullOrWhiteSpace(fio))
                return;

            if (_editingUser != null)
            {
                if (_editingUser.FIO != fio)
                {
                    await _userApiClient.UpdateUser(_editingUser.UserId, fio, selectedGroup.Id);
                }
            }
            else
            {
                await _userApiClient.CreateUser(fio, selectedGroup.Id);
            }

            Close();
        }
    }