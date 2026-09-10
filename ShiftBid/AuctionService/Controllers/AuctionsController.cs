using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext _context;

    public AuctionsController(AuctionDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions([FromQuery] string? date)
    {
        var query = _context.Auctions
            .Include(a => a.Item)
            .OrderBy(a => a.Item.Make)
            .AsQueryable();

        if (!string.IsNullOrEmpty(date))
        {
            if (DateTime.TryParse(date, out var parsedDate))
            {
                var utcDate = parsedDate.ToUniversalTime();
                query = query.Where(a => a.UpdatedAt.CompareTo(utcDate) > 0);
            }
        }

        var auctions = await query.ToListAsync();

        return Ok(auctions.Adapt<List<AuctionDto>>());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(string id)
    {
        var auction = await _context.Auctions
            .Include(a => a.Item)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (auction == null)
        {
            return NotFound();
        }

        return Ok(auction.Adapt<AuctionDto>());
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
    {
        var auction = createAuctionDto.Adapt<Auction>();

        // Set the seller from User claims (defaults to "test" until JWT auth is connected)
        auction.Seller = User.Identity?.Name ?? "test";

        _context.Auctions.Add(auction);

        var result = await _context.SaveChangesAsync() > 0;

        if (!result)
        {
            return BadRequest("Could not save changes to the database");
        }

        return CreatedAtAction(nameof(GetAuctionById), new { id = auction.Id }, auction.Adapt<AuctionDto>());
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(string id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await _context.Auctions
            .Include(a => a.Item)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (auction == null)
        {
            return NotFound();
        }

        // Check if caller is seller (enforced when authenticated)
        if (User.Identity?.IsAuthenticated == true && auction.Seller != User.Identity.Name)
        {
            return Forbid();
        }

        // Business rule: rejected if the auction already has bids
        if (auction.CurrentHighBid > 0)
        {
            return BadRequest("Cannot update an auction that already has bids");
        }

        // Partial update: Mapster only applies non-null fields
        updateAuctionDto.Adapt(auction.Item);
        auction.UpdatedAt = DateTime.UtcNow;

        var result = await _context.SaveChangesAsync() > 0;

        if (result)
        {
            return Ok();
        }

        return BadRequest("Problem saving changes to the database");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(string id)
    {
        var auction = await _context.Auctions.FindAsync(id);

        if (auction == null)
        {
            return NotFound();
        }

        // Check if caller is seller (enforced when authenticated)
        if (User.Identity?.IsAuthenticated == true && auction.Seller != User.Identity.Name)
        {
            return Forbid();
        }

        // Business rule: rejected if the auction already has bids
        if (auction.CurrentHighBid > 0)
        {
            return BadRequest("Cannot delete an auction that already has bids");
        }

        _context.Auctions.Remove(auction);

        var result = await _context.SaveChangesAsync() > 0;

        if (result)
        {
            return Ok();
        }

        return BadRequest("Could not update the database");
    }

    [HttpPost("test")]
    public ActionResult<string> TestAuth()
    {
        return Ok(User.Identity?.Name ?? "Anonymous");
    }
}
