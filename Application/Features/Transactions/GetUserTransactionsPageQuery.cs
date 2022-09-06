using Application.Pipelines;
using Application.Services;
using Contracts.Common;
using Contracts.Errors;
using Contracts.Transactions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Application.Features.Transactions;

[Access(AccessLevels.LoggedIn)]
public record GetUserTransactionsPageQuery(int Page, int ElementsPerPage = 25) : 
    IRequest<OneOf<PageDto<TransactionDto>, UserNotFound>>;

public class GetUserTransactionsPageQueryHandler : IRequestHandler<GetUserTransactionsPageQuery,
    OneOf<PageDto<TransactionDto>, UserNotFound>>
{
    private readonly IAuthService _authService;
    private readonly IAppDbContext _dbContext;

    public GetUserTransactionsPageQueryHandler(IAuthService authService, IAppDbContext dbContext)
    {
        _authService = authService;
        _dbContext = dbContext;
    }

    public async Task<OneOf<PageDto<TransactionDto>, UserNotFound>> Handle(GetUserTransactionsPageQuery request, CancellationToken cancellationToken)
    {
        if (request.ElementsPerPage <= 0) return new PageDto<TransactionDto>();
        
        var user = await _authService.GetCurrentUser(cancellationToken);
        if (user is null) return new UserNotFound();

        var query = _dbContext.Users
            .Where(x => x.Id == user.Id)
            .SelectMany(x => x.Transactions).AsQueryable();
        var totalElements = await query
            .CountAsync(cancellationToken);
        var totalPages = (totalElements - 1) / request.ElementsPerPage + 1;
        if (request.Page <= 0)
            return new PageDto<TransactionDto>()
            {
                CurrentPage = request.Page,
                ElementsPerPage = request.ElementsPerPage,
                PageElements = Array.Empty<TransactionDto>(),
                TotalElements = totalElements,
                TotalPages = totalPages,
            };
        var skips = (request.Page - 1) * request.ElementsPerPage;
        var elements = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip(skips)
            .Take(request.ElementsPerPage)
            .ToListAsync(cancellationToken);
        return new PageDto<TransactionDto>()
        {
            CurrentPage = request.Page,
            ElementsPerPage = request.ElementsPerPage,
            PageElements = elements.Select(x=> new TransactionDto()
            {
                Id = x.Id,
                Amount = x.Amount.GetUnits(),
                CreatedAt = x.CreatedAt,
                Type = (TransactionTypeDto)x.Type
            }).ToList(),
            TotalElements = totalElements,
            TotalPages = totalPages,
        };
    }
}