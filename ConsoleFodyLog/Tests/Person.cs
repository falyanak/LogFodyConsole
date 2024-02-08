using Microsoft.Diagnostics.Runtime.Utilities;
using System.Text.Json;

namespace ConsoleFodyLog
{

    //[Log(MethodNameFilter = "PersonWithUpperName")]
    public class Person
    {
        public Person(int id, string name, string description, string title)
        {
            Id = id;
            Name = name;
            Description = description;
            Title = title;
        }


        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }


        [Log]
        public async Task<string> UpperName(string name)
        {
            await Task.Delay(500);

            //     throw new Exception("Exception levée dans la méthode UpperName !");
            return name.ToUpper();
        }

        [Log]
        public async Task<Person> PersonWithUpperName(Person person)
        {
            string uName = await UpperName(person.Name);

            return new Person(person.Id, uName, person.Description, person.Title);
        }

        [Log]
        public Task PersonWithUpperDescription(Person person)
        {
           person.Description=person.Description.ToUpper();

           return Task.CompletedTask;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }

}
