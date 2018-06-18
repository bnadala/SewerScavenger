using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;

using SewerScavenger.Services;

namespace SewerScavenger.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        //public IDataStore DataStore => DependencyService.Get<IDataStore>() ?? MockDataStore.Instance;
        //public IDataStore DataStore => DependencyService.Get<IDataStore>() ?? SQLDataStore.Instance;
        
        #region RefactorLater

        private IDataStore DataStoreMock => DependencyService.Get<IDataStore>() ?? MockDataStore.Instance;
        private IDataStore DataStoreSql => DependencyService.Get<IDataStore>() ?? SQLDataStore.Instance;

        public IDataStore DataStore;

        public BaseViewModel()
        {
            SetDataStore(DataStoreEnum.Mock);
        }

        public enum DataStoreEnum { Unknown = 0, Sql = 1, Mock = 2 }

        public void SetDataStore(DataStoreEnum data)
        {
            switch (data)
            {
                case DataStoreEnum.Mock:
                    DataStore = DataStoreMock;
                    break;

                case DataStoreEnum.Sql:
                case DataStoreEnum.Unknown:
                default:
                    DataStore = DataStoreSql;
                    break;
            }
        }


        #endregion

        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
