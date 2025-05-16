namespace Application.Common.Interfaces;
using Domain.Entities;
using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
public interface IApplicationDbContext {


    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
} 
