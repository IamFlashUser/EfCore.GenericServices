﻿// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using GenericServices;

namespace Tests.Dtos
{
    [IncludeThen(nameof(Book.AuthorsLink), nameof(BookAuthor.Author))]
    public class AddNewAuthorToBookUsingIncludesDto : ILinkToEntity<Book>
    {
        public int BookId { get; set; }

        public Author AddThisAuthor { get; set; }
        public byte Order { get; set; }
    }
}