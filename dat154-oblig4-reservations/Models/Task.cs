using System;
using System.Collections.Generic;

namespace dat154_oblig4_reservations.Models
{
    public partial class Task
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public string? Status { get; set; }
        public string? Type { get; set; }
        public string? Note { get; set; }
        public int? RoomId { get; set; }
        public int? StaffId { get; set; }

        public virtual Room? Room { get; set; }
        public virtual User? Staff { get; set; }
    }
}
