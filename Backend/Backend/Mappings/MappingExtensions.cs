using Backend.DTOs;
using Backend.Models;

namespace Backend.Mappings
{
    public static class MappingExtensions
    {
        // User Mappings

        public static UserDto ToDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                CreatedAt = user.CreatedAt,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Semester = user.Semester,
                Phone = user.Phone
            };
        }

        public static User ToModel(this CreateUserDto dto)
        {
            return new User
            {
                Username = dto.Username,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Semester = dto.Semester,
                Phone = dto.Phone,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static void UpdateFromDto(this User user, UpdateUserDto dto)
        {
            if (dto.Username != null) user.Username = dto.Username;
            if (dto.FirstName != null) user.FirstName = dto.FirstName;
            if (dto.LastName != null) user.LastName = dto.LastName;
            if (dto.Semester.HasValue) user.Semester = dto.Semester.Value;
            if (dto.Phone != null) user.Phone = dto.Phone;
        }

        // Professor Mappings
        public static ProfessorDto ToDto(this Professor professor)
        {
            return new ProfessorDto
            {
                Id = professor.Id,
                CreatedAt = professor.CreatedAt,
                FirstName = professor.FirstName,
                LastName = professor.LastName,
                Mail = professor.Mail,
                Phone = professor.Phone,
                Office = professor.Office
            };
        }

        public static Professor ToModel(this CreateProfessorDto dto)
        {
            return new Professor
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Mail = dto.Mail,
                Phone = dto.Phone,
                Office = dto.Office,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static void UpdateFromDto(this Professor professor, UpdateProfessorDto dto)
        {
            if (dto.FirstName != null) professor.FirstName = dto.FirstName;
            if (dto.LastName != null) professor.LastName = dto.LastName;
            if (dto.Mail != null) professor.Mail = dto.Mail;
            if (dto.Phone != null) professor.Phone = dto.Phone;
            if (dto.Office != null) professor.Office = dto.Office;
        }

        // Course Mappings
        public static CourseDto ToDto(this Course course)
        {
            return new CourseDto
            {
                Id = course.Id,
                CreatedAt = course.CreatedAt,
                Title = course.Title,
                Semester = course.Semester,
                Description = course.Description,
                Grade = course.Grade,
                UserId = course.UserId,
                ProfessorId = course.ProfessorId
            };
        }

        public static Course ToModel(this CreateCourseDto dto)
        {
            return new Course
            {
                Title = dto.Title,
                Semester = dto.Semester,
                Description = dto.Description,
                Grade = dto.Grade,
                UserId = dto.UserId,
                ProfessorId = dto.ProfessorId,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static void UpdateFromDto(this Course course, UpdateCourseDto dto)
        {
            if (dto.Title != null) course.Title = dto.Title;
            if (dto.Semester.HasValue) course.Semester = dto.Semester.Value;
            if (dto.Description != null) course.Description = dto.Description;
            if (dto.Grade.HasValue) course.Grade = dto.Grade.Value;
            if (dto.UserId.HasValue) course.UserId = dto.UserId.Value;
            if (dto.ProfessorId.HasValue) course.ProfessorId = dto.ProfessorId.Value;
        }

        // Note Mappings
        public static NoteDto ToDto(this Note note)
        {
            return new NoteDto
            {
                Id = note.Id,
                CreatedAt = note.CreatedAt,
                Title = note.Title,
                Description = note.Description,
                Content = note.Content,
                LastUpdated = note.LastUpdated,
                Type = note.Type,
                CourseId = note.CourseId,
                ParentId = note.ParentId
            };
        }

        public static Note ToModel(this CreateNoteDto dto)
        {
            return new Note
            {
                Title = dto.Title,
                Description = dto.Description,
                Content = dto.Content,
                Type = dto.Type,
                CourseId = dto.CourseId,
                ParentId = dto.ParentId,
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };
        }

        public static void UpdateFromDto(this Note note, UpdateNoteDto dto)
        {
            if (dto.Title != null) note.Title = dto.Title;
            if (dto.Description != null) note.Description = dto.Description;
            if (dto.Content != null) note.Content = dto.Content;
            if (dto.Type != null) note.Type = dto.Type;
            if (dto.CourseId.HasValue) note.CourseId = dto.CourseId.Value;
            if (dto.ParentId.HasValue) note.ParentId = dto.ParentId.Value;
            note.LastUpdated = DateTime.UtcNow;
        }

        // Task Mappings
        public static TaskDto ToDto(this TaskItem task)
        {
            return new TaskDto
            {
                Id = task.Id,
                CreatedAt = task.CreatedAt,
                Title = task.Title,
                Type = task.Type,
                Description = task.Description,
                Date = task.Date,
                Completed = task.Completed,
                GradeMax = task.GradeMax,
                GradeEarned = task.GradeEarned,
                CourseId = task.CourseId
            };
        }

        public static TaskItem ToModel(this CreateTaskDto dto)
        {
            return new TaskItem
            {
                Title = dto.Title,
                Type = dto.Type,
                Description = dto.Description,
                Date = dto.Date,
                Completed = dto.Completed,
                GradeMax = dto.GradeMax,
                GradeEarned = dto.GradeEarned,
                CourseId = dto.CourseId,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static void UpdateFromDto(this TaskItem task, UpdateTaskDto dto)
        {
            if (dto.Title != null) task.Title = dto.Title;
            if (dto.Type != null) task.Type = dto.Type;
            if (dto.Description != null) task.Description = dto.Description;
            if (dto.Date.HasValue) task.Date = dto.Date.Value;
            if (dto.Completed.HasValue) task.Completed = dto.Completed.Value;
            if (dto.GradeMax.HasValue) task.GradeMax = dto.GradeMax.Value;
            if (dto.GradeEarned.HasValue) task.GradeEarned = dto.GradeEarned.Value;
            if (dto.CourseId.HasValue) task.CourseId = dto.CourseId.Value;
        }
    }
}