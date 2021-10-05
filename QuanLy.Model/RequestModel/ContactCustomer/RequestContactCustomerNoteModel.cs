using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Model
{
    public class RequestContactCustomerNoteModel : RequestCoreContactCustomerModel
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
