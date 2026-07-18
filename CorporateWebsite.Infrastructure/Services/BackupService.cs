using CorporateWebsite.Core.DTOs;
using CorporateWebsite.Core.Entities;
using CorporateWebsite.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CorporateWebsite.Infrastructure.Services;

public class BackupService : IBackupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly string _backupPath;

    public BackupService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _backupPath = configuration["Backup:Path"] ?? Path.Combine(Directory.GetCurrentDirectory(), "backups");
        
        if (!Directory.Exists(_backupPath))
        {
            Directory.CreateDirectory(_backupPath);
        }
    }

    public async Task<BackupDto> CreateBackupAsync(string name, string type = "Full", string? initiatedBy = null, bool isAutomatic = false)
    {
        var backup = new Backup
        {
            Name = name,
            Type = type,
            Status = "InProgress",
            InitiatedBy = initiatedBy,
            IsAutomatic = isAutomatic,
            StartedAt = DateTime.UtcNow,
            RetentionDays = _configuration.GetValue<int>("Backup:RetentionDays", 30),
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Backups.AddAsync(backup);
        await _unitOfWork.SaveChangesAsync();

        try
        {
            var fileName = $"backup_{backup.Id}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.zip";
            var filePath = Path.Combine(_backupPath, fileName);
            
            // Create backup based on type
            await CreateBackupFileAsync(filePath, type);
            
            var fileInfo = new FileInfo(filePath);
            backup.FilePath = filePath;
            backup.FileSize = fileInfo.Length;
            backup.Status = "Completed";
            backup.CompletedAt = DateTime.UtcNow;
            
            // Calculate checksum
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            using var stream = File.OpenRead(filePath);
            backup.Checksum = BitConverter.ToString(await sha256.ComputeHashAsync(stream)).Replace("-", "").ToLower();
        }
        catch (Exception ex)
        {
            backup.Status = "Failed";
            backup.ErrorMessage = ex.Message;
            backup.CompletedAt = DateTime.UtcNow;
        }

        await _unitOfWork.Backups.UpdateAsync(backup);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<BackupDto>(backup);
    }

    private async Task CreateBackupFileAsync(string filePath, string type)
    {
        using var zip = new System.IO.Compression.ZipArchive(File.Create(filePath), System.IO.Compression.ZipArchiveMode.Create);
        
        // Backup database
        if (type == "Full" || type == "Database")
        {
            // This would typically use pg_dump, mysqldump, or sqlcmd
            // For now, we'll create a placeholder
            var dbEntry = zip.CreateEntry("database.sql");
            using var dbStream = dbEntry.Open();
            using var writer = new StreamWriter(dbStream);
            await writer.WriteAsync("-- Database backup placeholder\n");
            await writer.WriteAsync($"-- Generated at: {DateTime.UtcNow}\n");
            await writer.WriteAsync($"-- Type: {type}\n");
        }

        // Backup files
        if (type == "Full" || type == "Files")
        {
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (Directory.Exists(uploadsPath))
            {
                var files = Directory.GetFiles(uploadsPath, "*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    var relativePath = file.Substring(uploadsPath.Length + 1).Replace("\\", "/");
                    var entry = zip.CreateEntry($"uploads/{relativePath}");
                    using var entryStream = entry.Open();
                    using var fileStream = File.OpenRead(file);
                    await fileStream.CopyToAsync(entryStream);
                }
            }
        }

        // Backup configuration
        if (type == "Full" || type == "Config")
        {
            var configEntry = zip.CreateEntry("config.json");
            using var configStream = configEntry.Open();
            using var writer = new StreamWriter(configStream);
            var config = new
            {
                BackupDate = DateTime.UtcNow,
                Type = type,
                Version = "1.0"
            };
            await writer.WriteAsync(JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
        }
    }

    public async Task<BackupDto?> GetByIdAsync(int id)
    {
        var backup = await _unitOfWork.Backups.GetByIdAsync(id);
        return backup != null ? _mapper.Map<BackupDto>(backup) : null;
    }

    public async Task<IReadOnlyList<BackupDto>> GetAllAsync()
    {
        var backups = await _unitOfWork.Backups.GetAllAsync(q => q.OrderByDescending(b => b.CreatedAt));
        return _mapper.Map<IReadOnlyList<BackupDto>>(backups);
    }

    public async Task<bool> RestoreAsync(int id)
    {
        var backup = await _unitOfWork.Backups.GetByIdAsync(id);
        if (backup == null || backup.Status != "Completed") return false;

        try
        {
            using var zip = System.IO.Compression.ZipFile.OpenRead(backup.FilePath);
            
            // Restore database
            var dbEntry = zip.GetEntry("database.sql");
            if (dbEntry != null)
            {
                // This would typically restore the database
                // For now, just log
            }

            // Restore files
            foreach (var entry in zip.Entries.Where(e => e.FullName.StartsWith("uploads/")))
            {
                var targetPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", entry.FullName);
                Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
                entry.ExtractToFile(targetPath, true);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task DeleteAsync(int id)
    {
        var backup = await _unitOfWork.Backups.GetByIdAsync(id);
        if (backup == null) return;

        if (File.Exists(backup.FilePath))
        {
            File.Delete(backup.FilePath);
        }

        await _unitOfWork.Backups.DeleteAsync(backup);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<Stream> DownloadAsync(int id)
    {
        var backup = await _unitOfWork.Backups.GetByIdAsync(id);
        if (backup == null || !File.Exists(backup.FilePath))
        {
            throw new FileNotFoundException("Backup file not found.");
        }

        return new FileStream(backup.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public async Task CleanupOldBackupsAsync(int retentionDays = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);
        var oldBackups = await _unitOfWork.Backups.GetAllAsync(b => b.CreatedAt < cutoffDate && b.Status == "Completed");

        foreach (var backup in oldBackups)
        {
            await DeleteAsync(backup.Id);
        }
    }

    public async Task<BackupStatusDto> GetStatusAsync()
    {
        var backups = await _unitOfWork.Backups.GetAllAsync();
        var lastBackup = backups.OrderByDescending(b => b.CreatedAt).FirstOrDefault();
        
        var totalSize = backups.Where(b => b.Status == "Completed").Sum(b => b.FileSize);
        
        // Get available disk space
        var driveInfo = new DriveInfo(Path.GetPathRoot(_backupPath) ?? "C:\\");
        var availableSpace = driveInfo.AvailableFreeSpace;

        return new BackupStatusDto
        {
            TotalBackups = backups.Count(b => b.Status == "Completed"),
            TotalSize = totalSize,
            LastBackupAt = lastBackup?.CreatedAt,
            LastBackupStatus = lastBackup?.Status,
            NextScheduledBackup = DateTime.UtcNow.AddDays(1), // Would come from scheduled tasks
            AvailableDiskSpace = (int)(availableSpace / (1024 * 1024)) // MB
        };
    }
}