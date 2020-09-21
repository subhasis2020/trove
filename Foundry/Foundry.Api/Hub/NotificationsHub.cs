using Foundry.Domain.Dto;
using Foundry.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Api
{
    public class NotificationsHub : Hub
    {
        //private readonly IBenefactorNotifications _notify;
        //public NotificationsHub(IBenefactorNotifications notify)
        //{
        //    _notify = notify;
        //}

        //public IEnumerable<BenefactorNotificationsDto> GetAllStocks(int benefactorId)
        //{
        //    var benefactor = _notify.GetBenefactorNotifications(benefactorId);
        //    return null;
        //}
    }
}
