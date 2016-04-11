using QuotesDB.Exporter;
using System.Collections.Generic;

namespace QuotesDB.DAO
{
    public interface IQuoteStore
    {
        Author CreateAuthor(Author author);
        void UpdateAuthor(Author author);
        List<Quote> GetQuotes(Author author);
        List<Author> GetAuthors(string search = null);
        Author GetAuthor(int id);

        int InsertQuote(Quote quote);
        void UpdateQuote(Quote quote);
        void UpdateQuoteRating(Quote quote);
        void UpdateQuoteCount(Quote quote);
        void DeleteQuote(Quote selectedQuote);
        Quote GetRandomQuote();

        int InsertTag(Tag tag);
        int UpdateTag(Tag tag);
        List<Tag> GetTags(string search = null);
        List<Tag> GetTagsForQuote(Quote quote);
        void UpdateTags(Quote qt, IEnumerable<string> tags);

        Bundle Export();
        void Import(Bundle data);
    }

}