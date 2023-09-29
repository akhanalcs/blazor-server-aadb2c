using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using MunsonPickles.Shared.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AZURE ADB2C Setup - Start
//https://learn.microsoft.com/en-us/aspnet/core/blazor/security/webassembly/hosted-with-azure-active-directory-b2c?view=aspnetcore-7.0&viewFallbackFrom=aspnetcore-8.0#authentication-service-support
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));

//https://learn.microsoft.com/en-us/aspnet/core/blazor/security/webassembly/hosted-with-azure-active-directory-b2c?view=aspnetcore-7.0#configure-useridentityname
builder.Services.Configure<JwtBearerOptions>(
    JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters.NameClaimType = "name";
    });

builder.Services.AddAuthorization();
// AZURE ADB2C Setup - End

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("user_read", policy =>
        policy
            //.RequireRole("user")
            .RequireClaim("scope", "read"));

builder.Services.AddAntiforgery();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

//Add your endpoints after these 2 calls ☝️ to protect them

// Add my pipeline stuffs
app.MapGet("/public", () => $"Time is {DateTime.UtcNow}.");
app.MapGet("/products", () =>
    {
        var products = new List<Product>
        {
            new Product
            {
                Id = 1,
                Description = "unBeetable",
                Name = "Red Pickled Beets",
                ProductType = "Pickle"
            },
            new Product
            {
                Id = 2,
                Description = "Sweet and a treat to make your toast the most",
                Name = "Strawberry Preserves",
                ProductType = "Preserves"
            }
        };
        
        return TypedResults.Ok(products);
    })
    .RequireAuthorization("user_read");

app.Run();