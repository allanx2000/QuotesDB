using QuotesDB.DAO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace QuotesDB.Exporter
{
    [Serializable]
    public class Bundle
    {
        //public List<Author> Authors { get; set; }
        public List<Quote> Quotes { get; set; }
        //public List<Tag> Tags { get; set; }

        public static void Serialize(string output, Bundle bundle)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Bundle));

            StreamWriter sw = new StreamWriter(output);

            ser.Serialize(sw, bundle);
            sw.Close();
        }

        public static Bundle Deserialize(string path)
        {

            XmlSerializer ser = new XmlSerializer(typeof(Bundle));
            StreamReader sr = new StreamReader(path);

            Bundle bundle = (Bundle) ser.Deserialize(sr);

            return bundle;
        }
    }
}
