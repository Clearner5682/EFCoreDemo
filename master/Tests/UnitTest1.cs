using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class UnitTest1
    {
        [Fact]
        public void TestMiddleware()
        {
            var builder = new CustomMiddlewareBuilder();
            builder.Use((str, next) => {

                str += "中间件1Start";
                Debug.WriteLine("中间件1Start");
                next.Invoke();
                str += "中间件1End";
                Debug.WriteLine("中间件1End");

                return Task.CompletedTask;
            });

            builder.Run(str => {
                Debug.WriteLine("短路中间件");

                return Task.CompletedTask;
            });

            builder.Use(next => {
                //return new StringDelegate(str => {
                //    str += "中间件2Start";
                //    next(str);
                //    str += "中间件2End";

                //    return Task.CompletedTask;
                //});


                return str =>
                {
                    str += "中间件2Start";
                    Debug.WriteLine("中间件2Start");
                    next(str);
                    str += "中间件2End";
                    Debug.WriteLine("中间件2End");

                    return Task.CompletedTask;
                };
            });

            builder.Invoke();

            Assert.True(true);
        }
    }
}
