﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataExtraction___MVC5.Models.Views;

namespace DataExtraction___MVC5.Infrastructure
{
    public interface IMailService
    {
        void SendMail(DataExtractionConfirmationEmail email);
    }
}