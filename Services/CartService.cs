using System;
using System.Collections.Generic;
using System.Linq;

namespace front__wasm.Services
{
    public class CartService
    {
        private List<Pages.Cart.CartItemModel> _cartItems = new List<Pages.Cart.CartItemModel>();
        
        public event Action? OnChange;
        
        public IReadOnlyList<Pages.Cart.CartItemModel> CartItems => _cartItems.AsReadOnly();
        
        public int Count => _cartItems.Sum(item => item.Quantity);
        
        public void AddItem(Pages.Cart.CartItemModel item)
        {
            var existingItem = _cartItems.FirstOrDefault(i => 
                i.Title == item.Title && 
                i.Description == item.Description);
                
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                _cartItems.Add(item);
            }
            
            NotifyStateChanged();
        }
        
        public void UpdateQuantity(Pages.Cart.CartItemModel item, int newQuantity)
        {
            var existingItem = _cartItems.FirstOrDefault(i => i == item);
            if (existingItem != null)
            {
                existingItem.Quantity = newQuantity;
                NotifyStateChanged();
            }
        }
        
        public void RemoveItem(Pages.Cart.CartItemModel item)
        {
            _cartItems.Remove(item);
            NotifyStateChanged();
        }
        
        public void ClearCart()
        {
            _cartItems.Clear();
            NotifyStateChanged();
        }
        
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}