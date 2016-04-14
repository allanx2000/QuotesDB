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

        public DataStore(string filename, bool isNew) : base(filename, isNew)
        {
            if (isNew)
            {
                CreateTables();
            }
        }
        
        #region Create Tables

        private void CreateTables()
        {
            CreateAuthorsTable();
            CreateQuotesTable();
            CreateTagMapTable();
            CreateTagsTable();
        }

        private string LoadFromFile(string file)
        {
            using (StreamReader sr = new StreamReader("TableScripts\\" + file))
            {
                string str = sr.ReadToEnd();
                sr.Close();
                
                return str;
            }
        }

        private void CreateTagsTable()
        {

            /*
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("CREATE TABLE " + TagsTable + " (");
            sb.AppendLine("ID integer NOT NULL PRIMARY KEY AUTOINCREMENT,");
            sb.AppendLine("Tag varchar(50) NOT NULL");
            sb.AppendLine(");");
            
            string cmd = sb.ToString();
            */

            string cmd = LoadFromFile("tags.sql");
            this.ExecuteNonQuery(cmd);
            
        }

        private void CreateQuotesTable()
        {
            /*
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("CREATE TABLE " + QuotesTable + " (");
            sb.AppendLine("ID integer NOT NULL PRIMARY KEY AUTOINCREMENT,");
            sb.AppendLine("AuthorId integer NOT NULL,");
            sb.AppendLine("Text varchar(200) NOT NULL,");
            sb.AppendLine("Count integer NOT NULL,");
            sb.AppendLine("Rating integer NOT NULL");
            sb.AppendLine(");");

            string cmd = sb.ToString();
            */

            string cmd = LoadFromFile("quotes.sql");
            this.ExecuteNonQuery(cmd);
            
        }

        private void CreateTagMapTable()
        {
            /*
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("CREATE TABLE " + TagMapTable + " (");
            sb.AppendLine("TagId integer NOT NULL,");
            sb.AppendLine("QuoteId integer NOT NULL,");
            sb.AppendLine("PRIMARY KEY (TagId, QuoteId)");
            sb.AppendLine(");");

            string cmd = sb.ToString();
            */

            string cmd = LoadFromFile("tagmap.sql");
            this.ExecuteNonQuery(cmd);

        }

        private void CreateAuthorsTable()
        {
            /*
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("CREATE TABLE " + AuthorsTable + " (");
            sb.AppendLine("ID integer NOT NULL PRIMARY KEY AUTOINCREMENT,");
            sb.AppendLine("Name  varchar(50) NOT NULL");
            sb.AppendLine(");");

            string cmd = sb.ToString();
            this.ExecuteNonQuery(cmd);
            */

            string cmd = LoadFromFile("authors.sql");
            this.ExecuteNonQuery(cmd);
        }

        #endregion

        #region Author

        public void DeleteAuthor(Author author)
        {
            /*
            var quotes = GetQuotes(author);

            foreach (var q in quotes)
            {
                DeleteQuote(q);
            }
            */

            string sql = "DELETE FROM {0} WHERE ID = {1}";
            sql = string.Format(sql, AuthorsTable, author.ID);

            ExecuteNonQuery(sql);
            
            
        }
        public Author CreateAuthor(Author author)
        {
            string sql = "INSERT INTO {0} VALUES({1},'{2}')";
            sql = String.Format(sql, AuthorsTable, 
                "NULL", 
                author.Name);

            ExecuteNonQuery(sql);

            author.ID = SQLUtils.GetLastInsertRow(this);
            return author;
        }

        public void UpdateAuthor(Author author)
        {
            throw new NotImplementedException();
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

        public List<Author> GetAuthors(string search = null)
        {
            string sql = "SELECT * FROM " + AuthorsTable;

            if (!String.IsNullOrEmpty(search))
                sql += String.Format(" WHERE Name LIKE '%{0}%'", search);

            sql += " ORDER BY Name ASC";

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

        #endregion

        #region Quotes
        
        public List<Quote> GetQuotes(Tag tag)
        {
            string sql = "SELECT q.* FROM {0} q JOIN {1} tm ON q.ID = tm.QuoteId WHERE tm.TagId={2}";
            sql = String.Format(sql, QuotesTable, TagMapTable, tag.ID);

            return ParseQuotes(ExecuteSelect(sql));
        }


        public List<Quote> GetQuotes(Author author)
        {
            string sql = "SELECT * FROM " + QuotesTable + " WHERE AuthorId=" + author.ID;

            return ParseQuotes(ExecuteSelect(sql));
        }

        private Quote GetQuote(int id)
        {
            string sql = "SELECT * FROM " + QuotesTable + " WHERE Id=" + id;

            return ParseQuotes(ExecuteSelect(sql)).FirstOrDefault();
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
                    //Rating = Convert.ToInt32(row["Rating"]), 
                    Text = row["Text"].ToString()
                };

                Author author = GetAuthor(Convert.ToInt32(row["AuthorID"]));
                q.Author = author;
                q.Tags = GetTagsForQuote(q);

                quotes.Add(q);
            }

            return quotes;
        }
        
        public Quote InsertQuote(Quote quote)
        {
            //TODO: Add insert Author, Tag?

            string sql = "INSERT INTO {0} VALUES({1},{2},'{3}',{4},{5})";
            sql = String.Format(sql, QuotesTable, 
                "NULL",
                quote.Author.ID, 
                 SQLUtils.SQLEncode(quote.Text), 
                 quote.Displayed, 
                 0); //TODO: Remove column

            ExecuteNonQuery(sql);
            
            quote.ID = SQLUtils.GetLastInsertRow(this);
            
            return quote;
        }
        
        private List<Quote> GetQuotes()
        {
            string sql = "SELECT * FROM " + QuotesTable;
            List<Quote> quotes = ParseQuotes(ExecuteSelect(sql));

            return quotes;
        }

        public void DeleteQuote(Quote quote)
        {
            string sql = "DELETE FROM {0} WHERE ID = {1}";
            sql = String.Format(sql, QuotesTable, quote.ID);
            ExecuteNonQuery(sql);

            /*
            //TODO: Create Foreign key mapping
            sql = "DELETE FROM {0} WHERE QuoteID = {1}";
            sql = String.Format(sql, TagMapTable, quote.ID);
            ExecuteNonQuery(sql);
            */
        }

        private static Random random = new Random(DateTime.Now.Millisecond);
        private int? min, max;

        private void ResetQuotesRange()
        {
            string sql = "SELECT MIN(ID) FROM " + QuotesTable;
            min = Convert.ToInt32(ExecuteScalar(sql));

            sql = "SELECT MAX(ID) FROM " + QuotesTable;
            max = Convert.ToInt32(ExecuteScalar(sql));

            max += 1; //Exclusive
        }

        public Quote GetRandomQuote()
        {
            if (min == null || max == null)
            {
                ResetQuotesRange();    
            }

            int count = GetTotalQuotes();
            if (count == 0)
                return null;

            Quote quote = null;


            while (quote == null)
            {
                int id = random.Next(min.Value, max.Value);
                quote = GetQuote(id);
            }

            return quote;
        }

        public int GetTotalQuotes()
        {
            string sql = "SELECT count(*) FROM " + QuotesTable;

            return Convert.ToInt32(ExecuteScalar(sql));
        }

        public void UpdateQuoteCount(Quote quote)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("UPDATE " + QuotesTable);
            sb.AppendLine("SET Count = " + quote.Displayed);
            sb.AppendLine("WHERE ID = " + quote.ID);

            string sql = sb.ToString();
            ExecuteNonQuery(sql);
        }

        /*
        public void UpdateQuoteRating(Quote quote)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("UPDATE " + QuotesTable);
            sb.AppendLine("SET Rating = 0" + quote.Rating);
            sb.AppendLine("WHERE ID = " + quote.ID);

            string sql = sb.ToString();
            ExecuteNonQuery(sql);
        }
        */

        public void UpdateQuote(Quote quote)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("UPDATE " + QuotesTable);

            List<string> parts = new List<string>();
            parts.Add("AuthorId = " + quote.Author.ID);
            parts.Add("Text = '" + SQLUtils.SQLEncode(quote.Text) + "'");
            parts.Add("Count = " + quote.Displayed);
            //parts.Add("Rating = " + quote.Rating);
            sb.AppendLine("SET " + String.Join(", ", parts));

            sb.AppendLine("WHERE ID = " + quote.ID);

            string sql = sb.ToString();
            ExecuteNonQuery(sql);

            UpdateTags(quote, quote.Tags.Select(x => x.TagName));
        }

        #endregion

        #region Tags
        public Tag InsertTag(Tag tag)
        {
            string sql = "INSERT INTO {0} VALUES({1},'{2}')";
            sql = String.Format(sql, TagsTable,
                "NULL",
                SQLUtils.SQLEncode(tag.TagName));

            ExecuteNonQuery(sql);

            tag.ID = SQLUtils.GetLastInsertRow(this);

            return tag;
        }

        public void UpdateTag(Tag tag)
        {
            throw new NotImplementedException();
        }

        public List<Tag> GetTags(string search = null)
        {
            string sql = "SELECT * FROM " + TagsTable;

            if (!String.IsNullOrEmpty(search))
                sql += String.Format(" WHERE Tag LIKE '%{0}%'", search);

            sql += " ORDER BY Tag ASC";

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

        public List<Tag> GetTagsForQuote(Quote quote)
        {
            int id = quote.ID;

            string sql = "SELECT t.ID, t.Tag FROM {0} t JOIN {1} m ON t.ID = m.TagId WHERE m.QuoteId={2} ORDER BY t.Tag ASC";
            sql = String.Format(sql, TagsTable, TagMapTable, id);

            var data = ExecuteSelect(sql);
            return ParseTags(data);
        }

        public void UpdateTags(Quote qt, List<Tag> tags)
        {
            string sql = "DELETE FROM {0} WHERE QuoteId={1}";
            sql = String.Format(sql, TagMapTable, qt.ID);
            ExecuteNonQuery(sql);

            var newTags = tags.Where(x => x.ID == 0);
            foreach (Tag t in newTags)
            {
                Tag tag = InsertTag(t);
            }

            foreach (Tag t in tags)
            {
                InsertTagMap(qt.ID, t.ID);
            }
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
                {
                    tag = InsertTag(new Tag() { TagName = t });
                }

                ids.Add(tag.ID);
            }

            foreach (int i in ids)
            {
                InsertTagMap(qt.ID, i);
            }
        }
        
        private Tag GetTag(string tag)
        {
            string sql = "SELECT * FROM " + TagsTable + " WHERE Tag = '" + tag + "'";
            var data = this.ExecuteSelect(sql);

            return ParseTags(data).FirstOrDefault();
        }


        public int GetQuotesCount(Tag tag)
        {
            string sql = "SELECT count(*) FROM {0} q JOIN {1} tm ON tm.QuoteId = q.Id WHERE tm.TagId = {2}";
            sql = String.Format(sql, QuotesTable, TagMapTable, tag.ID);

            return Convert.ToInt32(ExecuteScalar(sql));
        }

        public void DeleteTag(Tag tag)
        {
            /*
            string sql = "DELETE FROM {0} WHERE TagId = {1}";
            sql = String.Format(sql, TagMapTable, tag.ID);
            ExecuteNonQuery(sql);
            */

            string sql = "DELETE FROM {0} WHERE Id = {1}";
            sql = String.Format(sql, TagsTable, tag.ID);
            ExecuteNonQuery(sql);

        }

        #endregion

        private void InsertTagMap(int quoteId, int tagId)
        {
            string sql = "INSERT INTO {0} VALUES({1},{2})";
            sql = String.Format(sql, TagMapTable, tagId, quoteId);
            ExecuteNonQuery(sql);

        }

        #region Export/Import
        public Bundle Export()
        {
            Bundle bundle = new Bundle();
            bundle.Quotes = GetQuotes();
            
            foreach (Quote q in bundle.Quotes)
            {
                var tags = GetTagsForQuote(q);
                q.Tags = tags;
            }

            return bundle;
        }
        
        public void Import(Bundle data)
        {
            ClearTables();

            Dictionary<string, Tag> tagLookup = new Dictionary<string, Tag>();
            Dictionary<string, Author> authorLookup = new Dictionary<string, Author>();

            foreach (Quote quote in data.Quotes)
            {
                //Insert Author
                string a_name = quote.Author.Name;

                if (!authorLookup.ContainsKey(a_name))
                {
                    Author author = CreateAuthor(quote.Author);
                    authorLookup.Add(author.Name, author);
                }

                quote.Author = authorLookup[a_name];

                //Insert Tags

                List<Tag> newTags = new List<Tag>();

                foreach (Tag tag in quote.Tags)
                {
                    if (!tagLookup.ContainsKey(tag.TagName))
                    {
                        Tag t = InsertTag(tag);
                        tagLookup.Add(t.TagName, t);
                    }

                    newTags.Add(tagLookup[tag.TagName]);
                }

                quote.Tags = newTags;

                Quote quote2 = InsertQuote(quote);

                foreach (Tag t in newTags)
                {
                    InsertTagMap(quote2.ID, t.ID);
                }
                
            }
        }

        public void ClearTables()
        {
            string baseSQL = "DELETE FROM ";

            ExecuteNonQuery(baseSQL + AuthorsTable);
            ExecuteNonQuery(baseSQL + QuotesTable);
            ExecuteNonQuery(baseSQL + TagMapTable);
            ExecuteNonQuery(baseSQL + TagsTable);
        }

        

        #endregion

    }
}
