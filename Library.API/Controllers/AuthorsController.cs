﻿using AutoMapper;
using Library.API.Helpers;
using Library.API.Model;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Controllers
{
    [Route("api/authors")]
    public class AuthorsController:Controller
    {
        private ILabraryRepository _libraryRepository;

        public AuthorsController(ILabraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        [HttpGet()]
        public IActionResult GetAuthors()
        {
            var authorsFromRepo = _libraryRepository.GetAuthors();

            //var authors = new List<AuthorDto>();

            ////Çok fazla propery veya collection bulunan sınıflar için automap kullanılması önerilmektedir.

            //foreach (var author in authorsFromRepo)
            //{
            //    authors.Add(new AuthorDto()
            //    {
            //        Id = author.Id,
            //        Name = $"{author.FirstName}{author.LastName}",
            //        Genre = author.Genre,
            //        Age = author.DateOfBirth.GetCurrentAge()
            //    });
            //}

            var authors = Mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo);
            return new JsonResult(authors);
        }

        [HttpGet("{Id}")]
        public IActionResult GetAuthor(Guid id)
        {
            var authorFromRepo = _libraryRepository.GetAuthor(id);
            var author = Mapper.Map<AuthorDto>(authorFromRepo);
            return new JsonResult(author);
        }
    }
}
