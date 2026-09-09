using FlowerShop.Domain.Entities.Carts;
using FlowerShop.Domain.Entities.Categories;
using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.Domain.Entities.Flowers;
using FlowerShop.Domain.Entities.IdentityUser;
using FlowerShop.Domain.Entities.LoyaltyTransactions;
using FlowerShop.Domain.Entities.Notifications;
using FlowerShop.Domain.Entities.Ocassions;
using FlowerShop.Domain.Entities.Orders;
using FlowerShop.Domain.Entities.ProductFlowers;
using FlowerShop.Domain.Entities.ProductReviews;
using FlowerShop.Domain.Entities.Products;
using FlowerShop.Domain.Entities.ServiceReviews;
using FlowerShop.Domain.Enums;

namespace FlowerShop.UnitTests.Infrastructure.Persistence.Repositories;

public static class TestEntityFactory
{
    public static User CreateUser(string? id = null, string firstName = "Marko", string lastName = "Markovic")
    {
        var userId = id ?? Guid.NewGuid().ToString();
        return new User
        {
            Id = userId,
            UserName = userId,
            NormalizedUserName = userId.ToUpperInvariant(),
            Email = $"{userId}@test.com",
            NormalizedEmail = $"{userId}@test.com".ToUpperInvariant(),
            PhoneNumber = "0611234567",
            FirstName = firstName,
            LastName = lastName,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Category CreateCategory(string? name = null) => new()
    {
        Name = name ?? $"Kategorija-{Guid.NewGuid():N}"
    };

    public static Occasion CreateOccasion(string? name = null) => new()
    {
        Name = name ?? $"Prilika-{Guid.NewGuid():N}"
    };

    public static Flower CreateFlower(
        string? name = null,
        string color = "Crvena",
        FlowerCategory flowerCategory = FlowerCategory.Fresh,
        int stock = 10) => new()
    {
        Name = name ?? $"Cvet-{Guid.NewGuid():N}",
        Color = color,
        FlowerCategory = flowerCategory,
        Stock = stock
    };

    public static Product CreateProduct(
        Category category,
        User user,
        string? name = null,
        decimal price = 1000m,
        decimal? promoPrice = null,
        DiscountType discountType = DiscountType.None,
        int stock = 10,
        bool isDeleted = false,
        DateTime? createdAt = null) => new()
    {
        Name = name ?? $"Proizvod-{Guid.NewGuid():N}",
        ImageUrl = "product.jpg",
        Price = price,
        PromoPrice = promoPrice,
        DiscountType = discountType,
        Stock = stock,
        Category = category,
        User = user,
        CreatedBy = user.Id,
        CreatedAt = createdAt ?? DateTime.UtcNow,
        IsDeleted = isDeleted
    };

    public static ProductFlower CreateProductFlower(Product product, Flower flower, int quantity = 1) => new()
    {
        Product = product,
        Flower = flower,
        Quantity = quantity
    };

    public static ProductReview CreateProductReview(Product product, User user, decimal rating = 5, string? comment = null) => new()
    {
        Product = product,
        User = user,
        ReviewerId = user.Id,
        Rating = rating,
        Comment = comment,
        CreatedAt = DateTime.UtcNow
    };

    public static Cart CreateCart(User user) => new()
    {
        User = user,
        UserId = user.Id
    };

    public static CartItem CreateCartItem(Cart cart, Product product, int quantity = 1, decimal? price = null) => new()
    {
        Cart = cart,
        Product = product,
        Quantity = quantity,
        Price = price ?? product.Price
    };

    public static Deliverer CreateDeliverer(
        User user,
        VehicleType vehicleType = VehicleType.Car,
        DelivererStatus delivererStatus = DelivererStatus.Available) => new()
    {
        Id = user.Id,
        User = user,
        VehicleType = vehicleType,
        DelivererStatus = delivererStatus
    };

    public static Order CreateOrder(
        User user,
        Deliverer? deliverer = null,
        OrderStatus orderStatus = OrderStatus.Pending,
        DeliveryStatus deliveryStatus = DeliveryStatus.Standby,
        decimal orderPrice = 1000m,
        DateTime? createdAt = null,
        DateTime? orderDate = null,
        string? orderNumber = null,
        string? recipientFullName = null,
        string? city = null) => new()
    {
        OrderNumber = orderNumber ?? $"ORD-{Guid.NewGuid():N}",
        RecipientFullName = recipientFullName ?? "Petar Petrovic",
        RecipientPhoneNumber = "0611234567",
        OrderAddress = "Bulevar Oslobodjenja 1",
        City = city ?? "Beograd",
        ZipCode = "11000",
        User = user,
        UserId = user.Id,
        Deliverer = deliverer,
        DelivererId = deliverer?.Id,
        OrderStatus = orderStatus,
        DeliveryStatus = deliveryStatus,
        OrderDate = orderDate ?? DateTime.UtcNow,
        CreatedAt = createdAt ?? DateTime.UtcNow,
        OrderPrice = orderPrice
    };

    public static OrderItem CreateOrderItem(Order order, string? productName = null, int quantity = 1, decimal unitPrice = 1000m) => new()
    {
        Order = order,
        ProductName = productName ?? $"Stavka-{Guid.NewGuid():N}",
        Quantity = quantity,
        UnitPrice = unitPrice
    };

    public static ServiceReview CreateServiceReview(Order order, User reviewer, decimal rating = 5, string? comment = null) => new()
    {
        Order = order,
        Reviewer = reviewer,
        ReviewerId = reviewer.Id,
        Rating = rating,
        Comment = comment
    };

    public static LoyaltyTransaction CreateLoyaltyTransaction(
        User user,
        Order order,
        TransactionType transactionType = TransactionType.Earned,
        int previousPoints = 0,
        int currentPoints = 100,
        DateTime? transactionDate = null) => new()
    {
        User = user,
        UserId = user.Id,
        Order = order,
        OrderId = order.Id,
        TransactionType = transactionType,
        PreviousPoints = previousPoints,
        CurrentPoints = currentPoints,
        TransactionDate = transactionDate ?? DateTime.UtcNow
    };

    public static Notification CreateNotification(
        string title = "Naslov",
        string message = "Poruka",
        NotificationType notificationType = NotificationType.Information,
        NotificationEntityType notificationEntityType = NotificationEntityType.None,
        int? entityId = null) => new()
    {
        Title = title,
        Message = message,
        NotificationType = notificationType,
        NotificationEntityType = notificationEntityType,
        EntityId = entityId
    };

    public static NotificationRecipient CreateNotificationRecipient(Notification notification, User user, DateTime? readAt = null) => new()
    {
        Notification = notification,
        User = user,
        UserId = user.Id,
        ReadAt = readAt
    };
}
