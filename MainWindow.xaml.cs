using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        }

        private void runFIFO(object sender, RoutedEventArgs e)
        {
            manager.IsFIFO = true;
            bgworker.RunWorkerAsync();
        }

        private void runLRU(object sender, RoutedEventArgs e)
        {
            manager.IsFIFO = false;
            bgworker.RunWorkerAsync();
        }

    }
}
