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
            List<Follow> followingUsers = await followsRepository.Follows.Where(x => x.FollowingUserId == currentUser.Id).ToListAsync();
            return View(new HomeViewModel
            {
                Posts = await postsRepository.Posts.Where(c => followingUsers.Select(
                    s => s.FollowerUserId).Contains(c.AppUserId) || c.AppUserId == currentUser.Id).OrderByDescending(i => i.Id).ToListAsync(),
                CurrentUser = currentUser
            });
        }

        public async Task<IActionResult> ToolsPartial()
        {
            var currentUser = await userManager.GetUserAsync(User);
            return PartialView("_ToolsPartial", currentUser.UserName);
        }


        [HttpPost]
        public async Task<IActionResult> SearchUser(string searchString)
        {
            var searchUser = await userManager.Users.FirstOrDefaultAsync(x => x.UserName.Contains(searchString));
            if (searchUser == null)
            {
                return RedirectToAction("Index");
            }
            return Redirect($"http://localhost:5000/{searchUser.UserName}");
        }

        public async Task<IActionResult> FollowingPartial()
        {
            var currentUser = await userManager.GetUserAsync(User);
            List<Follow> followingUsers = await followsRepository.Follows.Where(x => x.FollowingUserId == currentUser.Id).ToListAsync();
            return PartialView("_FollowingPartial", followingUsers);
        }

        [HttpGet("{action}")]
        public  IActionResult CreatePost(Post p)
        {
            if (p?.Id != null)
            {
                return View(p);
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
                    await CreatePostMethod(TextArea, file, newFileName);
                    return RedirectToAction("Index");
                }
                await CreatePostMethod(TextArea, file, newFileName);
                return RedirectToAction("Index");
            }
            return View();
        }
       

        private async Task CreatePostMethod(string TextArea, IFormFile? file, string newFileName)
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
