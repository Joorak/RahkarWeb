using Application.Models;
using Domain.Entities;

namespace SharedUI.Interfaces;
public interface IEntityService<T> where T : BaseEntity
{ 
    Task<RequestResult<T>> Get(int id); 
    Task<RequestResult<T>> GetAll(); 
    Task<RequestResponse> Create(EntityRequest<T> entity);
    Task<RequestResponse> Update(EntityRequest<T> entity);
    Task<RequestResponse> Delete(int id); 
} 
