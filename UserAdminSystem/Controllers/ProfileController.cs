using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserAdminSystem.Data;
using UserAdminSystem.DTOs.Address;
using UserAdminSystem.DTOs.Persons;
using UserAdminSystem.Models;

namespace UserAdminSystem.Controllers;

[Route("api/profile")]
[ApiController]
[Authorize(Roles = "User, Admin")]
public class ProfileController(AppDbContext dbContext) : ControllerBase
{
    [HttpPost("get-persons")]
    public async Task<IActionResult> GetPersosns()
    {
        var personsDb = await dbContext.Persons.ToListAsync();
        return Ok(personsDb);
    }

    [HttpPost("add-person")]
    public async Task<IActionResult> AddPerson(AddPersonDto personDto)
    {
        var userDb = await dbContext.Users.FindAsync(personDto.UserId);
        if (userDb is null) return BadRequest("User account not exists");

        if (personDto.IsAccountHolder)
        {
            var checkPersons = await dbContext.Persons.Where(u => u.Id == personDto.UserId && u.IsAccountHolder).AnyAsync();
            if (!checkPersons) return BadRequest("There's already a person who owns this account");
        }

        var person = new Person
        {
            UserId = personDto.UserId,
            Name = personDto.Name,
            LastName = personDto.LastName,
            Birthday = personDto.Birthday,
            Curp = personDto.Curp,
            IsAccountHolder = personDto.IsAccountHolder
        };

        await dbContext.Persons.AddAsync(person);
        await dbContext.SaveChangesAsync();

        var userDb2 = await dbContext.Users.FindAsync(personDto.UserId);

        return Ok(userDb2);
    }

    [HttpPut("update-person/{userId}/{personId}")]
    public async Task<IActionResult> UpdatePerson(int userId, int personId, UpdatePersonDto personDto)
    {
        var userDb = await dbContext.Users.FindAsync(userId);
        if (userDb is null) return BadRequest("User account not exists");

        var personDb = await dbContext.Persons.FindAsync(personId);
        if (personDb is null) return NotFound("Person not exists");

        if (personDb.UserId != userDb.Id) return BadRequest("This person does not belong to this account");

        if (personDto.IsAccountHolder.HasValue && personDto.IsAccountHolder.Value)
        {
            var checkPersons = await dbContext.Persons.Where(u => u.UserId == userId && u.IsAccountHolder).AnyAsync();
            if (checkPersons)
            {
                //? If the user is trying to set this person as account holder, we need to check if there's already an account holder
                //? If there is, we need to remove the current account holder
                //? We can only have one account holder per user
                var currentHolder = await dbContext.Persons.FirstOrDefaultAsync(u => u.UserId == userId && u.IsAccountHolder);
                if (currentHolder != null)
                {
                    currentHolder.IsAccountHolder = false;
                    dbContext.Persons.Update(currentHolder);
                }

            }
        }

        personDb.Name = personDto.Name ?? personDb.Name;
        personDb.LastName = personDto.LastName ?? personDb.LastName;
        personDb.Birthday = personDto.Birthday ?? personDb.Birthday;
        personDb.Curp = personDto.Curp ?? personDb.Curp;
        personDb.IsAccountHolder = personDto.IsAccountHolder ?? personDb.IsAccountHolder;

        await dbContext.SaveChangesAsync();
        return Ok(personDb);
    }

    // Address Endpoints
    [HttpPost("add-person-address/{personId}")]
    public async Task<IActionResult> AddPersonAddress(int personId, AddAddressDto addressDto)
    {
        var personDb = await dbContext.Persons.FindAsync(personId);
        if (personDb is null) return NotFound("Person not found");

        var address = new Address
        {
            AddressName = addressDto.AddressName,
            Street = addressDto.Street,
            Neighborhood = addressDto.Neighborhood,
            City = addressDto.City,
            State = addressDto.State,
            PostalCode = addressDto.PostalCode,
            Country = addressDto.Country,
            PersonId = personId
        };

        await dbContext.Addresses.AddAsync(address);
        await dbContext.SaveChangesAsync();
        return Ok(address);
    }

    [HttpPut("update-person-address/{personId}/{addressId}")]
    public async Task<IActionResult> UpdatePersonAddress(int personId, int addressId, UpdateAddressDto addressDto)
    {
        var personDb = await dbContext.Persons.FindAsync(personId);
        if (personDb is null) return NotFound("Person not found");

        var addressDb = await dbContext.Addresses.FindAsync(addressId);
        if (addressDb is null) return NotFound("Address not found");

        if (addressDb.PersonId != personId) return BadRequest("This address does not belong to this person");

        addressDb.AddressName = addressDto.AddressName ?? addressDb.AddressName;
        addressDb.Street = addressDto.Street ?? addressDb.Street;
        addressDb.Neighborhood = addressDto.Neighborhood ?? addressDb.Neighborhood;
        addressDb.City = addressDto.City ?? addressDb.City;
        addressDb.State = addressDto.State ?? addressDb.State;
        addressDb.PostalCode = addressDto.PostalCode ?? addressDb.PostalCode;
        addressDb.Country = addressDto.Country ?? addressDb.Country;

        dbContext.Addresses.Update(addressDb);
        await dbContext.SaveChangesAsync();
        return Ok(addressDb);
    }

}