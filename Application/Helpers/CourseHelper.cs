namespace Application.Helpers
{
    public static class CourseHelper
    {
        public static string GenerateCourseCode(string name)
        {
            var random = new Random();
            var num = random.Next(100, 999);
            return $"{name.Substring(0, 3).ToUpper()}-{num}";
        }
    }
}
