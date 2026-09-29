using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.Cargos
{
    public class CargoRepository : GenericRepository<Cargo>, ICargoRepository
    {
        private readonly AppDbContext _context;

        public CargoRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private IQueryable<Cargo> WithDetails() => _context.Cargos
            .Include(c => c.Sender)
            .Include(c => c.Receiver)
            .Include(c => c.OriginBranch).ThenInclude(b => b.City)
            .Include(c => c.DestinationBranch).ThenInclude(b => b.City);

        public override async Task<List<Cargo>> GetAllAsync()
        {
            return await WithDetails()
                .OrderByDescending(c => c.ShipmentDate)
                .ToListAsync();
        }

        public override async Task<Cargo> GetByIdAsync(Guid id)
        {
            return await WithDetails().FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cargo> GetByTrackCodeAsync(string trackCode)
        {
            return await WithDetails().FirstOrDefaultAsync(c => c.TrackCode == trackCode);
        }

        public async Task<List<Cargo>> GetByBranchAsync(Guid branchId)
        {
            return await WithDetails()
                .Where(c => c.OriginBranchId == branchId || c.DestinationBranchId == branchId)
                .OrderByDescending(c => c.ShipmentDate)
                .ToListAsync();
        }

        public async Task<List<Cargo>> GetByCustomerAsync(Guid appUserId)
        {
            return await WithDetails()
                .Where(c => c.SenderId == appUserId || c.ReceiverId == appUserId)
                .OrderByDescending(c => c.ShipmentDate)
                .ToListAsync();
        }
        public async Task<(List<Cargo> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? statusGroup, string? search)
        {
            var query = ApplyFilters(WithDetails(), statusGroup, search);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(c => c.ShipmentDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(int All, int Transit, int Delivered, int Problem)> GetStatusCountsAsync(string? search)
        {
            var baseQuery = ApplyFilters(WithDetails(), null, search);

            var all = await baseQuery.CountAsync();
            var delivered = await baseQuery.CountAsync(c => c.CargoStatus == CargoStatus.Delivered);
            var problem = await baseQuery.CountAsync(c =>
                c.CargoStatus == CargoStatus.FailedDelivery ||
                c.CargoStatus == CargoStatus.InReturnProcess ||
                c.CargoStatus == CargoStatus.ReturnedToSender);
            var transit = all - delivered - problem;

            return (all, transit, delivered, problem);
        }

        private static IQueryable<Cargo> ApplyFilters(IQueryable<Cargo> query, string? statusGroup, string? search)
        {
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(c =>
                    c.TrackCode.ToLower().Contains(s) ||
                    c.OriginBranch.Name.ToLower().Contains(s) ||
                    c.DestinationBranch.Name.ToLower().Contains(s) ||
                    c.Sender.FirstName.ToLower().Contains(s) || c.Sender.LastName.ToLower().Contains(s) ||
                    c.Receiver.FirstName.ToLower().Contains(s) || c.Receiver.LastName.ToLower().Contains(s));
            }

            query = statusGroup switch
            {
                "delivered" => query.Where(c => c.CargoStatus == CargoStatus.Delivered),
                "problem" => query.Where(c =>
                    c.CargoStatus == CargoStatus.FailedDelivery ||
                    c.CargoStatus == CargoStatus.InReturnProcess ||
                    c.CargoStatus == CargoStatus.ReturnedToSender),
                "transit" => query.Where(c =>
                    c.CargoStatus != CargoStatus.Delivered &&
                    c.CargoStatus != CargoStatus.FailedDelivery &&
                    c.CargoStatus != CargoStatus.InReturnProcess &&
                    c.CargoStatus != CargoStatus.ReturnedToSender),
                _ => query
            };


            return query;

        }
        public async Task<List<Cargo>> GetDeliveredForTrainingAsync()
        {
            return await _context.Cargos
                .Include(c => c.OriginBranch)
                .Include(c => c.DestinationBranch)
                .Where(c => c.CargoStatus == CargoStatus.Delivered)
                .ToListAsync();
        }

        public async Task<List<Cargo>> GetPendingCargosForRoutingAsync()
        {
            return await _context.Cargos
                .Include(c => c.DestinationBranch)
                .Where(c => !c.IsDeleted && c.CargoStatus != CargoStatus.Delivered)
                .ToListAsync();
        }
    }
}