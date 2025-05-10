using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkShopPC.pgsPC
{
    public class WorkViewModel : INotifyPropertyChanged
    {
        private bool _isSelected;
        public CompletedWorks CompletedWork { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                    
                }
            }
        }

        public WorkViewModel(CompletedWorks work, bool isSelected = false)
        {
            CompletedWork = work;
            IsSelected = isSelected;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
