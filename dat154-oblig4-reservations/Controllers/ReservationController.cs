using dat154_oblig4_reservations.Data;
using dat154_oblig4_reservations.Models;
using Microsoft.AspNetCore.Mvc;

namespace dat154_oblig4_reservations.Controllers
{
    public class ReservationController : Controller
    {
        private readonly LoginManager _loginManager;
        private readonly ReservationManager _reservationManager;

        public ReservationController(LoginManager loginManager, ReservationManager reservationManager)
        {
            _loginManager = loginManager;
            _reservationManager = reservationManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? from, DateTime? to, int? beds, string? size, string? quality)
        {
            if (!_loginManager.IsLoggedIn(HttpContext.Session)) return RedirectToAction("Login", "Account", new { area = "Auth", returnurl = HttpContext.Request.Path });
            if (from != null) ViewData["From"] = from.Value.ToString("yyyy-MM-dd");
            if (to != null) ViewData["To"] = to.Value.ToString("yyyy-MM-dd");
            if (beds != null) ViewData["Beds"] = beds;
            if (size != null) ViewData["Size"] = size;
            if (quality != null) ViewData["Quality"] = quality;

            List<Room> rooms = await _reservationManager.GetAvailableRooms(from, to, beds, size, quality);
            ViewData["Data"] = rooms.Select(room => MakeView(room));
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(int id, DateTime from, DateTime to)
        {
            int? userId = _loginManager.GetLoggedInUserId(HttpContext.Session);
            if (userId == null) return RedirectToAction("Error", "Home");

            bool success = await _reservationManager.CreateReservation(id, userId.Value, from, to);
            if (!success) return RedirectToAction("Error", "Home");

            return RedirectToAction("MyReservations", "Reservation", null);
        }

        [HttpGet]
        public async Task<IActionResult> MyReservations()
        {
            if (!_loginManager.IsLoggedIn(HttpContext.Session)) return RedirectToAction("Login", "Account", new { area = "Auth", returnurl = HttpContext.Request.Path });

            int? userId = _loginManager.GetLoggedInUserId(HttpContext.Session);
            if (userId == null) return RedirectToAction("Error", "Home");

            List<Reservation> reservations = await _reservationManager.GetReservationsWithRoom(userId.Value);
            ViewData["Data"] = reservations.Select(reservation => MakeView(reservation));
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            int? userId = _loginManager.GetLoggedInUserId(HttpContext.Session);
            if (userId == null) return RedirectToAction("Error", "Home");

            bool success = await _reservationManager.DeleteReservation(id, userId.Value);
            if (!success) return RedirectToAction("Error", "Home");

            return Redirect(nameof(MyReservations));
        }

        [HttpGet]
        public async Task<IActionResult> AllReservations()
        {
            if (!_loginManager.IsLoggedIn(HttpContext.Session)) return RedirectToAction("Login", "Account", new { area = "Auth", returnurl = HttpContext.Request.Path });
            if (!_loginManager.IsLoggedInAsAdmin(HttpContext.Session))
            {
                return View();
            }

            List<Reservation> reservations = await _reservationManager.GetAllReservationsWithRoomAndCustomer();
            ViewData["Admin"] = "True";
            ViewData["Data"] = reservations.Select(reservation => MakeWholeView(reservation));
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReservationAsAdmin(int id)
        {
            if (!_loginManager.IsLoggedInAsAdmin(HttpContext.Session)) return RedirectToAction("Error", "Home");
            
            bool success = await _reservationManager.DeleteReservationAsAdmin(id);
            if (!success) return RedirectToAction("Error", "Home");

            return Redirect(nameof(AllReservations));
        }

        private static RoomViewModel MakeView(Room room)
        {
            return new RoomViewModel 
            {
                Id = room.Id,
                RoomNumber = room.Number,
                Beds = room.Beds,
                Size = room.Size,
                Quality = room.Quality
            };
        }
        private static ReservationViewModel MakeView(Reservation reservation)
        {
            string from = "", to = "";
            if (reservation.StartDate != null) from = reservation.StartDate.Value.ToString("yyyy-MM-dd");
            if (reservation.EndDate != null) to = reservation.EndDate.Value.ToString("yyyy-MM-dd");
            return new ReservationViewModel
            {
                Id = reservation.Id,
                From = from,
                To = to,
                RoomNumber = reservation.Room!.Number,
                Beds = reservation.Room.Beds,
                Size = reservation.Room.Size,
                Quality = reservation.Room.Quality
            };
        }

        private static WholeReservationViewModel MakeWholeView(Reservation reservation)
        {
            string from = "", to = "";
            if (reservation.StartDate != null) from = reservation.StartDate.Value.ToString("yyyy-MM-dd");
            if (reservation.EndDate != null) to = reservation.EndDate.Value.ToString("yyyy-MM-dd");
            return new WholeReservationViewModel
            {
                Id = reservation.Id,
                Username = reservation.Customer!.Username,
                From = from,
                To = to,
                RoomNumber = reservation.Room!.Number,
                Beds = reservation.Room!.Beds,
                Size = reservation.Room!.Size,
                Quality = reservation.Room!.Quality
            };
        }

        public struct RoomViewModel { public int Id; public string RoomNumber; public int? Beds; public string? Size; public string? Quality; }
        public struct ReservationViewModel { public int Id; public string RoomNumber; public string From; public string To; public int? Beds; public string? Size; public string? Quality; }
        public struct WholeReservationViewModel { public int Id; public string Username; public string RoomNumber; public string From; public string To; public int? Beds; public string? Size; public string? Quality; }
    }
}