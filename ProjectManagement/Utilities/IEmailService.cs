﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Utilities
{
    public interface IEmailService
    {
        Task Send(EmailMessage emailMessage);
    }
}
