using dat154_oblig4_reservations.Models;
using Microsoft.EntityFrameworkCore;

namespace dat154_oblig4_reservations.Data
{
    public class ReservationManager
    {

        private ApplicationDbContext _dbContext;

        public ReservationManager(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Room>> GetAvailableRooms(DateTime? from, DateTime? to, int? beds, string? size, string? quality)
        {
            var rooms = _dbContext.Rooms.AsQueryable();
            if (from != null && to != null)
            {
                rooms = rooms.Where(room => room.Reservations.Where(r => r.EndDate != null && r.StartDate != null && r.StartDate.Value < to && from < r.EndDate.Value).Count() == 0);
            }
            if (beds != null) rooms = rooms.Where(room => room.Beds == beds);
            if (size != null) rooms = rooms.Where(room => room.Size == size);
            if (quality != null) rooms = rooms.Where(room => room.Quality == quality);
            return await rooms.ToListAsync();
        }

        public async Task<bool> CreateReservation(int roomId, int userId, DateTime from, DateTime to)
        {
            bool taken = (await _dbContext.Reservations
                .Where(reservation => reservation.RoomId != null && reservation.RoomId == roomId)
                .Where(r => r.EndDate != null && r.StartDate != null && r.StartDate.Value < to && from < r.EndDate.Value)
                .CountAsync()) != 0;
            if (taken) return false;

            Reservation reservation = new Reservation { StartDate = from, EndDate = to, RoomId = roomId, CustomerId = userId };
            await _dbContext.Reservations.AddAsync(reservation);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<Reservation>> GetReservationsWithRoom(int userId)
        {
            return await _dbContext.Reservations.Where(reservation => reservation.CustomerId == userId).Include(r => r.Room).ToListAsync();
        }

        public async Task<List<Reservation>> GetAllReservationsWithRoomAndCustomer()
        {
            return await _dbContext.Reservations.Include(r => r.Room).Include(r => r.Customer).ToListAsync();
        }

        public async Task<bool> DeleteReservation(int id, int userId)
        {
            List<Reservation> reservations = await _dbContext.Reservations.Where(reservation => reservation.Id == id && reservation.CustomerId == userId).ToListAsync();
            if (reservations.Count != 1) return false;

            _dbContext.Reservations.Remove(reservations.First());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteReservationAsAdmin(int id)
        {
            List<Reservation> reservations = await _dbContext.Reservations.Where(reservation => reservation.Id == id).ToListAsync();
            if (reservations.Count != 1) return false;

            _dbContext.Reservations.Remove(reservations.First());
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}