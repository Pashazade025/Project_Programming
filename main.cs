// Marketplace Checkout System
// A comprehensive grocery store checkout system implementing the requirements
// including inventory management, payment processing, loyalty points, and receipt generation
// Created as per UML diagrams provided

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroceryStoreSystem
{
    #region Models

    /// <summary>
    /// Represents a product in the grocery store inventory
    /// </summary>
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Barcode { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; }
        public int PointsEarned { get; set; } // Points earned when purchasing this item

        public Item(int id, string name, decimal price, string barcode, int stock, string category, int pointsEarned)
        {
            Id = id;
            Name = name;
            Price = price;
            Barcode = barcode;
            Stock = stock;
            Category = category;
            PointsEarned = pointsEarned;
        }

        public override string ToString()
        {
            return $"{Name} - ${Price:F2}";
        }
    }

    /// <summary>
    /// Represents a customer in the grocery store system
    /// </summary>
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal CashBalance { get; set; }
        public decimal CardBalance { get; set; }
        public int LoyaltyPoints { get; set; }
        public List<Receipt> PurchaseHistory { get; set; }

        public Customer(int id, string name, decimal cashBalance, decimal cardBalance, int loyaltyPoints)
        {
            Id = id;
            Name = name;
            CashBalance = cashBalance;
            CardBalance = cardBalance;
            LoyaltyPoints = loyaltyPoints;
            PurchaseHistory = new List<Receipt>();
        }

        /// <summary>
        /// Deducts the specified amount from cash balance
        /// </summary>
        /// <param name="amount">Amount to deduct</param>
        /// <returns>True if successful, false if insufficient funds</returns>
        public bool DeductFromCash(decimal amount)
        {
            if (CashBalance >= amount)
            {
                CashBalance -= amount;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Deducts the specified amount from card balance
        /// </summary>
        /// <param name="amount">Amount to deduct</param>
        /// <returns>True if successful, false if insufficient funds</returns>
        public bool DeductFromCard(decimal amount)
        {
            if (CardBalance >= amount)
            {
                CardBalance -= amount;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds loyalty points to the customer's account
        /// </summary>
        /// <param name="points">Points to add</param>
        public void AddLoyaltyPoints(int points)
        {
            LoyaltyPoints += points;
        }

        /// <summary>
        /// Uses loyalty points for a discount
        /// </summary>
        /// <param name="points">Points to use</param>
        /// <returns>True if successful, false if insufficient points</returns>
        public bool UseLoyaltyPoints(int points)
        {
            if (LoyaltyPoints >= points)
            {
                LoyaltyPoints -= points;
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Represents a cart item in the checkout process
    /// </summary>
    public class CartItem
    {
        public Item Item { get; set; }
        public int Quantity { get; set; }

        public CartItem(Item item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }

        public decimal GetTotalPrice()
        {
            return Item.Price * Quantity;
        }

        public int GetTotalPoints()
        {
            return Item.PointsEarned * Quantity;
        }
    }

    /// <summary>
    /// Represents a receipt generated after a successful purchase
    /// </summary>
    public class Receipt
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public Customer Customer { get; set; }
        public List<CartItem> Items { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string PaymentMethod { get; set; }
        public int PointsEarned { get; set; }
        public int PointsRedeemed { get; set; }

        public Receipt(int id, Customer customer, List<CartItem> items, string paymentMethod, int pointsRedeemed)
        {
            Id = id;
            Date = DateTime.Now;
            Customer = customer;
            Items = new List<CartItem>(items);
            PaymentMethod = paymentMethod;
            PointsRedeemed = pointsRedeemed;
            
            // Calculate totals
            SubTotal = Items.Sum(item => item.GetTotalPrice());
            Tax = Math.Round(SubTotal * 0.08m, 2); // 8% tax rate
            Total = SubTotal + Tax;
            
            // Calculate points earned (if not redeemed)
            PointsEarned = pointsRedeemed > 0 ? 0 : Items.Sum(item => item.GetTotalPoints());
        }

        public string GenerateReceiptText()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("===========================================");
            sb.AppendLine("             GROCERY STORE                 ");
            sb.AppendLine("===========================================");
            sb.AppendLine($"Receipt ID: {Id}");
            sb.AppendLine($"Date: {Date}");
            sb.AppendLine($"Customer: {Customer.Name}");
            sb.AppendLine("-------------------------------------------");
            sb.AppendLine("Items:");
            
            foreach (var item in Items)
            {
                sb.AppendLine($"{item.Item.Name} x{item.Quantity} - ${item.Item.Price:F2} each = ${item.GetTotalPrice():F2}");
            }
            
            sb.AppendLine("-------------------------------------------");
            sb.AppendLine($"Subtotal: ${SubTotal:F2}");
            sb.AppendLine($"Tax (8%): ${Tax:F2}");
            
            if (PointsRedeemed > 0)
            {
                decimal discount = PointsRedeemed / 100m; // Each 100 points = $1 discount
                sb.AppendLine($"Points Redeemed: {PointsRedeemed} (${discount:F2} discount)");
                sb.AppendLine($"Total After Discount: ${Total - discount:F2}");
            }
            
            sb.AppendLine($"Total: ${Total:F2}");
            sb.AppendLine($"Payment Method: {PaymentMethod}");
            
            if (PointsEarned > 0)
            {
                sb.AppendLine($"Points Earned This Transaction: {PointsEarned}");
            }
            
            sb.AppendLine($"Current Points Balance: {Customer.LoyaltyPoints}");
            sb.AppendLine("===========================================");
            sb.AppendLine("          Thank You For Shopping!          ");
            sb.AppendLine("===========================================");
            
            return sb.ToString();
        }
    }

    /// <summary>
    /// Represents a cashier station in the grocery store
    /// </summary>
    public class CashierStation
    {
        public int Id { get; set; }
        public bool IsOpen { get; set; }
        public Queue<Customer> Line { get; set; }

        public CashierStation(int id)
        {
            Id = id;
            IsOpen = true;
            Line = new Queue<Customer>();
        }

        public int GetLineCount()
        {
            return Line.Count;
        }

        public void AddCustomerToLine(Customer customer)
        {
            Line.Enqueue(customer);
        }

        public Customer ProcessNextCustomer()
        {
            if (Line.Count > 0)
            {
                return Line.Dequeue();
            }
            return null;
        }
    }

    #endregion

    #region Components

    /// <summary>
    /// Handles inventory searching and management
    /// </summary>
    public class InventorySystem
    {
        private List<Item> _inventory;

        public InventorySystem()
        {
            _inventory = new List<Item>();
            InitializeInventory();
        }

        private void InitializeInventory()
        {
            // Add some sample items to the inventory
            _inventory.Add(new Item(1, "Milk", 3.99m, "1001", 50, "Dairy", 4));
            _inventory.Add(new Item(2, "Bread", 2.49m, "1002", 40, "Bakery", 2));
            _inventory.Add(new Item(3, "Eggs", 4.29m, "1003", 30, "Dairy", 4));
            _inventory.Add(new Item(4, "Apples", 0.99m, "1004", 100, "Produce", 1));
            _inventory.Add(new Item(5, "Chicken", 7.99m, "1005", 25, "Meat", 8));
            _inventory.Add(new Item(6, "Rice", 5.49m, "1006", 60, "Grains", 5));
            _inventory.Add(new Item(7, "Pasta", 1.99m, "1007", 80, "Grains", 2));
            _inventory.Add(new Item(8, "Orange Juice", 4.59m, "1008", 35, "Beverages", 5));
            _inventory.Add(new Item(9, "Chocolate", 3.29m, "1009", 70, "Snacks", 3));
            _inventory.Add(new Item(10, "Potato Chips", 2.99m, "1010", 65, "Snacks", 3));
        }

        /// <summary>
        /// Gets all items in the inventory
        /// </summary>
        /// <returns>List of all items</returns>
        public List<Item> GetAllItems()
        {
            return new List<Item>(_inventory);
        }

        /// <summary>
        /// Searches for items by category
        /// </summary>
        /// <param name="category">Category to search for</param>
        /// <returns>List of matching items</returns>
        public List<Item> SearchByCategory(string category)
        {
            return _inventory.Where(item => item.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /// <summary>
        /// Searches for an item by barcode
        /// </summary>
        /// <param name="barcode">Barcode to search for</param>
        /// <returns>Found item or null</returns>
        public Item GetItemByBarcode(string barcode)
        {
            return _inventory.FirstOrDefault(item => item.Barcode == barcode);
        }

        /// <summary>
        /// Updates the stock of an item
        /// </summary>
        /// <param name="itemId">ID of the item</param>
        /// <param name="quantity">Quantity to reduce</param>
        /// <returns>True if successful, false if insufficient stock</returns>
        public bool UpdateStock(int itemId, int quantity)
        {
            var item = _inventory.FirstOrDefault(i => i.Id == itemId);
            if (item != null && item.Stock >= quantity)
            {
                item.Stock -= quantity;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a new item to the inventory
        /// </summary>
        /// <param name="item">Item to add</param>
        public void AddItem(Item item)
        {
            _inventory.Add(item);
        }

        /// <summary>
        /// Removes an item from the inventory
        /// </summary>
        /// <param name="itemId">ID of the item to remove</param>
        /// <returns>True if successful, false if item not found</returns>
        public bool RemoveItem(int itemId)
        {
            var item = _inventory.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                _inventory.Remove(item);
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Handles scanning and processing cart items
    /// </summary>
    public class ScanningSystem
    {
        private InventorySystem _inventorySystem;

        public ScanningSystem(InventorySystem inventorySystem)
        {
            _inventorySystem = inventorySystem;
        }

        /// <summary>
        /// Scans an item by barcode and adds it to the cart
        /// </summary>
        /// <param name="barcode">Barcode to scan</param>
        /// <param name="cart">Cart to add the item to</param>
        /// <returns>True if successful, false if item not found</returns>
        public bool ScanItem(string barcode, List<CartItem> cart)
        {
            var item = _inventorySystem.GetItemByBarcode(barcode);
            if (item != null)
            {
                var existingItem = cart.FirstOrDefault(i => i.Item.Id == item.Id);
                if (existingItem != null)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    cart.Add(new CartItem(item, 1));
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Manually adds an item to the cart
        /// </summary>
        /// <param name="itemId">ID of the item to add</param>
        /// <param name="quantity">Quantity to add</param>
        /// <param name="cart">Cart to add the item to</param>
        /// <returns>True if successful, false if item not found</returns>
        public bool AddItemToCart(int itemId, int quantity, List<CartItem> cart)
        {
            var item = _inventorySystem.GetAllItems().FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                var existingItem = cart.FirstOrDefault(i => i.Item.Id == item.Id);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    cart.Add(new CartItem(item, quantity));
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes an item from the cart
        /// </summary>
        /// <param name="itemId">ID of the item to remove</param>
        /// <param name="cart">Cart to remove the item from</param>
        /// <returns>True if successful, false if item not found in cart</returns>
        public bool RemoveItemFromCart(int itemId, List<CartItem> cart)
        {
            var cartItem = cart.FirstOrDefault(i => i.Item.Id == itemId);
            if (cartItem != null)
            {
                cart.Remove(cartItem);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Updates the quantity of an item in the cart
        /// </summary>
        /// <param name="itemId">ID of the item to update</param>
        /// <param name="newQuantity">New quantity</param>
        /// <param name="cart">Cart to update</param>
        /// <returns>True if successful, false if item not found</returns>
        public bool UpdateCartItemQuantity(int itemId, int newQuantity, List<CartItem> cart)
        {
            var cartItem = cart.FirstOrDefault(i => i.Item.Id == itemId);
            if (cartItem != null)
            {
                if (newQuantity <= 0)
                {
                    cart.Remove(cartItem);
                }
                else
                {
                    cartItem.Quantity = newQuantity;
                }
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Handles payment processing
    /// </summary>
    public class PaymentSystem
    {
        /// <summary>
        /// Processes a payment
        /// </summary>
        /// <param name="customer">Customer making the payment</param>
        /// <param name="amount">Amount to pay</param>
        /// <param name="paymentMethod">Payment method (Cash or Card)</param>
        /// <returns>True if payment successful, false otherwise</returns>
        public bool ProcessPayment(Customer customer, decimal amount, string paymentMethod)
        {
            if (paymentMethod.Equals("Cash", StringComparison.OrdinalIgnoreCase))
            {
                return customer.DeductFromCash(amount);
            }
            else if (paymentMethod.Equals("Card", StringComparison.OrdinalIgnoreCase))
            {
                return customer.DeductFromCard(amount);
            }
            return false; // Invalid payment method
        }

        /// <summary>
        /// Checks if a customer has enough funds for a transaction
        /// </summary>
        /// <param name="customer">Customer to check</param>
        /// <param name="amount">Amount to check for</param>
        /// <param name="paymentMethod">Payment method to check</param>
        /// <returns>True if enough funds, false otherwise</returns>
        public bool CheckFunds(Customer customer, decimal amount, string paymentMethod)
        {
            if (paymentMethod.Equals("Cash", StringComparison.OrdinalIgnoreCase))
            {
                return customer.CashBalance >= amount;
            }
            else if (paymentMethod.Equals("Card", StringComparison.OrdinalIgnoreCase))
            {
                return customer.CardBalance >= amount;
            }
            return false; // Invalid payment method
        }

        /// <summary>
        /// Calculates the total amount for a cart of items
        /// </summary>
        /// <param name="cart">Cart to calculate total for</param>
        /// <returns>Total amount</returns>
        public decimal CalculateTotal(List<CartItem> cart)
        {
            decimal subtotal = cart.Sum(item => item.GetTotalPrice());
            decimal tax = Math.Round(subtotal * 0.08m, 2); // 8% tax rate
            return subtotal + tax;
        }
    }

    /// <summary>
    /// Handles card management including balance checking and updating
    /// </summary>
    public class CardManagementSystem
    {
        /// <summary>
        /// Gets the current balance of a customer's card
        /// </summary>
        /// <param name="customer">Customer to check</param>
        /// <returns>Card balance</returns>
        public decimal GetCardBalance(Customer customer)
        {
            return customer.CardBalance;
        }

        /// <summary>
        /// Verifies if the customer has enough funds on their card
        /// </summary>
        /// <param name="customer">Customer to check</param>
        /// <param name="amount">Amount to check for</param>
        /// <returns>True if enough funds, false otherwise</returns>
        public bool VerifyCardFunds(Customer customer, decimal amount)
        {
            return customer.CardBalance >= amount;
        }

        /// <summary>
        /// Updates the customer's card balance after a transaction
        /// </summary>
        /// <param name="customer">Customer to update</param>
        /// <param name="amount">Amount to deduct</param>
        /// <returns>True if successful, false if insufficient funds</returns>
        public bool UpdateCardBalance(Customer customer, decimal amount)
        {
            if (customer.CardBalance >= amount)
            {
                customer.CardBalance -= amount;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds funds to a customer's card
        /// </summary>
        /// <param name="customer">Customer to update</param>
        /// <param name="amount">Amount to add</param>
        public void AddFundsToCard(Customer customer, decimal amount)
        {
            customer.CardBalance += amount;
        }
    }

    /// <summary>
    /// Handles receipt generation and management
    /// </summary>
    public class ReceiptSystem
    {
        private int _nextReceiptId = 1;

        /// <summary>
        /// Creates a new receipt for a transaction
        /// </summary>
        /// <param name="customer">Customer who made the purchase</param>
        /// <param name="cart">Items purchased</param>
        /// <param name="paymentMethod">Method of payment</param>
        /// <param name="pointsRedeemed">Loyalty points redeemed</param>
        /// <returns>Generated receipt</returns>
        public Receipt CreateReceipt(Customer customer, List<CartItem> cart, string paymentMethod, int pointsRedeemed)
        {
            var receipt = new Receipt(_nextReceiptId++, customer, cart, paymentMethod, pointsRedeemed);
            customer.PurchaseHistory.Add(receipt);
            return receipt;
        }

        /// <summary>
        /// Prints a receipt
        /// </summary>
        /// <param name="receipt">Receipt to print</param>
        /// <returns>Formatted receipt text</returns>
        public string PrintReceipt(Receipt receipt)
        {
            return receipt.GenerateReceiptText();
        }

        /// <summary>
        /// Gets a customer's purchase history
        /// </summary>
        /// <param name="customer">Customer to get history for</param>
        /// <returns>List of receipts</returns>
        public List<Receipt> GetPurchaseHistory(Customer customer)
        {
            return customer.PurchaseHistory;
        }
    }

    /// <summary>
    /// Handles loyalty points management
    /// </summary>
    public class PointSystem
    {
        /// <summary>
        /// Awards loyalty points to a customer
        /// </summary>
        /// <param name="customer">Customer to award points to</param>
        /// <param name="points">Points to award</param>
        public void AwardPoints(Customer customer, int points)
        {
            customer.AddLoyaltyPoints(points);
        }

        /// <summary>
        /// Redeems loyalty points for a discount
        /// </summary>
        /// <param name="customer">Customer redeeming points</param>
        /// <param name="points">Points to redeem</param>
        /// <returns>Amount of discount (1 point = $0.01)</returns>
        public decimal RedeemPoints(Customer customer, int points)
        {
            if (customer.UseLoyaltyPoints(points))
            {
                return points / 100m; // Each 100 points = $1 discount
            }
            return 0;
        }

        /// <summary>
        /// Calculates points to be earned for a purchase
        /// </summary>
        /// <param name="cart">Items in the cart</param>
        /// <returns>Points to be earned</returns>
        public int CalculatePointsForPurchase(List<CartItem> cart)
        {
            return cart.Sum(item => item.GetTotalPoints());
        }

        /// <summary>
        /// Gets a customer's current points balance
        /// </summary>
        /// <param name="customer">Customer to check</param>
        /// <returns>Points balance</returns>
        public int GetPointsBalance(Customer customer)
        {
            return customer.LoyaltyPoints;
        }
    }

    #endregion

    #region Main Checkout System

    /// <summary>
    /// Main checkout system that integrates all other components
    /// </summary>
    public class CheckoutSystem
    {
        private InventorySystem _inventorySystem;
        private ScanningSystem _scanningSystem;
        private PaymentSystem _paymentSystem;
        private CardManagementSystem _cardManagementSystem;
        private ReceiptSystem _receiptSystem;
        private PointSystem _pointSystem;
        private List<CashierStation> _cashierStations;

        public CheckoutSystem()
        {
            _inventorySystem = new InventorySystem();
            _scanningSystem = new ScanningSystem(_inventorySystem);
            _paymentSystem = new PaymentSystem();
            _cardManagementSystem = new CardManagementSystem();
            _receiptSystem = new ReceiptSystem();
            _pointSystem = new PointSystem();
            _cashierStations = new List<CashierStation>();
            
            // Initialize cashier stations
            for (int i = 1; i <= 3; i++)
            {
                _cashierStations.Add(new CashierStation(i));
            }
        }

        /// <summary>
        /// Gets all items available for browsing
        /// </summary>
        /// <returns>List of all items</returns>
        public List<Item> BrowseItems()
        {
            return _inventorySystem.GetAllItems();
        }

        /// <summary>
        /// Scans an item and adds it to the cart
        /// </summary>
        /// <param name="barcode">Barcode to scan</param>
        /// <param name="cart">Cart to add the item to</param>
        /// <returns>True if successful, false if item not found</returns>
        public bool ScanItem(string barcode, List<CartItem> cart)
        {
            return _scanningSystem.ScanItem(barcode, cart);
        }

        /// <summary>
        /// Adds an item to the cart manually
        /// </summary>
        /// <param name="itemId">ID of the item to add</param>
        /// <param name="quantity">Quantity to add</param>
        /// <param name="cart">Cart to add the item to</param>
        /// <returns>True if successful, false if item not found</returns>
        public bool AddItemToCart(int itemId, int quantity, List<CartItem> cart)
        {
            return _scanningSystem.AddItemToCart(itemId, quantity, cart);
        }

        /// <summary>
        /// Removes an item from the cart
        /// </summary>
        /// <param name="itemId">ID of the item to remove</param>
        /// <param name="cart">Cart to remove the item from</param>
        /// <returns>True if successful, false if item not found</returns>
        public bool RemoveItemFromCart(int itemId, List<CartItem> cart)
        {
            return _scanningSystem.RemoveItemFromCart(itemId, cart);
        }

        /// <summary>
        /// Checks if there are enough funds for a transaction
        /// </summary>
        /// <param name="customer">Customer making the purchase</param>
        /// <param name="cart">Items in the cart</param>
        /// <param name="paymentMethod">Payment method</param>
        /// <param name="pointsToRedeem">Points to redeem for discount</param>
        /// <returns>True if enough funds, false otherwise</returns>
        public bool CheckFunds(Customer customer, List<CartItem> cart, string paymentMethod, int pointsToRedeem)
        {
            decimal total = _paymentSystem.CalculateTotal(cart);
            
            // Apply points discount if any
            if (pointsToRedeem > 0)
            {
                decimal discount = _pointSystem.RedeemPoints(customer, pointsToRedeem);
                total -= discount;
            }
            
            return _paymentSystem.CheckFunds(customer, total, paymentMethod);
        }

        /// <summary>
        /// Processes a checkout transaction
        /// </summary>
        /// <param name="customer">Customer making the purchase</param>
        /// <param name="cart">Items in the cart</param>
        /// <param name="paymentMethod">Payment method</param>
        /// <param name="pointsToRedeem">Points to redeem for discount</param>
        /// <returns>Receipt if successful, null otherwise</returns>
        public Receipt ProcessCheckout(Customer customer, List<CartItem> cart, string paymentMethod, int pointsToRedeem)
        {
            // Calculate total
            decimal total = _paymentSystem.CalculateTotal(cart);
            
            // Apply points discount if any
            decimal discount = 0;
            if (pointsToRedeem > 0 && customer.LoyaltyPoints >= pointsToRedeem)
            {
                discount = _pointSystem.RedeemPoints(customer, pointsToRedeem);
                total -= discount;
            }
            else
            {
                // Reset points to redeem if not enough
                pointsToRedeem = 0;
            }
            
            // Check funds and process payment
            if (_paymentSystem.CheckFunds(customer, total, paymentMethod))
            {
                if (_paymentSystem.ProcessPayment(customer, total, paymentMethod))
                {
                    // Update inventory stock
                    foreach (var item in cart)
                    {
                        _inventorySystem.UpdateStock(item.Item.Id, item.Quantity);
                    }
                    
                    // Create receipt
                    var receipt = _receiptSystem.CreateReceipt(customer, cart, paymentMethod, pointsToRedeem);
                    
                    // Award points if not redeeming
                    if (pointsToRedeem == 0)
                    {
                        int pointsEarned = _pointSystem.CalculatePointsForPurchase(cart);
                        _pointSystem.AwardPoints(customer, pointsEarned);
                    }
                    
                    return receipt;
                }
            }
            
            return null; // Checkout failed
        }

        /// <summary>
        /// Gets the number of cashier stations currently open
        /// </summary>
        /// <returns>Number of open cashiers</returns>
        public int GetOpenCashierCount()
        {
            return _cashierStations.Count(c => c.IsOpen);
        }

        /// <summary>
        /// Gets the current wait time based on the number of customers in line
        /// </summary>
        /// <returns>Estimated wait time in minutes</returns>
        public int GetEstimatedWaitTime()
        {
            int totalCustomers = _cashierStations.Sum(c => c.GetLineCount());
            int openCashiers = GetOpenCashierCount();
            
            if (openCashiers == 0)
                return 0;
                
            // Assume each customer takes about 3 minutes
            return (totalCustomers / openCashiers) * 3;
        }

        /// <summary>
        /// Adds a customer to the shortest line
        /// </summary>
        /// <param name="customer">Customer to add to line</param>
        public void AddCustomerToLine(Customer customer)
        {
            var shortestLine = _cashierStations
                .Where(c => c.IsOpen)
                .OrderBy(c => c.GetLineCount())
                .FirstOrDefault();
                
            if (shortestLine != null)
            {
                shortestLine.AddCustomerToLine(customer);
            }
        }

        /// <summary>
        /// Gets the items to remove if funds are insufficient
        /// </summary>
        /// <param name="cart">Current cart</param>
        /// <param name="availableFunds">Available funds</param>
        /// <returns>List of suggested items to remove</returns>
        public List<CartItem> SuggestItemsToRemove(List<CartItem> cart, decimal availableFunds)
        {
            decimal total = _paymentSystem.CalculateTotal(cart);
            decimal deficit = total - availableFunds;
            
            if (deficit <= 0)
                return new List<CartItem>(); // No need to remove anything
                
            // Sort items by price (highest first) to suggest removing expensive items first
            List<CartItem> itemsToRemove = new List<CartItem>();
            List<CartItem> sortedItems = cart.OrderByDescending(item => item.Item.Price).ToList();
            
            decimal amountToRemove = 0;
            foreach (var item in sortedItems)
            {
                if (amountToRemove < deficit)
                {
                    itemsToRemove.Add(item);
                    amountToRemove += item.GetTotalPrice();
                }
                else
                {
                    break;
                }
            }
            
            return itemsToRemove;
        }
    }

    #endregion

    #region User Interface

    /// <summary>
    /// Console-based user interface for the checkout system
    /// </summary>
    public class CheckoutConsoleUI
    {
        private CheckoutSystem _checkoutSystem;
        private Customer _currentCustomer;
        private List<CartItem> _currentCart;
        private string _selectedPaymentMethod;
        private int _pointsToRedeem;

        public CheckoutConsoleUI(CheckoutSystem checkoutSystem, Customer customer)
        {
            _checkoutSystem = checkoutSystem;
            _currentCustomer = customer;
            _currentCart = new List<CartItem>();
            _selectedPaymentMethod = "Cash"; // Default payment method
            _pointsToRedeem = 0;
        }

        /// <summary>
        /// Starts the checkout process
        /// </summary>
        public void Start()
        {
            bool exit = false;
            while (!exit)
            {
                DisplayMainMenu();
                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        BrowseItems();
                        break;
                    case "2":
                        AddItem();
                        break;
                    case "3":
                        ScanItem();
                        break;
                    case "4":
                        ChoosePaymentMethod();
                        break;
                    case "5":
                        RedeemPoints();
                        break;
                    case "6":
                        ViewCart();
                        break;
                    case "7":
                        RemoveItem();
                        break;
                    case "8":
                        Checkout();
                        break;
                    case "9":
                        ViewCustomerInfo();
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private void DisplayMainMenu()
        {
            Console.Clear();
            Console.WriteLine("===========================================");
            Console.WriteLine("        GROCERY STORE CHECKOUT SYSTEM      ");
            Console.WriteLine("===========================================");
            Console.WriteLine($"Customer: {_currentCustomer.Name}");
            Console.WriteLine($"Loyalty Points: {_currentCustomer.LoyaltyPoints}");
            Console.WriteLine($"Cash Balance: ${_currentCustomer.CashBalance:F2}");
            Console.WriteLine($"Card Balance: ${_currentCustomer.CardBalance:F2}");
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("1. Browse Items");
            Console.WriteLine("2. Add Item to Cart");
            Console.WriteLine("3. Scan Item");
            Console.WriteLine("4. Choose Payment Method");
            Console.WriteLine("5. Redeem Loyalty Points");
            Console.WriteLine("6. View Cart");
            Console.WriteLine("7. Remove Item from Cart");
            Console.WriteLine("8. Checkout");
            Console.WriteLine("9. View Customer Information");
            Console.WriteLine("0. Exit");
            Console.WriteLine("===========================================");
            Console.WriteLine($"Current Cart: {_currentCart.Count} items");
            decimal total = _currentCart.Sum(item => item.GetTotalPrice());
            Console.WriteLine($"Subtotal: ${total:F2}");
            decimal tax = Math.Round(total * 0.08m, 2);
            Console.WriteLine($"Tax (8%): ${tax:F2}");
            Console.WriteLine($"Total: ${total + tax:F2}");
            Console.WriteLine($"Selected Payment Method: {_selectedPaymentMethod}");
            if (_pointsToRedeem > 0)
            {
                decimal discount = _pointsToRedeem / 100m;
                Console.WriteLine($"Points to Redeem: {_pointsToRedeem} (${discount:F2} discount)");
                Console.WriteLine($"Final Total: ${total + tax - discount:F2}");
            }
            Console.WriteLine("===========================================");
        }

        private void BrowseItems()
        {
            Console.Clear();
            Console.WriteLine("===========================================");
            Console.WriteLine("              BROWSE ITEMS                 ");
            Console.WriteLine("===========================================");
            
            var items = _checkoutSystem.BrowseItems();
            var categories = items.Select(i => i.Category).Distinct().OrderBy(c => c).ToList();
            
            Console.WriteLine("Categories:");
            for (int i = 0; i < categories.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categories[i]}");
            }
            
            Console.Write("\nSelect a category (or 0 to show all): ");
            string input = Console.ReadLine();
            
            if (int.TryParse(input, out int categoryIndex))
            {
                List<Item> filteredItems;
                
                if (categoryIndex == 0)
                {
                    filteredItems = items;
                }
                else if (categoryIndex > 0 && categoryIndex <= categories.Count)
                {
                    string selectedCategory = categories[categoryIndex - 1];
                    filteredItems = items.Where(i => i.Category == selectedCategory).ToList();
                }
                else
                {
                    Console.WriteLine("Invalid category selection.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    return;
                }
                
                Console.WriteLine("\nAvailable Items:");
                Console.WriteLine("-------------------------------------------");
                Console.WriteLine("ID | Name                 | Price  | Stock");
                Console.WriteLine("-------------------------------------------");
                
                foreach (var item in filteredItems)
                {
                    Console.WriteLine($"{item.Id,-3}| {item.Name,-21} | ${item.Price,-6:F2}| {item.Stock}");
                }
            }
            else
            {
                Console.WriteLine("Invalid input.");
            }
            
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void AddItem()
        {
            Console.Clear();
            Console.WriteLine("===========================================");
            Console.WriteLine("              ADD ITEM TO CART             ");
            Console.WriteLine("===========================================");
            
            Console.Write("Enter item ID: ");
            if (int.TryParse(Console.ReadLine(), out int itemId))
            {
                Console.Write("Enter quantity: ");
                if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                {
                    if (_checkoutSystem.AddItemToCart(itemId, quantity, _currentCart))
                    {
                        Console.WriteLine("Item added to cart successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to add item. Item not found or insufficient stock.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid quantity.");
                }
            }
            else
            {
                Console.WriteLine("Invalid item ID.");
            }
            
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ScanItem()
        {
            Console.Clear();
            Console.WriteLine("===========================================");
            Console.WriteLine("                SCAN ITEM                  ");
            Console.WriteLine("===========================================");
            
            Console.Write("Enter barcode: ");
            string barcode = Console.ReadLine();
            
            if (!string.IsNullOrWhiteSpace(barcode))
            {
                if (_checkoutSystem.ScanItem(barcode, _currentCart))
                {
                    Console.WriteLine("Item scanned and added to cart successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to scan item. Barcode not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid barcode.");
            }
            
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ChoosePaymentMethod()
        {
            Console.Clear();
            Console.WriteLine("===========================================");
            Console.WriteLine("           CHOOSE PAYMENT METHOD           ");
            Console.WriteLine("===========================================");
            Console.WriteLine("1. Cash");
            Console.WriteLine("2. Card");
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine($"Cash Balance: ${_currentCustomer.CashBalance:F2}");
            Console.WriteLine($"Card Balance: ${_currentCustomer.CardBalance:F2}");
            Console.WriteLine("===========================================");
            
            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    _selectedPaymentMethod = "Cash";
                    Console.WriteLine("Payment method set to Cash.");
                    break;
                case "2":
                    _selectedPaymentMethod = "Card";
                    Console.WriteLine("Payment method set to Card.");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Defaulting to Cash.");
                    _selectedPaymentMethod = "Cash";
                    break;
            }
            
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void RedeemPoints()
        {
            Console.Clear();
            Console.WriteLine("===========================================");
            Console.WriteLine("           REDEEM LOYALTY POINTS           ");
            Console.WriteLine("===========================================");
            Console.WriteLine($"Available Points: {_currentCustomer.LoyaltyPoints}");
            Console.WriteLine("(100 points = $1.00 discount)");
            Console.WriteLine("===========================================");
            
            Console.Write("Enter points to redeem (or 0 to cancel): ");
            if (int.TryParse(Console.ReadLine(), out int points) && points >= 0)
            {
                if (points <= _currentCustomer.LoyaltyPoints)
                {
                    _pointsToRedeem = points;
                    decimal discount = points / 100m;
                    Console.WriteLine($"You will redeem {points} points for a ${discount:F2} discount.");
                }
                else
                {
                    Console.WriteLine("You don't have enough points.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input.");
            }
            
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ViewCart()
        {
            Console.Clear();
            Console.WriteLine("===========================================");
            Console.WriteLine("                VIEW CART                  ");
            Console.WriteLine("===========================================");
            
            if (_currentCart.Count == 0)
            {
                Console.WriteLine("Your cart is empty.");
            }
            else
            {
                Console.WriteLine("ID | Name                 | Price   | Qty | Total");
                Console.WriteLine("-------------------------------------------");
                
                foreach (var item in _currentCart)
                {
                    Console.WriteLine($"{item.Item.Id,-3}| {item.Item.Name,-21} | ${item.Item.Price,-7:F2}| {item.Quantity,-4}| ${item.GetTotalPrice():F2}");
                }
                
                Console.WriteLine("-------------------------------------------");
                decimal subtotal = _currentCart.Sum(item => item.GetTotalPrice());
                decimal tax = Math.Round(subtotal * 0.08m, 2);
                Console.WriteLine($"Subtotal: ${subtotal:F2}");
                Console.WriteLine($"Tax (8%): ${tax:F2}");
                
                if (_pointsToRedeem > 0)
                {
                    decimal discount = _pointsToRedeem / 100m;
                    Console.WriteLine($"Discount: ${discount:F2} ({_pointsToRedeem} points)");
                    Console.WriteLine($"Final Total: ${subtotal + tax - discount:F2}");
                }
                else
                {
                    Console.WriteLine($"Total: ${subtotal + tax:F2}");
                }
            }
            
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void RemoveItem()
        {
            Console.Clear();
            Console.WriteLine("===========================================");
            Console.WriteLine("           REMOVE ITEM FROM CART           ");
            Console.WriteLine("===========================================");
            
            if (_currentCart.Count == 0)
            {
                Console.WriteLine("Your cart is empty.");
            }
            else
            {
                ViewCart();
                
                Console.Write("\nEnter item ID to remove: ");
                if (int.TryParse(Console.ReadLine(), out int itemId))
                {
                    if (_checkoutSystem.RemoveItemFromCart(itemId, _currentCart))
                    {
                        Console.WriteLine("Item removed from cart successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to remove item. Item not found in cart.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid item ID.");
                }
            }
            
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void Checkout()
        {
            Console.Clear();
            Console.WriteLine("===========================================");
            Console.WriteLine("               CHECKOUT                    ");
            Console.WriteLine("===========================================");
            
            if (_currentCart.Count == 0)
            {
                Console.WriteLine("Your cart is empty. Nothing to checkout.");
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                return;
            }
            
            // Check funds
            bool enoughFunds = _checkoutSystem.CheckFunds(_currentCustomer, _currentCart, _selectedPaymentMethod, _pointsToRedeem);
            
            if (!enoughFunds)
            {
                decimal total = _currentCart.Sum(item => item.GetTotalPrice());
                decimal tax = Math.Round(total * 0.08m, 2);
                decimal finalTotal = total + tax;
                
                if (_pointsToRedeem > 0)
                {
                    decimal discount = _pointsToRedeem / 100m;
                    finalTotal -= discount;
                }
                
                decimal available = _selectedPaymentMethod == "Cash" ? _currentCustomer.CashBalance : _currentCustomer.CardBalance;
                
                Console.WriteLine("===========================================");
                Console.WriteLine("           NOT ENOUGH MONEY                ");
                Console.WriteLine("===========================================");
                Console.WriteLine($"Total Amount: ${finalTotal:F2}");
                Console.WriteLine($"Available {_selectedPaymentMethod}: ${available:F2}");
                Console.WriteLine($"Shortage: ${finalTotal - available:F2}");
                
                var itemsToRemove = _checkoutSystem.SuggestItemsToRemove(_currentCart, available);
                
                Console.WriteLine("\nSuggested items to remove:");
                foreach (var item in itemsToRemove)
                {
                    Console.WriteLine($"- {item.Item.Name} (${item.GetTotalPrice():F2})");
                }
                
                Console.WriteLine("\n1. Remove suggested items");
                Console.WriteLine("2. Change payment method");
                Console.WriteLine("3. Cancel checkout");
                
                Console.Write("\nEnter your choice: ");
                string choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        foreach (var item in itemsToRemove)
                        {
                            _checkoutSystem.RemoveItemFromCart(item.Item.Id, _currentCart);
                        }
                        Console.WriteLine("Suggested items removed from cart.");
                        break;
                    case "2":
                        _selectedPaymentMethod = _selectedPaymentMethod == "Cash" ? "Card" : "Cash";
                        Console.WriteLine($"Payment method changed to {_selectedPaymentMethod}.");
                        break;
                    case "3":
                    default:
                        Console.WriteLine("Checkout canceled.");
                        break;
                }
                
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                return;
            }
            
            // Process checkout
            var receipt = _checkoutSystem.ProcessCheckout(_currentCustomer, _currentCart, _selectedPaymentMethod, _pointsToRedeem);
            
            if (receipt != null)
            {
                Console.WriteLine("===========================================");
                Console.WriteLine("         CHECKOUT SUCCESSFUL               ");
                Console.WriteLine("===========================================");
                Console.WriteLine(receipt.GenerateReceiptText());
                
                // Reset cart and points after successful checkout
                _currentCart.Clear();
                _pointsToRedeem = 0;
            }
            else
            {
                Console.WriteLine("Checkout failed. Please try again.");
            }
            
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ViewCustomerInfo()
        {
            Console.Clear();
            Console.WriteLine("===========================================");
            Console.WriteLine("           CUSTOMER INFORMATION            ");
            Console.WriteLine("===========================================");
            Console.WriteLine($"Name: {_currentCustomer.Name}");
            Console.WriteLine($"ID: {_currentCustomer.Id}");
            Console.WriteLine($"Cash Balance: ${_currentCustomer.CashBalance:F2}");
            Console.WriteLine($"Card Balance: ${_currentCustomer.CardBalance:F2}");
            Console.WriteLine($"Loyalty Points: {_currentCustomer.LoyaltyPoints}");
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("Purchase History:");
            
            var history = _currentCustomer.PurchaseHistory;
            
            if (history.Count == 0)
            {
                Console.WriteLine("No purchase history available.");
            }
            else
            {
                foreach (var receipt in history)
                {
                    Console.WriteLine($"Receipt #{receipt.Id} - Date: {receipt.Date}");
                    Console.WriteLine($"Total: ${receipt.Total:F2} - Payment Method: {receipt.PaymentMethod}");
                    Console.WriteLine($"Items: {receipt.Items.Count}");
                    Console.WriteLine("-------------------------------------------");
                }
            }
            
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }

    #endregion

    #region Unit Tests

    /// <summary>
    /// Unit tests for the checkout system
    /// </summary>
    public class CheckoutSystemTests
    {
        /// <summary>
        /// Tests inventory search functionality
        /// </summary>
        public void TestInventorySearch()
        {
            // Arrange
            var inventorySystem = new InventorySystem();
            
            // Act
            var allItems = inventorySystem.GetAllItems();
            var dairyItems = inventorySystem.SearchByCategory("Dairy");
            var milkItem = inventorySystem.GetItemByBarcode("1001");
            
            // Assert
            Console.WriteLine("Test: Inventory Search");
            Console.WriteLine($"All items count: {allItems.Count} (Expected: > 0)");
            Console.WriteLine($"Dairy items count: {dairyItems.Count} (Expected: > 0)");
            Console.WriteLine($"Found milk by barcode: {milkItem != null && milkItem.Name == "Milk"} (Expected: True)");
            Console.WriteLine("-------------------------------------------");
        }

        /// <summary>
        /// Tests scanning functionality
        /// </summary>
        public void TestScanning()
        {
            // Arrange
            var inventorySystem = new InventorySystem();
            var scanningSystem = new ScanningSystem(inventorySystem);
            var cart = new List<CartItem>();
            
            // Act
            bool scanResult = scanningSystem.ScanItem("1001", cart); // Milk
            bool addResult = scanningSystem.AddItemToCart(2, 3, cart); // Bread x3
            
            // Assert
            Console.WriteLine("Test: Scanning");
            Console.WriteLine($"Scan result: {scanResult} (Expected: True)");
            Console.WriteLine($"Add result: {addResult} (Expected: True)");
            Console.WriteLine($"Cart count: {cart.Count} (Expected: 2)");
            Console.WriteLine($"Bread quantity: {cart.FirstOrDefault(i => i.Item.Id == 2)?.Quantity} (Expected: 3)");
            Console.WriteLine("-------------------------------------------");
        }

        /// <summary>
        /// Tests payment processing
        /// </summary>
        public void TestPaymentProcessing()
        {
            // Arrange
            var paymentSystem = new PaymentSystem();
            var customer = new Customer(1, "Test Customer", 100m, 200m, 500);
            var cart = new List<CartItem>();
            var item = new Item(1, "Test Item", 10m, "1001", 100, "Test", 10);
            cart.Add(new CartItem(item, 2));
            
            // Act
            decimal total = paymentSystem.CalculateTotal(cart);
            bool cashFunds = paymentSystem.CheckFunds(customer, total, "Cash");
            bool cardFunds = paymentSystem.CheckFunds(customer, total, "Card");
            bool cashPayment = paymentSystem.ProcessPayment(customer, total, "Cash");
            
            // Assert
            Console.WriteLine("Test: Payment Processing");
            Console.WriteLine($"Total: {total} (Expected: 21.60 [20 + 8% tax])");
            Console.WriteLine($"Has cash funds: {cashFunds} (Expected: True)");
            Console.WriteLine($"Has card funds: {cardFunds} (Expected: True)");
            Console.WriteLine($"Cash payment result: {cashPayment} (Expected: True)");
            Console.WriteLine($"New cash balance: {customer.CashBalance} (Expected: 78.40)");
            Console.WriteLine("-------------------------------------------");
        }

        /// <summary>
        /// Tests card management functionality
        /// </summary>
        public void TestCardManagement()
        {
            // Arrange
            var cardSystem = new CardManagementSystem();
            var customer = new Customer(1, "Test Customer", 100m, 200m, 500);
            
            // Act
            decimal balance = cardSystem.GetCardBalance(customer);
            bool verifyResult = cardSystem.VerifyCardFunds(customer, 150m);
            bool updateResult = cardSystem.UpdateCardBalance(customer, 50m);
            cardSystem.AddFundsToCard(customer, 25m);
            
            // Assert
            Console.WriteLine("Test: Card Management");
            Console.WriteLine($"Initial balance: {balance} (Expected: 200)");
            Console.WriteLine($"Verify funds result: {verifyResult} (Expected: True)");
            Console.WriteLine($"Update balance result: {updateResult} (Expected: True)");
            Console.WriteLine($"Balance after update: {customer.CardBalance} (Expected: 175)");
            Console.WriteLine("-------------------------------------------");
        }

        /// <summary>
        /// Tests receipt generation
        /// </summary>
        public void TestReceiptGeneration()
        {
            // Arrange
            var receiptSystem = new ReceiptSystem();
            var customer = new Customer(1, "Test Customer", 100m, 200m, 500);
            var cart = new List<CartItem>();
            var item = new Item(1, "Test Item", 10m, "1001", 100, "Test", 10);
            cart.Add(new CartItem(item, 2));
            
            // Act
            var receipt = receiptSystem.CreateReceipt(customer, cart, "Cash", 0);
            string printedReceipt = receiptSystem.PrintReceipt(receipt);
            var history = receiptSystem.GetPurchaseHistory(customer);
            
            // Assert
            Console.WriteLine("Test: Receipt Generation");
            Console.WriteLine($"Receipt created: {receipt != null} (Expected: True)");
            Console.WriteLine($"Receipt has items: {receipt.Items.Count == 2} (Expected: True)");
            Console.WriteLine($"History count: {history.Count} (Expected: 1)");
            Console.WriteLine($"Printed receipt length: {printedReceipt.Length > 0} (Expected: True)");
            Console.WriteLine("-------------------------------------------");
        }

        /// <summary>
        /// Tests loyalty points functionality
        /// </summary>
        public void TestLoyaltyPoints()
        {
            // Arrange
            var pointSystem = new PointSystem();
            var customer = new Customer(1, "Test Customer", 100m, 200m, 500);
            var cart = new List<CartItem>();
            var item = new Item(1, "Test Item", 10m, "1001", 100, "Test", 10);
            cart.Add(new CartItem(item, 2));
            
            // Act
            int pointsCalculated = pointSystem.CalculatePointsForPurchase(cart);
            int initialBalance = pointSystem.GetPointsBalance(customer);
            pointSystem.AwardPoints(customer, pointsCalculated);
            decimal discount = pointSystem.RedeemPoints(customer, 100);
            
            // Assert
            Console.WriteLine("Test: Loyalty Points");
            Console.WriteLine($"Points calculated: {pointsCalculated} (Expected: 20)");
            Console.WriteLine($"Initial balance: {initialBalance} (Expected: 500)");
            Console.WriteLine($"Balance after award: {customer.LoyaltyPoints} (Expected: 520)");
            Console.WriteLine($"Discount amount: {discount} (Expected: 1.00)");
            Console.WriteLine($"Final balance: {customer.LoyaltyPoints} (Expected: 420)");
            Console.WriteLine("-------------------------------------------");
        }

        /// <summary>
        /// Tests checkout process end-to-end
        /// </summary>
        public void TestCheckoutProcess()
        {
            // Arrange
            var checkoutSystem = new CheckoutSystem();
            var customer = new Customer(1, "Test Customer", 1000m, 2000m, 500);
            var cart = new List<CartItem>();
            
            // Act
            bool browseResult = checkoutSystem.BrowseItems().Count > 0;
            bool scanResult = checkoutSystem.ScanItem("1001", cart); // Milk
            bool addResult = checkoutSystem.AddItemToCart(2, 2, cart); // Bread x2
            bool enoughFunds = checkoutSystem.CheckFunds(customer, cart, "Cash", 0);
            var receipt = checkoutSystem.ProcessCheckout(customer, cart, "Cash", 0);
            
            // Assert
            Console.WriteLine("Test: Checkout Process");
            Console.WriteLine($"Browse result: {browseResult} (Expected: True)");
            Console.WriteLine($"Scan result: {scanResult} (Expected: True)");
            Console.WriteLine($"Add result: {addResult} (Expected: True)");
            Console.WriteLine($"Enough funds: {enoughFunds} (Expected: True)");
            Console.WriteLine($"Receipt generated: {receipt != null} (Expected: True)");
            Console.WriteLine("-------------------------------------------");
        }

        /// <summary>
        /// Runs all tests
        /// </summary>
        public void RunAllTests()
        {
            Console.Clear();
            Console.WriteLine("===========================================");
            Console.WriteLine("         RUNNING UNIT TESTS                ");
            Console.WriteLine("===========================================");
            
            TestInventorySearch();
            TestScanning();
            TestPaymentProcessing();
            TestCardManagement();
            TestReceiptGeneration();
            TestLoyaltyPoints();
            TestCheckoutProcess();
            
            Console.WriteLine("\nAll tests completed!");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }

    #endregion

    /// <summary>
    /// Main program class
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            // Create checkout system
            var checkoutSystem = new CheckoutSystem();
            
            // Create sample customer
            var customer = new Customer(1, "John Doe", 100.00m, 500.00m, 250);
            
            // Display menu to choose between running the application or unit tests
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("===========================================");
                Console.WriteLine("        GROCERY STORE SYSTEM MENU         ");
                Console.WriteLine("===========================================");
                Console.WriteLine("1. Start Checkout Application");
                Console.WriteLine("2. Run Unit Tests");
                Console.WriteLine("0. Exit");
                Console.WriteLine("===========================================");
                
                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        // Start checkout UI
                        var ui = new CheckoutConsoleUI(checkoutSystem, customer);
                        ui.Start();
                        break;
                    case "2":
                        // Run unit tests
                        var tests = new CheckoutSystemTests();
                        tests.RunAllTests();
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}