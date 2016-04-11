using Innouvous.Utils.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuotesDB.Exporter;

namespace QuotesDB.DAO
{
    
    class DataStore : Innouvous.Utils.Data.SQLiteClient, IQuoteStore
    {
        private const string AuthorsTable = "tbl_authors";
        private const string TagsTable = "tbl_tags";
        private const string QuotesTable = "tbl_quotes";
        private const string TagMapTable = "tbl_tag_map";

        #region Create Tables
        
        private void CreateTables()
        {
            CreateAuthorsTable();
            CreateQuotesTable();
            CreateTagMapTable();
            CreateTagsTable();
        }

        private void CreateTagsTable()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("CREATE TABLE " + TagsTable + " (");
            sb.AppendLine("ID integer NOT NULL PRIMARY KEY AUTOINCREMENT,");
            sb.AppendLine("Tag varchar(50) NOT NULL");
            sb.AppendLine(");");

            string cmd = sb.ToString();
            this.ExecuteNonQuery(cmd);
        }

        private void CreateQuotesTable()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("CREATE TABLE " + QuotesTable + " (");
            sb.AppendLine("ID integer NOT NULL PRIMARY KEY AUTOINCREMENT,");
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
            sb.AppendLine("ID integer NOT NULL PRIMARY KEY AUTOINCREMENT,");
            sb.AppendLine("Name  varchar(50) NOT NULL");
            sb.AppendLine(");");

