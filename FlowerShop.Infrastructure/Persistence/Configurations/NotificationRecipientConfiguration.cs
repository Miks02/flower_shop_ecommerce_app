using System;
using System.Collections.Generic;using FlowerShop.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowerShop.Infrastructure.Persistence.Configurations
{
    public class NotificationRecipientConfiguration : IEntityTypeConfiguration<NotificationRecipient>
    {
        public void Configure(EntityTypeBuilder<NotificationRecipient> builder)
        {
            builder.HasKey(nr => new { nr.NotificationId, nr.UserId });

            builder.HasOne(nr => nr.Notification)
                   .WithMany(n => n.Recipients)
                   .HasForeignKey(nr => nr.NotificationId);

            builder.HasOne(nr => nr.User)
                   .WithMany(u => u.Notifications)
                   .HasForeignKey(nr => nr.UserId);

        }
    }
}
