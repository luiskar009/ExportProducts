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
using System.Windows.Forms;
using static ExportProducts.Library;

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
            productsBox.Items.Clear();
            productsBox.SelectedIndex = productsBox.Items.Add("-- Seleccione el producto de Prestashop --");
            imageBox.SelectedIndex = imageBox.Items.Add("-- Seleccione una imagen --");
            attributeBox.SelectedIndex = attributeBox.Items.Add("-- Seleccione un atributo para la combinacion --");
            attributeBox2.SelectedIndex = attributeBox2.Items.Add("-- Seleccione un atributo para la combinacion --");
            attributeBox3.SelectedIndex = attributeBox3.Items.Add("-- Seleccione un atributo para la combinacion --");
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString.ToString()))
            {
                MySqlCommand cmd = new MySqlCommand("SELECT name FROM ps_product_lang ORDER BY name", conn);
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
                        attributeBox3.Items.Add(rdr[0].ToString());
                    }
                }
            }
        }

        protected void productsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (productsBox.SelectedItem.ToString() == "-- Seleccione el producto de Prestashop --")
                return;
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString.ToString()))
            {
                MySqlCommand cmd = new MySqlCommand($"SELECT id_product FROM ps_product_lang WHERE name = '{productsBox.SelectedItem.ToString()}';", conn);
                conn.Open();
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    idPrestashop.Text = rdr[0].ToString();
                }
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
        protected void imageBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (imageBox.Items.Count == 0)
                return;
            if (imageBox.SelectedItem.ToString() == "-- Seleccione una imagen --")
                return;
            ImageFactory imf = new ImageFactory(ConfigurationManager.AppSettings["baseUrl"].ToString(), ConfigurationManager.AppSettings["accImages"].ToString(), "");
            byte[] imgByte = imf.GetProductImage(Int64.Parse(idPrestashop.Text), Int64.Parse(imageBox.SelectedItem.ToString()));
            img.Source = LoadImage(imgByte);
        }

        public void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            CombinationFactory cf = new CombinationFactory(ConfigurationManager.AppSettings["baseUrl"].ToString(), ConfigurationManager.AppSettings["accCombination"].ToString(), "");
            combination newComb = createCombination(Int32.Parse(idPrestashop.Text), getAttributeID(attributeBox.SelectedItem.ToString()), getAttributeID(attributeBox2.SelectedItem.ToString()), getAttributeID(attributeBox3.SelectedItem.ToString()), price.Text);
            try
            {
                cf.Add(newComb);
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
                idPrestashop.Text = "";
                attributeBox.SelectedIndex = 0;
                attributeBox2.SelectedIndex = 0;
                attributeBox3.SelectedIndex = 0;
                price.Text = "";
                img.Source = null;
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        private void idPrestashop_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (idPrestashop.Text == "")
                return;
            string articulo = "";
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString.ToString()))
            {
                MySqlCommand cmd = new MySqlCommand($"SELECT name FROM ps_product_lang WHERE id_product = '{idPrestashop.Text}'", conn);
                conn.Open();
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    if (rdr.HasRows)
                        articulo = rdr[0].ToString();
                    else
                    {
                        //name.Clear();
                        //price.Clear();
                        productsBox.SelectedIndex = 0;
                        return;
                    }
                }
            }
            productsBox.SelectedIndex = productsBox.Items.IndexOf(articulo);
        }
        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
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



    }
}
