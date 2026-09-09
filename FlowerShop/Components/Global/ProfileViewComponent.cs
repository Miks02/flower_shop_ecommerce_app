using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Application.Common.Abstractions.Dto;
using FlowerShop.Application.Features.Dashboard.Queries.GetUserDashboard;
using FlowerShop.Web.ViewModels.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Web.Components.Global;

[Authorize]
public class ProfileViewComponent(IUserProvider userProvider, GetUserDashboardHandler getUserDashboardHandler) : ViewComponent
{

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userId = userProvider.GetCurrentUserId();
        var user = await userProvider.GetCurrentUserDetails(userId);

        if (User.IsInRole("User"))
        {
            var dashboardData = await getUserDashboardHandler.Handle(new GetUserDashboardQuery(userId), HttpContext.RequestAborted);
            var viewModel = CreateUserProfileViewModel(user, dashboardData);
            return View("User/Default", viewModel);
        }


        return View("NotFound");
    }

    private UserProfileViewModel CreateUserProfileViewModel(UserDetailsDto user, GetUserDashboardResponse dashboardData)
    {
        return new UserProfileViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            ProfilePicture = Url.Content(user.ProfilePicture),
            Status = user.Status,
            RegistrationDate = user.RegistrationDate.ToString("dd.MM.yyyy"),
            TotalOrders = dashboardData.TotalOrders,
            LoyaltyPoints = dashboardData.LoyaltyPoints,
            UpcomingOrders = dashboardData.UpcomingOrders,
            RecentOrders = dashboardData.RecentOrders.Select(o => new UserRecentOrderViewModel
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderPrice = o.OrderPrice,
                OrderStatus = o.OrderStatus,
                CreatedAt = o.CreatedAt
            }).ToList()
        };
    }
}