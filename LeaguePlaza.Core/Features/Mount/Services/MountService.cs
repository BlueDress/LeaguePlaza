using LeaguePlaza.Common.Constants;
using LeaguePlaza.Core.Features.Mount.Contracts;
using LeaguePlaza.Core.Features.Mount.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Mount.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Mount.Models.RequestData;
using LeaguePlaza.Core.Features.Mount.Models.ViewModels;
using LeaguePlaza.Core.Features.Pagination.Models;
using LeaguePlaza.Infrastructure.Data.Entities;
using LeaguePlaza.Infrastructure.Data.Enums;
using LeaguePlaza.Infrastructure.Data.Repository;
using LeaguePlaza.Infrastructure.Dropbox.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

namespace LeaguePlaza.Core.Features.Mount.Services
{
    public class MountService(IRepository repository, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, IDropboxService dropboxService) : IMountService
    {
        private const string ImageUploadPath = "/mounts/{0}/{1}/{2}";

        private readonly Dictionary<string, string> DefaultMountTypeImageUrls = new()
        {
            { "0", "https://www.dropbox.com/scl/fi/wyvpahi0salv5ii2v5i8r/ground-default.jpg?rlkey=br72tc41gyn9b59bqk5ahyyod&st=w147fofs&raw=1" },
            { "1", "https://www.dropbox.com/scl/fi/9n7d7geaprae40gjhcvyr/flying-default.jpg?rlkey=ktap2t7jjgo2j8oatka34rxdu&st=pfuagymz&raw=1" },
            { "2", "https://www.dropbox.com/scl/fi/soux4avtf2hlpjw2gguth/aquatic-default.jpg?rlkey=hlf1n9g4pts8zrcfiaiglddyu&st=om0epfjz&raw=1" },
        };

        private readonly IRepository _repository = repository;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IDropboxService _dropboxService = dropboxService;

        public async Task<MountsViewModel> CreateMountsViewModelAsync()
        {
            IEnumerable<MountEntity> mounts = await _repository.FindSpecificCountOrderedReadOnlyAsync<MountEntity, double>(MountConstants.PageOne, MountConstants.CountForPagination, true, m => m.Rating, m => true);
            int totalResults = await _repository.GetCountAsync<MountEntity>(m => true);

            var mountCardsContainerWithPaginationViewModel = new MountCardsContainerWithPaginationViewModel()
            {
                Mounts = mounts.Select(m => new MountDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = string.IsNullOrWhiteSpace(m.Description) ? MountConstants.NoDescriptionAvailable : m.Description,
                    RentPrice = m.RentPrice,
                    ImageUrl = m.ImageUrl,
                    Type = m.MountType.ToString(),
                    Rating = m.Rating,
                }),
                Pagination = new PaginationViewModel()
                {
                    CurrentPage = 1,
                    TotalPages = (int)Math.Ceiling(totalResults / 6d),
                },
            };

            return new MountsViewModel()
            {
                ViewModel = mountCardsContainerWithPaginationViewModel,
            };
        }

