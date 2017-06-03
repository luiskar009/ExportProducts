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
    /// Lógica de interacción para ExportOdacash.xaml
    /// </summary>
    public partial class ExportOdacash : Window
    {
        public ExportOdacash()
        {
            InitializeComponent();
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
            manufacturerBox.Items.Clear();
            manufacturerBox.SelectedIndex = manufacturerBox.Items.Add("-- Selecione el Fabricante de Prestashop --");
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString.ToString()))
            {
                MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT name FROM ps_category_lang WHERE id_shop = '1' AND id_category <> '1'", conn);
                conn.Open();
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        categoryBox.Items.Add(rdr[0].ToString());
                    }
                }
                MySqlCommand cmd2 = new MySqlCommand("SELECT name FROM ps_manufacturer", conn);
                using (MySqlDataReader rdr = cmd2.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        manufacturerBox.Items.Add(rdr[0].ToString());
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

            TextRange textRangeLarge = new TextRange(largeDesc.Document.ContentStart, largeDesc.Document.ContentEnd);
            TextRange textRangeShort = new TextRange(shortDesc.Document.ContentStart, shortDesc.Document.ContentEnd);

            product newProd = Library.createProduct(name.Text, yes.IsChecked, textRangeShort.Text, textRangeLarge.Text, price.Text, categoryBox.SelectedItem.ToString(), manufacturerBox.SelectedItem.ToString(), textStock.Text, textNoStock.Text);

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
                Library.insertInventory(name.Text, ConfigurationManager.AppSettings["idPrestashop"].ToString(), "0", idOdacash.Text);
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
                productsBox.SelectedIndex = 0;
                idOdacash.Text = "";
                name.Text = "";
                price.Text = "";
                shortDesc.Document.Blocks.Clear();
                largeDesc.Document.Blocks.Clear();
                categoryBox.SelectedIndex = 0;
                manufacturerBox.SelectedIndex = 0;
                textStock.Text = "";
                textNoStock.Text = "";
                imgBox.Items.Clear();
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
                for (int i = 0; ofd.FileNames.Length > i; i++)
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
