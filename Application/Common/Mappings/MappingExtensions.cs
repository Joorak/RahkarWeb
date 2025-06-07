using Application.Models;
using Microsoft.EntityFrameworkCore;
namespace Application.Common.Mappings; 
public static class MappingExtensions {
    //public static Task<List<TDestination>> ProjectToListAsync<TDestination>(this IQueryable queryable, AutoMapper.IConfigurationProvider configuration)
    //        => queryable.ProjectTo<TDestination>(configuration).ToListAsync();

    public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(this IQueryable<TDestination> queryable, int pageNumber, int pageSize)
            where TDestination : class
            => PaginatedList<TDestination>.CreateAsync(queryable.AsNoTracking(), pageNumber, pageSize);
} 
