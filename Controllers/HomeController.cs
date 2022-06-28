using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Exam.Models;
using Microsoft.AspNetCore.Http;


namespace Exam.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext _context;
    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _context = context;
        _logger = logger;
    }
    
    public IActionResult Index()
    {
        return View();
    }
    [HttpPost("user/register")]
    public IActionResult Register(User newUser)
    {
        if(ModelState.IsValid){
            if(_context.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }
            bool result =
            newUser.Password.Any(c => char.IsLetter(c)) &&
            newUser.Password.Any(c => char.IsDigit(c)) &&
            newUser.Password.Any(c => !char.IsLetterOrDigit(c));
            if(result){
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
            _context.Add(newUser);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("UserId", newUser.UserId);
            return RedirectToAction("Meetups");
            }
            else{
                    ModelState.AddModelError("Password", "Password must be a minimum of 8 characters, and contain 1 number, 1 letter, and 1 special character.");
                    return View("Index");
            }
        }
        else{
            return View("Index");
        }
    }
    [HttpGet("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }
    [HttpPost("user/login")]
    public IActionResult Login(LogUser newLogUser)
    {
        if(ModelState.IsValid){
            var userInDb = _context.Users.FirstOrDefault(u => u.Email == newLogUser.LogEmail);
            if(userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Email", "Invalid Login");
                    return View("Index");
                }
            var hasher = new PasswordHasher<LogUser>();
            var result = hasher.VerifyHashedPassword(newLogUser, userInDb.Password, newLogUser.LogPassword);
            if(result == 0)
                {
                    ModelState.AddModelError("Email", "Invalid Login");
                    return View("Index");
                }
            HttpContext.Session.SetInt32("UserId", userInDb.UserId);
            return RedirectToAction("Meetups");
        }
        else{
            return View("Index");
        }
    }
    public IActionResult Privacy()
    {
        return View();
    }
    [HttpGet("meetups")]
    public IActionResult Meetups()
    {
        if(HttpContext.Session.GetInt32("UserId") == null)
        {
            return RedirectToAction("Index");
        }

        ViewBag.NotLoggedIn = false;
        User? userInDb = _context.Users.FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("UserId"));
        ViewBag.LoggedIn = userInDb;
        ViewBag.AllMeetUps = _context.MeetUps.Include(d => d.Coordinator).Include(a => a.UsersParticipating).OrderBy(a => a.DateAndTime).ToList();
        return View();
    }
    [HttpGet("meetups/new")]
    public IActionResult NewMeetup()
    {
        if(HttpContext.Session.GetInt32("UserId") == null)
        {
            return RedirectToAction("Index");
        }
        ViewBag.NotLoggedIn = false;
        User? userInDb = _context.Users.FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("UserId"));
        return View();
    }
    [HttpPost("createmeetup")]
    public IActionResult CreateMeetup(MeetUp newMeetUp)
    {
        if(ModelState.IsValid)
        {
            if(DateTime.Now < newMeetUp.DateAndTime){
            newMeetUp.UserId = (int)HttpContext.Session.GetInt32("UserId");
            _context.Add(newMeetUp);
            _context.SaveChanges();
            return RedirectToAction("Meetups");
            }
            else{
                ModelState.AddModelError("DateAndTime", "Date cannot have already passed!");
                return View("NewMeetup");
            }
        }
        else{
            return View("NewMeetup");
        }
    }
    [HttpGet("meetups/{meetupId}")]
    public IActionResult ShowMeetUp(int meetupId)
    {
        if(HttpContext.Session.GetInt32("UserId") == null)
        {
            return RedirectToAction("Index");
        }
        ViewBag.NotLoggedIn = false;
        ViewBag.ThisMeetup = _context.MeetUps.Include(d => d.Coordinator).FirstOrDefault(a => a.MeetUpId == meetupId);
        ViewBag.ThisMeetupsParticipants = _context.MeetUps.Include(s => s.UsersParticipating).ThenInclude(d => d.Participant).FirstOrDefault(a => a.MeetUpId == meetupId);
        return View();
    }
    [HttpGet("join/{meetupId}")]
    public IActionResult Join(int meetupId)
    {
        Association newAssociation = new Association();
        newAssociation.UserId = (int)HttpContext.Session.GetInt32("UserId");
        newAssociation.MeetUpId = meetupId;
        _context.Add(newAssociation);
        _context.SaveChanges();
        return RedirectToAction("Meetups");
    }
    [HttpGet("delete/{meetupId}")]
    public IActionResult Delete(int meetupId)
    {
        MeetUp MeetUpToDelete = _context.MeetUps.Include(a => a.Coordinator).SingleOrDefault(a => a.MeetUpId == meetupId);
        if(MeetUpToDelete.Coordinator.UserId == (int)HttpContext.Session.GetInt32("UserId")){
        _context.MeetUps.Remove(MeetUpToDelete);
        _context.SaveChanges();
        return RedirectToAction("Meetups");
        }
        else{
            return View("Index");
        }

    }
    [HttpGet("leave/{meetupId}")]
    public IActionResult Leave(int meetupId)
    {
        Association toDelete = _context.Associations.SingleOrDefault(a => a.MeetUpId == meetupId && a.UserId == (int)HttpContext.Session.GetInt32("UserId"));
        _context.Associations.Remove(toDelete);
        _context.SaveChanges();
        return RedirectToAction("Meetups");
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
