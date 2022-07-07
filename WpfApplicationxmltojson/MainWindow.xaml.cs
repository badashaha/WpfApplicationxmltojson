using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.OleDb;
using System.IO;
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

namespace WpfApplication1xmltojson
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            var pathToExcel = @"C:\temp\file.xlsx";
            var sheetName = "sheetOne";

            //This connection string works if you have Office 2007+ installed and your 
            //data is saved in a .xlsx file
            var connectionString = String.Format(@"
            Provider=Microsoft.ACE.OLEDB.12.0;
            Data Source={0};
            Extended Properties=""Excel 12.0 Xml;HDR=YES""
        ", pathToExcel);

            //Creating and opening a data connection to the Excel sheet 
            using (var conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = String.Format(
                    @"SELECT * FROM [{0}$]",
                    sheetName
                    );


                using (var rdr = cmd.ExecuteReader())
                {
                    //LINQ query - when executed will create anonymous objects for each row
                    var query =
                        (from DbDataRecord row in rdr
                         select row).Select(x =>
                         {


                         //dynamic item = new ExpandoObject();
                         Dictionary<string, object> item = new Dictionary<string, object>();
                             item.Add(rdr.GetName(0), x[0]);
                             item.Add(rdr.GetName(1), x[1]);
                             item.Add(rdr.GetName(2), x[2]);
                             return item;

                         });

                    //Generates JSON from the LINQ query
                    var json = JsonConvert.SerializeObject(query, Formatting.Indented);
                   
                        File.WriteAllText(@"PeopleData.json", json);
                    //return json;
                }
            }
        }
    }
}
