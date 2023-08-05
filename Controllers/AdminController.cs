using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThreadsASP.Models;
using ThreadsASP.Models.Repositories;

namespace ThreadsASP.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private IReportsRepository _reportsRepository;
        private IUserRepository _userRepository;
        private UserManager<ApplicationUser> _userManager;
        private IPostsRepository _postsRepository;
        public AdminController(IPostsRepository postsRepository, 
            UserManager<ApplicationUser> userManager, IUserRepository userRepository,
            IReportsRepository reportsRepository)
        {
            _postsRepository = postsRepository;
            _userManager = userManager;
            _userRepository = userRepository;
            _reportsRepository = reportsRepository;
        }

        public IActionResult AdminSideBarPartial()
        {
            return PartialView("_AdminSideBarPartial");
        }

        public async Task<IActionResult> Posts()
        {
            var posts = await _postsRepository.Posts.ToListAsync();
            return View(posts);
        }

        public async Task<IActionResult> DeletePost(long postId)
        {
            var post = await _postsRepository.Posts.FirstOrDefaultAsync(x => x.Id == postId);
            if (post == null)
            {
                return RedirectToAction("Posts");
            }
            _postsRepository.DeletePost(post);
            return RedirectToAction("Posts");
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> BlockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == await _userManager.GetUserAsync(User))
            {
                return Content("You can't block yourself");
            }
            if (user != null)
            {
                user.IsBlocked = true;
                _userRepository.SaveNewProfileData();
                return RedirectToAction("Users");
            }
            return Content("Error");
        }

        public async Task<IActionResult> UnBlockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.IsBlocked = false;
                _userRepository.SaveNewProfileData();
                return RedirectToAction("Users");
            }
            return Content("Error");
        }

        public async Task<IActionResult> Reports()
        {
            var reports = await _reportsRepository.Reports.ToListAsync();
            return View(reports);
        }

        public async Task<IActionResult> DeleteReport(long reportId)
        {
            var report = await _reportsRepository.Reports.FirstOrDefaultAsync(x => x.Id == reportId);
            if (report != null)
            {
                _reportsRepository.DeleteReport(report);
                return RedirectToAction("Reports");
            }
            return Content("Error");
        }
    }
}
