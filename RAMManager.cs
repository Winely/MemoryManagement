using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;

namespace RAM
{
    enum dir { front, back, mid };
    class RAMManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        int missing = 0;
        List<int> worklist = new List<int>();
        public ProcessList processList = new ProcessList();
        public Memory[] memory = new Memory[4];
        LinkedList<int> FIFOList = new LinkedList<int>();
        LinkedList<int> LRUList = new LinkedList<int>();
        bool random = false;
        dir direc = dir.front;
        public bool stop { get; set; }
        bool isFIFO;
        bool current_miss;
        bool skip = false;
        int work_index;
        public MainWindow parent;
        public RAMManager()
        {
            stop = true;
            for (int i = 0; i < 320; i++) worklist.Add(i);
            for (int i = 0; i < 4; i++) memory[i] = new Memory();
        }

        public bool IsFIFO
        {
            get { return isFIFO; }
            set { isFIFO = value; }
        }
        public bool Skip
        {
            get { return skip; }
            set { skip = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Skip"));
                }
                }
        }
        public Memory[] RAMs
        {
            get { return memory; }
        }
        public int Missing
        {
            get { return missing; }
            set { 
                missing = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Missing"));
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

        int next(int command_index)
        {
            if (processList.Count == 320 || worklist.Count==0) return 322;
            Random ran = new Random();
            int n=command_index;
            if(random)
            {
                if(direc==dir.front)   //随机前跳
                {
                    if (work_index == 0) n = ran.Next(worklist.Count - 1);
                    else n = ran.Next(0, work_index - 1);
                    work_index--;
                }
                else if(direc==dir.back)        //随机后跳
                {
                    if (work_index > worklist.Count - 1) n = ran.Next(worklist.Count - 1);
                    else n = ran.Next(work_index, worklist.Count - 1);
                }
                else
                {
                    n = ran.Next(worklist.Count);
                }
            }
            else    //顺序执行
            {
                if (direc == dir.front) direc = dir.back;
                else if (direc == dir.mid) 
                { 
                    work_index = ran.Next(worklist.Count);
                    direc = dir.front;
                }
                else direc = dir.mid; 
            }
            random = !random;
            return n;
        }
        public void run()
        {
            //取随机位置
            Random ran = new Random();
            work_index = ran.Next(319);
            
            //运行
            if(isFIFO)      //FIFO
            {
                int command_index = work_index;
                int command = worklist[command_index];
                for (int i = 0; i < 320;i++ )
                {
                    if (stop) return;
                    if (command_index == 322) return;//一个记号
                    if (command_index >= worklist.Count || command_index<0)
                    {
                        i--;
                        command_index = next(command_index);
                        if (processList.Count == 320) return;
                        continue;
                    }
                    command = worklist[command_index];
                    worklist.RemoveAt(command_index);
                    current_miss = false;
                    int block = FIFO(command / 10);
                    memory[block].Page = command / 10;
                    AddProcess(i, command, command / 10, current_miss, block);
                    command_index = next(command_index);
                    sleep();
                }
            }
            else            //LRU
            {
                int command_index = work_index;
                int command = worklist[command_index];
                for (int i = 0; i < 320; i++)
                {
                    if (stop) return;
                    if (command_index >= worklist.Count||command_index<0)
                    {
                        i--;
                        command_index = next(command_index);
                        if (processList.Count == 320) return;
                        continue;
                    }
                    command = worklist[command_index];
                    worklist.RemoveAt(command_index);
                    current_miss = false;
                    int block = LRU(command / 10);
                    memory[block].Page = command / 10;
                    AddProcess(i, command, command / 10, current_miss, block);
                    command_index = next(command_index);
                    sleep();
                }
            }
        }

        void sleep()
        {
            if (skip) Thread.Sleep(10);
            else Thread.Sleep(300);
        }
        public void clear()
        {
            stop = true;
            missing = 0;
            worklist.Clear();
            for (int i = 0; i < 320; i++) worklist.Add(i);
            processList.Clear();
            FIFOList.Clear();
            LRUList.Clear();
            for (int i = 0; i < 4; i++) memory[i].Page = -1;
            random = false;
            direc = dir.mid;
            Skip = false;
        }
        public void AddProcess(int i, int c, int p, bool m, int r)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                parent.Dispatcher.Invoke(new Action(() =>
                {
                    processList.Add(i, c, p, m, r);
                    parent.dataGrid.ScrollIntoView(processList.Last());
                }), null);
            });
        }
    }

}
