using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
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
            dialogService = new DialogService();
            apiService = new ApiService();
            IsEnabled = true;
            Status = "Esperando para leer Ticket";
            Color = "Black";
        }
        #endregion

        #region Methods
       
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
                await dialogService.ShowMessage("Error", "Codigo del Ticket incorrecto");
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            //var client = new HttpClient();
            //client.BaseAddress = new Uri("http://checkticketsback.azurewebsites.net");
            //var url = string.Format("{0}{1}", "/api/Tickets/", TicketCode);
            //var response = await client.GetAsync(url);

            var response = await apiService.Get<Tickets>(
                "http://checkticketsback.azurewebsites.net",
                "/api/Tickets/",
                TicketCode);

            IsRunning = false;
            IsEnabled = true;

            if (response.IsSuccess)
            {
                Color = "Red";
                Status = string.Format("{0} ,{1}", TicketCode,"TICKET YA LEIDO");
                //Falta validar errores de conexión
                return;
            }

            var dateTime = DateTime.Now;
            var Date = string.Format("{0}-{1}-{2}T{3:00}:{4:00}:{5:00}", dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);

            var newTicket = new Tickets
            {
                TicketCode = TicketCode,
                Datetime = Date,
                UserId = user.UserId,
            };

            IsRunning = true;
            IsEnabled = false;

            //var request = JsonConvert.SerializeObject(newTicket);
            //var content = new StringContent(request, Encoding.UTF8, "application/json");
            //client = new HttpClient();
            //client.BaseAddress = new Uri("http://checkticketsback.azurewebsites.net");
            //url = string.Format("{0}{1}", "/api", "/Tickets");
            //response = await client.PostAsync(url, content);

            response = await apiService.Post<Tickets>(
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
            Status = Status = string.Format("{0} ,{1}", TicketCode, "ACCESO AUTORIZADO");
        }
        #endregion
    }
}
