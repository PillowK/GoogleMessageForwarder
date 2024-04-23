using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GoogleMessage.Wpf
{
    public abstract class BindableBase : INotifyPropertyChanged
    {
        private bool _isPending = false;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsPending
        {
            get { return _isPending; }
            set { SetProperty(ref _isPending, value); }
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            RaisePropertyChanged(propertyName);

            return true;
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }
    }
}
