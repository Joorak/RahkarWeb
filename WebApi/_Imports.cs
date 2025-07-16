

global using System.IdentityModel.Tokens.Jwt;
global using System.Net.Http.Headers;
global using System.Text;
global using System.Text.Json;
global using Application;
global using Application.Models;
global using Application.Exceptions;
global using Application.Interfaces;
global using Application.Utils;
global using Domain.Entities.Identity;
global using Infrastructure;
global using Infrastructure.Services;
global using Infrastructure.Persistence;
global using WebApi.Filters;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.IdentityModel.Tokens;
global using Serilog;
global using Serilog.Events;
global using Stripe;
global using Stripe.Checkout;
