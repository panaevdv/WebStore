using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebStore.Models
{
    public class Cart
    {
        private List<CartLine> cartLines = new List<CartLine>();

        public void AddItem(ProductModel product, int quantity)
        {
            if(quantity<0)
            {
                RemoveItem(product, -quantity);
                return;
            }
            CartLine line = cartLines
                .Where(p => p.Product.ProductId == product.ProductId)
                .FirstOrDefault();
            if (line == null)
                cartLines.Add(new CartLine { Product = product, Quantity = quantity, ProductId = product.ProductId });
            else
                line.Quantity += quantity;
        }

        public void RemoveItem(ProductModel product, int quantity)
        {
            CartLine line = cartLines
                .Where(p => p.Product.ProductId == product.ProductId)
                .FirstOrDefault();
            if (line != null)
            {
                if (line.Quantity-quantity >= 1)
                    line.Quantity -= quantity;
                else
                    cartLines.Remove(line);
            }
        }

        public int GetTotalItems()
        {
            int qnt = 0;
            foreach(CartLine line in cartLines)
            {
                qnt += line.Quantity;
            }
            return qnt;
        }

        public void RemoveLine(ProductModel product)
        {
            cartLines.RemoveAll(p => p.Product.ProductId == product.ProductId);
        }

        public double TotalValue()
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
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual ProductModel Product { get; set; }
        public int Quantity { get; set; }
        public int PurchaseId { get; set; }
        public Purchase Purchase { get; set; }
    }
}