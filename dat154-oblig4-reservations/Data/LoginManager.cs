using dat154_oblig4_reservations.Models;
using Microsoft.EntityFrameworkCore;

namespace dat154_oblig4_reservations.Data
{
    public class LoginManager
    {

        private ApplicationDbContext _dbContext;

        public LoginManager(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public void Login(ISession session, int userid, string username, bool admin) { session.SetInt32("userid", userid); session.SetString("username", username); session.SetString("admin", admin ? "true" : "false"); }
        public void Logout(ISession session) { session.Remove("userid"); session.Remove("username"); session.Remove("admin");  }
        public int? GetLoggedInUserId(ISession session) { return session.GetInt32("userid"); }
        public string? GetLoggedInDisplayName(ISession session) { return session.GetString("username"); }
        public bool IsLoggedIn(ISession session) { return GetLoggedInUserId(session) != null; }
        public bool IsLoggedInAsAdmin(ISession session) { return session.GetString("admin") == "true"; }
        public async Task<bool> TryLogin(ISession session, string username, string password)
        {
            List<User> users = await _dbContext.Users.Where(u => u.Username == username).ToListAsync();
            if (users.Count != 1) return false;

            User user = users.First();
            if (user.Password != password) return false; // for now, plaintext password

            Login(session, user.Id, user.Username, user.Staff == true);
            return true;
        }
        public async Task<RegisterResult> Register(ISession session, string username, string password, string name)
        {
            if (await _dbContext.Users.Where(u => u.Username == username).CountAsync() != 0) return RegisterResult.UserExists;

            User user = new User { Username = username, Password = password, Name = name };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            Login(session, user.Id, user.Username, false);
            return RegisterResult.Success;
        }

        public enum RegisterResult { Success, UserExists };
        
    }
}
