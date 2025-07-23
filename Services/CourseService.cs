using AutoMapper;
using Sep490_Eduseen_BE.Dtos.Course;
using Sep490_Eduseen_BE.Dtos.Review;
using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Repositories;
using Sep490_Eduseen_BE.Exceptions;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;


namespace Sep490_Eduseen_BE.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;

        public CourseService(ICourseRepository courseRepository, IMapper mapper)
        {
            _courseRepository = courseRepository;
            _mapper = mapper;
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

        public async Task<CourseDetailDto> GetCourseByIdAsync(int courseId, int? studentId)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                return null;
            }

            var courseDetailDto = _mapper.Map<CourseDetailDto>(course);

            // Nếu có studentId, lấy thông tin completion status của lectures và kiểm tra enrollment
            if (studentId.HasValue)
            {
                var completionStatus = await _courseRepository.GetLectureCompletionStatusAsync(studentId.Value, courseId);
                
                // Cập nhật IsCompleted cho từng lecture
                foreach (var section in courseDetailDto.Sections)
                {
                    foreach (var lecture in section.Lectures)
                    {
                        lecture.IsCompleted = completionStatus.ContainsKey(lecture.LectureId) ? completionStatus[lecture.LectureId] : false;
                    }
                }

                // Kiểm tra enrollment status với status = "Enrolled"
                courseDetailDto.IsEnrolled = await _courseRepository.IsUserEnrolledWithStatusAsync(studentId.Value, courseId, "Enrolled");
            }
            else
            {
                courseDetailDto.IsEnrolled = false;
            }

            return courseDetailDto;
        }

        public async Task<IEnumerable<CourseDto>> GetMyCoursesAsync(int studentId)
        {
            var courses = await _courseRepository.GetEnrolledCoursesByStudentIdAsync(studentId);
            var courseDtos = _mapper.Map<List<CourseDto>>(courses);

            // Lấy danh sách courseId mà user đã favorite
            var favoriteCourseIds = courses
                .Select(c => c.CourseId)
                .ToList();
            var favoriteIds = await _courseRepository.GetFavoriteCourseIdsAsync(studentId, favoriteCourseIds);

            foreach (var dto in courseDtos)
            {
                // Lấy tổng số lecture
                var lectures = await _courseRepository.GetLecturesByCourseIdAsync(dto.CourseId);
                dto.TotalLectures = lectures.Count();
                // Lấy số lecture đã hoàn thành
                dto.CompletedLectures = await _courseRepository.GetCompletedLecturesCountAsync(studentId, dto.CourseId);
                // Set isFavorite
                dto.IsFavorite = favoriteIds.Contains(dto.CourseId);
            }

            return courseDtos;
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

        public async Task<IEnumerable<ReviewDto>> GetTopReviewsAsync()
        {
            var reviews = await _courseRepository.GetTopReviewsAsync(3);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesByCategoryAsync(int categoryId, int? studentId = null)
        {
            var courses = await _courseRepository.GetCoursesByCategoryAsync(categoryId);
            var courseDtos = _mapper.Map<List<CourseDto>>(courses);

            if (studentId.HasValue)
            {
                foreach (var dto in courseDtos)
                {
                    var courseEntity = courses.FirstOrDefault(c => c.CourseId == dto.CourseId);
                    if (courseEntity != null)
                    {
                        dto.IsFavorite = courseEntity.Favorites.Any(f => f.StudentId == studentId.Value);
                    }
                }
            }
            else
            {
                foreach (var dto in courseDtos)
                {
                    dto.IsFavorite = false;
                }
            }

            return courseDtos;
        }

        public async Task<IEnumerable<CourseDto>> GetTopCoursesAsync(int count, int? studentId = null)
        {
            var courses = await _courseRepository.GetTopCoursesAsync(count);
            var courseDtos = _mapper.Map<List<CourseDto>>(courses);

            if (studentId.HasValue)
            {
                foreach (var dto in courseDtos)
                {
                    var entity = courses.FirstOrDefault(c => c.CourseId == dto.CourseId);
                    dto.IsFavorite = entity.Favorites.Any(f => f.StudentId == studentId.Value);
                }
            }
            return courseDtos;
        }

        // Admin methods implementation
        public async Task<IEnumerable<AdminCourseDto>> GetAllCoursesForAdminAsync()
        {
            var courses = await _courseRepository.GetAllCoursesForAdminAsync();
            return _mapper.Map<IEnumerable<AdminCourseDto>>(courses);
        }

        public async Task<AdminCourseDetailDto> GetCourseByIdForAdminAsync(int courseId)
        {
            var course = await _courseRepository.GetCourseByIdForAdminAsync(courseId);
            if (course == null)
            {
                return null;
            }
            return _mapper.Map<AdminCourseDetailDto>(course);
        }

        public async Task<CourseStatisticsDto> GetCourseStatisticsAsync()
        {
            var statistics = await _courseRepository.GetCourseStatisticsAsync();
            return statistics;
        }

        public async Task<(bool Success, string Message)> UpdateCourseStatusAsync(int courseId, bool isActive)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                return (false, "Course not found.");
            }

            // Sử dụng UpdatedAt để đánh dấu trạng thái thay vì IsActive
            course.UpdatedAt = DateTime.UtcNow;
            await _courseRepository.UpdateCourseAsync(course);

            return (true, $"Course {(isActive ? "activated" : "deactivated")} successfully.");
        }

        public async Task<(bool Success, string Message)> DeleteCourseByAdminAsync(int courseId)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                return (false, "Course not found.");
            }

            // Check if course has enrollments
            var enrollments = await _courseRepository.GetEnrollmentsByCourseIdAsync(courseId);
            if (enrollments.Any())
            {
                return (false, "Cannot delete course with existing enrollments.");
            }

            await _courseRepository.DeleteCourseAsync(courseId);
            return (true, "Course deleted successfully.");
        }

        public async Task<IEnumerable<AdminCourseDto>> GetPendingCoursesAsync()
        {
            var courses = await _courseRepository.GetPendingCoursesAsync();
            return _mapper.Map<IEnumerable<AdminCourseDto>>(courses);
        }

        public async Task<IEnumerable<AdminCourseDto>> GetCoursesByTeacherAsync(int teacherId)
        {
            var courses = await _courseRepository.GetCoursesByTeacherAsync(teacherId);
            return _mapper.Map<IEnumerable<AdminCourseDto>>(courses);
        }
    }
}
