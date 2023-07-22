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
        private UserManager<ApplicationUser> userManager;
        private readonly IFileUploadService fileUploadService;

        public HomeController(UserManager<ApplicationUser> manager,
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
                Posts = postsRepository.Posts.OrderByDescending(p => p.Id),
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
                    Where(p => p.AppUser.UserName == selectedAcc.UserName).OrderByDescending(p => p.Id),
                SelectedUser = selectedAcc,
                CurrentUser = await userManager.FindByNameAsync(User.Identity?.Name),
                IsCurrentUser = (selectedAcc.UserName == User.Identity?.Name) ? true : false
            });
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
                AppUser = await userManager.FindByNameAsync(User.Identity?.Name)
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
            var removePost = postsRepository.Posts.FirstOrDefault(p => p.Id == Id);
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
            var editPost = postsRepository.Posts.FirstOrDefault(p => p.Id == Id);
            if (editPost == null)
            {
                return NotFound();
            }
            DeleteImage(editPost.ImgName);
            return RedirectToAction("CreatePost", editPost);
        }

        [HttpPost]
        public async Task<IActionResult> SetProfilePicture(IFormFile? file)
        {
            var user = await userManager.FindByNameAsync(User.Identity?.Name);
            if (file != null)
            {
                await fileUploadService.UploadFileAsync(file);
                user.ProfileImgName = file.FileName;
                user.Save();
                return Redirect($"http://localhost:5000/{user.UserName}");
            }
            return Redirect($"http://localhost:5000/{user.UserName}");
        }

        private async Task CreatePostMethod(string TextArea, IFormFile? file)
        {

            var newPost = new Post()
            {
                AppUser = await userManager.FindByNameAsync(User.Identity?.Name),
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
