using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Career.Data;
using Career.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Career.Controllers
{
    /// <summary>
    /// The student controller 
    /// </summary>
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        #region Protected Members

        /// <summary>
        /// The scoped Application context
        /// </summary>
        protected ApplicationDbContext mContext;

        /// <summary>
        /// The manager for handling user creation, deletion, searching, roles etc...
        /// </summary>
        protected UserManager<ApplicationUser> mUserManager;

        /// <summary>
        /// The manager for handling signing in and out for our users
        /// </summary>
        protected SignInManager<ApplicationUser> mSignInManager;

        #endregion

        /// <summary>
        /// The default constructor
        /// </summary>
        /// <param name="context"> The injected context </param>
        /// <param name="userManager"> The injected User manager </param>
        /// <param name="signInManager"> The injected Sign in manager </param>
        public StudentController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            mContext = context;
            mUserManager = userManager;
            mSignInManager = signInManager;
        }

        /// <summary>
        /// Student profile/ the main view for the student
        /// </summary>
        /// <returns>Student details view</returns>
        public async Task<IActionResult> Index()
        {
            // Getting the current user
            var user = await mUserManager.GetUserAsync(User);

            // Getting the current student assigned to this user
            var student = mContext.Students.Where(x => x.User == user).Include("Skills").FirstOrDefault();

            // If we have no student assign, this means this is his first time here
            if (student == null)
            {
                // Redirect him to the add view so that the student enter his information
                return RedirectToAction("Add");
            }

            // Get the discussion that the current user involved in
            var discussions = mContext.Discussions.Where(d => d.From.Id == user.Id || d.To.Id == user.Id)
                .Include(x => x.From.Employer)
                .Include(x => x.To.Employer)
                .OrderByDescending(d => d.SendingDate)
                .ToList();

            // Pass all the details to the view model
            var model = new StudentProfileViewModel() { Student = student, Discussions = discussions };

            // if we have a student assign to this user, view his information,,,
            return View(model);
        }

        /// <summary>
        /// Giving the view which allow the Students to edit their information 
        /// </summary>
        /// <returns>Student information edit view</returns>
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            // Getting the current user
            var user = await mUserManager.GetUserAsync(User);

            // Getting the current student assigned to this user
            var student = mContext.Students.Where(x => x.User == user).FirstOrDefault();

            // If we have no student assign, this means this is his first time here
            if (student == null)
            {
                // Redirect him to the add view so that the student enter his information
                return RedirectToAction("Add");
            }

            // if we have a student assign to this user, allow him to edit his information,,,
            return View(student);
        }

        /// <summary>
        /// Editting the information of the current student
        /// </summary>
        /// <param name="student">Student model containing the information from the view</param>
        /// <param name="resume">Cv file</param>
        /// <returns>redirects to the appropriate view according to the results</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(Student student, IFormFile resume, IFormFile profilePicture)
        {
            // getting the current user
            var user = await mUserManager.GetUserAsync(User);

            // Reassign this student to the current user
            student.User = user;

            // Validation of the CV file
            // Check if there is CV file selected to be uploaded
            if (resume != null && resume.Length > 0)
            {
                // Check the size of the CV file to be less than 2MB
                if ((resume.Length / 1048576.0) > 2)
                {
                    // If Large, display error message
                    ModelState.AddModelError("", "CV file is too large");

                    // Let the user try again
                    return View(student);
                }

                // The supported types for the CV,
                var supportedTypes = new[] { "doc", "docx", "pdf" };

                // Get the file extension of the uploaded CV file
                var fileExt = Path.GetExtension(resume.FileName).Substring(1);

                // Check if the extension of the CV file is supported
                if (!supportedTypes.Contains(fileExt))
                {
                    // If not, display error message
                    ModelState.AddModelError("", "CV File Type Is Invalid - Only Upload WORD/PDF File");

                    // Let the user try again
                    return View(student);
                }

                // Making the CV file name
                student.CV = $"{student.User.Id}.{fileExt}";

                // Where to save the CV file 
                var path = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot", "Resumes",
                            student.CV);

                // Saving the CV file 
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await resume.CopyToAsync(stream);
                }
            }

            // Validation for the profile picture
            // Check if there is Image selected to be uploaded
            if (profilePicture != null && profilePicture.Length > 0)
            {
                // Check the size of the Profile picture file to be less than 2MB
                if ((profilePicture.Length / 1048576.0) > 2)
                {
                    // If larger, display error message
                    ModelState.AddModelError("", "Profile picture file is too large");

                    // Let the user try again
                    return View(student);
                }

                // Check the type of the profile picture image
                if (profilePicture.ContentType.ToLower().StartsWith("image/"))
                {
                    // If supported, get the etension of the profile picture file
                    var imageExt = Path.GetExtension(profilePicture.FileName).Substring(1);

                    // Making the name of the profile picture file 
                    student.Image = $"{student.User.Id}.{imageExt}";

                    // Where to save the profile picture file 
                    var imagePath = Path.Combine(
                                Directory.GetCurrentDirectory(), "wwwroot", "ProfilePictures", "Students",
                                student.Image);

                    // Saving the profile picture file 
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await profilePicture.CopyToAsync(stream);
                    }
                }
                else
                {
                    // If the type of the profile picture file is not supported, display error message
                    ModelState.AddModelError("", "Picture File Type Is Invalid - Only Upload PNG,JPG or JPEG File");

                    // Let the user try again
                    return View(student);
                }
            }

            // Update the changes done to the student
            mContext.Students.Update(student);

            // Save the changes to the database
            mContext.SaveChanges();

            // Redirect to the Student Profile (Index) view
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Giving the view which allow the new student to enter his information 
        /// </summary>
        /// <returns>Student information entry view</returns>
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// Adding the information to the new students
        /// </summary>
        /// <param name="student">Student model containing the information from the view</param>
        /// <param name="resume">Cv file</param>
        /// <returns>redirects to the appropriate view according to the results</returns>
        [HttpPost]
        public async Task<IActionResult> Add(Student student, IFormFile resume, IFormFile profilePicture)
        {
            // getting the current user
            var user = await mUserManager.GetUserAsync(User);

            // if yes, Reassign this student to the current user
            student.User = user;

            // Validation of the cv file
            // Making sure there is a CV file selected to be uploaded
            if (resume == null || resume.Length == 0)
            {
                // If not, display error message
                ModelState.AddModelError("", "CV file not selected");

                // Let the user try again
                return View(student);
            }

            // Check the size of the CV file to be less than 2MB
            if ((resume.Length / 1048576.0) > 2)
            {
                // If larger, display error message
                ModelState.AddModelError("", "the CV file is too large");

                // Let the user try again
                return View(student);
            }

            // The supported types for the CV,
            var supportedTypes = new[] { "doc", "docx", "pdf" };

            // Get the file extension of the uploaded CV file
            var fileExt = Path.GetExtension(resume.FileName).Substring(1);

            // Check if the extension of the CV file is supported
            if (!supportedTypes.Contains(fileExt))
            {
                // If not, display error message
                ModelState.AddModelError("", "CV File Type Is Invalid - Only Upload WORD/PDF File");

                // Let the user try again
                return View(student);
            }

            // Validation for the image file
            // Check if the user selected profile picture file to be uploaded
            if (profilePicture == null || profilePicture.Length == 0)
            {
                // If not, user the default
                student.Image = "DefaultStudent.png";
            }
            // If yes,
            else
            {
                // Check if the profile picture image size is less than 2MB
                if ((profilePicture.Length / 1048576.0) > 2)
                {
                    // If larger, display error message
                    ModelState.AddModelError("", "Profile picture file is too large");

                    // Let the user try again
                    return View(student);
                }
                // Check if the profile picture file type is supported
                if (profilePicture.ContentType.ToLower().StartsWith("image/"))
                {
                    // If supported, get the etension of the profile picture file
                    var imageExt = Path.GetExtension(profilePicture.FileName).Substring(1);

                    // Making the name of the profile picture file 
                    student.Image = $"{student.User.Id}.{imageExt}";

                    // Where to save the profile picture file 
                    var imagePath = Path.Combine(
                                Directory.GetCurrentDirectory(), "wwwroot", "ProfilePictures", "Students",
                                student.Image);

                    // Saving the profile picture file 
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await profilePicture.CopyToAsync(stream);
                    }
                }
                else
                {
                    // If the type of the profile picture file is not supported, display error message
                    ModelState.AddModelError("", "Picture File Type Is Invalid - Only Upload PNG,JPG or JPEG File");

                    // Let the user try again
                    return View(student);
                }
            }

            // Making the cv file name
            student.CV = $"{student.User.Id}.{fileExt}";

            // Where to save the file 
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot", "Resumes",
                        student.CV);

            // Saving the file 
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await resume.CopyToAsync(stream);
            }

            // Adding this new student to the students table
            mContext.Students.Add(student);

            // Save the changes to the database 
            mContext.SaveChanges();

            // Redirect to the Student Profile (Index) view
            return RedirectToAction("Index");

        }

        /// <summary>
        /// Giving the ablity to download the cv file
        /// </summary>
        /// <param name="filename">taking the file name</param>
        /// <returns>Download the file</returns>
        [HttpPost]
        public async Task<IActionResult> DownloadCV(string filename)
        {
            // Checking if the file name was passed,
            if (filename == null)
                // If not, display error message
                return Content("Error: CV File was is corrupted or deleted,,,");

            // The path to the file requisted
            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot", "Resumes", filename);

            // Making sure that the requisted file exists
            if (!System.IO.File.Exists(path))
            {
                // If not, display error message
                return Content("Error: CV File was is corrupted or deleted,,,");
            }

            // Using memory stream
            using (var memory = new MemoryStream())
            {
                // Using file stream with the path to the requisted file
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    // Copy the data from the file to the memory
                    await stream.CopyToAsync(memory);
                }

                // Reset the position of the memnory
                memory.Position = 0;

                // Return the data of the file from the memory
                return File(memory.ToArray(), GetContentType(path), Path.GetFileName(path));
            }
        }

        /// <summary>
        /// Getting the skills of the current student and pass them to the view to view them
        /// </summary>
        /// <returns>List of skills view with the ability to add and remove</returns>
        [HttpGet]
        public async Task<IActionResult> EditSkills()
        {
            // Get current user
            var user = await mUserManager.GetUserAsync(User);

            // Get the student of the same user
            var student = mContext.Students.Where(st => st.User.Id == user.Id).Include("Skills").First();

            // If there is no student linked to this user,
            if (student == null)
                // redirect him to add so that we make a student and link it to this user
                return RedirectToAction("Add");

            // Make sure that we have a list of skills 
            if (student.Skills == null)
                student.Skills = new List<Skill>();

            // Give the skills to the view to view them
            return View(student.Skills);
        }

        /// <summary>
        /// Action to add the passed skill to the current student, also check that there is no confilct 
        /// </summary>
        /// <param name="skillName">The skill name of the new skill</param>
        /// <returns>Redirect the user back to the same view, edit skill view</returns>
        [HttpPost]
        public async Task<IActionResult> AddSkill(string skillName)
        {
            // Get current user
            var user = await mUserManager.GetUserAsync(User);

            // Get the student of the same user
            var student = mContext.Students.Where(st => st.User.Id == user.Id).Include("Skills").First();

            // If this is the first skill, we need to initialize the skill property
            if (student.Skills == null)
                student.Skills = new List<Skill>();

            // Making sure this skill isn't already in this student
            if (student.Skills.Where(s => s.SkillName.Equals(skillName,StringComparison.OrdinalIgnoreCase)).Count() > 0)
            {
                // Gives error message
                ModelState.AddModelError("", "You already have this skill");

                // Give the skills to the view to view them
                return View("EditSkills", student.Skills);
            }

            // Add the skill to this student
            student.Skills.Add(new Skill() { SkillName = skillName });

            // Update the database 
            mContext.Students.Update(student);

            // Save the changes
            mContext.SaveChanges();

            // Give the skills to the view to view them
            return View("EditSkills", student.Skills);
        }

        /// <summary>
        /// Action to remove the skill with the passed id from the current user, also making sure that he has the skill before removing it.
        /// </summary>
        /// <param name="id">The id of the skill to be removed</param>
        /// <returns>Redirect the user back to the same view, edit skill view</returns>
        [HttpGet]
        public async Task<IActionResult> RemoveSkill(int id)
        {
            // Get current user
            var user = await mUserManager.GetUserAsync(User);

            // Get the student of the same user
            var student = mContext.Students.Where(st => st.User.Id == user.Id).Include("Skills").First();

            // Get the skill we want to delete
            var skill = student.Skills.Where(s => s.Id == id).FirstOrDefault();

            // If the student don't have this skill
            if (skill == null)
            {
                // Gives error message
                ModelState.AddModelError("", "You don't have this skill");

                // Give the skills to the view to view them
                return View("EditSkills", student.Skills);
            }

            // remove the skill from the user
            student.Skills.Remove(skill);

            // Update the database 
            mContext.Students.Update(student);

            // Save the changes
            mContext.SaveChanges();

            // Give the skills to the view to view them
            return View("EditSkills", student.Skills);
        }

        /// <summary>
        /// To search for the jobs that matches this student skills and display them
        /// </summary>
        /// <returns>A view that has the search resaults</returns>
        [HttpPost]
        public async Task<IActionResult> Match()
        {
            // Get current user
            var user = await mUserManager.GetUserAsync(User);

            // Get the student of the same user
            var student = mContext.Students.Where(st => st.User.Id == user.Id).Include("Skills").First();

            // If there is no student linked to this user,
            if (student == null)
                // redirect him to add so that we make a student and link it to this user
                return RedirectToAction("Add");

            // Get all the jobs that user skill satisfy
            List<Job> jobs = mContext.Jobs.Include("Skills").Include("JobOwner").
                                    // Making sure that thejob is viable and the student age is within the allowed age
                                    Where(j => j.IsApproved && !j.IsExpired && student.Age >= j.MinAge && student.Age <= j.MaxAge &&
                                                // Get student skills and compare them with the skill required by the job
                                                student.Skills.Select(s => s.SkillName.ToLower())
                                                    // Find matched string names, ignoreing the case 
                                                    .Intersect(j.Skills.Select(sk => sk.SkillName.ToLower()))
                                                        // If the number of matched skills was the same as the number of skills required
                                                        // by the job, then this job is available to this student
                                                        .Count() == j.Skills.Count())
                                                            // Sort the newer jobs first ... 
                                                            .OrderByDescending(j=>j.JobID).ToList();

            // Go to the view
            return View(jobs);
        }

        /// <summary>
        /// Get employer profile
        /// </summary>
        /// <param name="employerId">The id of the requisted employer</param>
        /// <returns>View with the employer detail and the ability to send message</returns>
        [HttpPost]
        public IActionResult EmployerProfile(int employerId)
        {
            // Get the employer that has the requisted Id
            var employer = mContext.Employers.Where(em => em.EmployerID == employerId).First();

            // Return the details of the that employer
            return View(employer);
        }

        /// <summary>
        /// Disscusion, sending message to job owner
        /// </summary>
        /// <param name="employerId">The id of the reciver of the message</param>
        /// <param name="description">The body of the message</param>
        /// <returns>Send confirmation email to the student and return him to his profile</returns>
        public async Task<IActionResult> SendMessage(int employerId, string description)
        {
            // Get current user
            var user = await mUserManager.GetUserAsync(User);

            // Get the employer that we are about to contact
            var employer = mContext.Employers.Where(em => em.EmployerID == employerId).Include("User").First();

            // Make message and it's parameter
            var discussion = new Discussion() { To = employer.User, From = user, Message = description, SendingDate = DateTime.Now };

            // Add the message to the database 
            mContext.Discussions.Add(discussion);

            // Save the changes
            mContext.SaveChanges();

            //TODO: Send email confirmation to the user

            // Redirect to the home 
            return RedirectToAction("Index");
        }

        /// <summary>
        /// View all the conversation related to this message
        /// </summary>
        /// <param name="discussion"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Conversation(Discussion discussion)
        {
            // Get the discussion that the user clicked on 
            discussion = mContext.Discussions.Where(dis => dis.MessageID == discussion.MessageID)
                                            // Including all the required data
                                            .Include(dis => dis.From)
                                            .Include(dis => dis.To).FirstOrDefault();

            // Getting the related messages from the database 
            var messages = mContext.Discussions
                                .Where(dis =>
                                (dis.From == discussion.From || dis.From == discussion.To) &&
                                (dis.To == discussion.To || dis.To == discussion.From))
                                    // Including all the required data
                                    .Include(m => m.From.Employer).Include(m => m.From.Student)
                                    .Include(m => m.To.Employer).Include(m => m.To.Student)
                                        // Sorting from the newer to the older
                                        .OrderByDescending(d => d.SendingDate).ToList();

            // Veiw the related messages and give the user the ability to replay
            return View(messages);
        }

        #region Private Helpers

        /// <summary>
        /// Determine the type of the file
        /// </summary>
        /// <param name="path">The file path / name</param>
        /// <returns>the type of the file</returns>
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        /// <summary>
        /// All the supported file type for download
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
            };
        }

        #endregion
    }
}