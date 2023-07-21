using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using ThreadsASP.FileUploadService;
using ThreadsASP.Models;
using ThreadsASP.Models.ViewModels;


namespace ThreadsASP.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IPostsRepository postsRepository;
        private UserManager<IdentityUser> userManager;
        private readonly IFileUploadService fileUploadService;

        public HomeController(UserManager<IdentityUser> manager,
            IPostsRepository posts, IFileUploadService fileUploadService)
        {
            postsRepository = posts;
            userManager = manager;
            this.fileUploadService = fileUploadService;
        }

        public async Task<IActionResult> Index()
        {
            return View(new UserPostViewModel
            {
                Posts = postsRepository.Posts.OrderByDescending(p => p.PostId),
                SelectedUser = null,
                CurrentUser = await userManager.FindByNameAsync(User.Identity?.Name),
                IsCurrentUser = true,
            });
        }

        [HttpGet("{accName}")]
        public async Task<IActionResult> UserAccPage(string accName)
        {
            var selectedAcc = await
            userManager.FindByNameAsync(accName);
            if (selectedAcc == null)
            {
                return NotFound();
            }
            return View(new UserPostViewModel
            {
                Posts = postsRepository.Posts.
                    Where(p => p.PostUserName == selectedAcc.UserName).OrderByDescending(p => p.PostId),
                SelectedUser = selectedAcc,
                CurrentUser = await userManager.FindByNameAsync(User.Identity?.Name),
                IsCurrentUser = (selectedAcc.UserName == User.Identity?.Name) ? true : false
            });
        }

        [HttpGet("{action}")]
        public async Task<IActionResult> CreatePost(Post p)
        {
            if (p?.PostId != null)
            {
                return View(p);
            }
            var newPost = new Post()
            {
               PostUserName = User.Identity?.Name,
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
                var oldPost = postsRepository.Posts.FirstOrDefault(p => p.PostId == Id);
                if (oldPost != null)
                {
                    postsRepository.DeletePost(oldPost);
                    await CreatePostMethod(TextArea, file);
                    return RedirectToAction("Index");
                }
                await CreatePostMethod(TextArea, file);
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public IActionResult DeletePost(long Id, string accName)
        {
            var removePost = postsRepository.Posts.FirstOrDefault(p => p.PostId == Id);
            if (removePost == null)
            {
                return NotFound();
            }
            DeleteImage(removePost.ImgName);
            postsRepository.DeletePost(removePost);
            return Redirect($"http://localhost:5000/{accName}");
        }
        [HttpPost]
        public IActionResult EditPost(long? Id)
        {
            var editPost = postsRepository.Posts.FirstOrDefault(p =>p.PostId == Id);
            if (editPost == null)
            {
                return NotFound();
            }
            DeleteImage(editPost.ImgName);
            return RedirectToAction("CreatePost", editPost);
        }

        private async Task CreatePostMethod(string TextArea, IFormFile? file)
        {
            var newPost = new Post()
            {
                PostUserName = User.Identity?.Name,
                Text = TextArea,
                Date = DateTime.Now.ToString("dd-MM-yyyy"),
                ImgName = file?.FileName
            };
            postsRepository.CreatePost(newPost);
            
        }

        private void DeleteImage(string? ImgName)
        {
            try
            {
                System.IO.File.Delete(@$"wwwroot\images\{ImgName}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
