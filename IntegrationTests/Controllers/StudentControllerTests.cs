using Application.Dtos.Common;
using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;
using Application.Services.Interfaces;
using Domain.Entities;
using IntegrationTests.Shared;
using Moq;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net;
using System.Text;
using System.Text.Json;

namespace IntegrationTests.Controllers
{
    public class StudentsControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly Mock<IStudentService> _mock;

        public StudentsControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _mock = factory.StudentServiceMock;
        }

        [Fact]
        public async Task GetAll_Should_Return_Ok()
        {
            // Arrange

            var studentDto = new StudentDto { Email = "ade@sample.com", FirstName = "Ade", MatricNumber = "STU-01-ABC" };
            _mock.Setup(s => s.SearchAsync(null, null, 1, 10, null))
                .ReturnsAsync(new PagedResult<StudentDto>(
                    new List<StudentDto>() { studentDto }, 1, 10, 1));

            // Act
            var response = await _client.GetAsync("/api/v1/Students");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PagedResult<StudentDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(studentDto.Email, result.Items[0].Email);
        }

        [Fact]
        public async Task GetById_Should_Return_404_When_NotFound()
        {
            var id = Guid.NewGuid();

            _mock.Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync((StudentDto?)null);

            var response = await _client.GetAsync($"/api/v1/Students/{id}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Create_Should_Return_201()
        {
            var dto = new StudentDto
            {
                Id = Guid.NewGuid(),
                Email = "test@test.com"
            };

            _mock.Setup(s => s.CreateAsync(It.IsAny<StudentRequestDto>()))
                .ReturnsAsync(dto);

            var content = new StringContent(
                JsonSerializer.Serialize(new StudentRequestDto
                {
                    FirstName = "John",
                    LastName = "Doe",
                    PhoneNo = "1234567890",
                    DateOfBirth = new DateOnly(2000, 1, 1),
                    Gender = Domain.Enums.Gender.Male,
                    Address = "123 Main",
                    Email = "test@test.com",
                    Password = "Password1234",
                    ConfirmPassword = "Password1234"
                }),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync("/api/v1/Students", content);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Create_Should_Return_BadRequest_When_Inpute_Not_Valid()
        {
            var dto = new StudentDto
            {
                Id = Guid.NewGuid(),
                Email = "test@test.com"
            };

            _mock.Setup(s => s.CreateAsync(It.IsAny<StudentRequestDto>()))
                .ReturnsAsync(dto);

            var content = new StringContent(
                JsonSerializer.Serialize(new StudentRequestDto
                {
                    FirstName = "John",
                    LastName = "Doe",
                    PhoneNo = "1234567890",
                    DateOfBirth = new DateOnly(2000, 1, 1),
                    Gender = Domain.Enums.Gender.Male,
                    Address = "123 Main",
                    Email = "testtestcom",
                    Password = "Password",
                    ConfirmPassword = "Password123"
                }),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync("/api/v1/Students", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Delete_Should_Return_404_When_NotFound()
        {
            var id = Guid.NewGuid();

            _mock.Setup(s => s.DeleteAsync(id)).ReturnsAsync(false);

            var response = await _client.DeleteAsync($"/api/v1/Students/{id}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task AcceptOffer_Should_Return_Forbidden_When_User_Not_Owner()
        {
            var id = Guid.NewGuid(); // different from auth user

            var response = await _client.PostAsync($"/api/v1/Students/{id}/accept-offer", null);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AcceptOffer_Should_Return_Ok_When_User_Is_Owner()
        {
            var id = Guid.Parse("11111111-1111-1111-1111-111111111111");

            _mock.Setup(s => s.AcceptAdmissionAsync(id))
                .ReturnsAsync(new StudentDto { Id = id });

            var response = await _client.PostAsync($"/api/v1/Students/{id}/accept-offer", null);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
