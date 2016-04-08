using System;

namespace QuotesDB.DAO
{
    [Serializable]
    public class Author
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}