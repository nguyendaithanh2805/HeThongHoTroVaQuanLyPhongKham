﻿using HeThongHoTroVaQuanLyPhongKham.Data;
using Microsoft.EntityFrameworkCore;

namespace HeThongHoTroVaQuanLyPhongKham.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly ILogger<T> _logger;

        public Repository(ApplicationDbContext context, ILogger<T> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<T> CreateAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Đã tạo {typeof(T).Name} thành công.");
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Đã xóa {typeof(T).Name} thành công.");
        }

        public async Task<IEnumerable<T>> FindAllAsync(int page, int pageSize, int pageSkip, string keyPropertyName)
        {
            var entities = await _context.Set<T>()
                .OrderBy(e => EF.Property<int>(e, keyPropertyName))
                .Skip(pageSkip)
                .Take(pageSize)
                .ToListAsync();
            _logger.LogInformation($"Đã lấy danh sách {typeof(T).Name} - trang {page} với {pageSize} bản ghi.");
            return entities;
        }

        public async Task<T> FindByIdAsync(int id, string keyPropertyName)
        {
            var entity = await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(e => EF.Property<int>(e, keyPropertyName) == id);
            _logger.LogInformation($"Đã tìm thấy {typeof(T).Name} với ID {id}.");
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Đã cập nhật {typeof(T).Name} thành công.");
            return entity;
        }

        public IQueryable<T> GetQueryable()
        {
            return _context.Set<T>().AsNoTracking();
        }

        public async Task<IEnumerable<T>> FindAllWithQueryAsync(IQueryable<T> query, int page, int pageSize, int pageSkip, string keyPropertyName)
        {
            var entities = await query
                .OrderBy(e => EF.Property<int>(e, keyPropertyName))
                .Skip(pageSkip)
                .Take(pageSize)
                .ToListAsync();
            _logger.LogInformation($"Đã lấy danh sách {typeof(T).Name} với truy vấn - trang {page} với {pageSize} bản ghi.");
            return entities;
        }

        public async Task<T> FindByIdWithQueryAsync(IQueryable<T> query, int id, string keyPropertyName)
        {
            _logger.LogInformation($"Query SQL: {query.ToQueryString()}");
            return await query
                .Where(e => EF.Property<int>(e, keyPropertyName) == id)
                .FirstOrDefaultAsync();
        }

        public async Task DeleteAsync(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Đã xóa {typeof(T).Name} thành công.");
        }

        public async Task<IEnumerable<T>> FindAllAsync(string keyPropertyName)
        {
            var entities = await _context.Set<T>()
                .OrderBy(e => EF.Property<int>(e, keyPropertyName))
                .ToListAsync();
            _logger.LogInformation($"Đã lấy danh sách {typeof(T).Name}.");
            return entities;
        }

        public async Task<int> CountAsync()
        {
            return await _context.Set<T>().CountAsync();
        }

        public async Task<T> FindByIdForUpdateAsync(int id, string keyPropertyName)
        {
            var entity = await _context.Set<T>()
                .FirstOrDefaultAsync(e => EF.Property<int>(e, keyPropertyName) == id);
            _logger.LogInformation($"Đã tìm thấy {typeof(T).Name} với ID {id}.");
            return entity;
        }
    }
}
