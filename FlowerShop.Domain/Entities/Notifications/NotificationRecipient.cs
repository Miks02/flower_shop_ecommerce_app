using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowerShop.Domain.Entities.IdentityUser;

namespace FlowerShop.Domain.Entities.Notifications
{
    public class NotificationRecipient
    {
        public int NotificationId { get; set; }
        public Notification Notification { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;

        public DateTime? ReadAt { get; set; }
    }
}
