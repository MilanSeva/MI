namespace MahantInv.Web.Infrastructure.Dtos.Category
{
    /// <summary>
    /// Category as returned by the typeahead search and embedded in product details.
    /// </summary>
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
