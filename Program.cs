using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Blog
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

            var context = new MyDbContext(loggerFactory);
            context.Database.EnsureCreated();
            InitializeData(context);

            var posts = await context.BlogPosts.AsNoTracking()
                .SelectMany(x => x.Comments.OrderByDescending(x => x.CreatedDate).Take(1),
                (q, w) => new { q.Title, w.Text, w.CreatedDate })
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();

            foreach (var item in posts)
            {
                Console.WriteLine(item.Title + " - " + item.Text + " (" + item.CreatedDate.Day + "." + item.CreatedDate.Month + ")");
            }
        }

        private static void InitializeData(MyDbContext context)
        {
            context.BlogPosts.Add(new BlogPost("Post1")
            {
                Comments = new List<BlogComment>()
                {
                    new BlogComment("1", new DateTime(2020, 3, 4)),
                    new BlogComment("2", new DateTime(2020, 3, 1)),
                }
            });
            context.BlogPosts.Add(new BlogPost("Post2")
            {
                Comments = new List<BlogComment>()
                {
                    new BlogComment("3", new DateTime(2020, 3, 5)),
                    new BlogComment("4", new DateTime(2020, 3, 6)),
                }
            });
            context.BlogPosts.Add(new BlogPost("Post3")
            {
                Comments = new List<BlogComment>()
                {
                    new BlogComment("5", new DateTime(2020, 2, 7)),
                    new BlogComment("6", new DateTime(2020, 2, 9)),
                    new BlogComment("7", new DateTime(2020, 2, 8)),
                }
            });
            context.SaveChanges();
        }
    }
}