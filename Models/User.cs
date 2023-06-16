namespace IdleGameServer.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = ""; // имя пользователя
        public int Age { get; set; } // возраст пользователя

        public User()
        {
        }

        public User(string name, int age)
        {
            Name = name;
            Age = age;
        }
    }
}
