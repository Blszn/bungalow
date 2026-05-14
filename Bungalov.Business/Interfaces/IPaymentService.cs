using Iyzipay.Model;

namespace Bungalov.Business.Interfaces;

public interface IPaymentService
{
    // Checkout formunu başlatan ve token dönen metod
    Task<CheckoutFormInitialize> InitializeCheckoutFormAsync(decimal price, int bungalowId, string userId, string callbackUrl);
    
    // Ödeme sonucunu kontrol eden metod
    Task<CheckoutForm> RetrieveCheckoutFormResultAsync(string token);
}
