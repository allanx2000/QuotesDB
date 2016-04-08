using System;

namespace QuotesDB.DAO
{
    [Serializable]
    public class Tag
    {
        public int ID { get; set; }
        public string TagName { get; set; }
    }
}