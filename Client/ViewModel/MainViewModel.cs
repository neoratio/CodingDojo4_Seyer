using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Client.ViewModel
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
        private Communication.Client clientcom;
        private bool isConnected = false;
        #region PROPERTIES
        public string ChatName { get; set; }
        public string Message { get; set; }
        public ObservableCollection<string> ReceivedMessages { get; set; }
        public RelayCommand ConnectBtnClickCmd { get; set; }
        public RelayCommand SendBtnClickCmd { get; set; }
        #endregion
        public MainViewModel()
        {
            Message = "";
            ReceivedMessages = new ObservableCollection<string>();

            ConnectBtnClickCmd = new RelayCommand(
                () =>
                {
                    isConnected = true;
                    clientcom = new Communication.Client("127.0.0.1", 10100, new Action<string>(NewMessageReceived), ClientDissconnected);

                },
            () =>
            {
                return (!isConnected);
            });

            SendBtnClickCmd = new RelayCommand(
                () => {
                    clientcom.Send(ChatName + ": " + Message);
                    ReceivedMessages.Add("YOU: " + Message);
                }, () => { return (isConnected && Message.Length >= 1); });
        }

        private void ClientDissconnected()
        {
            isConnected = false;
            CommandManager.InvalidateRequerySuggested();
        }

        private void NewMessageReceived(string message)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                ReceivedMessages.Add(message);
            });
        }
    }
}