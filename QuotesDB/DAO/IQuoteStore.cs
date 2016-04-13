using QuotesDB.Exporter;
using System.Collections.Generic;

namespace QuotesDB.DAO
{
    public interface IQuoteStore
    {
        Author CreateAuthor(Author author);
        void UpdateAuthor(Author author);
        List<Author> GetAuthors(string search = null);
        Author GetAuthor(int id);
        List<Quote> GetQuotes(Author author);
        Quote GetRandomQuote();
        Quote InsertQuote(Quote quote);
        void UpdateQuote(Quote quote);
        void UpdateQuoteRating(Quote quote);
        void UpdateQuoteCount(Quote quote);
        void DeleteQuote(Quote selectedQuote);
        int GetTotalQuotes();

        Tag InsertTag(Tag tag);
        void UpdateTag(Tag tag);
        List<Tag> GetTags(string search = null);
        List<Tag> GetTagsForQuote(Quote quote);
        void UpdateTags(Quote qt, IEnumerable<string> tags);
        int GetQuotesCount(Tag tag);

        Bundle Export();
        void Import(Bundle data);
        void DeleteAuthor(Author author);
        void DeleteTag(Tag tag);
    }

}