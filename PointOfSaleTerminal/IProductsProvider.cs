using System.Collections.Generic;

namespace PointOfSale
{
    public interface IProductsProvider
    {
        Dictionary<char, Product> GetProducts();
    }
}
