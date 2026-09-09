using FlowerShop.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.UnitTests.Infrastructure.Persistence.Repositories;

public class NotificationRepositoryTests : RepositoryTestBase
{
    private readonly NotificationRepository _sut;

    public NotificationRepositoryTests()
    {
        _sut = new NotificationRepository(Context);
    }

    [Fact]
    public async Task GetAllNotificationsByUserId_ReturnsRecipientsWithNotificationIncluded()
    {
        var user = TestEntityFactory.CreateUser();
        var notification = TestEntityFactory.CreateNotification("Naslov", "Poruka");
        var recipient = TestEntityFactory.CreateNotificationRecipient(notification, user);
        Context.Users.Add(user);
        Context.Notifications.Add(notification);
        Context.NotificationRecipients.Add(recipient);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        var result = await _sut.GetAllNotificationsByUserId(user.Id);

        result.Should().ContainSingle();
        result[0].Notification.Should().NotBeNull();
        result[0].Notification.Title.Should().Be("Naslov");
    }

    [Fact]
    public async Task GetAllNotificationsByUserId_WhenUserHasNoNotifications_ReturnsEmptyList()
    {
        var result = await _sut.GetAllNotificationsByUserId("unknown-user");

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task MarkNotificationsAsRead_SetsReadAtForAllUnreadNotificationsOfUser()
    {
        var user = TestEntityFactory.CreateUser();
        var notificationOne = TestEntityFactory.CreateNotification();
        var notificationTwo = TestEntityFactory.CreateNotification();
        var recipientOne = TestEntityFactory.CreateNotificationRecipient(notificationOne, user);
        var recipientTwo = TestEntityFactory.CreateNotificationRecipient(notificationTwo, user);
        Context.Users.Add(user);
        Context.Notifications.AddRange(notificationOne, notificationTwo);
        Context.NotificationRecipients.AddRange(recipientOne, recipientTwo);
        await Context.SaveChangesAsync();

        var updatedCount = await _sut.MarkNotificationsAsRead(user.Id);

        updatedCount.Should().Be(2);
        var recipients = await Context.NotificationRecipients.AsNoTracking().Where(r => r.UserId == user.Id).ToListAsync();
        recipients.Should().OnlyContain(r => r.ReadAt != null);
    }

    [Fact]
    public async Task AddNotificationWithMultipleRecipients_AddsNotificationWithRecipientForEachUserId()
    {
        var userOne = TestEntityFactory.CreateUser();
        var userTwo = TestEntityFactory.CreateUser();
        Context.Users.AddRange(userOne, userTwo);
        await Context.SaveChangesAsync();
        var notification = TestEntityFactory.CreateNotification("Naslov", "Poruka");

        _sut.AddNotificationWithMultipleRecipients([userOne.Id, userTwo.Id], notification);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        var recipients = await Context.NotificationRecipients.AsNoTracking()
            .Where(r => r.NotificationId == notification.Id)
            .ToListAsync();
        recipients.Select(r => r.UserId).Should().BeEquivalentTo([userOne.Id, userTwo.Id]);
    }

    [Fact]
    public async Task Add_AddsNotificationWithSingleRecipient()
    {
        var user = TestEntityFactory.CreateUser();
        Context.Users.Add(user);
        await Context.SaveChangesAsync();
        var notification = TestEntityFactory.CreateNotification("Naslov", "Poruka");

        _sut.Add(notification, user.Id);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        var recipients = await Context.NotificationRecipients.AsNoTracking()
            .Where(r => r.NotificationId == notification.Id)
            .ToListAsync();
        recipients.Should().ContainSingle(r => r.UserId == user.Id);
    }

    [Fact]
    public async Task CountUnreadAsync_ReturnsCountOfUnreadNotificationsForUser()
    {
        var user = TestEntityFactory.CreateUser();
        var unread = TestEntityFactory.CreateNotification();
        var read = TestEntityFactory.CreateNotification();
        var unreadRecipient = TestEntityFactory.CreateNotificationRecipient(unread, user);
        var readRecipient = TestEntityFactory.CreateNotificationRecipient(read, user, readAt: DateTime.UtcNow);
        Context.Users.Add(user);
        Context.Notifications.AddRange(unread, read);
        Context.NotificationRecipients.AddRange(unreadRecipient, readRecipient);
        await Context.SaveChangesAsync();

        var result = await _sut.CountUnreadAsync(user.Id);

        result.Should().Be(1);
    }

    [Fact]
    public async Task CountUnreadAsync_WhenUserHasNoUnreadNotifications_ReturnsZero()
    {
        var user = TestEntityFactory.CreateUser();
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        var result = await _sut.CountUnreadAsync(user.Id);

        result.Should().Be(0);
    }
}
