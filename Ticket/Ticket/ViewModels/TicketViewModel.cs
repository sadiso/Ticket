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
    public class TicketViewModel : Tickets, INotifyPropertyChanged
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
        private string status;
        private string color;
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

        public string Status
        {
            set
            {
                if (status != value)
                {
                    status = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status"));
                }
            }
            get
            {
                return status;
            }
        }

        public string Color
        {
            set
            {
                if (color != value)
                {
                    color = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Color"));
                }
            }
            get
            {
                return color;
            }
        }
        #endregion

        #region Constructors
        public TicketViewModel(User user)
        {
            this.user = user;
            IsEnabled = true;
            Status = "Esperando para leer Ticket";
            Color = "Black";
        }
        #endregion

        #region Methods
        private async void CheckTicketCode()
        {
            IsRunning = true;
            IsEnabled = false;
            var response = await apiService.GetTicket(
                "http://checkticketsback.azurewebsites.net",
                "/api/Tickets/",
                TicketCode);
            IsRunning = false;
            IsEnabled = true;

            if (response.IsSuccess)
            {
                Color = "Red";
                Status = "TICKET YA LEIDO";
                //Falta validar errores de conexión
                return;
            }

            SaveTicket();
        }

        private async void SaveTicket()
        {
            var dateTime = DateTime.Date;
            var newTicket = new Tickets
            {
                TicketCode = TicketCode,
                DateTime = dateTime,
                UserId = user.UserId,
            };

            IsRunning = true;
            IsEnabled = false;
            var response = await apiService.Post(
                "http://checkticketsback.azurewebsites.net",
                "/api",
                "/Tickets",
                newTicket);
            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await dialogService.ShowMessage("Error", response.Message);
                return;
            }

            Color = "Green";
            Status = "ACCESO AUTORIZADO";
        }
        #endregion

        #region Commands
        public ICommand CheckTicketCommand
        {
            get { return new RelayCommand(CheckTicket); }
        }

        private async void CheckTicket()
        {
            if (string.IsNullOrEmpty(TicketCode))
            {
                await dialogService.ShowMessage("Error", "Debe ingresar un codigo.");
                return;
            }

            if (TicketCode.Length != 4)
            {
                await dialogService.ShowMessage("Error", "Codigo del Ticket incorrecto.");
                return;
            }

            CheckTicketCode();
        }
        #endregion
    }
}
