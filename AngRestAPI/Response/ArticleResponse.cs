using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngRestAPI.Response
{
    public class ArticleResponse
    {
        public int Id { get; internal set; }
        public string Title { get; internal set; }
        public string ContentMain { get; internal set; }
        public string ContentSummary { get; internal set; }
        public DateTime PublishDate { get; internal set; }
        public string Picture { get; internal set; }
        public int ViewCount { get; internal set; }
        public CategoryResponse Category { get; internal set; }
        public int CommentCount { get; internal set; }
    }
}
