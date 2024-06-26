﻿using Infrastructure.Data;
using Domain.Models.Entities;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Lecturers Repository
    /// </summary>
    public class LecturersRepository : BaseRepository<Lecturer>, ILecturersRepository
    {
        private readonly ApplicationDbContext _context = null;

        /// <summary>
        /// Lecturers Repository constructor
        /// </summary>
        public LecturersRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all the lecturers
        /// </summary>
        /// <returns>Lecturers list</returns>
        public new Task<List<Lecturer>> GetAll()
        {
            return _context.Lecturer
                .Include(x => x.User)
                .Include(x => x.LecturerGroupRelations)
                .ToListAsync();
        }

        /// <summary>
        /// Gets lecturer by user id
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Lecturer by user id</returns>
        public Task<Lecturer> GetLecturerByUserId(string userId)
        {
            return _context.Lecturer
                .Include(x => x.User)
                .Include(x => x.LecturerGroupRelations)
                .Where(x => x.UserId.Equals(userId))
                .FirstOrDefaultAsync();
        }
    }
}
