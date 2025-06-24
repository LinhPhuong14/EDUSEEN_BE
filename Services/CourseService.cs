using AutoMapper;
using Sep490_Eduseen_BE.Dtos.Course;
using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Sep490_Eduseen_BE.Exceptions;
using Sep490_Eduseen_BE.Repositories.impl;
using Sep490_Eduseen_BE.Dtos;
using Microsoft.EntityFrameworkCore;
using Sep490_Eduseen_BE.Dtos.Teacher;

namespace Sep490_Eduseen_BE.Services
{
    public class CourseService : ICourseService
    {
        private readonly Sep490EduseenContext _context;
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;

        public CourseService(Sep490EduseenContext context , ICourseRepository courseRepository, IMapper mapper)
        {
            _courseRepository = courseRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync()
        {
            var courses = await _courseRepository.GetAllCoursesAsync();
            var courseDtos = _mapper.Map<IEnumerable<CourseDto>>(courses);
            return courseDtos;
        }

        public async Task<IEnumerable<CourseDto>> SearchCoursesAsync(string courseName)
        {
            var courses = await _courseRepository.SearchCoursesAsync(courseName);
            return _mapper.Map<IEnumerable<CourseDto>>(courses);
        }

        public async Task<CourseDetailDto> GetCourseByIdAsync(int courseId)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                return null;
            }
            return _mapper.Map<CourseDetailDto>(course);
        }

        public async Task<IEnumerable<CourseDto>> GetMyCoursesAsync(int studentId)
        {
            var courses = await _courseRepository.GetEnrolledCoursesByStudentIdAsync(studentId);
            return _mapper.Map<IEnumerable<CourseDto>>(courses);
        }

