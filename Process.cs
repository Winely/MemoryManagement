using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAM
{
    class Process   //记录进度
    {
        int index;
        int command;
        int page;
        bool missing;
        int memory;
        public Process(int i, int c, int p, bool m, int r)
        {
            index = i;
            command = c;
            page = p;
            missing = m;
            memory = r;
        }
    }

    class ProcessList : List<Process>
    {
        public void Add(int i, int c, int p, bool m, int r)
        {
            Process process = new Process(i, c, p, m, r);
            this.Add(process);
        }
    }
}
