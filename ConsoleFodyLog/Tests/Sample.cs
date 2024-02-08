using Microsoft.Extensions.Logging;
using static System.Console;

namespace ConsoleFodyLog
{

  //  [Log]
    public class Sample
    {
        private readonly ILogger _logger;

        //  [Log]
        public Sample(ILogger logger)
        {
            _logger = logger;
        }

        //  [Log]
        public async Task<Task> ManagePerson()
        {
            Person p = new Person(1, "Toto", "student", "The king");
            var o = new OtherClass();
            var perso = await o.DoSomeOtherWork(p);
        //    WriteLine("Nom mis en majuscule " + perso.Name);

         //   _logger.LogInformation("Nom mis en majuscule {nom}", perso.Name);

          //  var task = o.DoSomeOtherWork1(perso);

          //  WriteLine("Personne info " + perso);

            //DataStore<string> cities = new DataStore<string>();
            //cities.AddOrUpdate(0, "Paris");

            //cities.AddOrUpdateWithMsg(1, "Tokyo");

            //cities.GetData(0);
            //cities.GetData(1);

            return Task.CompletedTask;

        }
    }

    public class OtherClass
    {
 //       [HandleException]
        [Log]
        public async Task<Person> DoSomeOtherWork(Person person)
        {
           // throw new Exception("Une exception levée dans OtherClass.DoSomeOtherWork");
            var upperName = person.UpperName(person.Name);

            return await person.PersonWithUpperName(person);
        }

       // [Log]
        public Task DoSomeOtherWork1(Person person)
        {
            return person.PersonWithUpperDescription(person);
        }
    }

}
