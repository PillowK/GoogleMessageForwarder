using GoogleMessage.Services;
using GoogleMessage.Wpf;
using System.Threading;
using System.Windows.Input;

namespace GoogleMessage.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly AutoForwardWorker _autoForwardWorker;

        private string commandText = "Start Autoforward";
        private bool onWorking = false;

        public string CommandText
        {
            get { return commandText; }
            set { SetProperty(ref commandText, value); }
        }

        public ICommand StartAutoForward { get; set; }

        public MainWindowViewModel(         
            AutoForwardWorker autoForwardWorker)
        {      
            _autoForwardWorker = autoForwardWorker;

            StartAutoForward = new RelayCommand(HandleStartAutoForward);
        }  

        public void HandleStartAutoForward()
        {   
            if(onWorking)
            {
                onWorking = false;                
                _autoForwardWorker.StopAsync(CancellationToken.None);
                CommandText = "Start Autoforward";
            }
            else
            {
                onWorking = true;                
                _autoForwardWorker.StartAsync(CancellationToken.None);
                CommandText = "Stop Autoforward";
            }                                         
        }
    }
}
