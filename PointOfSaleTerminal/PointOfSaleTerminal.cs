using System;
using System.Collections.Generic;

namespace PointOfSale
{
    public class PointOfSaleTerminal
    {
        private readonly IProductsProvider productsProvider;
        private Dictionary<char, ScannedItem> scannedItems = new Dictionary<char, ScannedItem>();
        private Lazy<Dictionary<char, Product>> products;

        private Dictionary<char, Product> LoadProducts()
        {
            return productsProvider.GetProducts();
        }

        public PointOfSaleTerminal(IProductsProvider productsProvider)
        {
            this.productsProvider = productsProvider ?? throw new ArgumentNullException(nameof(productsProvider));
            products = new Lazy<Dictionary<char, Product>>(LoadProducts);
        }

        public void ScanProduct(char productCode)
        {
            if (scannedItems.ContainsKey(productCode))
            {
                scannedItems[productCode].Count++;
            }
            else
            {
                if (!products.Value.ContainsKey(productCode))
                {
                    throw new ProductNotFoundException();
                }
                scannedItems.Add( productCode, new ScannedItem { ItemCode = productCode, Count = 1 } );
            }
        }

        public decimal CalculateTotal()
        {
            decimal result = 0;
            foreach(var key in scannedItems.Keys)
            {
                var scannedItem = scannedItems[key];
                var product = products.Value[scannedItem.ItemCode];
                var total = product.Price * scannedItem.Count;

                if (product.VolumePrice > 0 && scannedItem.Count >= product.VolumeItemCount)
                {
                    var remainder = scannedItem.Count % product.VolumeItemCount;
                    total = (scannedItem.Count / product.VolumeItemCount) * product.VolumePrice + remainder * product.Price;
                }

                result += total;
            }

            return result;
        }

    }
}
