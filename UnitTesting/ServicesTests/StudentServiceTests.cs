using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;
using Application.Exceptions;
using Application.Repositories;
using Application.Services.Implementations;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTesting.ServicesTests
{
    public class StudentServiceTests
    {
        private readonly StudentService _service;
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IAuthService> _authServiceMock = new();
        private readonly Mock<IRoleRepository> _roleRepoMock = new();
        private readonly Mock<IStudentRepository> _studentRepoMock = new();
        private readonly Mock<ILogger<StudentService>> _loggerMock = new();


        public StudentServiceTests()
        {
            _service = new StudentService(_authServiceMock.Object, _roleRepoMock.Object, _studentRepoMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateAsync_Should_Create_Student_And_Return_Dto()
        {
            // Arrange
            var request = new StudentRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                Password = "Password123",
                Gender = Gender.Male,
                PhoneNo = "1234567890",
                Address = "Test Address"
            };

            var role = new Role { Id = Guid.NewGuid(), Name = "Student" };

            _roleRepoMock
                .Setup(r => r.GetByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(role);

            var student = new Student(
                "MAT123", request.FirstName, request.LastName, request.Email,
                request.Password, "56789ghjkl", Gender.Male, "0989988", "address", "Admin");

            _studentRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<Student>()))
                .ReturnsAsync(student);

            var expectedDto = new StudentDto { Email = request.Email, FirstName = request.FirstName, MatricNumber = student.MatricNumber };

            _mapperMock
                .Setup(m => m.Map<StudentDto>(It.IsAny<Student>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.Email, result.Email);
            Assert.Equal(expectedDto.FirstName, result.FirstName);
            Assert.Equal(expectedDto.MatricNumber, result.MatricNumber);
            _studentRepoMock.Verify(r => r.CreateAsync(It.IsAny<Student>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_When_NotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            _studentRepoMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Student?)null);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Create_Student_Should_ThrowValidationException_When_Email_Already_Exists()
        {
            // Arrange
            var request = new StudentRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                Password = "Password123",
                Gender = Gender.Male,
                PhoneNo = "1234567890",
                Address = "Test Address"
            };

            var role = new Role { Id = Guid.NewGuid(), Name = "Student" };

            _roleRepoMock
                .Setup(r => r.GetByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(role);

            var student = new Student(
                "MAT123", request.FirstName, request.LastName, request.Email,
                request.Password, "56789ghjkl", Gender.Male, "0989988", "address", "Admin");

            _studentRepoMock.Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync(student);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                _service.CreateAsync(request));
        }

        [Fact]
        public async Task GetByMatricAsync_Should_Throw_Unauthorized_When_Student_Accessing_Other_Record()
        {
            // Arrange
            var matric = "MAT123";

            var student = new Student(
                matric, "John", "Doe", "owner@test.com",
                "Password123", "56789ghjkl", Gender.Male, "123", "addr", "system"
            );

            _studentRepoMock
                .Setup(r => r.GetAsync(matric))
                .ReturnsAsync(student);

            _authServiceMock.Setup(a => a.IsStudent()).Returns(true);
            _authServiceMock.Setup(a => a.GetSignedInEmail()).Returns("hacker@test.com");

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.GetByMatricAsync(matric));
        }

        [Fact]
        public async Task GetByMatricAsync_Should_Return_Student_When_Not_Student()
        {
            // Arrange
            var matric = "MAT123";

            var student = new Student(
                matric, "John", "Doe", "owner@test.com",
                "Password123", "56789ghjkl", Gender.Male, "123", "addr", "system"
            );

            _studentRepoMock
                .Setup(r => r.GetAsync(matric))
                .ReturnsAsync(student);

            _authServiceMock.Setup(a => a.IsStudent()).Returns(false);
            _authServiceMock.Setup(a => a.GetSignedInEmail()).Returns("admin@test.com");

            var expectedDto = new StudentDto { Email = student.Email, FirstName = student.FirstName, MatricNumber = student.MatricNumber };

            _mapperMock
                .Setup(m => m.Map<StudentDto>(It.IsAny<Student>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.GetByMatricAsync(matric);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.Email, result.Email);
            Assert.Equal(expectedDto.FirstName, result.FirstName);
            Assert.Equal(expectedDto.MatricNumber, result.MatricNumber);
        }

        [Fact]
        public async Task GetByMatricAsync_Should_Throw_When_NotFound()
        {
            // Arrange
            var matric = "MAT123";

            _studentRepoMock
                .Setup(r => r.GetAsync(matric))
                .ReturnsAsync((Student?)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _service.GetByMatricAsync(matric));
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Only_Provided_Fields()
        {
            // Arrange
            var id = Guid.NewGuid();

            var student = new Student(
                "MAT123", "Old", "Name", "old@test.com",
                "Password123", "567890dfghjk", Gender.Male, "123", "addr", "system"
            );

            _studentRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(student);
            _studentRepoMock.Setup(r => r.UpdateAsync(student)).ReturnsAsync(student);

            var update = new StudentUpdateRequest
            {
                FirstName = "NewName",
                LastName = "NewLastName"
            };

            var expectedDto = new StudentDto { Email = student.Email, FirstName = update.FirstName, LastName = update.LastName, MatricNumber = student.MatricNumber };

            _mapperMock
                .Setup(m => m.Map<StudentDto>(It.IsAny<Student>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.UpdateAsync(id, update);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.FirstName, result.FirstName);
            Assert.Equal(expectedDto.LastName, result.LastName);
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_True_When_Deleted()
        {
            var id = Guid.NewGuid();

            _studentRepoMock
                .Setup(r => r.DeleteAsync(id))
                .ReturnsAsync(true);

            var result = await _service.DeleteAsync(id);

            Assert.True(result);
        }

        [Fact]
        public async Task AcceptAdmissionAsync_Should_Update_Status_To_Accepted()
        {
            // Arrange
            var id = Guid.NewGuid();

            var student = new Student(
                "MAT123", "John", "Doe", "test@test.com",
                "Password123", "456789dfghjk", Gender.Male, "123", "addr", "system"
            )
            {
                Status = StudentStatus.OfferedAdmission
            };

            _studentRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(student);
            _studentRepoMock.Setup(r => r.UpdateAsync(student)).ReturnsAsync(student);

            _mapperMock
                .Setup(m => m.Map<StudentDto>(It.IsAny<Student>()))
                .Returns(new StudentDto());

            // Act
            var result = await _service.AcceptAdmissionAsync(id);

            // Assert
            Assert.Equal(StudentStatus.Accepted, student.Status);
        }

        [Fact]
        public async Task AcceptAdmissionAsync_Should_Throw_When_Status_Invalid()
        {
            var id = Guid.NewGuid();

            var student = new Student(
                "MAT123", "John", "Doe", "test@test.com",
                "Password123", "456789dfghjk", Gender.Male, "123", "addr", "system"
            )
            {
                Status = StudentStatus.Pending
            };

            _studentRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(student);

            await Assert.ThrowsAsync<ValidationException>(() =>
                _service.AcceptAdmissionAsync(id));
        }
    }
}