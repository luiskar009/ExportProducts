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
using static ExportProducts.Library;
using System.ComponentModel;

namespace ExportProducts
{
    /// <summary>
    /// Lógica de interacción para CreateCombination.xaml
    /// </summary>
    public partial class CreateCombination : Window
    {
        public CreateCombination()
        {
            InitializeComponent();

            // Initializes the comboBox
            productsBox.SelectedIndex = productsBox.Items.Add("-- Seleccione el producto de Prestashop --");
            imageBox.SelectedIndex = imageBox.Items.Add("-- Seleccione una imagen --");
            attributeBox.SelectedIndex = attributeBox.Items.Add("-- Seleccione un atributo para la combinacion --");
            attributeBox2.SelectedIndex = attributeBox2.Items.Add("-- Seleccione un atributo para la combinacion --");
            odacashBox.SelectedIndex = odacashBox.Items.Add("-- Seleccione un producto de Odacash--");

            // Fill the comboBoxs with Prestashop products names and attribute names
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString.ToString()))
            {
                MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT name FROM ps_product_lang ORDER BY name", conn);
                conn.Open();
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        productsBox.Items.Add(rdr[0].ToString());
                    }
                }
                MySqlCommand cmd2 = new MySqlCommand("SELECT name FROM ps_attribute_lang ORDER BY name", conn);
                using (MySqlDataReader rdr = cmd2.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        attributeBox.Items.Add(rdr[0].ToString());
                        attributeBox2.Items.Add(rdr[0].ToString());
                    }
                }
            }

            // Fill the comboBox with Odacash products
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServer"].ConnectionString.ToString()))
            {
                SqlCommand cmd = new SqlCommand("SELECT DescripcionCorta FROM VI_prueba_art ORDER BY DescripcionCorta;", conn);
                conn.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        odacashBox.Items.Add(rdr[0].ToString());
                    }
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                            ///                                               
        ///                         Updates the Prestashop ID and images IDs                           ///
        ///                     when change the selected product in the comboBox                       ///               
        ///                                                                                            ///                                    
        //////////////////////////////////////////////////////////////////////////////////////////////////

        protected void productsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If no product is selected do nothing
            if (productsBox.SelectedItem.ToString() == "-- Seleccione el producto de Prestashop --")
                return;

            // Look for the product id in BBDD and update the value in the textBox
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString.ToString()))
            {
                MySqlCommand cmd = new MySqlCommand($"SELECT id_product FROM ps_product_lang WHERE name = '{productsBox.SelectedItem.ToString()}';", conn);
                conn.Open();
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    idPrestashop.Text = rdr[0].ToString();
                }

                // Update image comboBox with the selected product images ids
                img.Source = null;
                imageBox.Items.Clear();
                imageBox.SelectedIndex = imageBox.Items.Add("-- Seleccione una imagen --");
                ProductFactory pf = new ProductFactory(ConfigurationManager.AppSettings["baseUrl"].ToString(), ConfigurationManager.AppSettings["accProduct"].ToString(), "");
                ImageFactory imf = new ImageFactory(ConfigurationManager.AppSettings["baseUrl"].ToString(), ConfigurationManager.AppSettings["accImages"].ToString(), "");
                List<Bukimedia.PrestaSharp.Entities.FilterEntities.declination> imgIDs = imf.GetProductImages(Int64.Parse(idPrestashop.Text));
                foreach(Bukimedia.PrestaSharp.Entities.FilterEntities.declination element in imgIDs)
                {
                    imageBox.Items.Add(element.id);
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                            ///                                               
        ///                 Update Odacash ID Textbox when chages the selected product                 ///               
        ///                                                                                            ///                                    
        //////////////////////////////////////////////////////////////////////////////////////////////////

        protected void odacashBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If no product is selected do nothing
            if (odacashBox.SelectedItem.ToString() == "-- Seleccione un producto de Odacash--")
                return;

            // Look fot the id for the selected product in BBDD and update the value in the textBox
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServer"].ConnectionString.ToString()))
            {
                SqlCommand cmd = new SqlCommand($"SELECT Articulo FROM VI_prueba_art WHERE DescripcionCorta = '{odacashBox.SelectedItem.ToString()}';", conn);
                conn.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    idOdacash.Text = rdr[0].ToString();
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                            ///                                               
        ///                              Display the selected image                                    ///               
        ///                                                                                            ///                                    
        //////////////////////////////////////////////////////////////////////////////////////////////////

        protected void imageBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If the comboBox is empty do nothing
            if (imageBox.Items.Count == 0)
                return;
            // If no image is selected do nothing
            if (imageBox.SelectedItem.ToString() == "-- Seleccione una imagen --")
                return;
            // Look for the image and display it
            ImageFactory imf = new ImageFactory(ConfigurationManager.AppSettings["baseUrl"].ToString(), ConfigurationManager.AppSettings["accImages"].ToString(), "");
            byte[] imgByte = imf.GetProductImage(Int64.Parse(idPrestashop.Text), Int64.Parse(imageBox.SelectedItem.ToString()));
            img.Source = LoadImage(imgByte);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                            ///                                               
        ///                                Create the combination                                      ///               
        ///                                                                                            ///                                    
        //////////////////////////////////////////////////////////////////////////////////////////////////

        public void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            // If no product is selected display a messageBox 
            if (productsBox.SelectedItem.ToString() == "-- Seleccione el producto de Prestashop --")
            {
                System.Windows.MessageBox.Show("Elija un producto", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            // If no attribute is selected  display a messageBox 
            if ((attributeBox.SelectedItem.ToString() == "-- Seleccione un atributo para la combinacion --") && (attributeBox2.SelectedItem.ToString() == "-- Seleccione un atributo para la combinacion --"))
            {
                System.Windows.MessageBox.Show("Elija minimo un atributo para la combinacion", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            try
            {
                CombinationFactory cf = new CombinationFactory(ConfigurationManager.AppSettings["baseUrl"].ToString(), ConfigurationManager.AppSettings["accCombination"].ToString(), "");
                // Create a new combination
                combination newComb = createCombination(Int32.Parse(idPrestashop.Text), getAttributeID(attributeBox.SelectedItem.ToString()), getAttributeID(attributeBox2.SelectedItem.ToString()), price.Text, imageBox.SelectedItem.ToString());
            
                // Add the combination to the product
                cf.Add(newComb);

                // If an Odacash product is selected create a link to sync the  stock between Odacash and Prestashop
                if(idOdacash.Text != "")
                    Library.insertInventory(productsBox.SelectedItem.ToString(), idPrestashop.Text , newComb.id.ToString(), idOdacash.Text);
            }
            catch (Exception ex)
            {
                // Save the error message in a txt file
                using (StreamWriter writer = new StreamWriter(@"C:\ExportProduct\ExportProducts.txt", true))
                {
                    writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                       "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                }
            }
            finally
            {
                // Restart all the files when the execution is over
                productsBox.SelectedIndex = 0;
                idPrestashop.Text = "";
                attributeBox.SelectedIndex = 0;
                attributeBox2.SelectedIndex = 0;
                odacashBox.SelectedIndex = 0;
                price.Text = "";
                img.Source = null;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                            ///                                               
        ///                         Event to change focus when click outside                           ///               
        ///                                                                                            ///                                    
        //////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                            ///                                               
        ///              Event tu update the Prestashop comboBox when TextBox lost focus               ///               
        ///                                                                                            ///                                    
        //////////////////////////////////////////////////////////////////////////////////////////////////

        private void idPrestashop_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // If the textbox is empty do nothing
            if (idPrestashop.Text == "")
                return;
            string articulo = "";

            // Look for the product name for the id inserted in the textBox
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString.ToString()))
            {
                MySqlCommand cmd = new MySqlCommand($"SELECT name FROM ps_product_lang WHERE id_product = '{idPrestashop.Text}'", conn);
                conn.Open();
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    // If the products exist save the name
                    if (rdr.HasRows)
                        articulo = rdr[0].ToString();

                    // If not exist select the index and finish the execution
                    else
                    {
                        //name.Clear();
                        //price.Clear();
                        productsBox.SelectedIndex = 0;
                        return;
                    }
                }
            }

            // Update the comboBox item selection with the product name
            productsBox.SelectedIndex = productsBox.Items.IndexOf(articulo);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                            ///                                               
        ///              Event tu update the Odacash comboBox when TextBox lost focus                  ///               
        ///                                                                                            ///                                    
        //////////////////////////////////////////////////////////////////////////////////////////////////

        private void idOdacash_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // If the textBox is empty do nothing
            if (idOdacash.Text == "")
                return;
            string articulo = "";

            // Look for the product name for the id inserted in the textbox
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServer"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand($"SELECT DescripcionCorta FROM VI_prueba_art WHERE Articulo = '{idOdacash.Text}'", conn);
                conn.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    // If the products exist save the name
                    if (rdr.HasRows)
                        articulo = rdr[0].ToString();

                    // If not exist select the index and finish the execution
                    else
                    {
                        idOdacash.Text = "";
                        odacashBox.SelectedIndex = 0;
                        return;
                    }
                }
            }

            // Update the comboBox item selection with the product name
            odacashBox.SelectedIndex = odacashBox.Items.IndexOf(articulo);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                            ///                                               
        ///                         Convert an image in bytes to BitmapImage                           ///               
        ///                                                                                            ///                                    
        //////////////////////////////////////////////////////////////////////////////////////////////////

        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (MemoryStream mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
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

        private void btnEditMB_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
