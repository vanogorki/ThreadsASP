using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ThreadsASP.Models.ViewModels;
using ThreadsASP.Models;
using ThreadsASP.FileUploadService;
using ThreadsASP.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

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

        [Route("{accName}")]
        public async Task<IActionResult> Index(string accName)
        {
            var selectedUser = await _userManager.FindByNameAsync(accName);
            var currentUser = await _userManager.GetUserAsync(User);
            if (selectedUser == null)
            {
                return NotFound();
            }
            if (selectedUser.IsBlocked == true)
            {
                return View("Blocked", selectedUser.UserName);
            }
            return View(new UserPageViewModel
            {
                Posts = await _postsRepository.Posts.Where(x => x.AppUserId == selectedUser.Id).OrderByDescending(i => i.Id).ToListAsync(),
                IsFollowing = _followsRepository.IsFollowing(currentUser.Id, selectedUser.Id),
                SelectedUser = selectedUser,
                CurrentUser = currentUser,
                IsCurrentUser = (selectedUser == currentUser) ? true : false,
                FollowingCount = _followsRepository.Follows.Where(x => x.FollowingUserId == selectedUser.Id).Count(),
                FollowersCount = _followsRepository.Follows.Where(z => z.FollowerUserId == selectedUser.Id).Count()
            });
        }

        public IActionResult DeletePost(long Id, string accName)
        {
            var removePost = _postsRepository.Posts.FirstOrDefault(p => p.Id == Id);
            if (removePost == null)
            {
                return NotFound();
            }
            if (removePost.IsReposted == true)
            {
                removePost.Repost.RepostsCount--;
            }
            LocalFileService.DeleteImage(removePost.ImgName);
            _postsRepository.DeletePost(removePost);
            return Redirect($"http://localhost:5000/{accName}");
        }

        [Route("{action}")]
        public async Task<IActionResult> CropImage()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return View("CropImage", currentUser.ProfileImgName);
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
                _userRepository.SaveNewProfileData();
                return Json(new { Message = $"http://localhost:5000/{user.UserName}" });
            }
            return Json(new { Message = $"http://localhost:5000/{user.UserName}" });
        }

        [HttpPost]
        public async Task Follow(string selectedUserId)
        {
            var selectedUser = await _userManager.FindByIdAsync(selectedUserId);
            var currentUser = await _userManager.GetUserAsync(User);
            var unFollow = await _followsRepository.Follows.FirstOrDefaultAsync(x => x.FollowingUserId == currentUser.Id && x.FollowerUserId == selectedUser.Id);
            if (unFollow != null)
            {
                _followsRepository.UnFollow(unFollow);
                return;
            }
            _followsRepository.Follow(new Follow
            {
                FollowerUser = selectedUser,
                FollowerUserId = selectedUserId,
                FollowingUser = currentUser,
                FollowingUserId = selectedUserId
            });
        }

        [Route("{accName}/{action}")]
        public async Task<IActionResult> Followers(string accName)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var selectedUser = await _userManager.FindByNameAsync(accName);
            var currentUserFollows = await _followsRepository.Follows.Where(x => x.FollowingUserId == currentUser.Id).ToListAsync();

            var followersList = await _userManager.Users.Where(u => _followsRepository.Follows.Where(
                x => x.FollowerUserId == selectedUser.Id).Select(c => c.FollowingUserId).Contains(u.Id)).ToListAsync();

            return View("FollowList", new FollowListViewModel
            {
                Users = followersList,
                SelectedUser = selectedUser,
                CurrentUser = currentUser,
                CurrentUserFollows = currentUserFollows
            });
        }

        [Route("{accName}/{action}")]
        public async Task<IActionResult> Following(string accName)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var selectedUser = await _userManager.FindByNameAsync(accName);
            var currentUserFollows = await _followsRepository.Follows.Where(x => x.FollowingUserId == currentUser.Id).ToListAsync();

            var followingList = await _userManager.Users.Where(u => _followsRepository.Follows.Where(
                x => x.FollowingUserId == selectedUser.Id).Select(c => c.FollowerUserId).Contains(u.Id)).ToListAsync();

            return View("FollowList", new FollowListViewModel
            {
                Users = followingList,
                SelectedUser = selectedUser,
                CurrentUser = currentUser,
                CurrentUserFollows = currentUserFollows
            });
        }
    }
}
