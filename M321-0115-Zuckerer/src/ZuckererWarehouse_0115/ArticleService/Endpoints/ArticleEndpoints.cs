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

            group.MapGet("/List", async (ArticleDb db) =>
                await db.Articles.Select(x => ListArticlesDTO.FromArticle(x)).ToListAsync()
            )
            .WithName("List")
            .WithOpenApi();

            group.MapGet("/GetById/{id}", async (string id, ArticleDb db) =>
                await db.Articles.FindAsync(id)
                    is Article article
                        ? Results.Ok(GetArticleDTO.FromArticle(article))
                        : Results.NotFound()
            )
            .WithName("GetArticleById")
            .WithOpenApi();

            group.MapPost("/New", async (NewArticleDTO dto, ArticleDb db, MqSender mq) =>
            {
                var dbObj = new Article { ArticleNumber = dto.ArticleNumber, ShortDesc = dto.ShortDescription, Price = dto.Price };
                db.Articles.Add(dbObj);
                await db.SaveChangesAsync();
                mq.SendAsJson("ArticleService.Command.NewArticle", new { dbObj.Id, dto.ArticleNumber, dto.ShortDescription, dto.Price });
                return Results.Created($"/GetArticleById/{dbObj.Id.ToString()}", GetArticleDTO.FromArticle(dbObj));
            })
            .WithName("NewArticle")
            .WithOpenApi();

            group.MapPost("/Update/{id}", async (Guid id, UpdateArticleDTO dto, ArticleDb db, MqSender mq) =>
            {
                var dbObj = await db.Articles.FindAsync(id);
                if (dbObj == null)
                {
                    return Results.NotFound();
                }
                dbObj.ShortDesc = dto.ShortDescription;
                dbObj.Price = dto.Price;
                await db.SaveChangesAsync();
                mq.SendAsJson("ArticleService.Command.UpdateArticle", new { id, dto.CategoryId, dto.ShortDescription, dto.LongDescription, dto.Price, dto.Weight, dto.PackageDimensionH, dto.PackageDimensionL, dto.PackageDimensionW });
                return Results.Ok();
            })
            .WithName("UpdateArticle")
            .WithOpenApi();

            group.MapPost("/ProcessGoodsIn", async (GoodsInDTO dto, ArticleDb db, MqSender mq) =>
            {
                foreach (var receivedGood in dto.ReceivedGoods)
                {
                    var article = await db.Articles.FindAsync(receivedGood.Id);
                    if (article == null)
                    {
                        return Results.BadRequest();
                    }
                    article.QuantityInStock += receivedGood.Quantity;
                }
                await db.SaveChangesAsync();
                mq.SendAsJson("ArticleService.Command.ProcessGoodsIn", dto);
                return Results.Ok();
            })
            .WithName("ProcessGoodsIn")
            .WithOpenApi();

            group.MapPost("/ProcessGoodsOut", async (GoodsOutDTO dto, ArticleDb db, MqSender mq) =>
            {
                using var tx = await db.Database.BeginTransactionAsync();

                var article = await db.Articles.FindAsync(dto.ArticleId);
                if (article == null)
                {
                    return Results.BadRequest();
                }

                if (article.QuantityInStock < dto.Quantity)
                {
                    return Results.Conflict("Not enough goods in stock");
                }
                article.QuantityInStock -= dto.Quantity;

                Console.WriteLine($"ProcessGoodsOut: ArticleId={dto.ArticleId} Quantity={dto.Quantity} OrderId={dto.OrderId}");
                await db.SaveChangesAsync();
                mq.SendAsJson("ArticleService.Command.ProcessGoodsOut", dto);
                await tx.CommitAsync();
                return Results.Ok();
            })
            .WithName("ProcessGoodsOut")
            .WithOpenApi();
        }
    }

    public record ListArticlesDTO(string id, string ArticleNumber, string ShortDescription, int Price)
    {
        public static ListArticlesDTO FromArticle(Article article)
        {
            return new ListArticlesDTO(article.Id.ToString().ToLower(), article.ArticleNumber, article.ShortDesc, article.Price);
        }
    }

    public record GetArticleDTO(string id, string ArticleNumber, string ShortDescription, string LongDescription, int Price)
    {
        public static GetArticleDTO FromArticle(Article article)
        {
            return new GetArticleDTO(
                article.Id.ToString().ToLower(),
                article.ArticleNumber,
                article.ShortDesc,
                article.LongDesc ?? "",
                article.Price);
        }
    };

    public record NewArticleDTO(string ArticleNumber, string ShortDescription, int Price);

    public record UpdateArticleDTO(string ShortDescription, int Price, string? LongDescription, Guid? CategoryId, int? Weight, int? PackageDimensionW, int? PackageDimensionL, int? PackageDimensionH);

    public record GoodsInDTOListItem(Guid Id, int Quantity);
    public record GoodsInDTO(List<GoodsInDTOListItem> ReceivedGoods);

    public record GoodsOutDTO(Guid ArticleId, int Quantity, Guid OrderId);
}
