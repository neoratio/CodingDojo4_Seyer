using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System;

namespace Server.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private Communication.Server server;
        private const int port = 6666;
        private const string ip = "127.0.0.1";
        private bool isConnected = false;

        public RelayCommand StartBtnClickCmd { get; set; }
        public RelayCommand StopBtnClickCmd { get; set; }
        public RelayCommand DropClientBtnClickCmd { get; set; }
        public RelayCommand SaveToLogBtnClickCmd { get; set; }
        public ObservableCollection<string> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }

        private string selectedUser;
        public string SelectedUser
        {
            get
            {
                return selectedUser;
            }
            set
            {
                if (selectedUser == value)
                {
                    return;
                }

                selectedUser = value;
                RaiseAllCanExecuteChanged();
            }
        }

        public int NoOfReceivedMessages
        {
            get
            {
                return Messages.Count;
            }
        }
        public MainViewModel()
        {
            Messages = new ObservableCollection<string>();
            Users = new ObservableCollection<string>();

            StartBtnClickCmd = new RelayCommand(
                () =>
                {
                    server = new Communication.Server(ip, port, UpdateGuiWithNewMessage);
                    server.StartServer();
                    SetIsConnected(true);
                },
                () => { return !isConnected; });

            StopBtnClickCmd = new RelayCommand(
                () =>
                {
                    server.StopServer();
                    SetIsConnected(false);
                },
                () => { return isConnected; });

            DropClientBtnClickCmd = new RelayCommand(() =>
            {
                server.DisconnectClient(SelectedUser);
                Users.Remove(SelectedUser);
            },
                () => { return (SelectedUser != null); });

        }

        public void UpdateGuiWithNewMessage(string message)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                string name = message.Split(':')[0];
                string[] msgs = message.Split(':');
                string msg = msgs.Length > 1 ? msgs[1].TrimStart() : null;

                if (msg == "@quit")
                {
                    Users.Remove(name);
                    message = string.Format("{0} disconnected.", name);
                    return;
                }

                if (!Users.Contains(name))
                {
                    Users.Add(name);
                }
                Messages.Add(message);
                RaisePropertyChanged("NoOfReceivedMessages");
            });
        }

        private void SetIsConnected(bool value)
        {
            isConnected = value;
            RaiseAllCanExecuteChanged();
        }

        private void RaiseAllCanExecuteChanged()
        {
            StartBtnClickCmd.RaiseCanExecuteChanged();
            StopBtnClickCmd.RaiseCanExecuteChanged();
            DropClientBtnClickCmd.RaiseCanExecuteChanged();
        }
    }
}
