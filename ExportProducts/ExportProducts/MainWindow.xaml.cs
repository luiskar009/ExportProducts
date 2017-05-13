﻿using System;
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


namespace ExportProducts
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>



    public partial class MainWindow : Window
    {
        private string BaseUrl = ConfigurationManager.AppSettings["baseUrl"].ToString();
        private string Account = ConfigurationManager.AppSettings["accProduct"].ToString();
        private string Password = "";

        public MainWindow()
        {
            InitializeComponent();

        }

        public void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            ProductFactory pf = new ProductFactory(BaseUrl, Account, Password);
            product a = pf.Get(3068);

            product b = createProduct();

            pf.Add(b);

        }

        public product createProduct()
        {
            product prod = new product();
            prod.active = 1;
            prod.additional_shipping_cost = (Decimal)0.00;
            prod.advanced_stock_management = 0;
            prod.available_date = "000-00-00";
            prod.available_for_order = 1;
            prod.cache_default_attribute = 0;
            prod.condition = "new";
            prod.date_add = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            prod.date_upd = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            prod.depth = (Decimal)0.000000;
            prod.ecotax = (Decimal)0.000000;
            prod.height = (Decimal)0.000000;
            prod.id = getID();
            prod.AddName(createAuxLanguage("Prueba 1"));

            return prod;
        }

        public Bukimedia.PrestaSharp.Entities.AuxEntities.language createAuxLanguage(string name)
        {
            Bukimedia.PrestaSharp.Entities.AuxEntities.language auxLang = new Bukimedia.PrestaSharp.Entities.AuxEntities.language();
            auxLang.Value = name;
            auxLang.id = 1;
            return auxLang;
        }

        public int getID()
        {
            int id = 0;
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString.ToString()))
            {
                MySqlCommand cmd = new MySqlCommand("SELECT `AUTO_INCREMENT` FROM  INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'rosamaria_shop2' AND TABLE_NAME = 'ps_product'; ", conn);
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    id = (int)rdr[0];
                }
            }
            return id;
        }
    }
}