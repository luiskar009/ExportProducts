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

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                                                                    ///
        ///                                                  Create a new Product                                                              ///
        ///                                                                                                                                    ///
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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

        public static product editProduct(product prod, string name, bool? isChecked, string shortDesc, string largeDesc, string price, string category, string manufacturer, string stockDesc, string noStockDesc)
        {
            //if (isChecked ?? true)
            //    prod.active = 1;
            //else
            //    prod.active = 0;
            //if (category != "-- Selecione la Categoria de Prestashop --")
            //    prod.associations.categories.Add(createAuxcategory(getCategoryID(category)));
            //prod.associations.stock_availables.Add(createAuxStockAvailable());
            //prod.available_later.Add(createAuxLanguage(noStockDesc));
            //prod.available_now.Add(createAuxLanguage(stockDesc));
            //prod.description.Add(createAuxLanguage(largeDesc));
            //prod.description_short.Add(createAuxLanguage(shortDesc));
            //if (manufacturer != "-- Selecione el Fabricante de Prestashop --")
            //    prod.id_manufacturer = getManufacturerID(manufacturer);
            //if (name != prod.name[0].Value)
            //{
            //    //prod.name.RemoveAt(0);
            //    //prod.name.Add(createAuxLanguage(name));
            //    prod.name[0].Value = name;
            //}
            prod.price = Decimal.Round((Decimal.Parse(price) / (Decimal)1.21), 6);
            return prod;
        }

        public static void editProductFromBD(string id, string name, bool? isChecked, string shortDesc, string largeDesc, string price, string category, string manufacturer, string stockDesc, string noStockDesc)
        {
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString.ToString()))
            {
                MySqlCommand cmd = new MySqlCommand($"UPDATE ps_product_lang SET name = '{name}', description = '{largeDesc}', description_short = '{shortDesc}', available_now = '{stockDesc}', available_later = '{noStockDesc}' WHERE id_product = '{id}' ", conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                MySqlCommand cmd2 = new MySqlCommand($"UPDATE ps_product SET id_manufacturer = '{manufacturer}', id_category_default = '{category}', price = '{price}', active = '{isChecked}' WHERE id_product = '{id}' ", conn);
                cmd2.ExecuteNonQuery();
            }

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                                                                    ///
        ///                                                Create a new Combination                                                            ///
        ///                                                                                                                                    ///
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static combination createCombination(int id_product, int att1, int att2, string price, string idImage)
        {
            combination comb = new combination();

            if(att1 != 0)
            {
                comb.associations.product_option_values.Add(createPOV(att1));
            }
            if (att2 != 0)
            {
                comb.associations.product_option_values.Add(createPOV(att2));
            }
            if(idImage != "-- Seleccione una imagen --")
                comb.associations.images.Add(createImage(Int32.Parse(idImage)));
            comb.available_date = "0000-00-00";
            //comb.default_on = 1;
            comb.ean13 = "";
            //comb.ecotax = "";
            comb.id = getIDComb();
            comb.id_product = id_product;
            comb.location = "";
            comb.minimal_quantity = 1;
            if(price != "")
                comb.price = Decimal.Round((Decimal.Parse(price) / (Decimal)1.21), 6);
            comb.reference = "";
            comb.supplier_reference = "";
            comb.upc = "";
            return comb;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                                                                    ///
        ///                                              Create a new language element                                                         ///
        ///                                                                                                                                    ///
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Bukimedia.PrestaSharp.Entities.AuxEntities.language createAuxLanguage(string message)
        {
            Bukimedia.PrestaSharp.Entities.AuxEntities.language auxLang = new Bukimedia.PrestaSharp.Entities.AuxEntities.language();
            auxLang.Value = message;
            auxLang.id = 1;
            return auxLang;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                                                                    ///
        ///                                             Create a new Category element                                                          ///
        ///                                                                                                                                    ///
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Bukimedia.PrestaSharp.Entities.AuxEntities.category createAuxcategory(long category)
        {
            Bukimedia.PrestaSharp.Entities.AuxEntities.category auxCat = new Bukimedia.PrestaSharp.Entities.AuxEntities.category();
            auxCat.id = category;
            return auxCat;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                                                                    ///
        ///                                          Create a new Stock Available element                                                      ///
        ///                                                                                                                                    ///
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                                                                    ///
        ///                                           Create a new Product Option Value element                                                ///
        ///                                                                                                                                    ///
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Bukimedia.PrestaSharp.Entities.AuxEntities.product_option_value createPOV(int att)
        {
            Bukimedia.PrestaSharp.Entities.AuxEntities.product_option_value pov = new Bukimedia.PrestaSharp.Entities.AuxEntities.product_option_value();
            pov.id = att;
            return pov;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                                                                    ///
        ///                                              Create a new image element                                                            ///
        ///                                                                                                                                    ///
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Bukimedia.PrestaSharp.Entities.AuxEntities.image createImage(int id)
        {
            Bukimedia.PrestaSharp.Entities.AuxEntities.image a = new Bukimedia.PrestaSharp.Entities.AuxEntities.image();
            a.id = id;
            return a;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                                                                    ///
        ///                                           Get the next product id available                                                        ///
        ///                                                                                                                                    ///
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                                                                    ///
        ///                                           Get the next combination available                                                       ///
        ///                                                                                                                                    ///
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static int getIDComb()
        {
            int id = 0;
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString.ToString()))
            {
                MySqlCommand cmd = new MySqlCommand("SELECT `AUTO_INCREMENT` FROM  INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'rosamaria_shop2' AND TABLE_NAME = 'ps_product_attribute'; ", conn);
                conn.Open();
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    id = Int32.Parse(rdr[0].ToString());
                }
            }
            return id;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                                                                    ///
        ///                                                       Get category ID                                                              ///
        ///                                                                                                                                    ///
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static int getCategoryID(string category)
        {
            int id = 0;
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

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                                                                    ///
        ///                                                   Get manufacturer ID                                                              ///
        ///                                                                                                                                    ///
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static int getManufacturerID(string manufacturer)
        {
            int id = 0;
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

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                                                                    ///
        ///                                                     Get attribute ID                                                               ///
        ///                                                                                                                                    ///
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static int getAttributeID(string attribute)
        {
            int id = 0;
            if (attribute != "-- Seleccione un atributo para la combinacion --")
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlDB"].ConnectionString.ToString()))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand($"SELECT id_attribute FROM ps_attribute_lang WHERE NAME = '{attribute}'", conn);
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        rdr.Read();
                        id = Int32.Parse(rdr[0].ToString());
                    }
                }
            }
            return id;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///                                                                                                                                    ///
        ///                                       Create a link between Prestashop and Odacash ID                                              ///
        ///                                                                                                                                    ///
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
