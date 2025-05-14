using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Common.Data.Models
{
    public class BaseModel
    {
        public bool Active { get; set; } = true;
        public DateTime? AddedOn { get; set; } = DateTime.Now;
        public string? AddedBy { get; set; }
        public DateTime? UpdatedOn { get; set; } = DateTime.Now;
        public string? UpdatedBy { get; set; }
    }
}
