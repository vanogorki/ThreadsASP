using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ThreadsASP.Models.ViewModels;
using ThreadsASP.Models;
using ThreadsASP.FileUploadService;
using ThreadsASP.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;
using System.Drawing.Imaging;

namespace ThreadsASP.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private IPostsRepository _postsRepository;
        private UserManager<ApplicationUser> _userManager;
        private readonly IFileService _fileUploadService;
        private IUserRepository _userRepository;
        private IFollowsRepository _followsRepository;

        public UserProfileController(UserManager<ApplicationUser> userManager,
            IPostsRepository postsRepository, IFileService fileUploadService,
            IUserRepository userRepository, IFollowsRepository followsRepository)
        {
            _postsRepository = postsRepository;
            _userManager = userManager;
            _fileUploadService = fileUploadService;
            _userRepository = userRepository;
            _followsRepository = followsRepository;
        }

        [HttpGet("{accName}")]
        public async Task<IActionResult> Index(string accName)
        {
            var selectedUser = await _userManager.FindByNameAsync(accName);
            var currentUser = await _userManager.GetUserAsync(User);
            if (selectedUser == null)
            {
                return NotFound();
            }
            return View(new UserPageViewModel
            {
                Posts = _postsRepository.GetUserPosts(selectedUser.Id),
                IsFollowing = _followsRepository.IsFollowing(currentUser.Id, selectedUser.Id),
                SelectedUser = selectedUser,
                IsCurrentUser = (selectedUser == currentUser) ? true : false
            });
        }

        [HttpPost]
        public IActionResult DeletePost(long Id, string accName)
        {
            var removePost = _postsRepository.Posts.FirstOrDefault(p => p.Id == Id);
            if (removePost == null)
            {
                return NotFound();
            }
            if (_userManager.Users.FirstOrDefault(u => u.ProfileImgName == removePost.ImgName) == null)
            {
                LocalFileService.DeleteImage(removePost.ImgName);
            }
            _postsRepository.DeletePost(removePost);
            return Redirect($"http://localhost:5000/{accName}");
        }
        
        [HttpPost]
        public IActionResult EditPost(long? Id)
        {
            var editPost = _postsRepository.Posts.FirstOrDefault(p => p.Id == Id);
            if (editPost == null)
            {
                return NotFound();
            }
            return RedirectToAction("CreatePost", "Home", editPost);
        }

        [HttpGet("{action}")]
        public IActionResult CropImage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SetProfilePicture(IFormFile blob)
        {
            var user = await _userManager.GetUserAsync(User);
            if (blob != null)
            {
                var newFileName = Guid.NewGuid().ToString() + ".png";
                _fileUploadService.UploadProfileImage(blob, newFileName);
                if (user.ProfileImgName != "Default.jpg")
                {
                    LocalFileService.DeleteImage(user.ProfileImgName);
                }
                user.ProfileImgName = newFileName;
                _userRepository.SaveNewProfilePicture();
                return Json(new { Message = $"http://localhost:5000/{user.UserName}" });
            }
            return Json(new { Message = $"http://localhost:5000/{user.UserName}" });
        }

        public async Task<IActionResult> Follow(string selectedUserId)
        {
            var selectedUser = await _userManager.FindByIdAsync(selectedUserId);
            var currentUser = await _userManager.GetUserAsync(User);
            _followsRepository.Follow(currentUser, selectedUser);
            return Redirect($"http://localhost:5000/{selectedUser.UserName}");
        }

        public async Task<IActionResult> UnFollow(string selectedUserId)
        {
            var selectedUser = await _userManager.FindByIdAsync(selectedUserId);
            var currentUser = await _userManager.GetUserAsync(User);
            var unFollow = _followsRepository.Follows.Where(x => x.FollowingUserId == currentUser.Id && x.FollowerUserId == selectedUser.Id).FirstOrDefault();
            _followsRepository.UnFollow(unFollow);
            return Redirect($"http://localhost:5000/{selectedUser.UserName}");
        }
    }
}
