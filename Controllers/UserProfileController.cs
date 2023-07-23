using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ThreadsASP.Models.ViewModels;
using ThreadsASP.Models;
using ThreadsASP.FileUploadService;
using ThreadsASP.Models.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace ThreadsASP.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private IPostsRepository postsRepository;
        private UserManager<ApplicationUser> userManager;
        private readonly IFileService fileUploadService;
        private IUserRepository userRepository;
        private IFollowsRepository followsRepository;

        public UserProfileController(UserManager<ApplicationUser> userManager,
            IPostsRepository postsRepository, IFileService fileUploadService,
            IUserRepository userRepository, IFollowsRepository followsRepository)
        {
            this.postsRepository = postsRepository;
            this.userManager = userManager;
            this.fileUploadService = fileUploadService;
            this.userRepository = userRepository;
            this.followsRepository = followsRepository;
        }

        [HttpGet("{accName}")]
        public async Task<IActionResult> Index(string accName)
        {
            var selectedUser = await userManager.FindByNameAsync(accName);
            var currentUser = await userManager.GetUserAsync(User);
            if (selectedUser == null)
            {
                return NotFound();
            }
            return View(new UserPageViewModel
            {
                Posts = postsRepository.GetUserPosts(selectedUser.Id),
                IsFollowing = followsRepository.IsFollowing(currentUser.Id, selectedUser.Id),
                SelectedUser = selectedUser,
                CurrentUser = currentUser,
                IsCurrentUser = (selectedUser == currentUser) ? true : false
            });
        }

        [HttpPost]
        public IActionResult DeletePost(long Id, string accName)
        {
            var removePost = postsRepository.Posts.FirstOrDefault(p => p.Id == Id);
            if (removePost == null)
            {
                return NotFound();
            }
            LocalFileService.DeleteImage(removePost.ImgName);
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
            return RedirectToAction("CreatePost", "Home", editPost);
        }

        [HttpPost]
        public async Task<IActionResult> SetProfilePicture(IFormFile? file)
        {
            var user = await userManager.GetUserAsync(User);
            if (file != null)
            {
                await fileUploadService.UploadFileAsync(file);
                if (user.ProfileImgName != "Default.jpg")
                {
                    LocalFileService.DeleteImage(user.ProfileImgName);
                }
                user.ProfileImgName = file.FileName;
                userRepository.SaveNewProfilePicture();
                return Redirect($"http://localhost:5000/{user.UserName}");
            }
            return Redirect($"http://localhost:5000/{user.UserName}");
        }

        public async Task<IActionResult> Follow(string selectedUserId)
        {
            var selectedUser = await userManager.FindByIdAsync(selectedUserId);
            var currentUser = await userManager.GetUserAsync(User);
            followsRepository.Follow(currentUser, selectedUser);
            return Redirect($"http://localhost:5000/{selectedUser.UserName}");
        }

        public async Task<IActionResult> UnFollow(string selectedUserId)
        {
            var selectedUser = await userManager.FindByIdAsync(selectedUserId);
            var currentUser = await userManager.GetUserAsync(User);
            var unFollow = followsRepository.Follows.Where(x => x.FollowingUserId == currentUser.Id && x.FollowerUserId == selectedUser.Id).FirstOrDefault();
            followsRepository.UnFollow(unFollow);
            return Redirect($"http://localhost:5000/{selectedUser.UserName}");
        }
    }
}
