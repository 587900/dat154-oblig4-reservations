using System;
using System.Collections.Generic;

namespace dat154_oblig4_reservations.Models
{
    public partial class User
    {
        public User()
        {
            Reservations = new HashSet<Reservation>();
            Tasks = new HashSet<Task>();
        }

        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool? Staff { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
