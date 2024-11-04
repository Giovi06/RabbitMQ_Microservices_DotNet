using ArticleService.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MqLibrary;

namespace ArticleService.Endpoints
{
    public static class CategoryEndpoints
    {
        public static void MapCategoryEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/Category/").WithTags(nameof(Category));

            group.MapGet("/List", async (ArticleDb db) =>
                await db.Categories.Select(x => ListCategoriesDTO.FromCategory(x)).ToListAsync()
            )
            .WithName("ListCategories")
            .WithOpenApi();


            group.MapGet("/GetById/{id}", async (string id, ArticleDb db) =>
                await db.Categories.FindAsync(Guid.Parse(id))
                    is Category category
                        ? Results.Ok(GetCategoryDTO.FromCategory(category))
                        : Results.NotFound()
            )
            .WithName("GetCategoryById")
            .WithOpenApi();


            group.MapGet("/GetByName/{name}", async (string name, ArticleDb db) =>
                await db.Categories.Where((x) => x.Name.ToLower().Equals(name.ToLower())).SingleOrDefaultAsync()
                    is Category category
                        ? Results.Ok(GetCategoryDTO.FromCategory(category))
                        : Results.NotFound()
            )
            .WithName("GetCategoryByName")
            .WithOpenApi();


            group.MapPost("/New", async (NewCategoryDTO dto, ArticleDb db, MqSender mq) =>
            {
                var dbObj = new Category { Name = dto.Name };
                db.Categories.Add(dbObj);
                await db.SaveChangesAsync();
                mq.SendAsJson("ArticleService.Command.NewCategory", new { dbObj.Id, dto.Name });
                return Results.Created($"/GetCategoryById/{dbObj.Id}", GetCategoryDTO.FromCategory(dbObj));
            })
            .WithName("NewCategory")
            .WithOpenApi();


            group.MapPost("/Update/{id}", async (Guid id, UpdateCategoryDTO dto, ArticleDb db, MqSender mq) =>
            {
                var affected = await db.Categories
                    .Where(x => x.Id == id)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(m => m.Name, dto.Name)
                        .SetProperty(m => m.Description, dto.Description)
                    );
                if (affected > 0)
                {
                    mq.SendAsJson("ArticleService.Command.UpdateCategory", new { id, dto.Name, dto.Description });
                }
                return affected == 1 ? Results.Ok() : Results.NotFound();
            })
            .WithName("UpdateCategory")
            .WithOpenApi();



            group.MapPost("/Delete/{id}", async (Guid id, DeleteCategoryDTO dto, ArticleDb db, MqSender mq) =>
            {
                var oldCategory = await db.Categories.FindAsync(id);
                if (oldCategory == null)
                {
                    return Results.NotFound();
                }
                Category? newCategory = null;
                if (dto.MoveArticlesToCategory != null)
                {
                    newCategory = await db.Categories.FindAsync(dto.MoveArticlesToCategory);
                    if (newCategory == null)
                    {
                        return Results.BadRequest();
                    }
                }
                foreach (var article in await db.Articles.Where(x => x.Category == oldCategory).ToListAsync())
                {
                    article.Category = newCategory;
                }
                db.Categories.Remove(oldCategory);
                await db.SaveChangesAsync();
                mq.SendAsJson("ArticleService.Command.DeleteCategory", new { id, dto.MoveArticlesToCategory });
                return Results.NoContent();
            })
            .WithName("DeleteCategory")
            .WithOpenApi();
        }


    }

    public record NewCategoryDTO(string Name);
    public record GetCategoryDTO(string Id, string Name, string? Description)
    {
        public static GetCategoryDTO FromCategory(Category category)
        {
            return new GetCategoryDTO(category.Id.ToString().ToLower(), category.Name, category.Description);
        }
    }
    public record ListCategoriesDTO(string Id, string Name, string? Description)
    {
        public static ListCategoriesDTO FromCategory(Category category)
        {
            return new ListCategoriesDTO(category.Id.ToString().ToLower(), category.Name, category.Description);
        }
    }
    public record UpdateCategoryDTO(string Name, string? Description);
    public record DeleteCategoryDTO(Guid? MoveArticlesToCategory);
}
