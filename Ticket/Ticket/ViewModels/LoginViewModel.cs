using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Ticket.Models;
using Ticket.Services;

namespace Ticket.ViewModels
{
    public class LoginViewModel : Login, INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Attributes
        private User user;
        private DialogService dialogService;
        private ApiService apiService;
        private NavigationService navigationService;
        private bool isRunning;
        private bool isEnabled;
        #endregion

        #region Properties
        public bool IsRunning
        {
            set
            {
                if (isRunning != value)
                {
                    isRunning = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRunning"));
                }
            }
            get
            {
                return isRunning;
            }
        }

        public bool IsEnabled
        {
            set
            {
                if (isEnabled != value)
                {
                    isEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsEnabled"));
                }
            }
            get
            {
                return isEnabled;
            }
        }
        #endregion

        #region Constructors
        public LoginViewModel()
        {
            apiService = new ApiService();
            dialogService = new DialogService();
            navigationService = new NavigationService();
            IsEnabled = true;
        }
        #endregion

        #region Commands
        public ICommand LoginUserCommand
        {
            get { return new RelayCommand(LoginUser); }
        }

        private async void LoginUser()
        {
            if (string.IsNullOrEmpty(Email))
            {
                await dialogService.ShowMessage("Error", "You must enter a Email.");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                await dialogService.ShowMessage("Error", "You must enter a Password.");
                return;
            }
            
            var login = new Login
            {
                Email = Email,
                Password = Password,
                UserId = 0,
                FirstName = string.Empty,
                LastName = string.Empty,
            };

            IsRunning = true;
            IsEnabled = false;
            var response = await apiService.Post(
                "http://checkticketsback.azurewebsites.net",
                "/api",
                "/Users/Login",
                login);
            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await dialogService.ShowMessage("Error", response.Message);
                return;
            }

            user = (User)response.Result;

            var maninViewModel = MainViewModel.GetInstance();
            maninViewModel.Ticket = new TicketViewModel(user);
            await navigationService.Navigate("TicketPage");
        }
        #endregion
    }
}