        public async Task<(bool Success, string Message)> SaveFavoriteCourseAsync(int studentId, int courseId)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                return (false, "Course not found.");
            }

            var existingFavorite = await _courseRepository.GetFavoriteAsync(studentId, courseId);
            if (existingFavorite != null)
            {
                return (false, "Course is already in favorites.");
            }

            var favorite = new Favorite
            {
                StudentId = studentId,
                CourseId = courseId,
                CreatedAt = DateTime.UtcNow
            };

            await _courseRepository.AddFavoriteAsync(favorite);

            return (true, "Course added to favorites successfully.");
        }

        public async Task<(bool Success, string Message, IEnumerable<LectureDto> Data)> GetCourseMaterialsAsync(int studentId, int courseId)
        {
            var isEnrolled = await _courseRepository.IsUserEnrolledAsync(studentId, courseId);
            if (!isEnrolled)
            {
                return (false, "User is not enrolled in this course.", []);
            }

            var lectures = await _courseRepository.GetLecturesByCourseIdAsync(courseId);
            var lectureDtos = _mapper.Map<IEnumerable<LectureDto>>(lectures);

            return (true, "Successfully retrieved course materials.", lectureDtos);
        }

        public async Task<CourseProgressDto> GetCourseProgressAsync(int studentId, int courseId)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                throw new CourseNotFoundException($"Course with ID {courseId} not found.");
            }

            var isEnrolled = await _courseRepository.IsUserEnrolledAsync(studentId, courseId);
            if (!isEnrolled)
            {
                return null; 
            }

            var allLectures = await _courseRepository.GetLecturesByCourseIdAsync(courseId);
            var totalLectures = allLectures.Count();

            if (totalLectures == 0)
            {
                return new CourseProgressDto
                {
                    CourseId = courseId,
                    TotalLectures = 0,
                    CompletedLectures = 0,
                    ProgressPercentage = 0 // Or 0, depending on business logic for empty courses
                };
            }

            var completedLectures = await _courseRepository.GetCompletedLecturesCountAsync(studentId, courseId);

            return new CourseProgressDto
            {
                CourseId = courseId,
                TotalLectures = totalLectures,
                CompletedLectures = completedLectures,
                ProgressPercentage = Math.Round((double)completedLectures / totalLectures * 100, 2)
            };
        }

        public async Task<IEnumerable<CourseDto>> GetCompletedCoursesAsync(int studentId)
        {
            var enrolledCourses = await _courseRepository.GetEnrolledCoursesByStudentIdAsync(studentId);
            var completedCourses = new List<Course>();

            foreach (var course in enrolledCourses)
            {
                var totalLectures = (await _courseRepository.GetLecturesByCourseIdAsync(course.CourseId)).Count();
                if (totalLectures == 0)
                {
                    continue; // Skip courses with no lectures
                }

                var completedLecturesCount = await _courseRepository.GetCompletedLecturesCountAsync(studentId, course.CourseId);

                if (totalLectures == completedLecturesCount)
                {
                    completedCourses.Add(course);
                }
            }

            return _mapper.Map<IEnumerable<CourseDto>>(completedCourses);
        }

        public async Task<(bool Success, string Message)> RateCourseAsync(int studentId, int courseId, RateCourseRequestDto request)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                return (false, "Course not found.");
            }

            // Check if the user has completed the course
            var totalLectures = (await _courseRepository.GetLecturesByCourseIdAsync(courseId)).Count();
            if (totalLectures > 0)
            {
                var completedLectures = await _courseRepository.GetCompletedLecturesCountAsync(studentId, courseId);
                if (totalLectures != completedLectures)
                {
                    return (false, "You must complete the course before rating it.");
                }
            }
            
            var existingReview = await _courseRepository.GetReviewAsync(studentId, courseId);
            if (existingReview != null)
            {
                // Update existing review
                existingReview.Rating = request.Rating;
                existingReview.Comment = request.ReviewText;
                existingReview.CreatedAt = DateTime.UtcNow;
                await _courseRepository.UpdateReviewAsync(existingReview);
                return (true, "Your review has been updated successfully.");
            }
            else
            {
                // Add new review
                var newReview = new Review
                {
                    CourseId = courseId,
                    StudentId = studentId,
                    Rating = request.Rating,
                    Comment = request.ReviewText,
                    CreatedAt = DateTime.UtcNow
                };
                await _courseRepository.AddReviewAsync(newReview);
                return (true, "Thank you for your review.");
            }
        }
        /////////////////////////////////////////////////////////////

        

        /////////////////////////////////////////////////////////////
        public async Task<CourseDto?> GetCourseAsync(int courseId, int teacherId)
        {
            var course = await _context.Courses
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lectures)
                .FirstOrDefaultAsync(c => c.CourseId == courseId && c.TeacherId == teacherId);

            if (course == null) return null;

            return ToDto(course);
        }

        public async Task<CourseDto> CreateCourseAsync(CreateCourseDto dto, int teacherId)
        {
            var course = new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                Level = dto.Level,
                TeacherId = teacherId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Sections = dto.Sections.Select(s => new Section
                {
                    Title = s.Title,
                    Order = s.Order,
                    Lectures = s.Lectures.Select(l => new Lecture
                    {
                        Title = l.Title,
                        ContentType = l.ContentType,
                        ContentUrl = l.ContentUrl,
                        Duration = l.Duration,
                        Order = l.Order
                    }).ToList()
                }).ToList()
            };
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return ToDto(course);
        }

        public async Task<bool> UpdateCourseAsync(int courseId, UpdateCourseDto dto, int teacherId)
        {
            var course = await _context.Courses
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lectures)
                .FirstOrDefaultAsync(c => c.CourseId == courseId && c.TeacherId == teacherId);

            if (course == null) return false;

            course.Title = dto.Title;
            course.Description = dto.Description;
            course.CategoryId = dto.CategoryId;
            course.Level = dto.Level;
            course.UpdatedAt = DateTime.UtcNow;

            var sectionDict = course.Sections.ToDictionary(s => s.SectionId);

            var dtoSectionIds = dto.Sections.Where(s => s.SectionId.HasValue).Select(s => s.SectionId.Value).ToHashSet();

            var sectionsToRemove = course.Sections.Where(s => !dtoSectionIds.Contains(s.SectionId)).ToList();
            foreach (var section in sectionsToRemove)
            {
                _context.Lectures.RemoveRange(section.Lectures);
                _context.Sections.Remove(section);
            }

            foreach (var sectionDto in dto.Sections)
            {
                Section section;
                if (sectionDto.SectionId.HasValue && sectionDict.TryGetValue(sectionDto.SectionId.Value, out section!))
                {
                    section.Title = sectionDto.Title;
                    section.Order = sectionDto.Order;

                    var lectureDict = section.Lectures.ToDictionary(l => l.LectureId);
                    var dtoLectureIds = sectionDto.Lectures.Where(l => l.LectureId.HasValue).Select(l => l.LectureId.Value).ToHashSet();

                    var lecturesToRemove = section.Lectures.Where(l => !dtoLectureIds.Contains(l.LectureId)).ToList();
                    _context.Lectures.RemoveRange(lecturesToRemove);

                    foreach (var lectureDto in sectionDto.Lectures)
                    {
                        if (lectureDto.LectureId.HasValue && lectureDict.TryGetValue(lectureDto.LectureId.Value, out var lecture))
                        {
                            lecture.Title = lectureDto.Title;
                            lecture.ContentType = lectureDto.ContentType;
                            lecture.ContentUrl = lectureDto.ContentUrl;
                            lecture.Duration = lectureDto.Duration;
                            lecture.Order = lectureDto.Order;
                        }
                        else
                        {
                            section.Lectures.Add(new Lecture
                            {
                                Title = lectureDto.Title,
                                ContentType = lectureDto.ContentType,
                                ContentUrl = lectureDto.ContentUrl,
                                Duration = lectureDto.Duration,
                                Order = lectureDto.Order
                            });
                        }
                    }
                }
                else
                {
                    var newSection = new Section
                    {
                        Title = sectionDto.Title,
                        Order = sectionDto.Order,
                        Lectures = sectionDto.Lectures.Select(l => new Lecture
                        {
                            Title = l.Title,
                            ContentType = l.ContentType,
                            ContentUrl = l.ContentUrl,
                            Duration = l.Duration,
                            Order = l.Order
                        }).ToList()
                    };
                    course.Sections.Add(newSection);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteCourseAsync(int courseId, int teacherId)
        {
            var course = await _context.Courses
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lectures)
                .FirstOrDefaultAsync(c => c.CourseId == courseId && c.TeacherId == teacherId);

            if (course == null) return false;

            // Xóa lectures, sections, rồi course
            foreach (var section in course.Sections)
            {
                _context.Lectures.RemoveRange(section.Lectures);
            }
            _context.Sections.RemoveRange(course.Sections);
            _context.Courses.Remove(course);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CourseAnalysisDto> GetCourseAnalysisAsync(int courseId, int teacherId)
        {
            // Đảm bảo course thuộc về teacher này
            var isOwner = await _context.Courses.AnyAsync(c => c.CourseId == courseId && c.TeacherId == teacherId);
            if (!isOwner)
                throw new UnauthorizedAccessException("Bạn không có quyền xem phân tích khóa học này.");

            var totalEnrollments = await _context.Enrollments.CountAsync(e => e.CourseId == courseId);

            double completionRate = 0.0;
            if (totalEnrollments > 0)
            {
                var completed = await _context.Enrollments.CountAsync(e => e.CourseId == courseId && e.Status == "completed");
                completionRate = (double)completed / totalEnrollments;
            }

            var averageRating = await _context.Reviews
                .Where(r => r.CourseId == courseId)
                .AverageAsync(r => (double?)r.Rating) ?? 0.0;

            var studentIds = await _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Select(e => e.StudentId)
                .ToListAsync();

            double avgCompletedLectures = 0.0;
            if (studentIds.Count > 0)
            {
                var completedLectures = await _context.UserLectureProgresses 
                    .Where(p => studentIds.Contains(p.UserId) && p.IsCompleted == true)
                    .GroupBy(p => p.UserId)
                    .Select(g => g.Count())
                    .ToListAsync();

                if (completedLectures.Count > 0)
                    avgCompletedLectures = completedLectures.Average();
            }

            return new CourseAnalysisDto
            {
                CourseId = courseId,
                TotalEnrollments = totalEnrollments,
                CompletionRate = completionRate,
                AverageRating = averageRating,
                AvgCompletedLectures = avgCompletedLectures
            };
        }


        private static CourseDto ToDto(Course course) => new()
        {
            CourseId = course.CourseId,
            Title = course.Title,
            Description = course.Description,
            CategoryId = course.CategoryId,
            Level = course.Level,
            TeacherId = course.TeacherId,
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt,
            Sections = course.Sections
                .OrderBy(s => s.Order)
                .Select(s => new SectionDto
                {
                    SectionId = s.SectionId,
                    CourseId = s.CourseId,
                    Title = s.Title,
                    Order = s.Order,
                    Lectures = s.Lectures
                        .OrderBy(l => l.Order)
                        .Select(l => new LectureDto
                        {
                            LectureId = l.LectureId,
                            SectionId = l.SectionId,
                            Title = l.Title,
                            ContentType = l.ContentType,
                            ContentUrl = l.ContentUrl,
                            Duration = l.Duration,
                            Order = l.Order
                        }).ToList()
                }).ToList()
        };
        public async Task<HomeworkAnalysisDto> GetHomeworkAnalysisAsync(int assignmentId, int teacherId)
        {
            var assignment = await _context.Assignments
                .Include(a => a.Course)
                    .ThenInclude(c => c.Enrollments)
                        .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

            if (assignment == null) throw new Exception("Assignment not found");
            if (assignment.Course.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bạn không có quyền xem phân tích bài tập này.");

            var totalAssigned = assignment.Course.Enrollments.Count;

            var submissions = await _context.Submissions
                .Where(s => s.AssignmentId == assignmentId)
                .GroupBy(s => s.StudentId)
                .Select(g => g.OrderByDescending(s => s.AttemptNumber).FirstOrDefault())
                .ToListAsync();

            var totalSubmitted = submissions.Count;

            var lateSubmissionCount = submissions.Count(s => s.SubmittedAt != null && assignment.DueDate != null && s.SubmittedAt > assignment.DueDate);

            var gradedCount = submissions.Count(s => s.Grade != null);

            double completionRate = totalAssigned > 0 ? (double)totalSubmitted / totalAssigned : 0;
            double lateSubmissionRate = totalAssigned > 0 ? (double)lateSubmissionCount / totalAssigned : 0;
            double? averageGrade = submissions.Where(s => s.Grade != null).Any()
                ? (double?)submissions.Where(s => s.Grade != null).Average(s => (double)s.Grade!)
                : null;

            var gradeDistribution = submissions
                .Where(s => s.Grade != null)
                .GroupBy(s =>
                {
                    var grade = (double)s.Grade!;
                    if (grade < 6) return "0-5";
                    if (grade < 8) return "6-7";
                    return "8-10";
                })
                .ToDictionary(g => g.Key, g => g.Count());

            var submittedStudentIds = submissions.Select(s => s.StudentId).ToHashSet();
            var notSubmittedStudents = assignment.Course.Enrollments
                .Where(e => !submittedStudentIds.Contains(e.StudentId))
                .Select(e => new StudentInfoDto
                {
                    StudentId = e.StudentId,
                    Name = (e.Student.FirstName ?? "") + " " + (e.Student.LastName ?? ""),
                    Email = e.Student.Email
                }).ToList();

            return new HomeworkAnalysisDto
            {
                AssignmentId = assignmentId,
                TotalAssigned = totalAssigned,
                TotalSubmitted = totalSubmitted,
                CompletionRate = completionRate,
                LateSubmissionCount = lateSubmissionCount,
                LateSubmissionRate = lateSubmissionRate,
                AverageGrade = averageGrade,
                GradeDistribution = gradeDistribution,
                GradedCount = gradedCount,
                NotSubmittedStudents = notSubmittedStudents
            };
        }
    }
}
