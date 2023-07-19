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
                SelectedUserName = null,
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
                Posts = postsRepository.Posts.Where(p => p.UserName == acc.UserName).OrderByDescending(p => p.Id),
                SelectedUserName = acc.UserName,
                CurrentUserName = User.Identity?.Name,
                IsCurrentUser = (acc.UserName == User.Identity?.Name) ? true : false
            });
        }

        [HttpGet("{action}")]
        public IActionResult CreatePost(Post p)
        {
            if (p.Id != 0)
            {
                return View(p);
            }
            var newPost = new Post()
            {
                UserName = User.Identity?.Name,
                Text = null
            };
            return View(newPost);
        }

        [HttpPost("{action}")]
        public IActionResult CreatePost(int Id, string TextArea)
        {
            if (ModelState.IsValid)
            {
                var oldPost = postsRepository.Posts.FirstOrDefault(p => p.Id == Id);
                if (oldPost != null)
                {
                    postsRepository.DeletePost(oldPost);
                    var newEditedPost = new Post()
                    {
                        UserName = User.Identity?.Name,
                        Text = TextArea,
                        Date = DateTime.Now.ToString("dd-MM-yyyy"),
                    };
                    postsRepository.CreatePost(newEditedPost);
                    return RedirectToAction("Index");
                }
                var newPost = new Post()
                {
                    UserName = User.Identity?.Name,
                    Text = TextArea,
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
        [HttpPost]
        public IActionResult EditPost(int Id)
        {
            var editPost = postsRepository.Posts.FirstOrDefault(p =>p.Id == Id);
            if (editPost == null)
            {
                return NotFound();
            }
            return RedirectToAction("CreatePost", editPost);
        }
    }
}
