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
using System.ComponentModel;
using System.Data;

namespace ExportProducts
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class InsertInventory : Window
    {
        DataTable dt, dt2;
        MySqlDataAdapter sda;

        public InsertInventory()
        {
            InitializeComponent();

            articuloBoxMB.Items.Clear();
            articuloBoxMB.SelectedIndex = articuloBoxMB.Items.Add("-- Selecione el producto de Odacash --");
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServer"].ConnectionString.ToString()))
            {
                SqlCommand cmd = new SqlCommand("SELECT DescripcionCorta FROM VI_prueba_art ORDER BY DescripcionCorta;", conn);
                conn.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        articuloBoxMB.Items.Add(rdr[0].ToString());
                        articuloBoxTPB.Items.Add(rdr[0].ToString());
                    }
                }
            }
            idProductAttributeBox.Items.Clear();
            idProductAttributeBox.SelectedIndex = idProductAttributeBox.Items.Add("-- Selecione un Atributo --");
            productsBoxMB.Items.Clear();
            productsBoxMB.SelectedIndex = productsBoxMB.Items.Add("-- Selecione el producto de Prestashop --");
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString.ToString()))
            {
                MySqlCommand cmd2 = new MySqlCommand("SELECT DISTINCT name FROM ps_product_lang ORDER BY name", conn);
                conn.Open();
                using (MySqlDataReader rdr = cmd2.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        productsBoxMB.Items.Add(rdr[0].ToString());
                    }
                }

                // Look for products in the database and fill it in the comboBox
                MySqlCommand cmd = new MySqlCommand("SELECT Producto, id_product, id_product_attribute, Articulo, Baja FROM InventarioTablas ORDER BY Producto", conn);
                sda = new MySqlDataAdapter(cmd);
                dt = new DataTable("InventarioTablas");
                sda.Fill(dt);
                tablaSQL.ItemsSource = dt.DefaultView;
            }

            articuloBoxTPB.Items.Clear();
            articuloBoxTPB.SelectedIndex = articuloBoxMB.Items.Add("-- Selecione el producto de Odacash --");
            idProductAttributeBoxTPB.Items.Clear();
            idProductAttributeBoxTPB.SelectedIndex = idProductAttributeBoxTPB.Items.Add("-- Selecione un Atributo --");
            productsBoxTPB.Items.Clear();
            productsBoxTPB.SelectedIndex = productsBoxTPB.Items.Add("-- Selecione el producto de Prestashop --");
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB2"].ConnectionString.ToString()))
            {
                MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT name FROM ps_product_lang ORDER BY name", conn);
                conn.Open();
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        productsBoxTPB.Items.Add(rdr[0].ToString());
                    }
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                            ///                                                                                         
        ///         Updates the Prestashop ID when change the selected product in the comboBox         ///               
        ///                                                                                            ///                                    
        //////////////////////////////////////////////////////////////////////////////////////////////////

        protected void productsBoxMB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (productsBoxMB.SelectedItem.ToString() == "-- Selecione el producto de Prestashop --")
                return;
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString.ToString()))
            {
                MySqlCommand cmd = new MySqlCommand($"SELECT id_product FROM ps_product_lang WHERE name = '{productsBoxMB.SelectedItem.ToString()}';", conn);
                conn.Open();
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    boxIdProductMB.Text = rdr[0].ToString();
                }
                MySqlCommand cmd2 = new MySqlCommand($"SELECT id_product_attribute FROM ps_product_attribute WHERE id_product = '{boxIdProductMB.Text}';", conn);
                using (MySqlDataReader rdr = cmd2.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        idProductAttributeBox.Items.Clear();
                        idProductAttributeBox.SelectedIndex = idProductAttributeBox.Items.Add("-- Selecione un Atributo --");
                        while (rdr.Read())
                        {
                            idProductAttributeBox.Items.Add(rdr[0].ToString());
                        }
                    }
                    else
                    {
                        boxProductoMB.Text = "";
                        productsBoxMB.SelectedItem = 0;
                    }
                }
            }
            boxProductoMB.Text = productsBoxMB.SelectedItem.ToString();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                            ///                                                                                         
        ///         Updates the Odacash ID when change the selected product in the comboBox            ///               
        ///                                                                                            ///                                    
        //////////////////////////////////////////////////////////////////////////////////////////////////

        protected void articuloBoxMB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (articuloBoxMB.SelectedItem.ToString() == "-- Selecione el producto de Odacash --")
                return;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServer"].ConnectionString.ToString()))
            {
                SqlCommand cmd = new SqlCommand($"SELECT Articulo FROM VI_prueba_art WHERE DescripcionCorta = '{articuloBoxMB.SelectedItem.ToString()}';", conn);
                conn.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    boxArticuloMB.Text = rdr[0].ToString();
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                            ///                                                                                         
        ///         Updates the attribute ID when change the selected product in the comboBox          ///               
        ///                                                                                            ///                                    
        //////////////////////////////////////////////////////////////////////////////////////////////////

        protected void idProductAttributeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(idProductAttributeBox.HasItems)
            {
                if (idProductAttributeBox.SelectedItem.ToString() == "-- Selecione un Atributo --")
                {
                    boxAttributeMB.Text = "0";
                    return;
                }
                boxAttributeMB.Text = idProductAttributeBox.SelectedItem.ToString();
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                            ///                                                                                         
        ///         Updates the Prestashop ID when change the selected product in the comboBox         ///               
        ///                                                                                            ///                                    
        //////////////////////////////////////////////////////////////////////////////////////////////////

        protected void productsBoxTPB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (productsBoxTPB.SelectedItem.ToString() == "-- Selecione el producto de Prestashop --")
                return;
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB2"].ConnectionString.ToString()))
            {
                MySqlCommand cmd = new MySqlCommand($"SELECT id_product FROM ps_product_lang WHERE name = '{productsBoxTPB.SelectedItem.ToString()}';", conn);
                conn.Open();
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    boxIdProductTPB.Text = rdr[0].ToString();
                }
                MySqlCommand cmd2 = new MySqlCommand($"SELECT id_product_attribute FROM ps_product_attribute WHERE id_product = '{boxIdProductTPB.Text}';", conn);
                using (MySqlDataReader rdr = cmd2.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        idProductAttributeBoxTPB.Items.Clear();
                        idProductAttributeBoxTPB.SelectedIndex = idProductAttributeBoxTPB.Items.Add("-- Selecione un Atributo --");
                        while (rdr.Read())
                        {
                            idProductAttributeBoxTPB.Items.Add(rdr[0].ToString());
                        }
                    }
                    else
                    {
                        boxProductoTPB.Text = "";
                        productsBoxTPB.SelectedItem = 0;
                    }
                }
            }
            boxProductoTPB.Text = productsBoxTPB.SelectedItem.ToString();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                            ///                                                                                         
        ///         Updates the Odacash ID when change the selected product in the comboBox            ///               
        ///                                                                                            ///                                    
        //////////////////////////////////////////////////////////////////////////////////////////////////

        protected void articuloBoxTPB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (articuloBoxTPB.SelectedItem.ToString() == "-- Selecione el producto de Odacash --")
                return;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServer"].ConnectionString.ToString()))
            {
                SqlCommand cmd = new SqlCommand($"SELECT Articulo FROM VI_prueba_art WHERE DescripcionCorta = '{articuloBoxTPB.SelectedItem.ToString()}';", conn);
                conn.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    boxArticuloTPB.Text = rdr[0].ToString();
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                            ///                                                                                         
        ///         Updates the attribute ID when change the selected product in the comboBox          ///               
        ///                                                                                            ///                                    
        //////////////////////////////////////////////////////////////////////////////////////////////////

        protected void idProductAttributeBoxTPB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (idProductAttributeBoxTPB.HasItems)
            {
                if (idProductAttributeBoxTPB.SelectedItem.ToString() == "-- Selecione un Atributo --")
                {
                    boxAttributeTPB.Text = "0";
                    return;
                }
                boxAttributeTPB.Text = idProductAttributeBoxTPB.SelectedItem.ToString();
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                                                                    ///
        ///                                           Handle click button event Mima Bebes                                                     ///
        ///                                                                                                                                    ///
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void btnInsertMB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string Producto = boxProductoMB.Text;
                if (string.IsNullOrWhiteSpace(Producto))
                    MessageBox.Show("Error en el campo Producto", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                string id_product = boxIdProductMB.Text;
                if (string.IsNullOrWhiteSpace(id_product))
                    MessageBox.Show("Error en el campo id_producto", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                string id_product_attribute = boxAttributeMB.Text;
                if (string.IsNullOrWhiteSpace(id_product_attribute))
                    MessageBox.Show("Error en el campo id_producto_attribute", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                string Articulo = boxArticuloMB.Text;
                if (string.IsNullOrWhiteSpace(Articulo))
                    MessageBox.Show("Error en el campo Articulo", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);

                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString))
                {
                    MySqlCommand cmd = new MySqlCommand($"INSERT INTO InventarioTablas (Producto, id_product, id_product_attribute, Articulo) VALUES ('{Producto}', '{id_product}', '{id_product_attribute}', '{Articulo}')", conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                boxProductoMB.Clear();
                boxIdProductMB.Clear();
                boxAttributeMB.Clear();
                boxArticuloMB.Clear();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                                                                    ///
        ///                                          Handle click button event Todo para Bebes                                                 ///
        ///                                                                                                                                    ///
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void btnInsertTPB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string Producto = boxProductoTPB.Text;
                if (string.IsNullOrWhiteSpace(Producto))
                    MessageBox.Show("Error en el campo Producto", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                string id_product = boxIdProductTPB.Text;
                if (string.IsNullOrWhiteSpace(id_product))
                    MessageBox.Show("Error en el campo id_producto", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                string id_product_attribute = boxAttributeTPB.Text;
                if (string.IsNullOrWhiteSpace(id_product_attribute))
                    MessageBox.Show("Error en el campo id_producto_attribute", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                string Articulo = boxArticuloTPB.Text;
                if (string.IsNullOrWhiteSpace(Articulo))
                    MessageBox.Show("Error en el campo Articulo", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);

                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString))
                {
                    MySqlCommand cmd = new MySqlCommand($"INSERT INTO InventarioTablas (Producto, id_product_todo, id_product_todo_attribute, Articulo) VALUES ('{Producto}', '{id_product}', '{id_product_attribute}', '{Articulo}')", conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                boxProductoTPB.Clear();
                boxIdProductTPB.Clear();
                boxAttributeTPB.Clear();
                boxArticuloTPB.Clear();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                                                                    ///
        ///                                                    Update InventarioTablas                                                         ///
        ///                                                                                                                                    ///
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection connOrigen = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString))
            {
                DataTable changes = this.dt.GetChanges();
                connOrigen.Open();
                if (changes != null)
                {
                    MySqlCommandBuilder sqb = new MySqlCommandBuilder(sda);
                    int updatedRows = this.sda.Update(changes);
                    this.dt.AcceptChanges();
                }
            }
            MessageBox.Show("Se ha actualizado la Base de Datos", "Actualizado", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
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
        ///              Event tu update the Odacash comboBox when TextBox lost focus                  ///               
        ///                                                                                            ///                                    
        //////////////////////////////////////////////////////////////////////////////////////////////////

        private void idProductMB_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // If the textBox is empty do nothing
            if (boxIdProductMB.Text == "")
                return;
            string producto = "";

            // Look for the product name for the id inserted in the textbox
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand($"SELECT name FROM ps_product_lang WHERE id_product = '{boxIdProductMB.Text}'", conn);
                conn.Open();
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    // If the products exist save the name
                    if (rdr.HasRows)
                        producto = rdr[0].ToString();

                    // If not exist select the index and finish the execution
                    else
                    {
                        productsBoxMB.SelectedIndex = 0;
                        boxProductoMB.Text = "";
                        return;
                    }
                }
            }
            // Update the comboBox item selection with the product name
            productsBoxMB.SelectedIndex = productsBoxMB.Items.IndexOf(producto);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                            ///                                               
        ///              Event tu update the Odacash comboBox when TextBox lost focus                  ///               
        ///                                                                                            ///                                    
        //////////////////////////////////////////////////////////////////////////////////////////////////

        private void idOdacashMB_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // If the textBox is empty do nothing
            if (boxArticuloMB.Text == "")
                return;
            string articulo = "";

            // Look for the product name for the id inserted in the textbox
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServer"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand($"SELECT DescripcionCorta FROM VI_prueba_art WHERE Articulo = '{boxArticuloMB.Text}'", conn);
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
                        articuloBoxMB.SelectedIndex = 0;
                        return;
                    }
                }
            }
            // Update the comboBox item selection with the product name
            articuloBoxMB.SelectedIndex = articuloBoxMB.Items.IndexOf(articulo);
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
