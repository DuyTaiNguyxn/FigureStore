namespace FigureStore.Dtos
{
    public class UpdateUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public IFormFile? Avatar { get; set; }
    }

}
