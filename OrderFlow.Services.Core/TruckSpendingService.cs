using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Repository;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.TruckSpending;

namespace OrderFlow.Services.Core
{
    public class TruckSpendingService : BaseRepository, ITruckSpendingService
    {
        private readonly ITruckService _truckService;
        private readonly ITruckCourseService _truckCourseService;

        public TruckSpendingService(OrderFlowDbContext context,
                                    ITruckService truckService,
                                    ITruckCourseService truckCourseService) : base(context)
        {
            _truckService = truckService;
            _truckCourseService = truckCourseService;
        }

        public IQueryable<TruckSpending> GetAll()
        {
            return All<TruckSpending>().AsQueryable();
        }

        public async Task<IEnumerable<IndexTruckSpendingViewModel>> GetTruckSpendingsAsync(Guid? driverId = null, TruckSpendingQueryModel? query = null)
        {
            IQueryable<TruckSpending> spendings = GetAuthorizedSpendings(driverId).AsNoTracking();

            spendings = ApplyQuery(spendings, query);

            return await spendings.Select(spending => new IndexTruckSpendingViewModel
            {
                TruckSpendingID = spending.TruckSpendingID,
                TruckID = spending.TruckID,
                TruckLicensePlate = spending.Truck != null ? spending.Truck.LicensePlate : "No truck",
                TruckCourseID = spending.TruckCourseID,
                CourseDescription = spending.TruckCourse != null
                                                                    ? spending.TruckCourse.PickupAddress + " -> " + spending.TruckCourse.DeliverAddress + " at " + spending.TruckCourse.AssignedDate.ToString("dd/MM/yyyy")
                                                                    : "Not course related",
                Amount = spending.Amount,
                PaymentDate = spending.PaymentDate,
                PaymentDescription = spending.PaymentDescription,
                PaymentMethod = spending.PaymentMethod.ToString()
            }).ToListAsync();
        }

        public async Task<DetailsTruckSpendingViewModel?> GetTruckSpendingDetailsAsync(Guid spendingId, Guid? driverId = null)
        {
            if (spendingId == Guid.Empty)
            {
                return null;
            }

            return await this.GetAuthorizedSpendings(driverId)
                             .AsNoTracking()
                             .Where(spending => spending.TruckSpendingID == spendingId)
                             .Select(spending => new DetailsTruckSpendingViewModel
                             {
                                 TruckSpendingID = spending.TruckSpendingID,
                                 TruckID = spending.TruckID,
                                 TruckLicensePlate = spending.Truck != null ? spending.Truck.LicensePlate : "No truck",
                                 TruckCourseID = spending.TruckCourseID,
                                 CourseDescription = spending.TruckCourse != null
                                     ? spending.TruckCourse.PickupAddress + " -> " + spending.TruckCourse.DeliverAddress + " at " + spending.TruckCourse.AssignedDate.ToString("dd/MM/yyyy")
                                     : "Not course related",
                                 Amount = spending.Amount,
                                 PaymentDate = spending.PaymentDate,
                                 PaymentDescription = spending.PaymentDescription,
                                 PaymentMethod = spending.PaymentMethod.ToString()
                             })
                             .SingleOrDefaultAsync();
        }

        public async Task<CreateTruckSpendingViewModel?> GetTruckSpendingForEditAsync(Guid spendingId, Guid? driverId = null)
        {
            if (spendingId == Guid.Empty)
            {
                return null;
            }

            return await this.GetAuthorizedSpendings(driverId)
                             .AsNoTracking()
                             .Where(spending => spending.TruckSpendingID == spendingId)
                             .Select(spending => new CreateTruckSpendingViewModel
                             {
                                 TruckID = spending.TruckID,
                                 TruckCourseID = spending.TruckCourseID,
                                 Amount = spending.Amount,
                                 PaymentDate = spending.PaymentDate,
                                 PaymentDescription = spending.PaymentDescription,
                                 PaymentMethod = spending.PaymentMethod
                             })
                             .SingleOrDefaultAsync();
        }

