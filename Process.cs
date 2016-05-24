using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.ObjectModel;

namespace RAM
{
    class Process   //记录进度
    {
        int index;
        int command;
        int page;
        bool missing;
        int memory;
        public int Index
        {
            get { return index; }
        }

        public int Command { get { return command; } }
        public int Page { get { return page; } }
        public bool Missing { get { return missing; } }
        public int Memory { get { return memory; } }
        public Process(int i, int c, int p, bool m, int r)
        {
            index = i;
            command = c;
            page = p;
            missing = m;
            memory = r;
        }
    }

    class ProcessList : ObservableCollection<Process>
    {
        public void Add(int i, int c, int p, bool m, int r)
        {
            Process process = new Process(i, c, p, m, r);
            this.Add(process);            
        }
    }
}
