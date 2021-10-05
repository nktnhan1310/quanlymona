using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Entities
{
    public class ContactCustomerNotes : BaseContactCustomer
    {
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Ghi chú HTML
        /// </summary>
        public string NoteHTML { get; set; }
    }
}
