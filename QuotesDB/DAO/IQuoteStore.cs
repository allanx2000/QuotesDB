using System.Collections.Generic;

namespace QuotesDB.DAO
{
    public interface IQuoteStore
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
        Author GetAuthor(int id);
        List<Tag> GetTags(string search = null);
        List<Tag> GetTagsForQuote(Quote quote);
        void UpdateTags(Quote qt, IEnumerable<string> tags);
    }

}