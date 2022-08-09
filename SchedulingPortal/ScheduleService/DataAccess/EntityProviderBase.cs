using ProxyReference;
using ScheduleService.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScheduleService.DataAccess
{
    public abstract class EntityProviderBase
    {
        protected IEnumerable<enuResultCodes> EntityNotFoundCodes => new[]
        {
            enuResultCodes.ClubNotFound,
            enuResultCodes.LeagueNotFound,
            enuResultCodes.OrganizationNotFound,
            enuResultCodes.TeamNotFound,
            enuResultCodes.TournamentNotFound,
            enuResultCodes.VenueNotFound,
            enuResultCodes.AgeGroupNotFound,
            enuResultCodes.GameNotFound,
        };

        protected IEnumerable<enuResultCodes> NoMatchingItemCodes => new[]
        {
            enuResultCodes.NoMatchingGames,
            enuResultCodes.NoMatchingStandings,
            enuResultCodes.NoMatchingTeams,
            enuResultCodes.NoMatchingTournaments,
            enuResultCodes.NoMatchingVenues,
        };

        protected T ProcessResponse<T>(
            string apiMethodName,
            BaseResponse response,
            T result)
        {
            if (response == null)
            {
                throw new AffinityServerErrorException($"{apiMethodName} call returned null.");
            }

            if (response.resultCode != enuResultCodes.Success)
            {
                if (EntityNotFoundCodes.Contains(response.resultCode))
                {
                    throw new AffinityNotFoundException(
                        $"{apiMethodName} not found due to error code {response.resultCode}.");
                }

                if (NoMatchingItemCodes.Contains(response.resultCode))
                {
                    return result;
                }

                throw new AffinityServerErrorException(
                    $"{apiMethodName} call returned error code {response.resultCode}.");
            }

            if (result == null)
            {
                throw new AffinityServerErrorException(
                    $"{apiMethodName} call returned null in result field!");
            }

            return result;
        }

        protected IEnumerable<T> ProcessArrayResponse<T>(
            string apiMethodName,
            BaseResponse response,
            IEnumerable<T> result)
        {
            var processedResult = ProcessResponse(apiMethodName, response, result);

            return processedResult ?? Array.Empty<T>();
        }
    }
}
