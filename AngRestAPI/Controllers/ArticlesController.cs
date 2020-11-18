using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AngRestAPI.Models;
using AngRestAPI.Response;


namespace AngRestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly angContext _context;

        public ArticlesController(angContext context)
        {
            _context = context;
        }

        // GET: api/Articles
        [HttpGet]
        public IEnumerable<Article> GetArticle()
        {

            return _context.Article;
        }

        [HttpGet("{page}/{pageSize}")]

        public IActionResult GetArticle(int page = 1, int pageSize = 5)
        {

            try
            {
                IQueryable<Article> query;
                query = _context.Article.Include(x => x.Category).Include(y => y.Comment).OrderByDescending(z => z.PublishDate);
                int totalCount = query.Count();
                var articlesResponse = query.Skip((pageSize * (page - 1))).Take(5).ToList().Select(x => new ArticleResponse()
                {
                    Id = x.Id,
                    Title = x.Title,
                    ContentSummary = x.ContentSummary,
                    ContentMain = x.ContentMain,
                    Picture = x.Picture,
                    ViewCount = x.ViewCount,
                    CommentCount = x.Comment.Count,
                    Category = new CategoryResponse() { Id = x.Category.Id, Name = x.Category.Name }

                });

                var result = new
                {
                    TotalCount = totalCount,
                    Articles = articlesResponse
                };
                return Ok(result);

            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetArticleWithCategory/{categoryId}/{page}/{pageSize}")]
        public IActionResult GetArticleWithCategory(int categoryId,int page=1, int pageSize=5)
        {
            IQueryable<Article> query = _context.Article.Include(x => x.Category).Include(y => y.Comment).Where(z => z.CategoryId == categoryId).OrderByDescending(x => x.PublishDate);
            var queryResult = ArticlePagination(query, page, pageSize);
            var result = new
            {
                TotalCount = queryResult.Item2,
                Articles = queryResult.Item1
            };
            return Ok(result);
        }

        // GET: api/Articles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticle([FromRoute] int id)
        {

            var article = _context.Article.Include(x => x.Category).Include(y => y.Comment).FirstOrDefault(z => z.Id == id);
            if(article == null)
            {
                return NotFound();
            }

            ArticleResponse articleResponse = new ArticleResponse()
            {
                Id = article.Id,
                Title = article.Title,
                ContentMain = article.ContentMain,
                ContentSummary = article.ContentSummary,
                Picture = article.Picture,
                PublishDate = article.PublishDate,
                ViewCount = article.ViewCount,
                CommentCount = article.Comment.Count,
                Category = new CategoryResponse() { Id = article.Category.Id, Name = article.Category.Name }
            };
            return Ok(articleResponse);

        }

        // PUT: api/Articles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticle([FromRoute] int id, [FromBody] Article article)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != article.Id)
            {
                return BadRequest();
            }

            _context.Entry(article).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Articles
        [HttpPost]
        public async Task<IActionResult> PostArticle([FromBody] Article article)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Article.Add(article);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetArticle", new { id = article.Id }, article);
        }

        // DELETE: api/Articles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var article = await _context.Article.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            _context.Article.Remove(article);
            await _context.SaveChangesAsync();

            return Ok(article);
        }

        private bool ArticleExists(int id)
        {
            return _context.Article.Any(e => e.Id == id);
        }
        public System.Tuple<IEnumerable<ArticleResponse>, int> ArticlePagination(IQueryable<Article> query, int page, int pageSize)
        {
            int totalCount = query.Count();
            var articlesResponse = query.Skip((pageSize * (page - 1))).Take(5).ToList().Select(x => new ArticleResponse()
            {
                Id = x.Id,
                Title = x.Title,
                ContentSummary = x.ContentSummary,
                ContentMain = x.ContentMain,
                Picture = x.Picture,
                ViewCount = x.ViewCount,
                CommentCount = x.Comment.Count,
                Category = new CategoryResponse() { Id = x.Category.Id, Name = x.Category.Name }

            });
            return new System.Tuple<IEnumerable<ArticleResponse>, int>(articlesResponse, totalCount);
        }

    }
}