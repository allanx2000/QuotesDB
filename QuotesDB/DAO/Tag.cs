using System;

namespace QuotesDB.DAO
{
    [Serializable]
    public class Tag
    {
        public int ID { get; set; }
        public string TagName { get; set; }

        public override bool Equals(object obj)
        {
            Tag other = obj as Tag;
            if (other == null)
                return false;
            
            return other.ID == this.ID;
        }
    }
}