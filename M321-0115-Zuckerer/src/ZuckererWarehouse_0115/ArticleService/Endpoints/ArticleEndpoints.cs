using ArticleService.Model;
using Microsoft.EntityFrameworkCore;
using MqLibrary;

namespace ArticleService.Endpoints
{
    public static class ArticleEndpoints
    {
        public static void MapArticleEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/Article").WithTags(nameof(Article));

            group.MapGet("", async (ArticleDb db) =>
                await db.Articles.ToListAsync()
            )
            .WithName("List")
            .WithOpenApi();

            group.MapGet("/{id}", async (string id, ArticleDb db) =>
            {
                if (Guid.TryParse(id, out var guidId))
                {
                    var article = await db.Articles.FindAsync(guidId);
                    return article != null ? Results.Ok(article) : Results.NotFound();
                }
                else
                {
                    return Results.BadRequest("Invalid GUID format.");
                }
            }
            )
            .WithName("GetArticleById")
            .WithOpenApi();

            group.MapPost("", async (Article dto, ArticleDb db) =>
            {
                var dbObj = new Article { ArticleNumber = dto.ArticleNumber, ShortDesc = dto.ShortDesc, Price = dto.Price };
                db.Articles.Add(dbObj);
                await db.SaveChangesAsync();
                return Results.Created($"/{dbObj.Id.ToString()}", dbObj);
            })
            .WithName("NewArticle")
            .WithOpenApi();

            group.MapPut("/{id}", async (Guid id, Article dto, ArticleDb db) =>
            {
                var dbObj = await db.Articles.FindAsync(id);
                if (dbObj == null)
                {
                    return Results.NotFound();
                }
                await db.SaveChangesAsync();
                return Results.Ok();
            })
            .WithName("UpdateArticle")
            .WithOpenApi();
            
            group.MapDelete("/{id}", async (Guid id, ArticleDb db) =>
            {
                var dbObj = await db.Articles.FindAsync(id);
                if (dbObj == null)
                {
                    return Results.NotFound();
                }
                db.Articles.Remove(dbObj);
                await db.SaveChangesAsync();
                return Results.Ok();
            })
            .WithName("DeleteArticle")
            .WithOpenApi();
        }
    }
    
}
