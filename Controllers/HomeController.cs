using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThreadsASP.FileUploadService;
using ThreadsASP.Models;
using ThreadsASP.Models.Repositories;
using ThreadsASP.Models.ViewModels;


namespace ThreadsASP.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IPostsRepository _postsRepository;
        private UserManager<ApplicationUser> _userManager;
        private readonly IFileService _fileUploadService;
        private IFollowsRepository _followsRepository;

        public HomeController(UserManager<ApplicationUser> userManager,
            IPostsRepository postsRepository, IFileService fileUploadService, IFollowsRepository followsRepository)
        {
            _postsRepository = postsRepository;
            _userManager = userManager;
            _fileUploadService = fileUploadService;
            _followsRepository = followsRepository;
        }


        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var follows = await _followsRepository.Follows.Where(x => x.FollowingUserId == currentUser.Id).ToListAsync();
            return View(new HomeViewModel
            {
                Posts = await _postsRepository.Posts.Where(c => follows.Select(
                    s => s.FollowerUserId).Contains(c.AppUserId) || c.AppUserId == currentUser.Id).OrderByDescending(i => i.Id).ToListAsync(),
                CurrentUser = currentUser
            });
        }
        

        public async Task<IActionResult> SideBarPartial()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return PartialView("_SideBarPartial", currentUser.UserName);
        }

        public async Task<IActionResult> SearchBarPartial()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var suggestingUsers = await _userManager.Users.Where(
                s => s.Id != currentUser.Id && !_followsRepository.Follows.Where(
                    x => x.FollowingUserId == currentUser.Id).Select(d => d.FollowerUserId).Contains(s.Id)).ToListAsync();

            var rand = new Random();
            suggestingUsers = suggestingUsers.OrderBy(_ => rand.Next()).Take(5).ToList();
            return PartialView("_SearchBarPartial", suggestingUsers);
        }


        [HttpGet("{action}")]
        public async Task<IActionResult> SearchResult(string searchString)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserFollows = await _followsRepository.Follows.Where(x => x.FollowingUserId == currentUser.Id).ToListAsync();

            var searchUsers = await _userManager.Users.Where(x => x.UserName.Contains(searchString) || 
                x.FirstName.Contains(searchString) || x.LastName.Contains(searchString)).Take(5).ToListAsync();

            var searchPosts = await _postsRepository.Posts.Where(x => x.Text.Contains(searchString)).Take(5).ToListAsync();
            return View(new SearchResultViewModel
            {
                SearchUsers = searchUsers,
                SearchPosts = searchPosts,
                SearchString = searchString,
                CurrentUserId = currentUser.Id,
                CurrentUserFollows = currentUserFollows
            });
        }
        

        [HttpGet("{action}")]
        public async Task<IActionResult> CreatePost(Post post)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (post?.Id != 0 && post.AppUserId == currentUser.Id)
            {
                post.AppUser = currentUser;
                return View(post);
            }
            return View(new Post
            {
                AppUser = currentUser
            });
        }


        [HttpPost("{action}")]
        public async Task<IActionResult> CreatePost(long? Id, string TextArea, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                var newFileName = Guid.NewGuid().ToString() + ".png";
                if (file != null)
                {
                    await _fileUploadService.UploadPostImageAsync(file, newFileName);
                }
                var oldPost = _postsRepository.Posts.FirstOrDefault(p => p.Id == Id);
                if (oldPost != null)
                {
                    LocalFileService.DeleteImage(oldPost.ImgName);
                    _postsRepository.DeletePost(oldPost);
                    await CreatePostMethodAsync(TextArea, file, newFileName);
                    return RedirectToAction("Index");
                }
                await CreatePostMethodAsync(TextArea, file, newFileName);
                return RedirectToAction("Index");
            }
            return View();
        }


        private async Task CreatePostMethodAsync(string TextArea, IFormFile? file, string newFileName)
        {
            var newPost = new Post()
            {
                AppUser = await _userManager.GetUserAsync(User),
                Text = TextArea,
                Date = DateTime.Now.ToString("MMMM dd"),
                ImgName = (file != null) ? newFileName : null
            };
            _postsRepository.CreatePost(newPost);
        }
    }
}
