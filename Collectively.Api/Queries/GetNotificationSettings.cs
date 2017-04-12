﻿using Collectively.Common.Queries;

namespace Collectively.Api.Queries
{
    public class GetNotificationSettings : IAuthenticatedQuery
    {
        public string UserId { get; set; }
    }
}