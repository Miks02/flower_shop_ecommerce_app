using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerShop.Domain.Entities.Notifications
{
    public enum NotificationEntityType
    {
        None,
        Order,
        Review,
        DeliveryReview,
        Product
    }
}
