using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using SteamProject.DAL.Abstract;
using SteamProject.DAL.Concrete;
using SteamProject.Models;

namespace SteamProject.DAL.Concrete;

public class CompetitionRepository : Repository<Competition>,  ICompetitionRepository
{
    private readonly ICompetitionPlayerRepository _competitionPlayerRepository;
    private readonly ICompetitionVoteRepository _competitionVoteRepository;
    private readonly IUserGameInfoRepository _userGameInfoRepository;
    private readonly IUserRepository _userRepository;
    private readonly SteamInfoDbContext _ctx;

    public CompetitionRepository(SteamInfoDbContext ctx, ICompetitionPlayerRepository competitionPlayerRepository, ICompetitionVoteRepository competitionVoteRepository, IUserGameInfoRepository userGameInfoRepository, IUserRepository userRepository) : base(ctx)
    {
        _competitionPlayerRepository = competitionPlayerRepository;
        _ctx = ctx;
        _competitionVoteRepository = competitionVoteRepository;
        _userGameInfoRepository = userGameInfoRepository;
        _userRepository = userRepository;
    }

    public Competition GetCompetitionById(int id)
    {
        return GetAll().Where( c => c.Id == id ).FirstOrDefault();
    }

    public Competition GetCompetitionByCompPlayerAndGameId( CompetitionPlayer player, int gameId )
    {
        return GetAll().Where( c => c.GameId == gameId && c.CompetitionPlayers.Contains( player ) ).FirstOrDefault();
    }

    public List<Competition> GetAllCompetitionsForUser( List<CompetitionPlayer> entries )
    {
        if ( entries == null || entries.Count() == 0 )
            return null;
        
        var returnMe = new List<Competition>();
        foreach( var competitionPlayer in entries )
        {
            var CompsFound = new List<Competition>();
            CompsFound = GetAll().Where( c => c.CompetitionPlayers.Contains( competitionPlayer ) ).ToList<Competition>();
            foreach( var comp in CompsFound )
                returnMe.Add( comp );
        }

        return returnMe;
    }

    public List<Competition> GetCurrentCompetitionsBySteamId(string steamId)
    {
        // Get all competition players for the user
        var competitionPlayers = _competitionPlayerRepository.GetCompetitionIdsBySteamId(steamId);

        if (competitionPlayers == null)
        {
            return null;
        }

        // Get the current date
        var currentDate = DateTime.Now;

        // Get all competitions for the competition players that are currently ongoing
        return GetAllCompetitionsForUser(competitionPlayers)
            .Where(c => c.StartDate <= currentDate && c.EndDate >= currentDate)
            .ToList();
    }

    public List<Competition> GetPreviousCompetitionsBySteamId(string steamId)
    {
        // Get all competition players for the user
        var competitionPlayers = _competitionPlayerRepository.GetCompetitionIdsBySteamId(steamId);

        if (competitionPlayers == null)
        {
            return null;
        }

        // Get the current date
        var currentDate = DateTime.Now;

        // Get all competitions for the competition players that have ended
        return GetAllCompetitionsForUser(competitionPlayers)
            .Where(c => c.EndDate < currentDate)
            .ToList();
    }
    public int GetTotalUsers(int competitionId)
    {
        // Get the competition by its Id
        var competition = GetCompetitionById(competitionId);

        // If no such competition exists, return 0 (or throw an exception, if that's more appropriate in your case)
        if (competition == null)
        {
            return 0;
        }

        // Return the count of CompetitionPlayers
        return competition.CompetitionPlayers.Count;
    }

    
    public bool HasVoteSucceeded(int competitionId)
    {
        int totalUsers = GetTotalUsers(competitionId);
        int positiveVotes = _competitionVoteRepository.GetPositiveVotesCount(competitionId);

        // If the number of positive votes is greater than or equal to half the number of users, return true
        return (positiveVotes > totalUsers / 2.0);
    }

    public IEnumerable<Game> GetSharedGames(int competitionId)
    {
        // Get the competition by its Id
        var competition = GetCompetitionById(competitionId);

        // If no such competition exists, throw an exception
        if (competition == null)
        {
            throw new Exception($"Competition with ID {competitionId} not found");
        }

        // Get the players in the competition
        var players = competition.CompetitionPlayers;

        // Fetch all games for each player
        List<List<Game>> allPlayerGames = new List<List<Game>>();
        foreach (var player in players)
        {
            if (player.SteamId != null)
            {
                // Get the user for the current player
                var user = _userRepository.GetUserBySteamId(player.SteamId);


                // If no such user exists, throw an exception
                if (user == null)
                {
                    allPlayerGames.Add(new List<Game>());
                }
                else
                {
                    // Fetch the games for the user
                    var userGamesInfo = _userGameInfoRepository.GetAllUserGameInfo(user.Id);

                    // Convert the UserGameInfo list to a Game list
                    var playerGames = userGamesInfo.Select(ugi => ugi.Game).ToList();

                    allPlayerGames.Add(playerGames);
                }
            }
            
        }

        // Find the intersection of all game lists
        var sharedGames = allPlayerGames
            .Skip(1)
            .Aggregate(new HashSet<Game>(allPlayerGames.First()), (h, e) => { h.IntersectWith(e); return h; });

        return sharedGames;
    }
    public Competition UpdateGameForCompetition(int competitionId, int newGameId)
    {
        // Get the competition by its Id
        var competition = GetCompetitionById(competitionId);

        // If no such competition exists, throw an exception
        if (competition == null)
        {
            throw new Exception($"Competition with ID {competitionId} not found");
        }

        // Calculate the original competition duration
        TimeSpan originalDuration = competition.EndDate - competition.StartDate;

        // Update the game Id
        competition.GameId = newGameId;

        // Reset the start and end dates
        competition.StartDate = DateTime.Now;
        competition.EndDate = competition.StartDate + originalDuration;

        // Reset the status
        competition.StatusId = 1;

        // Reset CompetitionGameAchievements, CompetitionPlayers, and CompetitionVotes
        competition.CompetitionVotes = new List<CompetitionVote>();

        // Reassign the existing players to the new competition
        var currentPlayers = competition.CompetitionPlayers;
        competition.CompetitionPlayers = new List<CompetitionPlayer>();
        foreach (var player in currentPlayers)
        {
            competition.CompetitionPlayers.Add(player);
        }

        // Save changes
        _ctx.SaveChanges();

        return competition;
    }






}