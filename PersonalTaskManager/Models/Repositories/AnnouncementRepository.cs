using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.SignalR;
using PersonalTaskManager.Models;
using PersonalTaskManager.Hubs;
using  PersonalTaskManager.Models.Repositories;
using Dapper;

public class AnnouncementRepository : IAnnouncementRepository
{ 
     private readonly string _connString;
    private readonly IHubContext<AnnouncementHub> _hubContext;

    public AnnouncementRepository(IConfiguration configuration, IHubContext<AnnouncementHub> hubContext)
    {
        _connString = configuration.GetConnectionString("DefaultConnection");
        _hubContext = hubContext;
    }
    public async Task<IEnumerable<Announcement>> GetAllAsync()
    {
       using (var conn = new SqlConnection(_connString))
        {
            string sql = "SELECT * FROM Announcements ORDER BY CreatedAt DESC";
            return await conn.QueryAsync<Announcement>(sql);
        }
    }

    public async Task CreateAsync(string title, string message)
    {
        using var conn = new SqlConnection(_connString);
        string sql = @"
            INSERT INTO Announcements (Title, Message, CreatedAt)
            VALUES (@Title, @Message, @CreatedAt)";
        var announcement = new Announcement { Title = title, Message = message, CreatedAt = DateTime.UtcNow };
        await conn.ExecuteAsync(sql, announcement);

        // Send the new announcement to all connected clients
        await _hubContext.Clients.All.SendAsync("ReceiveAnnouncement", announcement);
    }
}