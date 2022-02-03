using InClassApp.Models.Dtos;
using InClassApp.Models.Entities;
using InClassApp.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InClassApp.Controllers
{
    /// <summary>
    /// Controller for user management
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ILecturersRepository _lecturersRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        /// <summary>
        /// Users controller constructor
        /// </summary>
        public UsersController(IUserRepository userRepository, IServiceProvider serviceProvider,
            ILecturersRepository lecturersRepository, IStudentRepository studentRepository)
        {
            _userRepository = userRepository;
            _lecturersRepository = lecturersRepository;
            _studentRepository = studentRepository;
            _userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        }

        /// <summary>
        /// Gets view with all users list
        /// </summary>
        /// <returns>Users list view</returns>
        public async Task<IActionResult> UsersList()
        {
            var users = await _userRepository.GetAll();
            Dictionary<string, string> userRolesByUserId = new Dictionary<string, string>();

            foreach(var user in users)
            {
                var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                if (role != null)
                {
                    userRolesByUserId.Add(user.Id, role);
                }
            }
            ViewData["UserRolesDictionary"] = userRolesByUserId;
            return View(users);
        }

        /// <summary>
        /// Gets view with user details
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>User details view</returns>
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userRepository.GetById(id);
            if (user == null)
            {
                return NotFound();
            }
            var currentUserRoles = await _userManager.GetRolesAsync(user);

            ViewData["Role"] = currentUserRoles.FirstOrDefault();
            return View(user);
        }

        /// <summary>
        /// Gets view with user edit form
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>Edit user view</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userRepository.GetById(id);
            if (user == null)
            {
                return NotFound();
            }
            var currentUserRoles = await _userManager.GetRolesAsync(user);

            SaveUserDto model = new SaveUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Role = currentUserRoles.FirstOrDefault()
            };

            ViewData["Roles"] = (await _roleManager.Roles.ToListAsync()).Select(x => x.Name);
            return View(model);
        }

        /// <summary>
        /// Saves edited user if form is valid
        /// </summary>
        /// <param name="id">User id</param>
        /// <param name="userDto">User dto to save</param>
        /// <returns>Users list view if saved successfully; otherwise showes an error message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Name,Surname,Email,PhoneNumber, Index, Role,Id")] SaveUserDto userDto)
        {
            if (string.IsNullOrEmpty(id) || !id.Equals(userDto.Id))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userToUpdate = await _userRepository.GetById(id);
                    var currentRoles = await _userManager.GetRolesAsync(userToUpdate);

                    if(!userToUpdate.Email.Equals(userDto.Email) && await _userManager.FindByEmailAsync(userDto.Email) != null)
                    {
                        return View(userDto);
                    }

                    userToUpdate.Name = userDto.Name;
                    userToUpdate.Surname = userDto.Surname;
                    userToUpdate.PhoneNumber = userDto.PhoneNumber;
                    userToUpdate.Email = userDto.Email;

                    await _userRepository.Update(userToUpdate);

                    if (!currentRoles.Contains(userDto.Role))
                    {
                        await UserRoleChangedUpdateContext(id, currentRoles, userToUpdate, userDto);
                    }
                    else if((currentRoles.Contains("Student")))
                    {
                        var student = await _studentRepository.GetStudentByUserId(id);
                        student.Index = userDto.Index;
                        await _studentRepository.Update(student);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await UserExists(userDto.Id)))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(UsersList));
            }
            return View(userDto);
        }

        private async Task<IdentityResult> UserRoleChangedUpdateContext(string userId, IList<string> currentRoles, AppUser userToUpdate, SaveUserDto userDto)
        {
            if (currentRoles.Count > 0)
            {
                if (currentRoles.Contains("Lecturer"))
                {
                    var lecturer = await _lecturersRepository.GetLecturerByUserId(userId);
                    await _lecturersRepository.Delete(lecturer.Id);
                }
                else if (currentRoles.Contains("Student"))
                {
                    var student = await _studentRepository.GetStudentByUserId(userId);
                    await _studentRepository.Delete(student.Id);
                }
                await _userManager.RemoveFromRolesAsync(userToUpdate, currentRoles);
            }

            var result = await _userManager.AddToRoleAsync(userToUpdate, userDto.Role);

            if (userDto.Role.Equals("Lecturer"))
            {
                var lecturer = new Lecturer
                {
                    UserId = userId
                };
                await _lecturersRepository.Add(lecturer);
            }
            else if (userDto.Role.Equals("Student"))
            {
                var student = new Student
                {
                    UserId = userId,
                    Index = userDto.Index
                };
                await _studentRepository.Add(student);
            }
            return result;
        }

        private async Task<bool> UserExists(string id)
        {
            var users = await _userRepository.GetAll();
            return users.Any(e => e.Id == id);
        }
    }
}
