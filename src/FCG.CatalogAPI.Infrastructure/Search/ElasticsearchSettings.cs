namespace FCG.CatalogAPI.Infrastructure.Search;

public class ElasticsearchSettings
{
    public string Uri { get; set; } = string.Empty;
    public string IndexName { get; set; } = "fcg-games";
}
