using Bungalov.Business.Interfaces;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bungalov.Business.Services;

public class IyzipayPaymentService : IPaymentService
{
    private readonly Options _options;

    public IyzipayPaymentService(IConfiguration configuration)
    {
        _options = new Options
        {
            ApiKey = configuration["Iyzipay:ApiKey"],
            SecretKey = configuration["Iyzipay:SecretKey"],
            BaseUrl = configuration["Iyzipay:BaseUrl"]
        };
    }

    public async Task<CheckoutFormInitialize> InitializeCheckoutFormAsync(decimal price, int bungalowId, string userId, string callbackUrl)
    {
        CreateCheckoutFormInitializeRequest request = new CreateCheckoutFormInitializeRequest();
        request.Locale = Locale.TR.ToString();
        request.ConversationId = bungalowId.ToString();
        request.Price = price.ToString("F2").Replace(",", ".");
        request.PaidPrice = price.ToString("F2").Replace(",", ".");
        request.Currency = Currency.TRY.ToString();
        request.BasketId = "B" + bungalowId + "U" + userId;
        request.PaymentGroup = PaymentGroup.PRODUCT.ToString();
        request.CallbackUrl = callbackUrl;

        Buyer buyer = new Buyer();
        buyer.Id = userId;
        buyer.Name = "Test";
        buyer.Surname = "Kullanici";
        buyer.GsmNumber = "+905350000000";
        buyer.Email = "test@test.com";
        buyer.IdentityNumber = "74300864791";
        buyer.LastLoginDate = "2015-10-05 12:43:35";
        buyer.RegistrationDate = "2013-04-21 15:12:09";
        buyer.RegistrationAddress = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
        buyer.Ip = "85.34.78.112";
        buyer.City = "Istanbul";
        buyer.Country = "Turkey";
        buyer.ZipCode = "34732";
        request.Buyer = buyer;

        Address shippingAddress = new Address();
        shippingAddress.ContactName = "Test Kullanici";
        shippingAddress.City = "Istanbul";
        shippingAddress.Country = "Turkey";
        shippingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
        shippingAddress.ZipCode = "34732";
        request.ShippingAddress = shippingAddress;

        Address billingAddress = new Address();
        billingAddress.ContactName = "Test Kullanici";
        billingAddress.City = "Istanbul";
        billingAddress.Country = "Turkey";
        billingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
        billingAddress.ZipCode = "34732";
        request.BillingAddress = billingAddress;

        List<BasketItem> basketItems = new List<BasketItem>();
        BasketItem firstBasketItem = new BasketItem();
        firstBasketItem.Id = "BI101";
        firstBasketItem.Name = "Bungalov Konaklama";
        firstBasketItem.Category1 = "Konaklama";
        firstBasketItem.ItemType = BasketItemType.VIRTUAL.ToString();
        firstBasketItem.Price = price.ToString("F2").Replace(",", ".");
        basketItems.Add(firstBasketItem);
        request.BasketItems = basketItems;

        return await Task.Run(() => CheckoutFormInitialize.Create(request, _options));
    }

    public async Task<CheckoutForm> RetrieveCheckoutFormResultAsync(string token)
    {
        RetrieveCheckoutFormRequest request = new RetrieveCheckoutFormRequest();
        request.Token = token;

        return await Task.Run(() => CheckoutForm.Retrieve(request, _options));
    }
}
