using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuotesDB.DAO
{
    interface IQuoteStore
    {
        int CreateAuthor(Author author);
        void UpdateAuthor(Author author);
        List<Quote> GetQuotes(Author author);
        List<Author> GetAuthors(string search = null);

        int InsertQuote(Quote quote);
        void UpdateQuote(Quote quote);
        
        //Random Quote
        //Find Quote..
        
        int InsertTag(Tag tag);
        int UpdateTag(Tag tag);
        List<Tag> GetTags(string search = null);

    }

    class DataStore : Innouvous.Utils.Data.SQLiteClient
    {
        private const string AuthorsTable = "tbl_authors";
        private const string TagsTable = "tbl_tags";
        private const string QuotesTable = "tbl_quotes";
        private const string TagMapTable = "tbl_tag_map";
        private string databasePath;

        #region Create Tables

        private void CreateTagsTable()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("CREATE TABLE " + TagsTable + " (");
            sb.AppendLine("ID integer NOT NULL,");
            sb.AppendLine("Tag varchar(50) NOT NULL PRIMARY KEY");
            sb.AppendLine(");");

            string cmd = sb.ToString();
            this.ExecuteNonQuery(cmd);
        }

        private void CreateQuotesTable()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("CREATE TABLE " + QuotesTable + " (");
            sb.AppendLine("ID integer NOT NULL,");
            sb.AppendLine("AuthorId integer NOT NULL,");
            sb.AppendLine("Text varchar(200) NOT NULL,");
            sb.AppendLine("Count integer NOT NULL,");
            sb.AppendLine("Rating integer NOT NULL");
            sb.AppendLine(");");

            //TODO: Add Author FK

            string cmd = sb.ToString();
            this.ExecuteNonQuery(cmd);
        }

        private void CreateTagMapTable()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("CREATE TABLE " + TagMapTable + " (");
            sb.AppendLine("TagId integer NOT NULL,");
            sb.AppendLine("QuoteId integer NOT NULL,");
            sb.AppendLine("PRIMARY KEY (TagId, QuoteId)");
            sb.AppendLine(");");

            //TODO: FK Quote, Tags

            string cmd = sb.ToString();
            this.ExecuteNonQuery(cmd);
        }

        private void CreateAuthorsTable()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("CREATE TABLE " + AuthorsTable + " (");
            sb.AppendLine("ID integer NOT NULL,");
            sb.AppendLine("Name  varchar(50) NOT NULL PRIMARY KEY");
            sb.AppendLine(");");

            string cmd = sb.ToString();
            this.ExecuteNonQuery(cmd);
        }

        #endregion

        public DataStore(string filename, bool isNew) : base(filename, isNew)
        {
            if (isNew)
            {
                CreateTables();
            }
        }
        

        private void CreateTables()
        {
            CreateAuthorsTable();
            CreateQuotesTable();
            CreateTagMapTable();
            CreateTagsTable();
        }
    }
}
