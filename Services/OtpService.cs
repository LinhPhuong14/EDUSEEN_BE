using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sep490_Eduseen_BE.Dtos.Auth;
using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Repositories;

namespace Sep490_Eduseen_BE.Services
{
    public class OtpService : IOtpService
    {
        private readonly Sep490EduseenContext _context;

        public OtpService(Sep490EduseenContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task SaveOtpAsync( string email, string otp)
        {

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email is required.", nameof(email));
            }
            if (string.IsNullOrEmpty(otp))
            {
                throw new ArgumentException("OTP is required.", nameof(otp));
            }

            var otpEntry = new Otp
            {
                Email = email,
                OtpCode = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                CreatedAt = DateTime.UtcNow,
                IsUsed = false
            };

            await _context.Otps.AddAsync(otpEntry);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> VerifyOtpAsync(ConfirmOtpDTO confirmOtpDTO)
        {
            if (confirmOtpDTO == null || string.IsNullOrEmpty(confirmOtpDTO.Email) || string.IsNullOrEmpty(confirmOtpDTO.Otp))
            {
                throw new ArgumentNullException(nameof(confirmOtpDTO), "Request body, email, and OTP cannot be null or empty.");
            }

            var otpEntry = await _context.Otps
                .FirstOrDefaultAsync(o =>
                                         o.Email == confirmOtpDTO.Email &&
                                         o.OtpCode == confirmOtpDTO.Otp &&
                                         o.IsUsed == false);

            if (otpEntry == null)
            {
                return false;
            }

            if (otpEntry.ExpiresAt < DateTime.UtcNow)
            {
                _context.Otps.Remove(otpEntry);
                await _context.SaveChangesAsync();
                return false;
            }

            otpEntry.IsUsed = true;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}