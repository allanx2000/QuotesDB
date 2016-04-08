using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuotesDB.DAO
{
    [Serializable]
    public class Quote
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public int AuthorId { get; set; }
        public int Displayed { get; set; }
        public int Rating { get; set; }


        public List<Tag> Tags { get; set; }
    }
}
