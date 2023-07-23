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
                CurrentUser = await userManager.GetUserAsync(User)
            });
        }

        public async Task<IActionResult> ToolsPartial()
        {
            var currentUser = await userManager.GetUserAsync(User);
            return PartialView("_ToolsPartial", currentUser.UserName);
        }

        public async Task<IActionResult> FollowingPartial()
        {
            var currentUser = await userManager.GetUserAsync(User);
            List<Follow> followingUsers = await followsRepository.Follows.Where(x => x.FollowingUserId == currentUser.Id).ToListAsync();
            return PartialView("_FollowingPartial", followingUsers);
        }

        [HttpGet("{action}")]
        public async Task<IActionResult> CreatePost(Post p)
        {
            if (p?.Id != null)
            {
                return View(p);
            }
            var newPost = new Post()
            {
                AppUser = await userManager.GetUserAsync(User)
            };
            return View(newPost);
        }

        
        [HttpPost("{action}")]
        public async Task<IActionResult> CreatePost(long? Id, string TextArea, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    await fileUploadService.UploadFileAsync(file);
                }
                var oldPost = postsRepository.Posts.FirstOrDefault(p => p.Id == Id);
                if (oldPost != null)
                {
                    LocalFileService.DeleteImage(oldPost.ImgName);
                    postsRepository.DeletePost(oldPost);
                    await CreatePostMethod(TextArea, file);
                    return RedirectToAction("Index");
                }
                await CreatePostMethod(TextArea, file);
                return RedirectToAction("Index");
            }
            return View();
        }
       

        private async Task CreatePostMethod(string TextArea, IFormFile? file)
        {

            var newPost = new Post()
            {
                AppUser = await userManager.GetUserAsync(User),
                Text = TextArea,
                Date = DateTime.Now.ToString("dd-MM-yyyy"),
                ImgName = file?.FileName
            };
            postsRepository.CreatePost(newPost);
        }
    }
}