            string cmd = sb.ToString();
            this.ExecuteNonQuery(cmd);
        }

        #endregion

        public Author CreateAuthor(Author author)
        {
            string sql = "INSERT INTO {0} VALUES({1},'{2}')";
            sql = String.Format(sql, AuthorsTable, 
                (author.ID > 0? author.ID.ToString() : "NULL"), 
                author.Name);

            ExecuteNonQuery(sql);

            author.ID = SQLUtils.GetLastInsertRow(this);
            return author;
        }

        public void UpdateAuthor(Author author)
        {
            throw new NotImplementedException();
        }

        public List<Quote> GetQuotes(Author author)
        {
            string sql = "SELECT * FROM " + QuotesTable + " WHERE AuthorId=" + author.ID;

            return ParseQuotes(ExecuteSelect(sql));
        }

        private List<Quote> ParseQuotes(DataTable data)
        {
            List<Quote> quotes = new List<Quote>();

            foreach (DataRow row in data.Rows)
            {
                var q = new Quote()
                {
                    ID = Convert.ToInt32(row["ID"]),
                    Displayed = Convert.ToInt32(row["Count"]),
                    Rating = Convert.ToInt32(row["Rating"]), 
                    Text = row["Text"].ToString()
                };

                Author author = GetAuthor(Convert.ToInt32(row["AuthorID"]));
                q.Author = author;
                q.Tags = GetTagsForQuote(q);

                quotes.Add(q);
            }

            return quotes;
        }

        public List<Author> GetAuthors(string search = null)
        {
            string sql = "SELECT * FROM " + AuthorsTable;

            if (!String.IsNullOrEmpty(search))
                sql += String.Format(" WHERE Name LIKE '%{0}%'", search);

            var data = this.ExecuteSelect(sql);

            List<Author> authors = new List<Author>();
            foreach (DataRow row in data.Rows)
            {
                var author = new Author()
                {
                    ID = Convert.ToInt32(row["ID"]),
                    Name = row["Name"].ToString()
                };

                authors.Add(author);
            }

            return authors;
        }

        public int InsertQuote(Quote quote)
        {
            string sql = "INSERT INTO {0} VALUES({1},{2},'{3}',{4},{5})";
            sql = String.Format(sql, QuotesTable, 
                quote.ID > 0? quote.ID.ToString() : "NULL",
                quote.Author.ID, 
                 SQLUtils.SQLEncode(quote.Text), 
                 quote.Displayed, 
                 quote.Rating);

            ExecuteNonQuery(sql);
            
            return SQLUtils.GetLastInsertRow(this);
        }

        public void UpdateQuote(Quote quote)
        {
            throw new NotImplementedException();
        }

        public int InsertTag(Tag tag)
        {
            throw new NotImplementedException();
        }

        public int UpdateTag(Tag tag)
        {
            throw new NotImplementedException();
        }

        public List<Tag> GetTags(string search = null)
        {
            string sql = "SELECT * FROM " + TagsTable;

            if (!String.IsNullOrEmpty(search))
                sql += String.Format(" WHERE Tag LIKE '%{0}%'", search);

            var data = this.ExecuteSelect(sql);
            List<Tag> tags = ParseTags(data);

            return tags;
        }

        private static List<Tag> ParseTags(DataTable data)
        {
            List<Tag> tags = new List<Tag>();
            foreach (DataRow row in data.Rows)
            {
                var tag = new Tag()
                {
                    ID = Convert.ToInt32(row["ID"]),
                    TagName = row["Tag"].ToString()
                };

                tags.Add(tag);
            }

            return tags;
        }

        public Author GetAuthor(int id)
        {
            string sql = "SELECT * FROM {0} WHERE ID = {1}";
            sql = String.Format(sql, AuthorsTable, id);
            var data = this.ExecuteSelect(sql);

            var row = data.Rows[0];

            var author = new Author()
                {
                    ID = Convert.ToInt32(row["ID"]),
                    Name = row["Name"].ToString()
                };
            
            return author;
        }

        public List<Tag> GetTagsForQuote(Quote quote)
        {
            int id = quote.ID;

            string sql = "SELECT t.ID, t.Tag FROM {0} t JOIN {1} m ON t.ID = m.TagId WHERE m.QuoteId={2}";
            sql = String.Format(sql, TagsTable, TagMapTable, id);

            var data = ExecuteSelect(sql);
            return ParseTags(data);
        }

        public void UpdateTags(Quote qt, IEnumerable<string> tags)
        {
            string sql = "DELETE FROM {0} WHERE QuoteId={1}";
            sql = String.Format(sql, TagMapTable, qt.ID);
            ExecuteNonQuery(sql);

            List<int> ids = new List<int>();
            foreach (string t in tags)
            {
                Tag tag = GetTag(t);
                if (tag == null)
                    ids.Add(InsertTag(new Tag() { TagName = t }));
                else
                    ids.Add(tag.ID);
            }

            foreach (int i in ids)
            {
                InsertTagMap(qt.ID, i);
            }
        }

        private void InsertTagMap(int quoteId, int tagId)
        {
            string sql = "INSERT INTO {0} VALUES({1},{2})";
            sql = String.Format(sql, TagMapTable, tagId, quoteId);
            ExecuteNonQuery(sql); 

        }

        private Tag GetTag(string tag)
        {
            string sql = "SELECT * FROM " + TagsTable + " WHERE Tag = '" + tag + "'";
            var data = this.ExecuteSelect(sql);

            return ParseTags(data).FirstOrDefault();
        }

        public Bundle Export()
        {
            Bundle bundle = new Bundle();
            bundle.Quotes = GetQuotes();
            bundle.Authors = GetAuthors();
            bundle.Tags = GetTags();

            foreach (Quote q in bundle.Quotes)
            {
                var tags = GetTagsForQuote(q);
                q.Tags = tags;
            }

            return bundle;
        }

        private List<Quote> GetQuotes()
        {
            string sql = "SELECT * FROM " + QuotesTable;
            List<Quote> quotes = ParseQuotes(ExecuteSelect(sql));

            return quotes;
        }

        public void Import(Bundle data)
        {
            throw new NotImplementedException();
        }

        public DataStore(string filename, bool isNew) : base(filename, isNew)
        {
            if (isNew)
            {
                CreateTables();
            }
        }
        
    }
}
