using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace RAM
{
    class RAMManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        int missing = 0;
        bool[] worklist = new bool[320];
        ProcessList processList;
        Memory[] memory = new Memory[4];
        LinkedList<int> FIFOList;
        LinkedList<int> LRUList;
        bool random = false;
        bool front = true;
        bool isFIFO;
        bool current_miss;

        public RAMManager()
        {
            //可能要改
            for (int i = 0; i < 320; i++) worklist[i] = true;
        }

        public int Missing
        {
            get { return missing; }
            set { missing = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("missing"));
                }
            }
        }

        int pageIn(int page)   //返回某页所在内存块序号，无则返回-1
        {
            for(int i=0;i<4;i++)
            {
                if (memory[i].Page == page) return i;
            }
            return -1;
        }

        int FIFO(int page)
        {
            int block = pageIn(page);
            if(block==-1)
            {
                Missing++;
                current_miss = true;
                if (FIFOList.Count < 4) 
                {
                    block = FIFOList.Count();
                    FIFOList.AddLast(page);
                }
                else
                {
                    block = pageIn(FIFOList.First());
                    FIFOList.RemoveFirst();
                    FIFOList.AddLast(page);
                }
            }
            return block;
        }

        int LRU(int page)
        {
            int block = pageIn(page);
            if(block==-1)
            {
                Missing++;
                current_miss = true;
                if (LRUList.Count < 4)
                {
                    block = LRUList.Count;
                    LRUList.AddFirst(page);
                }
                else
                {
                    block = pageIn(LRUList.Last());
                    LRUList.RemoveLast();
                    LRUList.AddFirst(page);
                }
            }
            else
            {
                LRUList.Remove(LRUList.Find(page));
                LRUList.AddFirst(page);
            }
            return block;
        }

        int next(int command)
        {
            int n=command;
            if(random)
            {
                Random ran = new Random();
                if(front)   //随机前跳
                {
                    do { n = ran.Next(0, command); }
                    while (!worklist[n]);
                }
                else        //随机后跳
                {
                    do { n = ran.Next(command+1,319); }
                    while (!worklist[n]);
                }
                front = !front;
            }
            else    //顺序执行
            {
                do { n++; }
                while (!worklist[n]);
            }
            random = !random;
            return n;
        }
        //?????
        public void run()
        {
            Random ran = new Random();
            int command = ran.Next(319);
            if(isFIFO)      //FIFO
            {
                for (int i=0;i<320;i++ )
                {
                    current_miss = false;
                    int block = FIFO(command / 10);
                    memory[block].Page = command / 10;
                    processList.Add(i, command, command / 10, current_miss, block);
                    command = next(command);
                }
            }
            else            //LRU
            {
                for(int i=0;i<320;i++)
                {
                    current_miss = false;
                    int block = LRU(command / 10);
                    memory[block].Page = command / 10;
                    processList.Add(i, command, command / 10, current_miss, block);
                    command = next(command);
                }
            }
        }
    }

}
