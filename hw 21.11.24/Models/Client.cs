namespace hw_21._11._24.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }

        public virtual List<Product> Products { get; set; } = null!;
    }

}
