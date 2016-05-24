using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace RAM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RAMManager manager = new RAMManager();
        private BackgroundWorker bgworker = new BackgroundWorker();
  
        public MainWindow()
        {
            manager.parent = this;
            InitializeComponent();
            bindings();
            bgworker.DoWork += DoWorkHandler;
        }
        
        private void DoWorkHandler(object sender, DoWorkEventArgs e)
        {
            manager.run();
            MessageBox.Show("缺页率为："+(Convert.ToDouble(manager.Missing)/3.2).ToString()+"%");
        }

        private void bindings()
        {
            //内存块显示页
            Binding[] ramBindings = new Binding[4];
            PageConverter pageConverter = new PageConverter();
            for(int i=0;i<4;i++)
            {
                ramBindings[i] = new Binding("RAMs[" + i.ToString() + "].Page");
                ramBindings[i].Mode = BindingMode.OneWay;
                ramBindings[i].Source = manager;
                ramBindings[i].Converter = pageConverter;
                Label label = (Label)FindName("ram" + i.ToString());
                label.SetBinding(Label.ContentProperty, ramBindings[i]);
            }
            //缺页率显示页
            Binding missing = new Binding("Missing");
            missing.Mode = BindingMode.OneWay;
            missing.Source = manager;
            Label p = (Label)FindName("missing_page");
            p.SetBinding(Label.ContentProperty, missing);
            //快进按钮
            SkipConverter skipconverter = new SkipConverter();
            Binding skipBinding = new Binding("Skip");
            skipBinding.Mode = BindingMode.OneWay;
            skipBinding.Source = manager;
            skipBinding.Converter = skipconverter;
            Button btn = (Button)FindName("skiping");
            btn.SetBinding(Button.ContentProperty, skipBinding);
            //进度表
            dataGrid.ItemsSource = manager.processList;
        }
        private void DataGrid_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch(e.Column.Header.ToString())
            {
                case "index": e.Column.Header = "索引";break;
                case "command": e.Column.Header = "指令"; break;
                case "page": e.Column.Header = "页"; break;
                case "missing": e.Column.Header = "缺失"; break;
                case "memory": e.Column.Header = "内存块"; break;
                default: break;
            }
        }

        //按钮事件
        private void runFIFO(object sender, RoutedEventArgs e)
        {
            manager.stop = false;
            manager.IsFIFO = true;
            bgworker.RunWorkerAsync();
            setBTN(false);
        }
        private void runLRU(object sender, RoutedEventArgs e)
        {
            manager.stop = false;
            manager.IsFIFO = false;
            bgworker.RunWorkerAsync();
            setBTN(false);
        }
        private void skip(object sender, RoutedEventArgs e)
        {
            manager.Skip = !manager.Skip;
        }
        private void Clear(object sender, RoutedEventArgs e)
        {
            manager.clear();
            setBTN(true);
        }
        private void setBTN(bool isEnabled)
        {
            Button btn = (Button)FindName("fifo");
            btn.IsEnabled = isEnabled;
            btn = (Button)FindName("lru");
            btn.IsEnabled = isEnabled;
        }
    }

}
