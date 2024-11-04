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
                await db.Articles.Include(x => x.Category).Select(x => ListArticlesDTO.FromArticle(x)).ToListAsync()
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
                Category? category = null;
                if (dto.CategoryId != null)
                {
                    category = await db.Categories.FindAsync(dto.CategoryId);
                    if (category == null)
                    {
                        return Results.BadRequest();
                    }
                }
                var dbObj = await db.Articles.FindAsync(id);
                if (dbObj == null)
                {
                    return Results.NotFound();
                }
                dbObj.ShortDesc = dto.ShortDescription;
                dbObj.LongDesc = dto.LongDescription;
                dbObj.Price = dto.Price;
                dbObj.Weight = dto.Weight;
                dbObj.PackageDimensionH = dto.PackageDimensionH;
                dbObj.PackageDimensionL = dto.PackageDimensionL;
                dbObj.PackageDimensionW = dto.PackageDimensionW;
                dbObj.Category = category;
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

    public record ListArticlesDTO(string id, string ArticleNumber, string ShortDescription, int Price, string? Category)
    {
        public static ListArticlesDTO FromArticle(Article article)
        {
            return new ListArticlesDTO(article.Id.ToString().ToLower(), article.ArticleNumber, article.ShortDesc, article.Price, article.Category?.Name);
        }
    }

    public record GetArticleDTO(string id, string ArticleNumber, string ShortDescription, string LongDescription, string PackageSize, string PackageWeight, int Price)
    {
        public static GetArticleDTO FromArticle(Article article)
        {
            var pkgSize = article.PackageDimensionH != null && article.PackageDimensionW != null && article.PackageDimensionL != null ? $"{article.PackageDimensionL}mm x {article.PackageDimensionW}mm x {article.PackageDimensionH}mm" : "not specified";
            var pkgWeight = article.Weight == null ? "not specified" : (
                article.Weight <= 1000 ? $"{article.Weight}g" : $"{article.Weight / 1000}kg"
                );

            return new GetArticleDTO(
                article.Id.ToString().ToLower(),
                article.ArticleNumber,
                article.ShortDesc,
                article.LongDesc ?? "",
                pkgSize,
                pkgWeight,
                article.Price);
        }
    };

    public record NewArticleDTO(string ArticleNumber, string ShortDescription, int Price);

    public record UpdateArticleDTO(string ShortDescription, int Price, string? LongDescription, Guid? CategoryId, int? Weight, int? PackageDimensionW, int? PackageDimensionL, int? PackageDimensionH);

    public record GoodsInDTOListItem(Guid Id, int Quantity);
    public record GoodsInDTO(List<GoodsInDTOListItem> ReceivedGoods);

    public record GoodsOutDTO(Guid ArticleId, int Quantity, Guid OrderId);
}
