using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebStore.Models
{
    public class Cart
    {
        private List<CartLine> cartLines = new List<CartLine>();

        public void AddItem(ProductModel product, int quantity)
        {
            CartLine line = cartLines
                .Where(p => p.Product.ProductId == product.ProductId)
                .FirstOrDefault();
            if (line == null)
                cartLines.Add(new CartLine { Product = product, Quantity = quantity });
            else
                line.Quantity += quantity;
        }

        public void RemoveLine(ProductPhoto product)
        {
            cartLines.RemoveAll(p => p.Product.ProductId == product.ProductId);
        }

        public double ComputeValue()
        {
            return cartLines.Sum(p => p.Product.Price * p.Quantity);
        }

        public void Clear()
        {
            cartLines.Clear();
        }

        public IEnumerable<CartLine> Lines
        {
            get { return cartLines; }
        }
    }

    public class CartLine
    {
        public ProductModel Product { get; set; }
        public int Quantity { get; set; }
    }
}