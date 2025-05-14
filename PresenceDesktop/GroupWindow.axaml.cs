using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using data.RemoteData.RemoteDataBase;
using data.RemoteData.RemoteDatabase.DAO;
using PresenceHTTPClient.Interfaces;

namespace PresenceDesktop;

public partial class GroupWindow : Window
    {
        private readonly IGroupAPIClient _groupApiClient;
        private readonly IUserAPIClient _userApiClient;
        private List<GroupDAO> _groups = new();

        
        public GroupWindow()
        {
            InitializeComponent();
        }
        public GroupWindow(IGroupAPIClient groupApiClient, IUserAPIClient userApiClient)
        {
            InitializeComponent();
            _groupApiClient = groupApiClient;
            _userApiClient = userApiClient;

            GroupComboBox.SelectionChanged += (_, _) => LoadUsers();

            Loaded += async (_, _) => await LoadGroupsAsync();
        }

        private async Task LoadGroupsAsync()
        {
            _groups = await _groupApiClient.GetGroupsAsync();
            GroupComboBox.ItemsSource = _groups;
            if (_groups.Count > 0 && GroupComboBox.SelectedIndex == -1)
                GroupComboBox.SelectedIndex = 0;
            LoadUsers();
        }

        private async void LoadUsers()
        {
            if (GroupComboBox.SelectedItem is not GroupDAO selectedGroup)
                return;

            var groupsWithUsers = await _groupApiClient.GetGroupsWithUsersAsync();
            var selected = groupsWithUsers.FirstOrDefault(g => g.ID == selectedGroup.Id);
            UsersList.ItemsSource = selected?.Users ?? new List<UserDAO>();
        }

        private async void BtnAddUser_Click(object? sender, RoutedEventArgs e)
        {
            var addWindow = new AddOrEditUserWindow(_userApiClient, _groupApiClient);
            await addWindow.ShowDialog(this);
            LoadUsers();
        }

        private async void EditUser_Click(object? sender, RoutedEventArgs e)
        {
            if (UsersList.SelectedItem is UserDAO selectedUser)
            {
                var editWindow = new AddOrEditUserWindow(_userApiClient, _groupApiClient, selectedUser);
                await editWindow.ShowDialog(this);
                LoadUsers();
            }
        }

        private async void DeleteUser_Click(object? sender, RoutedEventArgs e)
        {
            if (UsersList.SelectedItem is UserDAO selectedUser)
            {
                await _userApiClient.DeleteUserAsync(selectedUser.UserId);
                LoadUsers();
            }
        }
    }