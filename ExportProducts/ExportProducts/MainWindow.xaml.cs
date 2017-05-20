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
            WindowStartupLocation = WindowStartupLocation.CenterScreen; 
            productsBox.Items.Clear();
            productsBox.SelectedIndex = productsBox.Items.Add("-- Selecione la Base de Datos --");
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServer"].ConnectionString.ToString()))
            {
                SqlCommand cmd = new SqlCommand("SELECT DescripcionCorta FROM VI_prueba_art ORDER BY DescripcionCorta;", conn);
                conn.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        productsBox.Items.Add(rdr[0].ToString());
                    }
                }
            }
        }

        public void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            ProductFactory pf = new ProductFactory(BaseUrl, Account, Password);
            //product a = pf.Get(3084);
            product b = createProduct();
            try
            {
                pf.Add(b);
            }
            catch(Exception ex)
            {
                using (StreamWriter writer = new StreamWriter($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\error.txt", true))
                {
                    writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                       "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                }
            }
        }

        public product createProduct()
        {
            product prod = new product();
            prod.active = 1;
            //prod.additional_shipping_cost = (Decimal)0.00;
            prod.advanced_stock_management = 0;
            prod.associations.categories.Add(createAuxcategory(2));
            prod.associations.stock_availables.Add(createAuxStockAvailable());
            prod.available_date = "0000-00-00";
            prod.available_for_order = 1;
            prod.available_later.Add(createAuxLanguage(""));
            prod.available_now.Add(createAuxLanguage(""));
            prod.cache_default_attribute = 0;
            prod.cache_has_attachments = 0;
            prod.cache_is_pack = 0;
            prod.condition = "new";
            prod.customizable = 0;
            prod.date_add = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            prod.date_upd = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            //prod.depth = (Decimal)0.000000;
            prod.description.Add(createAuxLanguage(""));
            prod.description_short.Add(createAuxLanguage(""));
            prod.ean13 = "";
            //prod.ecotax = (Decimal)0.000000;
            //prod.height = (Decimal)0.000000;
            prod.id = getID();
            prod.id_category_default = 2;
            prod.id_default_combination = null;
            prod.id_default_image = null;
            prod.id_manufacturer = 0;
            prod.id_product_redirected = 0;
            prod.id_shop_default = 1;
            prod.id_supplier = 0;
            prod.id_tax_rules_group = 1;
            prod.indexed = 1;
            prod.is_virtual = 0;
            prod.link_rewrite.Add(createAuxLanguage("prueba-1"));
            prod.location = "";
            prod.meta_description.Add(createAuxLanguage(""));
            prod.meta_keywords.Add(createAuxLanguage(""));
            prod.meta_title.Add(createAuxLanguage(""));
            prod.minimal_quantity = 1;
            prod.name.Add(createAuxLanguage("Prueba 1"));
            prod.on_sale = 0;
            prod.online_only = 0;
            prod.price = (Decimal)16.528926;
            prod.quantity_discount = 0;
            prod.redirect_type = "404";
            prod.reference = "";
            prod.show_price = 1;
            prod.supplier_reference = "";
            prod.text_fields = 0;
            //prod.unit_price_ratio = (Decimal)0.000000;
            prod.unity = "";
            prod.upc = "";
            prod.uploadable_files = 0;
            prod.visibility = "both";
            //prod.weight = (Decimal)0.000000;
            //prod.wholesale_price = (Decimal)0.000000;
            //prod.width = (Decimal)0.000000;

            return prod;
        }

        public Bukimedia.PrestaSharp.Entities.AuxEntities.language createAuxLanguage(string message)
        {
            Bukimedia.PrestaSharp.Entities.AuxEntities.language auxLang = new Bukimedia.PrestaSharp.Entities.AuxEntities.language();
            auxLang.Value = message;
            auxLang.id = 1;
            return auxLang;
        }

        public Bukimedia.PrestaSharp.Entities.AuxEntities.category createAuxcategory(long category)
        {
            Bukimedia.PrestaSharp.Entities.AuxEntities.category auxCat = new Bukimedia.PrestaSharp.Entities.AuxEntities.category();
            auxCat.id = category;
            return auxCat;
        }

        public Bukimedia.PrestaSharp.Entities.AuxEntities.stock_available createAuxStockAvailable()
        {
            Bukimedia.PrestaSharp.Entities.AuxEntities.stock_available stockAva = new Bukimedia.PrestaSharp.Entities.AuxEntities.stock_available();
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString.ToString()))
            {
                MySqlCommand cmd = new MySqlCommand("SELECT `AUTO_INCREMENT` FROM  INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'rosamaria_shop2' AND TABLE_NAME = 'ps_stock_available'; ", conn);
                conn.Open();
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    stockAva.id = Int32.Parse(rdr[0].ToString());
                }
            }
            stockAva.id_product_attribute = 0;
            return stockAva;
        }

        public int getID()
        {
            int id = 0;
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString.ToString()))
            {
                MySqlCommand cmd = new MySqlCommand("SELECT `AUTO_INCREMENT` FROM  INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'rosamaria_shop2' AND TABLE_NAME = 'ps_product'; ", conn);
                conn.Open();
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    id = Int32.Parse(rdr[0].ToString());
                }
            }
            return id;
        }
    }
}