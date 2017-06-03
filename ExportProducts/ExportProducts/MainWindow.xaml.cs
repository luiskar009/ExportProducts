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
using System.Data.SqlClient;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
using System.IO;
using Bukimedia.PrestaSharp.Factories;
using Bukimedia.PrestaSharp.Entities;
using System.Windows.Forms;


namespace ExportProducts
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.Title = "Gestion Prestashop - Odacash";
        }

        protected void ExportOdacashBtn_Click(object sender, RoutedEventArgs e)
        {
            ExportOdacash eo = new ExportOdacash();
            eo.Show();
            eo.Owner = this;
        }

        protected void CreateProductBtn_Click(object sender, RoutedEventArgs e)
        {
            CreateProduct cp = new CreateProduct();
            cp.Show();
            cp.Owner = this;
        }

        protected void CreateCombinationBtn_Click(object sender, RoutedEventArgs e)
        {
            CreateCombination cc = new CreateCombination();
            cc.Show();
            cc.Owner = this;
        }

        protected void InsertInventoryBtn_Click(object sender, RoutedEventArgs e)
        {
            InsertInventory ii = new InsertInventory();
            ii.Show();
            ii.Owner = this;
        }
    }
}