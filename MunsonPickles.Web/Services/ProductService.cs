using MunsonPickles.Shared.Models;

namespace MunsonPickles.Web.Services;

public class ProductService
{
    private readonly HttpClient _productClient;

    public ProductService(HttpClient httpClient, IConfiguration config)
    {
        _productClient = httpClient;
        _productClient.BaseAddress = new Uri(config["DownstreamApi:BaseUrl"]!);
    }

    public async Task<IEnumerable<Product>?> GetAllProductsAsync()
    {
        var products = await _productClient.GetFromJsonAsync<IEnumerable<Product>>("/products");
        return products;
    }
}