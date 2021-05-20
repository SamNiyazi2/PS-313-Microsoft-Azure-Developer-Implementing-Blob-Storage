using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WiredBrainCoffee.AdminApp.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            // 05/20/2021 09:32 am - SSN - [20210520-0654] - [001] - M06-03 - Fetch metadata from a blob
            // Testing
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
           // PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
