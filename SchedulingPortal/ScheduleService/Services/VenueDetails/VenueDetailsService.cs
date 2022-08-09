using AutoMapper;
using Common.Measurement;
using Microsoft.Extensions.Logging;
using Model.Dto.VenueDetails;
using ProxyReference;
using ScheduleService.DataAccess;
using System;
using System.Threading.Tasks;

namespace ScheduleService.Services.VenueDetails
{
    public class VenueDetailsService : IVenueDetailsService
    {
        private readonly ITournamentVenueDetailsProvider _venueDetailsProvider;
        private readonly IMapper _mapper;
        private readonly ILogger<VenueDetailsService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="VenueDetailsService"/> class.
        /// </summary>
        /// <param name="venueDetailsProvider">Venue details provider.</param>
        /// <param name="mapper">Mapper to convert into DTOs.</param>
        /// <param name="logger">Logger.</param>
        public VenueDetailsService(
            ITournamentVenueDetailsProvider venueDetailsProvider,
            IMapper mapper,
            ILogger<VenueDetailsService> logger)
        {
            _venueDetailsProvider = venueDetailsProvider ?? throw new ArgumentNullException(nameof(venueDetailsProvider));
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Gets venue details data by organizationId and venueId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="venueId">Key of the venue.</param>
        /// <returns>Venue details mapped data.</returns>
        public async Task<VenueDetailsDto> GetVenueDetailsServiceAsync(string organizationId, string venueId)
        {
            using var performanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.VenueDetailsMeasurement,
                nameof(_venueDetailsProvider.GetVenueDetailsAsync));
            TournamentVenue tournamentVenue = await _venueDetailsProvider.GetVenueDetailsAsync(organizationId, venueId);
            performanceMeter.Stop();
            return _mapper.Map<VenueDetailsDto>(tournamentVenue);
        }
    }
}
