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
using System.ComponentModel;
using static ExportProducts.Library;

namespace ExportProducts
{
    /// <summary>
    /// Lógica de interacción para CreateProduct.xaml
    /// </summary>
    public partial class CreateProduct : Window
    {
        public CreateProduct()
        {
            InitializeComponent();
            // Inicialized the comboBoxs
            categoryBox.SelectedIndex = categoryBox.Items.Add("-- Selecione la Categoria de Prestashop --");
            manufacturerBox.SelectedIndex = manufacturerBox.Items.Add("-- Selecione el Fabricante de Prestashop --");
            productsBox.SelectedIndex = productsBox.Items.Add("-- Selecione un producto para editarlo --");

            // Fill the comboBox with products and manufacturers
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString.ToString()))
            {
                MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT name FROM ps_category_lang WHERE id_shop = '1' AND id_category <> '1'", conn);
                cmd.CommandTimeout = 5000;
                conn.Open();
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        categoryBox.Items.Add(rdr[0].ToString());
                    }
                }
                MySqlCommand cmd2 = new MySqlCommand("SELECT name FROM ps_manufacturer", conn);
                cmd2.CommandTimeout = 5000;
                using (MySqlDataReader rdr = cmd2.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        manufacturerBox.Items.Add(rdr[0].ToString());
                    }
                }
                MySqlCommand cmd3 = new MySqlCommand("SELECT DISTINCT name FROM ps_product_lang", conn);
                cmd3.CommandTimeout = 5000;
                using (MySqlDataReader rdr = cmd3.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        productsBox.Items.Add(rdr[0].ToString());
                    }
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                            ///                                                                                         
        ///         Updates the Prestashop ID when change the selected product in the comboBox         ///               
        ///                                                                                            ///                                    
        //////////////////////////////////////////////////////////////////////////////////////////////////

        protected void productsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (productsBox.SelectedItem.ToString() == "-- Selecione un producto para editarlo --")
                return;
            string id = "";
            shortDesc.Document.Blocks.Clear();
            largeDesc.Document.Blocks.Clear();
            ProductFactory pf = new ProductFactory(ConfigurationManager.AppSettings["baseUrl"].ToString(), ConfigurationManager.AppSettings["accProduct"].ToString(), "");
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString.ToString()))
            {
                MySqlCommand cmd = new MySqlCommand($"SELECT id_product FROM ps_product_lang WHERE name = '{productsBox.SelectedItem.ToString()}'", conn);
                conn.Open();
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    id = rdr[0].ToString();
                }
            }
            product prod = pf.Get(Int64.Parse(id));
            name.Text = prod.name[0].Value;
            if (prod.active == 1)
                yes.IsChecked = true;
            else
                no.IsChecked = true;
            shortDesc.AppendText(prod.description_short[0].Value);
            largeDesc.AppendText(prod.description[0].Value);
            price.Text = $"{Decimal.Round(prod.price * (Decimal)1.21, 6)}";
            textNoStock.Text = prod.available_later[0].Value;
            textStock.Text = prod.available_now[0].Value;
            string idCat = prod.associations.categories[0].id.ToString();
            string idMan = prod.id_manufacturer.ToString();
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString.ToString()))
            {
                MySqlCommand cmd = new MySqlCommand($"SELECT name FROM ps_category_lang WHERE id_category = '{idCat}'", conn);
                conn.Open();
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        rdr.Read();
                        categoryBox.SelectedItem = rdr[0].ToString();
                    }
                }
                MySqlCommand cmd2 = new MySqlCommand($"SELECT name FROM ps_manufacturer WHERE id_manufacturer = '{idMan}'", conn);
                using (MySqlDataReader rdr = cmd2.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        rdr.Read();
                        manufacturerBox.SelectedItem = rdr[0].ToString();
                    }
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                            ///                                               
        ///                             Click event to Create a Product                                ///               
        ///                                                                                            ///                                    
        //////////////////////////////////////////////////////////////////////////////////////////////////

        public void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            ProductFactory pf = new ProductFactory(ConfigurationManager.AppSettings["baseUrl"].ToString(), ConfigurationManager.AppSettings["accProduct"].ToString(), "");
            ImageFactory imf = new ImageFactory(ConfigurationManager.AppSettings["baseUrl"].ToString(), ConfigurationManager.AppSettings["accImages"].ToString(), "");
            TextRange textRangeLarge = new TextRange(largeDesc.Document.ContentStart, largeDesc.Document.ContentEnd);
            TextRange textRangeShort = new TextRange(shortDesc.Document.ContentStart, shortDesc.Document.ContentEnd);

            try
            {
                // Create new product
                product newProd = createProduct(name.Text, yes.IsChecked, textRangeShort.Text, textRangeLarge.Text, price.Text, categoryBox.SelectedItem.ToString(), manufacturerBox.SelectedItem.ToString(), textStock.Text, textNoStock.Text);
            
                // Insert it to the web
                pf.Add(newProd);

                // If the product have images insert it too.
                if (imgBox.HasItems)
                {
                    for (int i = 0; imgBox.Items.Count > i; i++)
                    {
                        imf.AddProductImage((long)newProd.id, imgBox.Items[i].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                // Display errors in a txt and alert the user with a messageBox
                using (StreamWriter writer = new StreamWriter($@"C:\ExportProduct\CreateProduct.txt", true))
                {
                    writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                       "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                    System.Windows.MessageBox.Show("Se ha pruducido un error. Puede ver el contenido del mismo en el log del programa", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                }
            }
            finally
            {
                // Restart the fields when the executions ends
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

        public void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if(productsBox.SelectedItem.ToString() == "-- Selecione un producto para editarlo --")
            {
                System.Windows.MessageBox.Show("Seleccione el producto a editar", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            ProductFactory pf = new ProductFactory(ConfigurationManager.AppSettings["baseUrl"].ToString(), ConfigurationManager.AppSettings["accProduct"].ToString(), "");
            ImageFactory imf = new ImageFactory(ConfigurationManager.AppSettings["baseUrl"].ToString(), ConfigurationManager.AppSettings["accImages"].ToString(), "");
            TextRange textRangeLarge = new TextRange(largeDesc.Document.ContentStart, largeDesc.Document.ContentEnd);
            TextRange textRangeShort = new TextRange(shortDesc.Document.ContentStart, shortDesc.Document.ContentEnd);
            string id = "";
            string idcat = "";
            string idman = "";

            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString.ToString()))
            {
                MySqlCommand cmd = new MySqlCommand($"SELECT id_product FROM ps_product_lang WHERE name = '{productsBox.SelectedItem.ToString()}'", conn);
                conn.Open();
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    id = rdr[0].ToString();
                }
                MySqlCommand cmd2 = new MySqlCommand($"SELECT id_product FROM ps_product_lang WHERE id_product = '{id}'", conn);
                using (MySqlDataReader rdr = cmd2.ExecuteReader())
                {
                    rdr.Read();
                    idcat = rdr[0].ToString();
                }
                MySqlCommand cmd3 = new MySqlCommand($"SELECT id_manufacturer FROM ps_manufacturer WHERE name = '{manufacturerBox.SelectedItem.ToString()}'", conn);
                using (MySqlDataReader rdr = cmd3.ExecuteReader())
                {
                    rdr.Read();
                    idman = rdr[0].ToString();
                }
            }

            try
            {
                //// Edit product
                //product editProd = editProduct(pf.Get(Int64.Parse(id)), name.Text, yes.IsChecked, textRangeShort.Text, textRangeLarge.Text, price.Text, categoryBox.SelectedItem.ToString(), manufacturerBox.SelectedItem.ToString(), textStock.Text, textNoStock.Text);
                //// Insert it to the web
                //pf.Update(pf.Get(Int64.Parse(id)));

                editProductFromBD(id, name.Text, yes.IsChecked, textRangeShort.Text, textRangeLarge.Text, price.Text, idcat, idman, textStock.Text, textNoStock.Text);

                // If the product have images insert it too.
                if (imgBox.HasItems)
                {
                    for (int i = 0; imgBox.Items.Count > i; i++)
                    {
                         //imf.AddProductImage((long)editProd.id, imgBox.Items[i].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                // Display errors in a txt and alert the user with a messageBox
                using (StreamWriter writer = new StreamWriter($@"C:\ExportProducts\CreateProduct.txt", true))
                {
                    writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                       "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                    System.Windows.MessageBox.Show("Se ha producido un error. Puede ver el contenido del mismo en el log del programa", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                }
            }
            finally
            {
                // Restart the fields when the executions ends
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

        //////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                            ///                                               
        ///                    Click event to display a window to look for images                      ///               
        ///                                                                                            ///                                    
        //////////////////////////////////////////////////////////////////////////////////////////////////

        public void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                imgBox.Items.Clear();
                // Open a window to look for a image
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Imagenes(.jpg, .png, .gif)|*.jpg;*.png;*.gif|" + "Todos los ficheros |*.*";
                ofd.Multiselect = true;
                ofd.ShowDialog();

                // Save the path of the images
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

        //////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                            ///                                        
        ///              Event to prevent MainWindow to minimize when closes this window               ///         
        ///                                                                                            ///                                        
        //////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void OnClosing(CancelEventArgs e)
        {
            this.Owner = null;
        }
    }
}
