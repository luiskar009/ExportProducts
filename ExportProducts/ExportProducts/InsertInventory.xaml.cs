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
        DataTable dt;
        MySqlDataAdapter sda;

        public InsertInventory()
        {
            InitializeComponent();

            // Look for products in the database and fill it in the comboBox
            using (MySqlConnection connOrigen = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand("SELECT Producto, id_product, id_product_attribute, Articulo, Baja FROM InventarioTablas ORDER BY Producto", connOrigen);
                connOrigen.Open();
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    ProductCombo.SelectedIndex = ProductCombo.Items.Add("-- Selecione el producto a modificar --");
                    while (rdr.Read())
                    {
                        ProductCombo.Items.Add(rdr["Producto"].ToString());
                    }
                }
                sda = new MySqlDataAdapter(cmd);
                dt = new DataTable("InventarioTablas");
                sda.Fill(dt);
                tablaSQL.ItemsSource = dt.DefaultView;
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
        ///              Event to prevent MainWindow to minimize when closes this window               ///         
        ///                                                                                            ///                                        
        //////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void OnClosing(CancelEventArgs e)
        {
            this.Owner = null;
        }
    }
}
