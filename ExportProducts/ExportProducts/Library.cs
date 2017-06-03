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
    class Library
    {
        public static product createProduct(string name, bool? isChecked, string shortDesc, string largeDesc, string price, string category, string manufacturer, string stockDesc, string noStockDesc)
        {
            product prod = new product();
            if (isChecked ?? true)
                prod.active = 1;
            else
                prod.active = 0;
            //prod.additional_shipping_cost = (Decimal)0.00;
            prod.advanced_stock_management = 0;
            if (category != "-- Selecione la Categoria de Prestashop --")
                prod.associations.categories.Add(createAuxcategory(getCategoryID(category)));
            prod.associations.stock_availables.Add(createAuxStockAvailable());
            prod.available_date = "0000-00-00";
            prod.available_for_order = 1;
            prod.available_later.Add(createAuxLanguage(noStockDesc));
            prod.available_now.Add(createAuxLanguage(stockDesc));
            prod.cache_default_attribute = 0;
            prod.cache_has_attachments = 0;
            prod.cache_is_pack = 0;
            prod.condition = "new";
            prod.customizable = 0;
            prod.date_add = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            prod.date_upd = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            //prod.depth = (Decimal)0.000000;
            prod.description.Add(createAuxLanguage(largeDesc));
            prod.description_short.Add(createAuxLanguage(shortDesc));
            prod.ean13 = "";
            //prod.ecotax = (Decimal)0.000000;
            //prod.height = (Decimal)0.000000;
            prod.id = getID();
            prod.id_category_default = 2;
            prod.id_default_combination = null;
            prod.id_default_image = null;
            if (manufacturer != "-- Selecione el Fabricante de Prestashop --")
                prod.id_manufacturer = getManufacturerID(manufacturer);
            prod.id_product_redirected = 0;
            prod.id_shop_default = 1;
            prod.id_supplier = 0;
            prod.id_tax_rules_group = 1;
            prod.indexed = 1;
            prod.is_virtual = 0;
            prod.link_rewrite.Add(createAuxLanguage((name.ToLower().Replace(" ", "-"))));
            prod.location = "";
            prod.meta_description.Add(createAuxLanguage(""));
            prod.meta_keywords.Add(createAuxLanguage(""));
            prod.meta_title.Add(createAuxLanguage(""));
            prod.minimal_quantity = 1;
            prod.name.Add(createAuxLanguage(name));
            prod.on_sale = 0;
            prod.online_only = 0;
            prod.price = Decimal.Round((Decimal.Parse(price) / (Decimal)1.21), 6);
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

        public static combination createCombination(int id_product, string att1, string att2, string att3)
        {
            combination comb = new combination();
            comb.id_product = id_product;
            //comb.associations.product_option_values.Add();


            return comb;
        }

        public static Bukimedia.PrestaSharp.Entities.AuxEntities.language createAuxLanguage(string message)
        {
            Bukimedia.PrestaSharp.Entities.AuxEntities.language auxLang = new Bukimedia.PrestaSharp.Entities.AuxEntities.language();
            auxLang.Value = message;
            auxLang.id = 1;
            return auxLang;
        }

        public static Bukimedia.PrestaSharp.Entities.AuxEntities.category createAuxcategory(long category)
        {
            Bukimedia.PrestaSharp.Entities.AuxEntities.category auxCat = new Bukimedia.PrestaSharp.Entities.AuxEntities.category();
            auxCat.id = category;
            return auxCat;
        }

        public static Bukimedia.PrestaSharp.Entities.AuxEntities.stock_available createAuxStockAvailable()
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

        public static int getID()
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

        public static int getCategoryID(string category)
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

        public static int getManufacturerID(string manufacturer)
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

        public static void insertInventory(string Producto, string id_product, string id_product_attribute, string Articulo)
        {
           

            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand($"INSERT INTO InventarioTablas (Producto, id_product, id_product_attribute, Articulo) VALUES ('{Producto}', '{id_product}', '{id_product_attribute}', '{Articulo}')", conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
