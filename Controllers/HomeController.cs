﻿using Microsoft.AspNetCore.Authorization;
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
        private IPostsRepository _postsRepository;
        private UserManager<ApplicationUser> _userManager;
        private readonly IFileService _fileUploadService;
        private IFollowsRepository _followsRepository;
        private ILikesRepository _likesRepository;

        public HomeController(UserManager<ApplicationUser> userManager,
            IPostsRepository postsRepository, IFileService fileUploadService, 
            IFollowsRepository followsRepository, ILikesRepository likesRepository)
        {
            _postsRepository = postsRepository;
            _userManager = userManager;
            _fileUploadService = fileUploadService;
            _followsRepository = followsRepository;
            _likesRepository = likesRepository;
        }


        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var follows = await _followsRepository.Follows.Where(x => x.FollowingUserId == currentUser.Id).ToListAsync();
            return View(new HomeViewModel
            {
                Posts = await _postsRepository.Posts.Where(c => follows.Select(
                    s => s.FollowerUserId).Contains(c.AppUserId) || c.AppUserId == currentUser.Id).OrderByDescending(i => i.Id).ToListAsync(),
                CurrentUser = currentUser
            });
        }
        

        public async Task<IActionResult> SideBarPartial()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return PartialView("_SideBarPartial", currentUser.UserName);
        }

        public async Task<IActionResult> SearchBarPartial()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var suggestingUsers = await _userManager.Users.Where(
                s => s.Id != currentUser.Id && !_followsRepository.Follows.Where(
                    x => x.FollowingUserId == currentUser.Id).Select(d => d.FollowerUserId).Contains(s.Id)).ToListAsync();

            var rand = new Random();
            suggestingUsers = suggestingUsers.OrderBy(_ => rand.Next()).Take(5).ToList();
            return PartialView("_SearchBarPartial", suggestingUsers);
        }


        [HttpGet("{action}")]
        public async Task<IActionResult> SearchResult(string searchString)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserFollows = await _followsRepository.Follows.Where(x => x.FollowingUserId == currentUser.Id).ToListAsync();

            var searchUsers = await _userManager.Users.Where(x => x.UserName.Contains(searchString) || 
                x.FirstName.Contains(searchString) || x.LastName.Contains(searchString)).Take(5).ToListAsync();

            var searchPosts = await _postsRepository.Posts.Where(x => x.Text.Contains(searchString)).Take(5).ToListAsync();
            return View(new SearchResultViewModel
            {
                SearchUsers = searchUsers,
                SearchPosts = searchPosts,
                SearchString = searchString,
                CurrentUserId = currentUser.Id,
                CurrentUserFollows = currentUserFollows
            });
        }
        

        [HttpGet("{action}")]
        public async Task<IActionResult> CreatePost(Post editPost, long? repostId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var repost = await _postsRepository.Posts.FirstOrDefaultAsync(x => x.Id == repostId);
            if (repost != null)
            {
                return View(new CreatePostViewModel
                {
                    NewPost = new Post { AppUser = currentUser },
                    Repost = repost
                });
            }
            if (editPost?.Id != 0 && editPost.AppUserId == currentUser.Id)
            {
                editPost.AppUser = currentUser;
                return View(new CreatePostViewModel
                {
                    NewPost = editPost
                });
            }
            return View(new CreatePostViewModel
            {
                NewPost = new Post { AppUser =  currentUser }
            });
        }


        [HttpPost("{action}")]
        public async Task<IActionResult> CreatePost(long? editPostId, long? repostId, string TextArea, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                var newFileName = Guid.NewGuid().ToString() + ".png";
                if (file != null)
                {
                    await _fileUploadService.UploadPostImageAsync(file, newFileName);
                }
                if (editPostId != 0)
                {
                    var oldPost = _postsRepository.Posts.FirstOrDefault(p => p.Id == editPostId);
                    LocalFileService.DeleteImage(oldPost.ImgName);
                    _postsRepository.DeletePost(oldPost);
                    await CreatePostMethodAsync(TextArea, file, newFileName, null);
                    return RedirectToAction("Index");
                }
                if (repostId != 0)
                {
                    var repost = _postsRepository.Posts.FirstOrDefault(r => r.Id == repostId);
                    await CreatePostMethodAsync(TextArea, file, newFileName, repost);
                    return RedirectToAction("Index");
                }
                await CreatePostMethodAsync(TextArea, file, newFileName, null);
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendLike(long postId)
        {
            var post = await _postsRepository.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            var currentUser = await _userManager.GetUserAsync(User);
            _likesRepository.AddLike(new Like
            {
                User = currentUser,
                UserId = currentUser.Id,
                Post = post,
                PostId = post.Id
            });
            string jsCode = "<script>window.history.back();</script>";
            return Content(jsCode, "text/html");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveLike(long postId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var like = await _likesRepository.Likes.FirstOrDefaultAsync(x => x.UserId == currentUser.Id && x.PostId == postId);
            var post = await _postsRepository.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            _likesRepository.RemoveLike(like);
            string jsCode = "<script>window.history.back();</script>";
            return Content(jsCode, "text/html");
        }

        private async Task CreatePostMethodAsync(string TextArea, IFormFile? file, string newFileName, Post? repost)
        {
            var newPost = new Post()
            {
                AppUser = await _userManager.GetUserAsync(User),
                Text = TextArea,
                Date = DateTime.Now.ToString("MMMM dd"),
                ImgName = (file != null) ? newFileName : null,
                Repost = repost,
                RepostId = repost?.Id
            };
            if (repost != null)
            {
                repost.RepostsCount++;
            }
            _postsRepository.CreatePost(newPost);
        }
    }
}
