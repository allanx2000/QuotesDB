using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace QuotesDB.DAO
{
    [Serializable]
    public class Quote
    {
        public int ID { get; set; }
        public string Text { get; set; }
        //public int AuthorId { get; set; }
        
        public int Displayed { get; set; }
        public int Rating { get; set; }
        
        public List<Tag> Tags { get; set; }
        public Author Author { get; set; }

        private string tagsText;
        [XmlIgnore]
        public string TagsText
        {
            get
            {
                if (tagsText == null)
                {
                    if (Tags == null)
                        tagsText = "";
                    else 
                        tagsText = String.Join(", ", Tags.Select(x => x.TagName));
                }

                return tagsText;
            }
        }
    }
}
