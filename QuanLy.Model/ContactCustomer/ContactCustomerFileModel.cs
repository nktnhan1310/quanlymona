﻿using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Model
{
    public class ContactCustomerFileModel : BaseContactCustomerFileModel
    {
        /// <summary>
        /// Loại file
        /// </summary>
        public int TypeId { get; set; }

    }
}
