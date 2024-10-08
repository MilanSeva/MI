﻿using AutoMapper;
using MahantInv.Infrastructure.Entities;
using MahantInv.Infrastructure.Interfaces;
using MahantInv.Infrastructure.Utility;
using MahantInv.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MahantInv.Web.Api
{
    public class NotificationApiController : BaseApiController
    {
        private readonly ILogger<NotificationApiController> _logger;
        private readonly IProductInventoryRepository _productInventoryReposiroty;
        private readonly IAsyncRepository<Notification> _notificationRepository;
        public NotificationApiController(IAsyncRepository<Notification> notificationRepository, IProductInventoryRepository productInventoryReposiroty, ILogger<NotificationApiController> logger, IMapper mapper) : base(mapper)
        {
            _logger = logger;
            _productInventoryReposiroty = productInventoryReposiroty;
            _notificationRepository = notificationRepository;
        }
        [HttpGet("notification/pendingornotified")]
        public async Task<IActionResult> PendingNotification(int notificationId)
        {
            try
            {
                var myNotifications = await _productInventoryReposiroty.GetNotificationByStatus(new List<string>() { Meta.NotificationStatusTypes.Pending, Meta.NotificationStatusTypes.Notified });
                var model = new
                {
                    pendingNotificationCount = myNotifications.Count(n => n.Status == Meta.NotificationStatusTypes.Pending),
                    myNotifications = myNotifications
                };
                return Ok(model);
            }
            catch (Exception e)
            {
                string GUID = Guid.NewGuid().ToString();
                _logger.LogError(e, GUID, null);
                return BadRequest(new
                {
                    success = false,
                    errors = new[] { "Unexpected Error " + GUID
    }
                });
            }
        }

        [HttpPost("notification/notified")]
        public async Task<IActionResult> NotificationMarkAsNotified(List<int> notificationIds)
        {
            try
            {
                foreach (var notificationId in notificationIds)
                {
                    Notification notification = await _notificationRepository.GetByIdAsync(notificationId);
                    notification.ModifiedAt = Meta.Now;
                    notification.Status = Meta.NotificationStatusTypes.Notified;
                    await _notificationRepository.UpdateAsync(notification);
                }
                return Ok();
            }
            catch (Exception e)
            {
                string GUID = Guid.NewGuid().ToString();
                _logger.LogError(e, GUID, null);
                return BadRequest(new { success = false, errors = new[] { "Unexpected Error " + GUID } });
            }
        }
        [HttpPost("notification/read")]
        public async Task<IActionResult> NotificationMarkAsRead([FromBody] int id)
        {
            try
            {
                Notification notification = await _notificationRepository.GetByIdAsync(id);
                notification.ModifiedAt = Meta.Now;
                notification.Status = Meta.NotificationStatusTypes.Read;
                await _notificationRepository.UpdateAsync(notification);

                return Ok();
            }
            catch (Exception e)
            {
                string GUID = Guid.NewGuid().ToString();
                _logger.LogError(e, GUID, null);
                return BadRequest(new { success = false, errors = new[] { "Unexpected Error " + GUID } });
            }
        }
    }
}
