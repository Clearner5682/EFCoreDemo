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

                str += "�м��1Start";
                Debug.WriteLine("�м��1Start");
                next.Invoke();
                str += "�м��1End";
                Debug.WriteLine("�м��1End");

                return Task.CompletedTask;
            });

            builder.Run(str => {
                Debug.WriteLine("��·�м��");

                return Task.CompletedTask;
            });

            builder.Use(next => {
                //return new StringDelegate(str => {
                //    str += "�м��2Start";
                //    next(str);
                //    str += "�м��2End";

                //    return Task.CompletedTask;
                //});


                return str =>
                {
                    str += "�м��2Start";
                    Debug.WriteLine("�м��2Start");
                    next(str);
                    str += "�м��2End";
                    Debug.WriteLine("�м��2End");

                    return Task.CompletedTask;
                };
            });

            builder.Invoke();

            Assert.True(true);
        }
    }
}
