namespace Application.Cart;

public interface ICartService
{
    List<CartItem> GetCart();
    void AddItem(int productId, string productName, decimal unitPrice, int quantity = 1);
    void UpdateQuantity(int productId, int quantity);
    void RemoveItem(int productId);
    void Clear();
    decimal GetTotal();
    int GetItemCount();
}
