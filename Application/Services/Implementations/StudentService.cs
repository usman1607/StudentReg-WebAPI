using Application.Dtos.Common;
using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;
using Application.Exceptions;
using Application.Helpers;
using Application.Repositories;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace Application.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<StudentService> _logger;

        public StudentService(
            IStudentRepository studentRepository,
            IMapper mapper,
            ILogger<StudentService> logger)
        {
            _studentRepository = studentRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<StudentDto> CreateAsync(StudentRequestDto request)
        {
            _logger.LogInformation("Creating new student with email: {Email}", request.Email);

            // Generate matriculation number
            var matricNumber = UserHelper.GenerateMatricNumber();

            // Create password hash (in production, use proper hashing)
            var (hash, salt) = UserHelper.GeneratePasswordHash(request.Password);

            var student = new Student(
                matricNumber: matricNumber,
                firstName: request.FirstName,
                lastName: request.LastName,
                email: request.Email,
                passwordHash: hash,
                hashSalt: salt,
                gender: request.Gender,
                phoneNumber: request.PhoneNo,
                address: request.Address,
                createdBy: request.Email
            );

            var createdStudent = await _studentRepository.CreateAsync(student);
            _logger.LogInformation("Student created successfully with ID: {StudentId}, MatricNumber: {MatricNumber}",
                createdStudent.Id, createdStudent.MatricNumber);

            return _mapper.Map<StudentDto>(createdStudent);
        }

        public async Task<StudentDto?> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Fetching student by ID: {StudentId}", id);
            var student = await _studentRepository.GetByIdAsync(id);

            if (student == null)
            {
                _logger.LogWarning("Student with ID: {StudentId} not found", id);
                return null;
            }

            return _mapper.Map<StudentDto>(student);
        }

        public async Task<StudentDto?> GetByMatricAsync(string matricNo)
        {
            _logger.LogInformation("Fetching student by MatricNumber: {MatricNumber}", matricNo);
            var student = await _studentRepository.GetAsync(matricNo);

            if (student == null)
            {
                _logger.LogWarning("Student with MatricNumber: {MatricNumber} not found", matricNo);
                return null;
            }

            return _mapper.Map<StudentDto>(student);
        }

        public async Task<List<StudentDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all students");
            var students = await _studentRepository.GetAllAsync();
            _logger.LogInformation("Found {Count} students", students.Count);
            return _mapper.Map<List<StudentDto>>(students);
        }

        public async Task<PagedResult<StudentDto>> SearchAsync(string? searchTerm, StudentStatus? status, int page, int pageSize, string? sortBy)
        {
            _logger.LogInformation(
                "Searching students - SearchTerm: {SearchTerm}, Page: {Page}, PageSize: {PageSize}, SortBy: {SortBy}",
                searchTerm, page, pageSize, sortBy);

            // Ensure valid pagination parameters
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var pagedStudents = await _studentRepository.SearchAsync(searchTerm, status, page, pageSize, sortBy);

            _logger.LogInformation("Search returned {Count} students out of {Total} total",
                pagedStudents.Items.Count, pagedStudents.TotalCount);

            var studentDtos = _mapper.Map<List<StudentDto>>(pagedStudents.Items);

            return new PagedResult<StudentDto>(studentDtos, page, pageSize, pagedStudents.TotalCount);
        }

        public async Task<StudentDto?> UpdateAsync(Guid id, StudentUpdateRequest updateRequest)
        {
            _logger.LogInformation("Updating student with ID: {StudentId}", id);

            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null)
            {
                _logger.LogWarning("Cannot update - Student with ID: {StudentId} not found", id);
                return null;
            }

            // Apply updates only for non-null values
            if (!string.IsNullOrEmpty(updateRequest.FirstName))
                student.FirstName = updateRequest.FirstName;

            if (!string.IsNullOrEmpty(updateRequest.LastName))
                student.LastName = updateRequest.LastName;

            if (!string.IsNullOrEmpty(updateRequest.Email))
                student.Email = updateRequest.Email;

            if (!string.IsNullOrEmpty(updateRequest.PhoneNumber))
                student.PhoneNumber = updateRequest.PhoneNumber;

            if (!string.IsNullOrEmpty(updateRequest.Address))
                student.Address = updateRequest.Address;

            if (updateRequest.Gender.HasValue)
                student.Gender = updateRequest.Gender.Value;

            student.UpdatedBy = "System";
            student.UpdatedDate = DateTime.UtcNow;

            var updatedStudent = await _studentRepository.UpdateAsync(student);
            _logger.LogInformation("Student with ID: {StudentId} updated successfully", id);

            return _mapper.Map<StudentDto>(updatedStudent);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            _logger.LogInformation("Deleting student with ID: {StudentId}", id);

            var result = await _studentRepository.DeleteAsync(id);

            if (result)
                _logger.LogInformation("Student with ID: {StudentId} deleted successfully", id);
            else
                _logger.LogWarning("Cannot delete - Student with ID: {StudentId} not found", id);

            return result;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _studentRepository.ExistsAsync(id);
        }

        public async Task<StudentDto> AcceptAdmissionAsync(Guid studentId)
        {
            _logger.LogInformation("Student {StudentId} accepting admission offer", studentId);

            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null)
            {
                throw new EntityNotFoundException($"Student with ID '{studentId}' not found.");
            }

            if (student.Status != Domain.Enums.StudentStatus.OfferedAdmission)
            {
                throw new ValidationException($"Student must have OfferedAdmission status to accept. Current status: {student.Status}");
            }

            student.Status = Domain.Enums.StudentStatus.Accepted;
            student.UpdatedBy = student.Email;
            student.UpdatedDate = DateTime.UtcNow;

            await _studentRepository.UpdateAsync(student);

            _logger.LogInformation("Student {StudentId} accepted admission. Status changed to Accepted", studentId);

            return _mapper.Map<StudentDto>(student);
        }

    }
}