        public async Task<bool> CreateTruckSpendingAsync(CreateTruckSpendingViewModel model, Guid? driverId = null)
        {
            if (model == null || model.TruckID == null || model.TruckID == Guid.Empty || model.Amount <= 0)
            {
                return false;
            }

            if (!await CanUseTruckAsync(model.TruckID.Value, driverId) ||
                !await CanUseCourseAsync(model.TruckCourseID, model.TruckID.Value, driverId))
            {
                return false;
            }

            await this.AddAsync(new TruckSpending
            {
                TruckID = model.TruckID,
                TruckCourseID = model.TruckCourseID,
                Amount = model.Amount,
                PaymentDate = model.PaymentDate,
                PaymentDescription = model.PaymentDescription,
                PaymentMethod = model.PaymentMethod
            });

            return await this.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateTruckSpendingAsync(Guid spendingId, CreateTruckSpendingViewModel model, Guid? driverId = null)
        {
            if (spendingId == Guid.Empty || model == null || model.TruckID == null || model.TruckID == Guid.Empty || model.Amount <= 0)
            {
                return false;
            }

            TruckSpending? spending = await this.GetAuthorizedSpendings(driverId)
                                                .Where(ts => ts.TruckSpendingID == spendingId)
                                                .SingleOrDefaultAsync();

            if (spending == null ||
                !await CanUseTruckAsync(model.TruckID.Value, driverId) ||
                !await CanUseCourseAsync(model.TruckCourseID, model.TruckID.Value, driverId))
            {
                return false;
            }

            spending.TruckID = model.TruckID;
            spending.TruckCourseID = model.TruckCourseID;
            spending.Amount = model.Amount;
            spending.PaymentDate = model.PaymentDate;
            spending.PaymentDescription = model.PaymentDescription;
            spending.PaymentMethod = model.PaymentMethod;

            return await this.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteTruckSpendingAsync(Guid spendingId)
        {
            if (spendingId == Guid.Empty)
            {
                return false;
            }

            TruckSpending? spending = await this.GetAll()
                                                .Where(ts => ts.TruckSpendingID == spendingId)
                                                .SingleOrDefaultAsync();

            if (spending == null)
            {
                return false;
            }

            this.Delete(spending);

            return await this.SaveChangesAsync() > 0;
        }

        public async Task<Dictionary<Guid, string>> GetTruckOptionsAsync(Guid? driverId = null)
        {
            IQueryable<Truck> trucks = _truckService.GetAll().AsNoTracking().Where(truck => !truck.IsDeleted);

            if (driverId.HasValue)
            {
                trucks = trucks.Where(truck => truck.DriverID == driverId.Value);
            }

            return await trucks
                .OrderBy(truck => truck.LicensePlate)
                .Select(truck => new { truck.TruckID, truck.LicensePlate })
                .ToDictionaryAsync(truck => truck.TruckID, truck => truck.LicensePlate);
        }

        public async Task<Dictionary<Guid, string>> GetCourseOptionsAsync(Guid? truckId = null, Guid? driverId = null)
        {
            IQueryable<TruckCourse> courses = _truckCourseService.GetAll()
                                                                 .AsNoTracking()
                                                                 .Include(course => course.Truck);

            if (truckId.HasValue)
            {
                courses = courses.Where(course => course.TruckID == truckId.Value);
            }

            if (driverId.HasValue)
            {
                courses = courses.Where(course => course.Truck != null && course.Truck.DriverID == driverId.Value);
            }

            return await courses.OrderByDescending(course => course.AssignedDate)
                                .Select(course => new { course.TruckCourseID, course.PickupAddress, course.DeliverAddress, course.AssignedDate, course!.Truck!.LicensePlate })
                                .ToDictionaryAsync(
                                    course => course.TruckCourseID,
                                    course => course.PickupAddress + " -> " + course.DeliverAddress + ((!string.IsNullOrEmpty(course.LicensePlate)) ? " driven by " + course.LicensePlate : "") + " (" + course.AssignedDate.ToString("dd/MM/yyyy") + ")");
        }

        private IQueryable<TruckSpending> GetAuthorizedSpendings(Guid? driverId)
        {
            IQueryable<TruckSpending> spendings = this.GetAll()
                                                      .Include(spending => spending.Truck)
                                                      .Include(spending => spending.TruckCourse);

            if (driverId.HasValue)
            {
                spendings = spendings.Where(spending => spending.Truck != null && spending.Truck.DriverID == driverId.Value);
            }

            return spendings;
        }

        private async Task<bool> CanUseTruckAsync(Guid truckId, Guid? driverId)
        {
            IQueryable<Truck> trucks = _truckService.GetAll().Where(truck => truck.TruckID == truckId && !truck.IsDeleted);

            if (driverId.HasValue)
            {
                trucks = trucks.Where(truck => truck.DriverID == driverId.Value);
            }

            return await trucks.AnyAsync();
        }

        private async Task<bool> CanUseCourseAsync(Guid? courseId, Guid truckId, Guid? driverId)
        {
            if (courseId == null || courseId == Guid.Empty)
            {
                return true;
            }

            IQueryable<TruckCourse> courses = _truckCourseService.GetAll()
                                                                 .Include(course => course.Truck)
                                                                 .Where(course => course.TruckCourseID == courseId.Value && course.TruckID == truckId);

            if (driverId.HasValue)
            {
                courses = courses.Where(course => course.Truck != null && course.Truck.DriverID == driverId.Value);
            }

            return await courses.AnyAsync();
        }

        private static IQueryable<TruckSpending> ApplyQuery(IQueryable<TruckSpending> spendings, TruckSpendingQueryModel? query)
        {
            query ??= new TruckSpendingQueryModel();

            if (query.TruckID.HasValue)
            {
                spendings = spendings.Where(spending => spending.TruckID == query.TruckID.Value);
            }

            if (query.TruckCourseID.HasValue)
            {
                spendings = spendings.Where(spending => spending.TruckCourseID == query.TruckCourseID.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                spendings = spendings.Where(spending => spending.PaymentDescription.Contains(query.Search));
            }

            return (query.SortOrder switch
            {
                "date_asc" => spendings.OrderBy(spending => spending.PaymentDate),
                "amount_desc" => spendings.OrderByDescending(spending => spending.Amount),
                "amount_asc" => spendings.OrderBy(spending => spending.Amount),
                _ => spendings.OrderByDescending(spending => spending.PaymentDate)
            })
            .Skip((Math.Max(query.Page, 1) - 1) * Math.Max(query.PageSize, 1))
            .Take(Math.Max(query.PageSize, 1));
        }
    }
}
