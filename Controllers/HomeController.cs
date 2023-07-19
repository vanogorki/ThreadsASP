using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SportsStore.Models;
using ThreadsASP.Models;
using ThreadsASP.Models.ViewModels;


namespace ThreadsASP.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IPostsRepository postsRepository;

        private UserManager<IdentityUser> userManager;

        public HomeController(UserManager<IdentityUser> manager, IPostsRepository posts)
        {
            postsRepository = posts;
            userManager = manager;
        }

        public IActionResult Index()
        {
            return View(new UserPostViewModel
            {
                Posts = postsRepository.Posts.OrderByDescending(p => p.Id),
                CurrentUserName = User.Identity?.Name,
                IsCurrentUser = true
            });
        }

        [HttpGet("{accName}")]
        public async Task<IActionResult> UserAccPage(string accName)
        {
            var acc = await
            userManager.FindByNameAsync(accName);
            if (acc == null)
            {
                return NotFound();
            }
            return View(new UserPostViewModel
            {
                Posts = postsRepository.Posts.Where(p => p.UserName == accName).OrderByDescending(p => p.Id),
                SelectedUserName = accName,
                CurrentUserName = User.Identity?.Name,
                IsCurrentUser = (accName == User.Identity?.Name) ? true : false
            });
        }

        [HttpGet("{action}")]
        public IActionResult CreatePost()
        {
            var newPost = new Post()
            {
                UserName = User.Identity?.Name
            };
            return View(newPost);
        }

        [HttpPost("{action}")]
        public IActionResult CreatePost(string UserName, string Text)
        {
            if (ModelState.IsValid)
            {
                var newPost = new Post()
                {
                    UserName = UserName,
                    Text = Text,
                    Date = DateTime.Now.ToString("dd-MM-yyyy"),
                };
                postsRepository.CreatePost(newPost);
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public IActionResult DeletePost(int Id, string accName)
        {
            var removePost = postsRepository.Posts.FirstOrDefault(p => p.Id == Id);
            if (removePost == null)
            {
                return NotFound();
            }
            postsRepository.DeletePost(removePost);
            return Redirect($"http://localhost:5000/{accName}");
        }
    }
}
