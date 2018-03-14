﻿// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices.Internal.Decoders;
using GenericServices.PublicButHidden;
using Microsoft.EntityFrameworkCore;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesPublic
{
    public class TestDirectAccess
    {
        [Fact]
        public void TestGetSingleOnEntityOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var service = new GenericService<EfCoreContext>(context);

                //ATTEMPT
                var book = service.GetSingle<Book>(1);

                //VERIFY
                service.HasErrors.ShouldBeFalse(string.Join("\n", service.Errors));
                book.BookId.ShouldEqual(1);
                context.Entry(book).State.ShouldEqual(EntityState.Unchanged);
            }
        }

        [Fact]
        public void TestGetSingleOnEntityNotFound()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var service = new GenericService<EfCoreContext>(context);

                //ATTEMPT
                var book = service.GetSingle<Book>(99);

                //VERIFY
                service.HasErrors.ShouldBeTrue();
                service.Errors.First().ErrorMessage.ShouldEqual("Sorry, I could not find the Book you were looking for.");
                book.ShouldBeNull();

            }
        }

        [Fact]
        public void TestManyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var service = new GenericService<EfCoreContext>(context);

                //ATTEMPT
                var books = service.GetManyNoTracked<Book>();

                //VERIFY
                service.HasErrors.ShouldBeFalse(string.Join("\n", service.Errors));
                books.Count().ShouldEqual(4);
                context.Entry(books.ToList().First()).State.ShouldEqual(EntityState.Detached);
            }
        }

        [Fact]
        public void TestCreateEntityOk()
        {
            //SETUP
            var unique = Guid.NewGuid().ToString();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
            }
            using (var context = new EfCoreContext(options))
            {
                var service = new GenericService<EfCoreContext>(context);

                //ATTEMPT
                var author = new Author { AuthorId = 1, Name = "New Name", Email = unique };
                service.Create(author);
                //VERIFY
                service.HasErrors.ShouldBeFalse(string.Join("\n", service.Errors));
            }
            using (var context = new EfCoreContext(options))
            {
                context.Authors.Count().ShouldEqual(1);
                context.Authors.Find(1).Email.ShouldEqual(unique);
            }
        }

        [Fact]
        public void TestUpdateOnEntityAlreadyTrackedOk()
        {
            //SETUP
            var unique = Guid.NewGuid().ToString();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }
            using (var context = new EfCoreContext(options))
            {
                var service = new GenericService<EfCoreContext>(context);

                //ATTEMPT
                var author = service.GetSingle<Author>(1);
                author.Email = unique;
                service.Update(author);
                //VERIFY
                service.HasErrors.ShouldBeFalse(string.Join("\n", service.Errors));
            }
            using (var context = new EfCoreContext(options))
            {
                context.Authors.Find(1).Email.ShouldEqual(unique);
            }
        }

        [Fact]
        public void TestUpdateOnEntityNotTrackedOk()
        {
            //SETUP
            var unique = Guid.NewGuid().ToString();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }
            using (var context = new EfCoreContext(options))
            {
                var service = new GenericService<EfCoreContext>(context);
                var logs = context.SetupLogging();

                //ATTEMPT
                var author = new Author {AuthorId = 1, Name = "New Name", Email = unique};
                service.Update(author);

                //VERIFY
                service.HasErrors.ShouldBeFalse(string.Join("\n", service.Errors));
            }
            using (var context = new EfCoreContext(options))
            {
                context.Authors.Find(1).Email.ShouldEqual(unique);
            }
        }

        [Fact]
        public void TestDeleteEntityOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }
            using (var context = new EfCoreContext(options))
            {
                var service = new GenericService<EfCoreContext>(context);

                //ATTEMPT
                service.Delete<Book>(1);

                //VERIFY
                service.HasErrors.ShouldBeFalse(string.Join("\n", service.Errors));
            }
            using (var context = new EfCoreContext(options))
            {
                context.Books.Count().ShouldEqual(3);
            }
        }
    }
}