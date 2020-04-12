using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;
using Autofac;
using Autofac.Core;
using Database;
using Model;

namespace Web.Controllers
{
    public class TestController : Controller
    {
        private MyContext _context;
        IDependencyService _dependencyService1;
        IDependencyService _dependencyService2;
        TextWriter _textWriter;
        IGreetService _greetService;
        IRepository<string> _strRepository;
        IRepository<int> _intRepository;

        public TestController(MyContext context,IDependencyService dependencyService1,IDependencyService dependencyService2,TextWriter textWriter,IRepository<string> strRepository,IRepository<int> intRepository,ILifetimeScope scope)
        {
            _context = context;
            _dependencyService1 = dependencyService1;
            _dependencyService2 = dependencyService2;
            _textWriter = textWriter;
            _greetService = scope.Resolve<IGreetService>(new NamedParameter("name", "coderwhy"));
            _strRepository = strRepository;
            _intRepository = intRepository;

            var dependency = scope.Resolve<DependencyService>();
            dependency.Record("自己既是类型也是实现");

            Test();// 本地函数，从C#8开始支持
            void Test()
            {
                Console.WriteLine(1_000);
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            Console.WriteLine($"dependencyService1==dependencyService1?{_dependencyService1 == _dependencyService2}");
            _textWriter.Write("hello");
            Console.WriteLine(_textWriter.ToString());
            _greetService.Greet("greet");
            _strRepository.Get();
            _intRepository.Get();

            return Ok(Request.Path);
        }

        [HttpGet]
        public IActionResult Search()
        {
            var all = _context.Provinces.ToList();
            string name = "广东省";
            var provinces = _context.Provinces.Where(o=>EF.Functions.Like(o.Name,$"%{name}%")).ToList();
            //var provinces2 = (from a in _context.Provinces
            //                  select a).ToList();
            var model = _context.Provinces.Find(9);
              

            return Ok();
        }

        [HttpGet]
        public IActionResult SearchComplex()
        {
            // 1、预加载，使用Include
            var provinces = _context.Provinces
                .Where(o=>o.Name=="湖北省")
                .Include(o => o.Cities)
                .ThenInclude(o=>o.CityCompanies)
                .ThenInclude(o=>o.City)
                .ToList();

            // 2、映射查询，使用Select
            var selectData = _context.Provinces
                .Include(o => o.Cities)
                //.Select(o => new { ProvinceId = o.Id, ProvinceName = o.Name, CityCount = o.Cities.Count })
                .Select(o=> new { ProvinceId = o.Id, ProvinceName = o.Name, Cities=o.Cities.Where(x=>x.Name=="武汉") })
                .ToList<dynamic>();




            return Ok();
        }

        [HttpGet]
        public IActionResult SearchLazyload()
        {
            var provinces = _context.Provinces.ToList();
            var selectData = (from a in provinces
                              select a.Cities into b
                              from c in b
                              select new { CityId = c.Id, CityName = c.Name }).ToList();
            List<City> cities = new List<City>();
            foreach(var province in provinces)
            {
                foreach(var city in province.Cities)
                {
                    cities.Add(city);
                }
            }


            return Ok(selectData);
        }

        [HttpGet]
        public IActionResult Add()
        {
            Province guangdong = new Province
            {
                Name = "广东省",
                Population = 8000_0000,
                Cities=new List<City>
                {
                    new City
                    {
                        Name="广州市"
                    },
                    new City
                    {
                        Name="深圳市"
                    },
                    new City
                    {
                        Name="佛山市"
                    }
                }
            };
            Province hubei = new Province
            {
                Name = "湖北省",
                Population = 5000_0000,
                Cities = new List<City>
                {
                    new City
                    {
                        Name="武汉"
                    }
                }
            };

            _context.Provinces.Add(guangdong);
            //_context.Add<Province>(province);
            //_context.Provinces.AddRange();
            //_context.AddRange(guangdong,hubei);
            //_context.AddRange(new List<Province> { guangdong, hubei });
            _context.SaveChanges();

            //using (var trans = _context.Database.BeginTransaction())
            //{
            //    trans.Commit();
            //    trans.Rollback();
            //}

            return Ok();
        }

        [HttpPost]
        public IActionResult Add([FromBody]Province model)
        {
            var test = HttpContext.Request;
            _context.Provinces.Add(model);
            _context.SaveChanges();

            return Ok();
        }

        [HttpGet]
        public IActionResult Update()
        {
            var model = _context.Provinces.Find(9);
            model.Name += "1";
            //_context2.Provinces.Update(model);
            //_context2.Entry(model).State = EntityState.Modified;
            //_context2.SaveChanges();

            return Ok();
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var model = _context.Provinces.Find(id);
            _context.Provinces.Remove(model);
            //_context.RemoveRange();
            _context.SaveChanges();

            return Ok();
        }
    }
}