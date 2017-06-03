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

namespace ExportProducts
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class InsertInventory : Window
    {
        public InsertInventory()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            using (MySqlConnection connOrigen = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand("SELECT Producto FROM InventarioTablas ORDER BY Producto", connOrigen);
                connOrigen.Open();
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {

                    ProductCombo.Items.Clear();
                    ProductCombo.SelectedIndex = ProductCombo.Items.Add("-- Selecione el producto a modificar --");
                    while (rdr.Read())
                    {
                        ProductCombo.Items.Add(rdr["Producto"].ToString());
                    }
                }
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
                String Producto = boxProductoMB.Text;
                if (String.IsNullOrWhiteSpace(Producto))
                    MessageBox.Show("Error en el campo Producto", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                String id_product = boxIdProductMB.Text;
                if (String.IsNullOrWhiteSpace(id_product))
                    MessageBox.Show("Error en el campo id_producto", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                String id_product_attribute = boxAttributeMB.Text;
                if (String.IsNullOrWhiteSpace(id_product_attribute))
                    MessageBox.Show("Error en el campo id_producto_attribute", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                String Articulo = boxArticuloMB.Text;
                if (String.IsNullOrWhiteSpace(Articulo))
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
                String Producto = boxProductoTPB.Text;
                if (String.IsNullOrWhiteSpace(Producto))
                    MessageBox.Show("Error en el campo Producto", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                String id_product = boxIdProductTPB.Text;
                if (String.IsNullOrWhiteSpace(id_product))
                    MessageBox.Show("Error en el campo id_producto", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                String id_product_attribute = boxAttributeTPB.Text;
                if (String.IsNullOrWhiteSpace(id_product_attribute))
                    MessageBox.Show("Error en el campo id_producto_attribute", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                String Articulo = boxArticuloTPB.Text;
                if (String.IsNullOrWhiteSpace(Articulo))
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
        ///                                              Handle click button event Editar                                                      ///
        ///                                                                                                                                    ///
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void btnInsertEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String Producto = ProductCombo.SelectedItem.ToString();
                if (Producto == "-- Selecione el producto a modificar --")
                    MessageBox.Show("Elija el producto a modificar", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                String id_product = boxIdProductEdit.Text;
                if (String.IsNullOrWhiteSpace(id_product))
                    MessageBox.Show("Error en el campo id_producto", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                String id_product_attribute = boxAttributeEdit.Text;
                if (String.IsNullOrWhiteSpace(id_product_attribute))
                    MessageBox.Show("Error en el campo id_producto_attribute", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);

                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString))
                {
                    MySqlCommand cmd = null;
                    if (MB.IsChecked ?? false)
                        cmd = new MySqlCommand($"UPDATE InventarioTablas SET id_product = '{id_product}', id_product_attribute = '{id_product_attribute}' WHERE Producto = '{Producto}'", conn);
                    if (TPB.IsChecked ?? false)
                        cmd = new MySqlCommand($"UPDATE InventarioTablas SET id_product_todo = '{id_product}', id_product_todo_attribute = '{id_product_attribute}' WHERE Producto = '{Producto}'", conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                boxIdProductEdit.Clear();
                boxAttributeEdit.Clear();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        void NewWindowAsDialog(object sender, RoutedEventArgs e)
        {
            MainWindow MainWindow = new MainWindow();
            MainWindow.Owner = this;
            MainWindow.ShowDialog();
        }
        void NormalNewWindow(object sender, RoutedEventArgs e)
        {
            MainWindow myOwnedWindow = new MainWindow();
            myOwnedWindow.Owner = this;
            myOwnedWindow.Show();
        }

    }
}
