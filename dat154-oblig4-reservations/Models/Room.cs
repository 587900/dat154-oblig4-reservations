using System;
using System.Collections.Generic;

namespace dat154_oblig4_reservations.Models
{
    public partial class Room
    {
        public Room()
        {
            Reservations = new HashSet<Reservation>();
            Tasks = new HashSet<Task>();
        }

        public int Id { get; set; }
        public string Number { get; set; } = null!;
        public int? Beds { get; set; }
        public string? Size { get; set; }
        public string? Quality { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
