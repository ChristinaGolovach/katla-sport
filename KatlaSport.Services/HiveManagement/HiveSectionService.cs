using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using KatlaSport.DataAccess;
using KatlaSport.DataAccess.ProductStoreHive;
using DbHiveSection = KatlaSport.DataAccess.ProductStoreHive.StoreHiveSection;

namespace KatlaSport.Services.HiveManagement
{
    /// <summary>
    /// Represents a hive section service.
    /// </summary>
    public class HiveSectionService : IHiveSectionService
    {
        private readonly IProductStoreHiveContext _context;
        private readonly IUserContext _userContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="HiveSectionService"/> class with specified <see cref="IProductStoreHiveContext"/> and <see cref="IUserContext"/>.
        /// </summary>
        /// <param name="context">A <see cref="IProductStoreHiveContext"/>.</param>
        /// <param name="userContext">A <see cref="IUserContext"/>.</param>
        public HiveSectionService(IProductStoreHiveContext context, IUserContext userContext)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userContext = userContext ?? throw new ArgumentNullException();
        }

        /// <inheritdoc/>
        public async Task<List<HiveSectionListItem>> GetHiveSectionsAsync()
        {
            var dbHiveSections = await _context.Sections.OrderBy(s => s.Id).ToArrayAsync();
            var hiveSections = dbHiveSections.Select(s => Mapper.Map<HiveSectionListItem>(s)).ToList();
            return hiveSections;
        }

        /// <inheritdoc/>
        public async Task<HiveSection> GetHiveSectionAsync(int hiveSectionId)
        {
            var dbHiveSection = await GetExistingHiveSection(hiveSectionId).ConfigureAwait(false);

            return Mapper.Map<DbHiveSection, HiveSection>(dbHiveSection);
        }

        /// <inheritdoc/>
        public async Task<List<HiveSectionListItem>> GetHiveSectionsAsync(int hiveId)
        {
            var dbHiveSections = await _context.Sections.Where(s => s.StoreHiveId == hiveId).OrderBy(s => s.Id).ToArrayAsync();
            var hiveSections = dbHiveSections.Select(s => Mapper.Map<HiveSectionListItem>(s)).ToList();
            return hiveSections;
        }

        /// <inheritdoc/>
        public async Task<HiveSection> CreateHiveSectionAsync(UpdateHiveSectionRequest createRequest)
        {
            await CheckExistingHiveSectionCode(createRequest.Code, s => s.Code == createRequest.Code).ConfigureAwait(false);

            var dbHiveSection = Mapper.Map<UpdateHiveSectionRequest, DbHiveSection>(createRequest);

            dbHiveSection.CreatedBy = _userContext.UserId;
            dbHiveSection.LastUpdatedBy = _userContext.UserId;
            _context.Sections.Add(dbHiveSection);

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return Mapper.Map<DbHiveSection, HiveSection>(dbHiveSection);
        }

        /// <inheritdoc/>
        public async Task<HiveSection> UpdateHiveSectionAsync(int hiveSectionId, UpdateHiveSectionRequest updateRequest)
        {
            var dbHiveSection = await GetExistingHiveSection(hiveSectionId).ConfigureAwait(false);

            await CheckExistingHiveSectionCode(updateRequest.Code, s => s.Code == updateRequest.Code && s.Id != hiveSectionId).ConfigureAwait(false);

            dbHiveSection.LastUpdatedBy = _userContext.UserId;

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return Mapper.Map<DbHiveSection, HiveSection>(dbHiveSection);
        }

        /// <inheritdoc/>
        public async Task DeleteHiveSectionAsync(int hiveSectionId)
        {
            var dbHiveSection = await GetExistingHiveSection(hiveSectionId).ConfigureAwait(false);

            if (dbHiveSection.IsDeleted == false)
            {
                throw new RequestedResourceHasConflictException($"The section with id - {hiveSectionId} is not marked for removal. You can not delete it.");
            }

            _context.Sections.Remove(dbHiveSection);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task SetStatusAsync(int hiveSectionId, bool deletedStatus)
        {
            var dbHiveSection = await GetExistingHiveSection(hiveSectionId).ConfigureAwait(false);

            if (dbHiveSection.IsDeleted != deletedStatus)
            {
                dbHiveSection.IsDeleted = deletedStatus;
                dbHiveSection.LastUpdated = DateTime.UtcNow;
                dbHiveSection.LastUpdatedBy = _userContext.UserId;
                await _context.SaveChangesAsync();
            }
        }

        private async Task<DbHiveSection> GetExistingHiveSection(int hiveSectionId)
        {
            var dbHiveSection = await _context.Sections.FirstOrDefaultAsync(s => s.Id == hiveSectionId).ConfigureAwait(false);

            if (dbHiveSection == null)
            {
                throw new RequestedResourceNotFoundException($"The hive section with id {dbHiveSection} not found.");
            }

            return dbHiveSection;
        }

        private async Task CheckExistingHiveSectionCode(string sectionCode, Expression<Func<DbHiveSection, bool>> predicate)
        {
            var dbHiveSectionWithDuplicateCode = await _context.Sections.FirstOrDefaultAsync(predicate).ConfigureAwait(false);

            if (dbHiveSectionWithDuplicateCode != null)
            {
                throw new RequestedResourceHasConflictException($"The hive section with code {sectionCode} already exists.");
            }
        }
    }
}