        public async Task<ViewMountViewModel> CreateViewMountViewModelAsync(int id)
        {
            var mount = await _repository.FindByIdAsync<MountEntity>(id) ?? new();
            IEnumerable<MountEntity> recommendedMounts = await _repository.FindSpecificCountReadOnlyAsync<MountEntity>(MountConstants.RecommendedQuestsCount, m => m.Id != id && m.MountType == mount.MountType);

            return new ViewMountViewModel()
            {
                Mount = new MountDto()
                {
                    Id = mount.Id,
                    Name = mount.Name,
                    Description = string.IsNullOrWhiteSpace(mount.Description) ? MountConstants.NoDescriptionAvailable : mount.Description,
                    RentPrice = mount.RentPrice,
                    ImageUrl = mount.ImageUrl,
                    Type = mount.MountType.ToString(),
                    Rating = mount.Rating,
                },
                RecommendedMounts = recommendedMounts.Select(m => new MountDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = string.IsNullOrWhiteSpace(m.Description) ? MountConstants.NoDescriptionAvailable : m.Description,
                    RentPrice = m.RentPrice,
                    ImageUrl = m.ImageUrl,
                    Type = m.MountType.ToString(),
                    Rating = m.Rating,
                }),
            };
        }

        public async Task<MountRentHistoryViewModel> CreateMountRentHistoryViewModelAsync()
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User!);

            if (currentUser == null)
            {
                return new MountRentHistoryViewModel();
            }

            IEnumerable<MountRentalEntity> mountRentals = await _repository.FindAllReadOnlyAsync<MountRentalEntity>(mr => mr.UserId == currentUser.Id, mr => mr.Mount);

            return new MountRentHistoryViewModel()
            {
                MountRentals = mountRentals.Select(mr => new MountRentalDto()
                {
                    Id = mr.Id,
                    StartDate = mr.StartDate,
                    EndDate = mr.EndDate,
                    MountId = mr.MountId,
                    MountName = mr.Mount.Name,
                }),
            };
        }

        public async Task<MountCardsContainerWithPaginationViewModel> CreateMountCardsContainerWithPaginationViewModelAsync(FilterAndSortMountsRequestData filterAndSortMountsRequestData)
        {
            if (!((!filterAndSortMountsRequestData.StartDate.HasValue && !filterAndSortMountsRequestData.EndDate.HasValue) ||
                (filterAndSortMountsRequestData.StartDate.HasValue && filterAndSortMountsRequestData.EndDate.HasValue && filterAndSortMountsRequestData.StartDate.Value.Date >= DateTime.UtcNow.Date && filterAndSortMountsRequestData.StartDate <= filterAndSortMountsRequestData.EndDate)))
            {
                return new MountCardsContainerWithPaginationViewModel();
            }

            Expression<Func<MountEntity, bool>> dateIntervalExpression = filterAndSortMountsRequestData.StartDate.HasValue && filterAndSortMountsRequestData.EndDate.HasValue
                ? m => m.MountRentals.All(mr => filterAndSortMountsRequestData.EndDate <= mr.StartDate || filterAndSortMountsRequestData.StartDate >= mr.EndDate)
                : m => true;

            Expression<Func<MountEntity, bool>> searchExpression = string.IsNullOrWhiteSpace(filterAndSortMountsRequestData.SearchTerm)
                ? m => true
                : m => m.Name.Contains(filterAndSortMountsRequestData.SearchTerm) || (m.Description != null && m.Description.Contains(filterAndSortMountsRequestData.SearchTerm));

            string[] typeFilters = filterAndSortMountsRequestData.TypeFilters?.Split(',') ?? [];

            Expression<Func<MountEntity, bool>> typeFiltersExpression = typeFilters.Length != 0
                ? m => typeFilters.Select(f => (MountType)Enum.Parse(typeof(MountType), f)).Contains(m.MountType)
                : m => true;

            ParameterExpression parameter = Expression.Parameter(typeof(MountEntity), "m");
            Expression<Func<MountEntity, bool>> combinedFilterExpression = Expression.Lambda<Func<MountEntity, bool>>(
                Expression.AndAlso(
                    Expression.AndAlso(
                        Expression.Invoke(dateIntervalExpression, parameter),
                        Expression.Invoke(searchExpression, parameter)),
                    Expression.Invoke(typeFiltersExpression, parameter)),
                parameter);

            int totalFilteredAndSortedMountCount = await _repository.GetCountAsync(combinedFilterExpression);

            if (totalFilteredAndSortedMountCount == 0)
            {
                return new MountCardsContainerWithPaginationViewModel();
            }

            int pageToShow = Math.Min(totalFilteredAndSortedMountCount / MountConstants.CountForPagination + 1, filterAndSortMountsRequestData.CurrentPage);

            Expression<Func<MountEntity, object>> sortExpression = filterAndSortMountsRequestData.SortBy == "Price" ? m => m.RentPrice : m => m.Rating;

            var filteredAndSortedMounts = await _repository.FindSpecificCountOrderedReadOnlyAsync(pageToShow, MountConstants.CountForPagination, filterAndSortMountsRequestData.OrderIsDescending, sortExpression, combinedFilterExpression);

            return new MountCardsContainerWithPaginationViewModel()
            {
                Mounts = filteredAndSortedMounts.Select(m => new MountDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = string.IsNullOrWhiteSpace(m.Description) ? MountConstants.NoDescriptionAvailable : m.Description,
                    RentPrice = m.RentPrice,
                    ImageUrl = m.ImageUrl,
                    Type = m.MountType.ToString(),
                    Rating = m.Rating,
                }),
                Pagination = new PaginationViewModel()
                {
                    CurrentPage = pageToShow,
                    TotalPages = (int)Math.Ceiling(totalFilteredAndSortedMountCount / 6d),
                },
            };
        }

        public async Task<string> RentMountAsync(RentMountDto rentMountDto)
        {
            if (rentMountDto.StartDate > rentMountDto.EndDate)
            {
                return "Something went wrong";
            }

            ApplicationUser? currentUser = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User!);

            if (currentUser == null)
            {
                return "Something went wrong";
            }

            var mountToRent = await _repository.FindByIdAsync<MountEntity>(rentMountDto.MountId);

            if (mountToRent == null)
            {
                return "Something went wrong";
            }

            IEnumerable<MountRentalEntity> mountRentals = await _repository.FindAllReadOnlyAsync<MountRentalEntity>(mr => mr.MountId == mountToRent.Id);

            if (mountRentals.All(mr => rentMountDto.EndDate <= mr.StartDate || rentMountDto.StartDate >= mr.EndDate))
            {
                var newMountRental = new MountRentalEntity()
                {
                    StartDate = rentMountDto.StartDate,
                    EndDate = rentMountDto.EndDate,
                    UserId = currentUser.Id,
                    MountId = mountToRent.Id,
                };

                await _repository.AddAsync(newMountRental);
                await _repository.SaveChangesAsync();

                return "Mount rented successfully for the chosen interval";
            }
            else
            {
                return "The mount is not available for the chosen interval";
            }
        }

        public async Task<string> AddOrUpadeMountRatingAsync(RateMountDto rateMountDto)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User!);

            if (currentUser == null)
            {
                return "Something went wrong";
            }

            var mountToRate = await _repository.FindByIdAsync<MountEntity>(rateMountDto.MountId);

            if (mountToRate == null)
            {
                return "Something went wrong";
            }

            IEnumerable<MountRatingEntity> currentMountRatings = await _repository.FindAllAsync<MountRatingEntity>(mr => mr.MountId == rateMountDto.MountId);
            MountRatingEntity? usersCurrentMountRating = currentMountRatings.FirstOrDefault(mr => mr.UserId == currentUser.Id);

            if (usersCurrentMountRating != null)
            {
                usersCurrentMountRating.Rating = rateMountDto.Rating;
            }
            else
            {
                var newMountRating = new MountRatingEntity()
                {
                    Rating = rateMountDto.Rating,
                    UserId = currentUser.Id,
                    MountId = rateMountDto.MountId,
                };

                await _repository.AddAsync(newMountRating);
            }

            mountToRate.Rating = currentMountRatings.Average(mr => mr.Rating);

            await _repository.SaveChangesAsync();

            return "Mount rated successfully";
        }

        public async Task CancelMountRentAsync(int id)
        {
            MountRentalEntity? mountRentalToCancel = await _repository.FindByIdAsync<MountRentalEntity>(id);

            if (mountRentalToCancel != null)
            {
                _repository.Remove(mountRentalToCancel);
            }
        }

        public async Task CreateMountsAsync(CreateMountDto createMountDto)
        {
            var dateCreated = DateTime.Now;

            string imageUrl = string.Empty;

            if (createMountDto.Image != null)
            {
                string accessToken = await _dropboxService.GetAccessToken();

                if (!string.IsNullOrEmpty(accessToken))
                {
                    string uploadPath = string.Format(ImageUploadPath, createMountDto.Name, dateCreated.ToLongTimeString(), createMountDto.Image.FileName);
                    imageUrl = await _dropboxService.UploadImage(createMountDto.Image, uploadPath, accessToken);
                }
            }

            imageUrl = string.IsNullOrEmpty(imageUrl) ? DefaultMountTypeImageUrls[createMountDto.MountType] : imageUrl;

            var newMount = new MountEntity()
            {
                Name = createMountDto.Name,
                Description = createMountDto.Description,
                Created = dateCreated,
                RentPrice = createMountDto.RentPrice,
                MountType = (MountType)Enum.Parse(typeof(MountType), createMountDto.MountType),
                ImageUrl = imageUrl,
            };

            await _repository.AddAsync(newMount);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdateMountAsync(UpdateMountDto updateMountDto)
        {
            var mountToUpdate = await _repository.FindByIdAsync<MountEntity>(updateMountDto.Id);

            if (mountToUpdate != null)
            {
                mountToUpdate.Name = updateMountDto.Name;
                mountToUpdate.Description = updateMountDto.Description;
                mountToUpdate.RentPrice = updateMountDto.RentPrice;
                mountToUpdate.MountType = (MountType)Enum.Parse(typeof(MountType), updateMountDto.MountType);

                if (updateMountDto.Image != null)
                {
                    string accessToken = await _dropboxService.GetAccessToken();

                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        string uploadPath = string.Format(ImageUploadPath, updateMountDto.Name, mountToUpdate.Created.ToLongTimeString(), updateMountDto.Image.FileName);
                        string imageUrl = await _dropboxService.UploadImage(updateMountDto.Image, uploadPath, accessToken);

                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            mountToUpdate.ImageUrl = imageUrl;
                        }
                    }
                }

                _repository.Update(mountToUpdate);
                await _repository.SaveChangesAsync();
            }
        }

        public async Task DeleteMountAsync(int id)
        {
            var mountToRemove = await _repository.FindByIdAsync<MountEntity>(id);

            if (mountToRemove != null)
            {
                _repository.Remove(mountToRemove);
                await _repository.SaveChangesAsync();
            }
        }
    }
}
