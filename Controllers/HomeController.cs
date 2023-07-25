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
        private IPostsRepository postsRepository;
        private UserManager<ApplicationUser> userManager;
        private readonly IFileService fileUploadService;
        private IFollowsRepository followsRepository;

        public HomeController(UserManager<ApplicationUser> userManager,
            IPostsRepository postsRepository, IFileService fileUploadService, IFollowsRepository followsRepository)
        {
            this.postsRepository = postsRepository;
            this.userManager = userManager;
            this.fileUploadService = fileUploadService;
            this.followsRepository = followsRepository;
        }


        public async Task<IActionResult> Index()
        {
            var currentUser = await userManager.GetUserAsync(User);
            var follows = await followsRepository.Follows.Where(x => x.FollowingUserId == currentUser.Id).ToListAsync();
            return View(new HomeViewModel
            {
                Posts = await postsRepository.Posts.Where(c => follows.Select(
                    s => s.FollowerUserId).Contains(c.AppUserId) || c.AppUserId == currentUser.Id).OrderByDescending(i => i.Id).ToListAsync(),
                CurrentUser = currentUser
            });
        }

        public async Task<IActionResult> ToolsPartial()
        {
            var currentUser = await userManager.GetUserAsync(User);
            return PartialView("_ToolsPartial", currentUser.UserName);
        }


        [HttpPost("{action}")]
        public async Task<IActionResult> SearchResult(string searchString)
        {
            var searchUsers = await userManager.Users.Where(x => x.UserName.Contains(searchString)).Take(4).ToListAsync();
            var searchPosts = await postsRepository.Posts.Where(x => x.Text.Contains(searchString)).Take(4).ToListAsync();
            return View(new SearchResultViewModel
            {
                SearchUsers = searchUsers,
                SearchPosts = searchPosts,
                SearchString = searchString
            });
        }

        public async Task<IActionResult> SuggestPartial()
        {
            var currentUser = await userManager.GetUserAsync(User);

            var suggestingUsers = await userManager.Users.Where(
                s => s.Id != currentUser.Id && !followsRepository.Follows.Where(
                    x => x.FollowingUserId == currentUser.Id).Select(d => d.FollowerUserId).Contains(s.Id)).ToListAsync();

            var rand = new Random();
            suggestingUsers = suggestingUsers.OrderBy(_ => rand.Next()).Take(5).ToList();
            return PartialView("_SuggestPartial", suggestingUsers);
        }

        [HttpGet("{action}")]
        public IActionResult CreatePost(Post post)
        {
            if (post?.Id != null)
            {
                return View(post);
            }
            return View();
        }


        [HttpPost("{action}")]
        public async Task<IActionResult> CreatePost(long? Id, string TextArea, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                var newFileName = Guid.NewGuid().ToString() + ".png";
                if (file != null)
                {
                    await fileUploadService.UploadPostImageAsync(file, newFileName);
                }
                var oldPost = postsRepository.Posts.FirstOrDefault(p => p.Id == Id);
                if (oldPost != null)
                {
                    LocalFileService.DeleteImage(oldPost.ImgName);
                    postsRepository.DeletePost(oldPost);
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
                AppUser = await userManager.GetUserAsync(User),
                Text = TextArea,
                Date = DateTime.Now.ToString("dd-MM-yyyy"),
                ImgName = (file != null) ? newFileName : null
            };
            postsRepository.CreatePost(newPost);
        }
    }
}
