using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace RAM
{
    class Memory : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        int page = -1;
        public int Page
        {
            get { return page; }
            set
            {
                page = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Page"));
                }
            }
        }

        public bool isEmpty { get { return (Page == -1); } }
    }
}
