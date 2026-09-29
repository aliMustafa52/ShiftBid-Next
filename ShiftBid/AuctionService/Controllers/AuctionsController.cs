using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using Contracts;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController(AuctionDbContext context, 
    IDbContextOutbox<AuctionDbContext> dbContextOutbox) : ControllerBase
{
    private readonly AuctionDbContext _context = context;
    private readonly IDbContextOutbox<AuctionDbContext> _dbContextOutbox = dbContextOutbox;

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions([FromQuery] string? date)
    {
        var query = _context.Auctions
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

        var auctions = await query
            .ProjectToType<AuctionDto>()
            .ToListAsync();

        return Ok(auctions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(string id)
    {
        var auction = await _context.Auctions
            .Where(a => a.Id == id)
            .ProjectToType<AuctionDto>()
            .FirstOrDefaultAsync();

        if (auction == null)
        {
            return NotFound();
        }

        return Ok(auction);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
    {
        var auction = createAuctionDto.Adapt<Auction>();

        // Set the seller from User claims (defaults to "test" until JWT auth is connected)
        auction.Seller = User.Identity?.Name ?? "test";

        _context.Auctions.Add(auction);

        var newAuction = auction.Adapt<AuctionDto>();

        await _dbContextOutbox.PublishAsync(newAuction.Adapt<AuctionCreated>());

        await _dbContextOutbox.SaveChangesAndFlushMessagesAsync();

        return CreatedAtAction(nameof(GetAuctionById), new { id = auction.Id }, newAuction);
    }

    [Authorize]
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

        await _dbContextOutbox.PublishAsync(auction.Adapt<AuctionUpdated>());

        await _dbContextOutbox.SaveChangesAndFlushMessagesAsync();

        return NoContent();
    }

    [Authorize]
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

        await _dbContextOutbox.PublishAsync(new AuctionDeleted { Id = auction.Id });

        await _dbContextOutbox.SaveChangesAndFlushMessagesAsync();

        return Ok();
    }

    [HttpPost("test")]
    public ActionResult<string> TestAuth()
    {
        return Ok(User.Identity?.Name ?? "Anonymous");
    }
}
