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
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            productsBox.Items.Clear();
            productsBox.SelectedIndex = productsBox.Items.Add("-- Selecione el producto de Odacash --");
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
            categoryBox.Items.Clear();
            categoryBox.SelectedIndex = categoryBox.Items.Add("-- Selecione la Categoria de Prestashop --");
            categoryBox2.SelectedIndex = categoryBox2.Items.Add("-- Selecione la Categoria de Prestashop --");
            manufacturerBox.Items.Clear();
            manufacturerBox.SelectedIndex = manufacturerBox.Items.Add("-- Selecione el Fabricante de Prestashop --");
            manufacturerBox2.SelectedIndex = manufacturerBox2.Items.Add("-- Selecione el Fabricante de Prestashop --");
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString.ToString()))
            {
                MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT name FROM ps_category_lang WHERE id_shop = '1' AND id_category <> '1'", conn);
                conn.Open();
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        categoryBox.Items.Add(rdr[0].ToString());
                        categoryBox2.Items.Add(rdr[0].ToString());
                    }
                }
                MySqlCommand cmd2 = new MySqlCommand("SELECT name FROM ps_manufacturer", conn);
                using (MySqlDataReader rdr = cmd2.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        manufacturerBox.Items.Add(rdr[0].ToString());
                        manufacturerBox2.Items.Add(rdr[0].ToString());
                    }
                }
            }
        }

        protected void productsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (productsBox.SelectedItem.ToString() == "-- Selecione el producto de Odacash --")
                return;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServer"].ConnectionString.ToString()))
            {
                SqlCommand cmd = new SqlCommand($"SELECT Articulo FROM VI_prueba_art WHERE DescripcionCorta = '{productsBox.SelectedItem.ToString()}';", conn);
                conn.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    idOdacash.Text = rdr[0].ToString();
                }
                SqlCommand cmd2 = new SqlCommand($"SELECT PrecioSalida FROM ocarttap WHERE Articulo = '{idOdacash.Text}';", conn);
                using (SqlDataReader rdr = cmd2.ExecuteReader())
                {
                    rdr.Read();
                    price.Text = rdr["PrecioSalida"].ToString();
                }
            }
            name.Text = productsBox.SelectedItem.ToString();
            textStock.Text = "ENVIO 48-72H";
            textNoStock.Text = "ENVIO 2 SEMANAS";
        }

        public void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            if (productsBox.SelectedItem.ToString() == "-- Selecione el producto de Odacash --")
            {
                System.Windows.MessageBox.Show("Elija un producto", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                return;
            }

            ProductFactory pf = new ProductFactory(ConfigurationManager.AppSettings["baseUrl"].ToString(), ConfigurationManager.AppSettings["accProduct"].ToString(), "");
            ImageFactory imf = new ImageFactory(ConfigurationManager.AppSettings["baseUrl"].ToString(), ConfigurationManager.AppSettings["accImages"].ToString(), "");
            product newProd = importProduct();

            try
            {
                pf.Add(newProd);
                if (imgBox.HasItems)
                {
                    for (int i = 0; imgBox.Items.Count > i; i++)
                    {
                        imf.AddProductImage((long)newProd.id, imgBox.Items[i].ToString());
                    }
                }
                insertInventory();
            }
            catch (Exception ex)
            {
                using (StreamWriter writer = new StreamWriter($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\ExportProducts.txt", true))
                {
                    writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                       "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                }
            }
            finally
            {
                name.Text = "";
                price.Text = "";
                categoryBox.SelectedIndex = 0;
                manufacturerBox.SelectedIndex = 0;
                textStock.Text = "";
                textNoStock.Text = "";
                imgBox.Items.Clear();
            }
        }

        public void btnInsert_Click2(object sender, RoutedEventArgs e)
        {
            ProductFactory pf = new ProductFactory(ConfigurationManager.AppSettings["baseUrl"].ToString(), ConfigurationManager.AppSettings["accProduct"].ToString(), "");
            ImageFactory imf = new ImageFactory(ConfigurationManager.AppSettings["baseUrl"].ToString(), ConfigurationManager.AppSettings["accImages"].ToString(), "");

            product a = pf.Get(2);

            product newProd = createProduct();
            try
            {
                pf.Add(newProd);
                if (imgBox2.HasItems)
                {
                    for(int i = 0; imgBox2.Items.Count > i; i++)
                    {
                        imf.AddProductImage((long)newProd.id, imgBox2.Items[i].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter writer = new StreamWriter($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\error.txt", true))
                {
                    writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                       "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                }
            }
            finally
            {
                name2.Text = "";
                price2.Text = "";
                categoryBox2.SelectedIndex = 0;
                manufacturerBox2.SelectedIndex = 0;
                textStock2.Text = "";
                textNoStock2.Text = "";
                imgBox2.Items.Clear();
            }
        }

        public void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                imgBox.Items.Clear();
                // Abre la ventana para buscar el archivo
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Imagenes(.jpg, .png, .gif)|*.jpg;*.png;*.gif|" + "Todos los ficheros |*.*";
                ofd.Multiselect = true;
                ofd.ShowDialog();

                // Guarda la ruta de los archivos
                for(int i = 0; ofd.FileNames.Length > i; i++)
                {
                    imgBox.Items.Add(ofd.FileNames[i]);
                }
                imgBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        public void btnOpenFile_Click2(object sender, RoutedEventArgs e)
        {
            try
            {
                imgBox2.Items.Clear();
                // Abre la ventana para buscar el archivo
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Imagenes(.jpg, .png, .gif)|*.jpg;*.png;*.gif|" + "Todos los ficheros |*.*";
                ofd.Multiselect = true;
                ofd.ShowDialog();

                // Guarda la ruta de los archivos
                for (int i = 0; ofd.FileNames.Length > i; i++)
                {
                    imgBox2.Items.Add(ofd.FileNames[i]);
                }
                imgBox2.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        public product importProduct()
        {
            product prod = new product();
            if (yes.IsChecked == true)
                prod.active = 1;
            if (no.IsChecked == true)
                prod.active = 0;
            //prod.additional_shipping_cost = (Decimal)0.00;
            prod.advanced_stock_management = 0;
            if (categoryBox.SelectedItem.ToString() != "-- Selecione la Categoria de Prestashop --")
                prod.associations.categories.Add(createAuxcategory(getCategoryID(categoryBox.SelectedItem.ToString())));
            prod.associations.stock_availables.Add(createAuxStockAvailable());
            prod.available_date = "0000-00-00";
            prod.available_for_order = 1;
            prod.available_later.Add(createAuxLanguage(textNoStock.Text));
            prod.available_now.Add(createAuxLanguage(textStock.Text));
            prod.cache_default_attribute = 0;
            prod.cache_has_attachments = 0;
            prod.cache_is_pack = 0;
            prod.condition = "new";
            prod.customizable = 0;
            prod.date_add = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            prod.date_upd = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            //prod.depth = (Decimal)0.000000;
            TextRange textRangeLarge = new TextRange(largeDesc.Document.ContentStart, largeDesc.Document.ContentEnd);
            prod.description.Add(createAuxLanguage(textRangeLarge.Text));
            TextRange textRangeShort = new TextRange(shortDesc.Document.ContentStart, shortDesc.Document.ContentEnd);
            prod.description_short.Add(createAuxLanguage(textRangeShort.Text));
            prod.ean13 = "";
            //prod.ecotax = (Decimal)0.000000;
            //prod.height = (Decimal)0.000000;
            prod.id = getID();
            prod.id_category_default = 2;
            prod.id_default_combination = null;
            prod.id_default_image = null;
            if (manufacturerBox.SelectedItem.ToString() != "-- Selecione el Fabricante de Prestashop --")
                prod.id_manufacturer = getManufacturerID(manufacturerBox.SelectedItem.ToString());
            prod.id_product_redirected = 0;
            prod.id_shop_default = 1;
            prod.id_supplier = 0;
            prod.id_tax_rules_group = 1;
            prod.indexed = 1;
            prod.is_virtual = 0;
            prod.link_rewrite.Add(createAuxLanguage((name.Text.ToLower().Replace(" ", "-"))));
            prod.location = "";
            prod.meta_description.Add(createAuxLanguage(""));
            prod.meta_keywords.Add(createAuxLanguage(""));
            prod.meta_title.Add(createAuxLanguage(""));
            prod.minimal_quantity = 1;
            prod.name.Add(createAuxLanguage(name.Text));
            prod.on_sale = 0;
            prod.online_only = 0;
            prod.price = Decimal.Round((Decimal.Parse(price.Text) / (Decimal)1.21), 6);
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

        public product createProduct()
        {
            product prod = new product();
            if (yes2.IsChecked == true)
                prod.active = 1;
            if (no2.IsChecked == true)
                prod.active = 0;
            //prod.additional_shipping_cost = (Decimal)0.00;
            prod.advanced_stock_management = 0;
            if (categoryBox2.SelectedItem.ToString() != "-- Selecione la Categoria de Prestashop --")
                prod.associations.categories.Add(createAuxcategory(getCategoryID(categoryBox2.SelectedItem.ToString())));
            prod.associations.stock_availables.Add(createAuxStockAvailable());
            prod.available_date = "0000-00-00";
            prod.available_for_order = 1;
            prod.available_later.Add(createAuxLanguage(textNoStock2.Text));
            prod.available_now.Add(createAuxLanguage(textStock2.Text));
            prod.cache_default_attribute = 0;
            prod.cache_has_attachments = 0;
            prod.cache_is_pack = 0;
            prod.condition = "new";
            prod.customizable = 0;
            prod.date_add = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            prod.date_upd = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            //prod.depth = (Decimal)0.000000;
            TextRange textRangeLarge = new TextRange(largeDesc2.Document.ContentStart, largeDesc2.Document.ContentEnd);
            prod.description.Add(createAuxLanguage(textRangeLarge.Text));
            TextRange textRangeShort = new TextRange(shortDesc2.Document.ContentStart, shortDesc2.Document.ContentEnd);
            prod.description_short.Add(createAuxLanguage(textRangeShort.Text));
            prod.ean13 = "";
            //prod.ecotax = (Decimal)0.000000;
            //prod.height = (Decimal)0.000000;
            prod.id = getID();
            prod.id_category_default = 2;
            prod.id_default_combination = null;
            prod.id_default_image = null;
            if (manufacturerBox2.SelectedItem.ToString() != "-- Selecione el Fabricante de Prestashop --")
                prod.id_manufacturer = getManufacturerID(manufacturerBox2.SelectedItem.ToString());
            prod.id_product_redirected = 0;
            prod.id_shop_default = 1;
            prod.id_supplier = 0;
            prod.id_tax_rules_group = 1;
            prod.indexed = 1;
            prod.is_virtual = 0;
            prod.link_rewrite.Add(createAuxLanguage((name2.Text.ToLower().Replace(" ", "-"))));
            prod.location = "";
            prod.meta_description.Add(createAuxLanguage(""));
            prod.meta_keywords.Add(createAuxLanguage(""));
            prod.meta_title.Add(createAuxLanguage(""));
            prod.minimal_quantity = 1;
            prod.name.Add(createAuxLanguage(name2.Text));
            prod.on_sale = 0;
            prod.online_only = 0;
            prod.price = Decimal.Round((Decimal.Parse(price2.Text) / (Decimal)1.21), 6);
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
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString.ToString()))
            {
                MySqlCommand cmd = new MySqlCommand("SELECT `AUTO_INCREMENT` FROM  INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'rosamaria_shop2' AND TABLE_NAME = 'ps_product'; ", conn);
                conn.Open();
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    ConfigurationManager.AppSettings["idPrestashop"] = rdr[0].ToString();
                }
            }
            return Int32.Parse(ConfigurationManager.AppSettings["idPrestashop"].ToString());
        }

        public int getCategoryID(string category)
        {
            int id = 2;
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString.ToString()))
            {
                MySqlCommand cmd = new MySqlCommand($"SELECT DISTINCT id_category FROM ps_category_lang WHERE name = '{category}'; ", conn);
                conn.Open();
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    id = Int32.Parse(rdr[0].ToString());
                }
            }
            return id;
        }

        public int getManufacturerID(string manufacturer)
        {
            int id = 2;
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString.ToString()))
            {
                MySqlCommand cmd = new MySqlCommand($"SELECT DISTINCT id_manufacturer FROM ps_manufacturer WHERE name = '{manufacturer}'; ", conn);
                conn.Open();
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    id = Int32.Parse(rdr[0].ToString());
                }
            }
            return id;
        }

        public void insertInventory()
        {
            string Producto = name.Text;
            string id_product = ConfigurationManager.AppSettings["idPrestashop"].ToString();
            string id_product_attribute = "0";
            string Articulo = idOdacash.Text;

            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand($"INSERT INTO InventarioTablas (Producto, id_product, id_product_attribute, Articulo) VALUES ('{Producto}', '{id_product}', '{id_product_attribute}', '{Articulo}')", conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        private void idOdacash_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (idOdacash.Text == "")
                return;
            string articulo = "";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServer"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand($"SELECT DescripcionCorta FROM VI_prueba_art WHERE Articulo = '{idOdacash.Text}'", conn);
                conn.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    if (rdr.HasRows)
                        articulo = rdr[0].ToString();
                    else
                    {
                        name.Clear();
                        price.Clear();
                        productsBox.SelectedIndex = 0;
                        return;
                    }
                }
            }
            productsBox.SelectedIndex = productsBox.Items.IndexOf(articulo);
        }
    }
}