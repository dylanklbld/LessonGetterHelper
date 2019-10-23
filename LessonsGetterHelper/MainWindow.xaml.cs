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
using WebClientHandler.Helpers;
using WebClientHandler.Scripting;
using WebClientHandler.Scripting.Clients;
using WebClientHandler.Scripting.Scripts;

namespace LessonsGetterHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            StartAnalyze();

        }

        public void StartAnalyze()
        {
            ITatarEduClient myClient = new HttpClientCustomClient();

            myClient.Logon("4909002335", "vita785@@");

            JournalFullDataCollector.InitStuff(myClient.GetJournalHtml(), myClient);
            JournalFullDataCollector.CollectFullClassesDataAsync();
        }
    }
}
