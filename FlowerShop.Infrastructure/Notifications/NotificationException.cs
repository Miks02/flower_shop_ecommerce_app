using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerShop.Infrastructure.Notifications
{
    public class NotificationException : Exception
    {
        public NotificationException(string message = "A notification error has occurred.") : base(message)
        {
        }
    }
}
