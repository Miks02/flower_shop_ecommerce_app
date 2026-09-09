using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.IdentityUser;
using FlowerShop.Domain.Entities.Notifications;
using FlowerShop.Infrastructure.Notifications;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace FlowerShop.UnitTests.Infrastructure.Notifications;

public class NotificationServiceTests
{
    private readonly INotificationRepository _notificationRepo = Substitute.For<INotificationRepository>();
    private readonly UserManager<User> _userManager = Substitute.For<UserManager<User>>(
        Substitute.For<IUserStore<User>>(), null, null, null, null, null, null, null, null);
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly NotificationService _sut;

    public NotificationServiceTests()
    {
        _sut = new NotificationService(_notificationRepo, _userManager, _unitOfWork);
    }

    private static User CreateUser(string id) => new()
    {
        Id = id,
        FirstName = "Marko",
        LastName = "Markovic"
    };

    private void SetUpUsers(params User[] users)
    {
        var mockDbSet = users.ToList().BuildMockDbSet();
        _userManager.Users.Returns(mockDbSet);
    }

    [Fact]
    public async Task SendNotificationAsync_WhenRecipientExists_AddsNotificationAndSaves()
    {
        const string userId = "buyer-1";
        SetUpUsers(CreateUser(userId));

        await _sut.SendNotificationAsync(
            userId, "Naslov", "Poruka", NotificationType.Success, NotificationEntityType.Order, 5);

        _notificationRepo.Received(1).Add(
            Arg.Is<Notification>(n =>
                n.Title == "Naslov" &&
                n.Message == "Poruka" &&
                n.NotificationType == NotificationType.Success &&
                n.NotificationEntityType == NotificationEntityType.Order &&
                n.EntityId == 5),
            userId);
        await _unitOfWork.Received(1).SaveAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendNotificationAsync_WhenRecipientDoesNotExist_ThrowsNotificationException()
    {
        SetUpUsers();

        var act = async () => await _sut.SendNotificationAsync("unknown-user", "Naslov", "Poruka");

        await act.Should().ThrowAsync<NotificationException>();
        _notificationRepo.DidNotReceive().Add(Arg.Any<Notification>(), Arg.Any<string>());
        await _unitOfWork.DidNotReceive().SaveAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendMultipleNotificationsAsync_WhenAllRecipientsExist_AddsNotificationWithMultipleRecipientsAndSaves()
    {
        string[] userIds = ["buyer-1", "buyer-2"];
        SetUpUsers(userIds.Select(CreateUser).ToArray());

        await _sut.SendMultipleNotificationsAsync(userIds, "Naslov", "Poruka");

        _notificationRepo.Received(1).AddNotificationWithMultipleRecipients(
            Arg.Is<IReadOnlyList<string>>(ids => ids.SequenceEqual(userIds)),
            Arg.Is<Notification>(n => n.Title == "Naslov" && n.Message == "Poruka"));
        await _unitOfWork.Received(1).SaveAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendMultipleNotificationsAsync_WhenSomeRecipientsDoNotExist_ThrowsNotificationException()
    {
        string[] userIds = ["buyer-1", "buyer-2"];
        SetUpUsers(CreateUser("buyer-1"));

        var act = async () => await _sut.SendMultipleNotificationsAsync(userIds, "Naslov", "Poruka");

        await act.Should().ThrowAsync<NotificationException>();
        _notificationRepo.DidNotReceive().AddNotificationWithMultipleRecipients(
            Arg.Any<IReadOnlyList<string>>(), Arg.Any<Notification>());
        await _unitOfWork.DidNotReceive().SaveAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendNotificationsToAllAdminsAsync_WhenAdminsExist_AddsNotificationForAllAdminsAndSaves()
    {
        List<User> admins = [CreateUser("admin-1"), CreateUser("admin-2")];
        _userManager.GetUsersInRoleAsync("Admin").Returns((IList<User>)admins);

        await _sut.SendNotificationsToAllAdminsAsync("Naslov", "Poruka");

        _notificationRepo.Received(1).AddNotificationWithMultipleRecipients(
            Arg.Is<IReadOnlyList<string>>(ids => ids.SequenceEqual(admins.Select(a => a.Id))),
            Arg.Any<Notification>());
        await _unitOfWork.Received(1).SaveAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendNotificationsToAllAdminsAsync_WhenNoAdminsExist_ThrowsNotificationException()
    {
        _userManager.GetUsersInRoleAsync("Admin").Returns((IList<User>)new List<User>());

        var act = async () => await _sut.SendNotificationsToAllAdminsAsync("Naslov", "Poruka");

        await act.Should().ThrowAsync<NotificationException>();
        _notificationRepo.DidNotReceive().AddNotificationWithMultipleRecipients(
            Arg.Any<IReadOnlyList<string>>(), Arg.Any<Notification>());
        await _unitOfWork.DidNotReceive().SaveAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendNotificationsToAllDeliverersAsync_WhenDeliverersExist_AddsNotificationForAllDeliverersAndSaves()
    {
        List<User> deliverers = [CreateUser("deliverer-1"), CreateUser("deliverer-2")];
        _userManager.GetUsersInRoleAsync("Deliverer").Returns((IList<User>)deliverers);

        await _sut.SendNotificationsToAllDeliverersAsync("Naslov", "Poruka");

        _notificationRepo.Received(1).AddNotificationWithMultipleRecipients(
            Arg.Is<IReadOnlyList<string>>(ids => ids.SequenceEqual(deliverers.Select(d => d.Id))),
            Arg.Any<Notification>());
        await _unitOfWork.Received(1).SaveAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendNotificationsToAllDeliverersAsync_WhenNoDeliverersExist_ThrowsNotificationException()
    {
        _userManager.GetUsersInRoleAsync("Deliverer").Returns((IList<User>)new List<User>());

        var act = async () => await _sut.SendNotificationsToAllDeliverersAsync("Naslov", "Poruka");

        await act.Should().ThrowAsync<NotificationException>();
        _notificationRepo.DidNotReceive().AddNotificationWithMultipleRecipients(
            Arg.Any<IReadOnlyList<string>>(), Arg.Any<Notification>());
        await _unitOfWork.DidNotReceive().SaveAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendNotificationsToAllUsersAsync_WhenUsersExist_AddsNotificationForAllUsersAndSaves()
    {
        List<User> users = [CreateUser("buyer-1"), CreateUser("buyer-2")];
        _userManager.GetUsersInRoleAsync("User").Returns((IList<User>)users);

        await _sut.SendNotificationsToAllUsersAsync("Naslov", "Poruka");

        _notificationRepo.Received(1).AddNotificationWithMultipleRecipients(
            Arg.Is<IReadOnlyList<string>>(ids => ids.SequenceEqual(users.Select(u => u.Id))),
            Arg.Any<Notification>());
        await _unitOfWork.Received(1).SaveAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendNotificationsToAllUsersAsync_WhenNoUsersExist_ThrowsNotificationException()
    {
        _userManager.GetUsersInRoleAsync("User").Returns((IList<User>)new List<User>());

        var act = async () => await _sut.SendNotificationsToAllUsersAsync("Naslov", "Poruka");

        await act.Should().ThrowAsync<NotificationException>();
        _notificationRepo.DidNotReceive().AddNotificationWithMultipleRecipients(
            Arg.Any<IReadOnlyList<string>>(), Arg.Any<Notification>());
        await _unitOfWork.DidNotReceive().SaveAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MarkAllAsReadAsync_CallsRepositoryWithGivenUserId()
    {
        const string userId = "buyer-1";

        await _sut.MarkAllAsReadAsync(userId);

        await _notificationRepo.Received(1).MarkNotificationsAsRead(userId);
    }
}
