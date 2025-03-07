﻿using LibApp.Data;
using LibApp.Dtos;
using LibApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace LibApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewRentalsController : ControllerBase
    {
        public NewRentalsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signIn)
        {
            _context = context;
            _userManager = userManager;
            _signIn = signIn;
        }

        [HttpPost]
        public IActionResult CreateNewRental([FromBody] NewRentalDto newRental)
        {
            var customer = _context.Customers
                .Include(c => c.MembershipType)
                .Single(c => c.Id == newRental.CustomerId);

            var books = _context.Books
                .Include(b => b.Genre)
                .Where(b => newRental.BookIds.Contains(b.Id))
                .ToList();

            foreach (var book in books)
            {
                if (book.NumberAvailable == 0)
                    return BadRequest("Book is not available");

                book.NumberAvailable--;
                var rental = new Rental()
                {
                    Customer = customer,
                    Book = book,
                    DateRented = DateTime.Now
                };

                _context.Rentals.Add(rental);
            }

            _context.SaveChanges();

            return Ok();
        }

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signIn;
    }
}
