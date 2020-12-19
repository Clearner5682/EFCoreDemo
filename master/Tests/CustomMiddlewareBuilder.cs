using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class CustomMiddlewareBuilder
    {
        public IList<Func<StringDelegate, StringDelegate>> _components = new List<Func<StringDelegate, StringDelegate>>();

        public void Use(Func<StringDelegate, StringDelegate> middleware)
        {
            _components.Add(middleware);
        }

        public void Use(Func<string, Func<Task>, Task> middleware)
        {
            this.Use(next =>
            {
                return str =>
                {
                    Func<Task> simpleNext = () => next(str);

                    return middleware(str, simpleNext);
                };
            });
        }

        public void Run(StringDelegate middleware)
        {
            _components.Add(str=>middleware);
        }

        public StringDelegate Build()
        {
            StringDelegate app = str =>
            {
                Debug.WriteLine("EndPoint中间件");

                return Task.CompletedTask;
            };

            foreach(var component in _components.Reverse())
            {
                app = component(app);
            }

            return app;
        }

        public void Invoke()
        {
            var stringDelegate = this.Build();

            stringDelegate("");
        }
    }
}
