using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web
{
    // 用于测试Autofac的注入和析出
    public interface IDependencyService
    {
        void Record(string message);
    }

    public class DependencyService : IDependencyService
    {
        private List<string> listMessage = new List<string>();
        public void Record(string message)
        {
            listMessage.Add(message);
            Console.WriteLine(string.Join(",",listMessage));
        }
    }

    public interface IGreetService
    {
        void Greet(string message);
    }

    public class GreetService:IGreetService
    {
        private string _name { get; set; }
        public GreetService(string name)
        {
            _name = name;
        }

        public void Greet(string message)
        {
            Console.WriteLine($"{_name}:{message}");
        }
    }

    public interface IRepository<T>
    {
        void Get();
    }

    public class Repository<T> : IRepository<T>
    {
        public void Get()
        {
            Console.WriteLine(typeof(T).Name);
        }
    }
}
