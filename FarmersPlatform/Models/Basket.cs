namespace FarmersPlatform.Models
{
    public class Basket
    {
        private static Basket instance;

        private List<Product> products;
        private double totalPrice;
        private int numberOfItems;

        private Basket()
        {
            products = new List<Product>();
            totalPrice = 0;
            numberOfItems = 0;
        }

        public static Basket GetInstance()
        {
            if (instance == null)
            {
                instance = new Basket();
            }
            return instance;
        }

        public void UpdateTotalPrice(double price)
        {
            totalPrice = price;
        }

        public void AddProduct(Product product)
        {
            var existingProduct = products.FirstOrDefault(p => p.Name == product.Name);

            if (existingProduct == null)
            {
                // If the product is not in the basket, set the quantity to 1
                product.Quantity = 1;
                products.Add(product);
                numberOfItems++;
            }
            else
            {
                // If the product is already in the basket, increment the quantity
                existingProduct.Quantity++;
            }
            totalPrice += product.Price;
        }

        public void RemoveProduct(Product product)
        {
            products.Remove(product);
            totalPrice -= product.Price;
            numberOfItems--;
        }

        public List<Product> GetProducts()
        {
            return products;
        }

        public double GetTotalPrice()
        {
            return totalPrice;
        }

        public int GetNumberOfItems()
        {
            return numberOfItems;
        }
    }
}
