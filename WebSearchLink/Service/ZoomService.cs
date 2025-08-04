using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using WebSearchLink.Models;
using System.IO;


namespace WebSearchLink.Service
{
    public class ZoomService : IZoomService
    {
        private readonly HttpClient _client;
        private readonly HttpClient _clientDownload;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly DbAba3d6Amsernest1234567Context _context;
        private readonly IWebHostEnvironment _env;
        public ZoomService(IHttpClientFactory factory, IConfiguration configuration, IMemoryCache memoryCache, DbAba3d6Amsernest1234567Context context, IWebHostEnvironment env)
        {
            _client = factory.CreateClient("Zoom");
            _clientDownload = factory.CreateClient("ZoomDownload");
            _configuration = configuration;
            _cache = memoryCache;
            _context = context;
            _env = env;
        }
        public async Task<ResponseModel<string>> GetAccessTokenZoom()
        {
            var clientId = _configuration["Zoom:ClientId"];
            var clientSecret = _configuration["Zoom:ClientSecret"];
            var accountId = _configuration["Zoom:AccountId"];
            var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            var response = await _client.PostAsync($"oauth/token?grant_type=account_credentials&account_id={accountId}", null);
            var json = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(json))
            {
                ResponseModel<string> responseModel = new()
                {
                    IsSussess = false,
                    Message = "Failed to retrieve access token."
                };
                return responseModel;
            }
            else
            {
                var tokenObj = JsonConvert.DeserializeObject<RoomTokenResponse>(json);
                if (tokenObj != null)
                {
                    string? token = tokenObj.access_token;
                    int? expires = tokenObj.expires_in;
                    string? type = tokenObj.token_type;
                    _cache.Set("zoom_access_token", token, TimeSpan.FromMinutes(55));
                    return new ResponseModel<string>
                    {
                        IsSussess = true,
                        Message = "Access token retrieved successfully.",
                        Data = token,
                        DataList = new List<string> { type ?? string.Empty, expires?.ToString() ?? string.Empty }
                    };
                }
                return new ResponseModel<string>
                {
                    IsSussess = false,
                    Message = "Failed to parse access token response."
                };
            }
        }
        public async Task<bool> IsRecordingExist(string fileId)
        {
            var isRecordingExist = await _context.RecordingFiles.AnyAsync(m => m.FileId == fileId);
            if (isRecordingExist)
            {
                return true; // recording already exists
            }
            return false;
        }
        public async Task<bool> IsMeetingExist(string UuId)
        {
            var isUuidExist = await _context.Meetings.AnyAsync(m => m.Uuid == UuId);
            if (isUuidExist)
            {
                return true; // Meeting already exists
            }
            return false;
        }
        public static string SanitizeFileName(string fileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName;
        }
        public async Task<ResponseModel<ZoomRecordingFile>> GetNewRecordingsAsync()
        {
            var isrefreshToken = await RefeshAccessTokenZoom();
            if (!isrefreshToken.IsSussess)
            {
                return new ResponseModel<ZoomRecordingFile>
                {
                    IsSussess = false,
                    Message = isrefreshToken.Message
                };
            }

            var accessToken = _cache.Get<string>("zoom_access_token");
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var toDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var fromDate = DateTime.UtcNow.AddMonths(-1).ToString("yyyy-MM-dd");

            var responseModel = new ResponseModel<ZoomRecordingFile>
            {
                IsSussess = true,
                Message = "Recordings retrieved successfully.",
                DataList = new List<ZoomRecordingFile>()
            };

            string? nextPageToken = null;

            do
            {
                var url = $"v2/accounts/me/recordings?from={fromDate}&to={toDate}&page_size=100";
                if (!string.IsNullOrEmpty(nextPageToken))
                    url += $"&next_page_token={nextPageToken}";

                var response = await _client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    break;

                var json = await response.Content.ReadAsStringAsync();
                var meetings = JsonConvert.DeserializeObject<ZoomRecordingListResponse>(json);
                if (meetings?.Meetings == null || meetings.Meetings.Count == 0)
                    break;

                foreach (var meeting in meetings.Meetings)
                {
                    if (meeting == null || string.IsNullOrEmpty(meeting.Uuid))
                        continue;

                    // Check if meeting already saved
                    if (await IsMeetingExist(meeting.Uuid) == false)
                    {
                        var detailRes = await _client.GetAsync($"v2/meetings/{meeting.Uuid}/recordings");
                        if (!detailRes.IsSuccessStatusCode)
                            continue;

                        var detailJson = await detailRes.Content.ReadAsStringAsync();
                        var detail = JsonConvert.DeserializeObject<ZoomMeetingRecordingDetail>(detailJson);

                        if (detail?.RecordingFiles != null)
                        {
                            await _context.Meetings.AddAsync(new Meeting
                            {
                                Uuid = meeting.Uuid,
                                Topic = detail.Topic,
                                StartTime = detail.StartTime
                            });

                            foreach (var file in detail.RecordingFiles)
                            {
                                if (file == null || string.IsNullOrEmpty(file.DownloadUrl) || file.FileType != "MP4")
                                    continue;

                                if (await IsRecordingExist(file.Id) == false)
                                {
                                    await _context.RecordingFiles.AddAsync(new RecordingFile
                                    {
                                        FileId = file.Id,
                                        FileName = file.FileType,
                                        DownloadUrl = file.DownloadUrl,
                                        MeetingUuid = meeting.Uuid,
                                        DownloadedAt = null,
                                        FileType = file.FileType
                                    });
                                }

                                responseModel.DataList.Add(new ZoomRecordingFile
                                {
                                    Id = file.Id,
                                    FileType = file.FileType,
                                    FileSize = file.FileSize,
                                    DownloadUrl = file.DownloadUrl,
                                    PlayUrl = file.PlayUrl,
                                    RecordingStart = file.RecordingStart,
                                    RecordingEnd = file.RecordingEnd
                                });
                            }
                        }
                    }
                }

                nextPageToken = meetings.NextPageToken;

            } while (!string.IsNullOrEmpty(nextPageToken));

            await _context.SaveChangesAsync();

            if (responseModel.DataList.Count == 0)
            {
                responseModel.IsSussess = false;
                responseModel.Message = "No recordings found.";
            }

            return responseModel;
        }
        public async Task<ResponseModel<string>> RefeshAccessTokenZoom()
        {
            if (_cache.TryGetValue("zoom_access_token", out object? cachedValue) && cachedValue is string accessToken)
            {
                return await Task.FromResult(new ResponseModel<string>
                {
                    IsSussess = true,
                    Message = "Access token is still valid.",
                    Data = accessToken
                });
            }
            else
            {
                var response = await GetAccessTokenZoom();
                if (response.IsSussess)
                {
                    // Cache the new access token
                    _cache.Set("zoom_access_token", response.Data, TimeSpan.FromMinutes(55));
                    return response;
                }
            }
            return new ResponseModel<string>
            {
                IsSussess = false,
                Message = "Failed to refresh access token."
            };
        }
        public async Task<ResponseModel<string>> SaveRecordingToServerAsync(string downloadUrl, string fileName)
        {
            try
            {
                var isrefreshToken = await RefeshAccessTokenZoom();
                if (!isrefreshToken.IsSussess)
                {
                    return new ResponseModel<string>
                    {
                        IsSussess = false,
                        Message = isrefreshToken.Message
                    };
                }
                // Check if the access token is cached
                var accessToken = _cache.Get<string>("zoom_access_token");

                fileName = SanitizeFileName(fileName);
                string savePath = Path.Combine(_env.WebRootPath, "videos");
                if (!Directory.Exists(savePath))
                    Directory.CreateDirectory(savePath);
                var separator = downloadUrl.Contains("?") ? "&" : "?";
                string fullDownloadUrl = $"{downloadUrl}{separator}access_token={accessToken}";
                var response = await _clientDownload.GetAsync(fullDownloadUrl, HttpCompletionOption.ResponseHeadersRead);
                if (response.IsSuccessStatusCode)
                {
                    long? fileSizeBytes = response.Content.Headers.ContentLength;
                    double fileSizeMB = fileSizeBytes.HasValue ? Math.Round(fileSizeBytes.Value / 1024.0 / 1024.0, 2) : 0;

                    // checkSizeFile
                    var videoPath = Path.Combine(_env.WebRootPath, "videos");
                    var checkSizeVideos = GetFolderSizeInMB(videoPath) + fileSizeMB;

                    if (checkSizeVideos >= 2600)
                    {
                        return new ResponseModel<string>
                        {
                            IsSussess = false,
                            Message = "File size exceeds the limit of 2.5GB."
                        };
                    }

                    string filePath = Path.Combine(savePath, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        await response.Content.CopyToAsync(fileStream);
                        await fileStream.FlushAsync();
                    }

                    await Task.Delay(300);


                    return new ResponseModel<string>
                    {
                        IsSussess = true,
                        Message = "Recording saved successfully.",
                        Data = filePath
                    };
                }
                else
                {
                    return new ResponseModel<string>
                    {
                        IsSussess = false,
                        Message = $"Failed to download recording. Status code: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel<string>
                {
                    IsSussess = false,
                    Message = $"Error saving recording to database: {ex.Message}"
                };
            }
        }
        public async Task<ResponseModel<string>> SaveNewRecordingsAsync()
        {
            var listLink = await _context.RecordingFiles.Include(m => m.MeetingUu)
                .Where(m => m.DownloadedAt == null && m.MeetingUu != null)
                .Select(m => new { m.DownloadUrl, Topic = m.MeetingUu!.Topic ?? string.Empty, FileId = m.FileId })
                .ToListAsync();
            List<string> listDownloadUrlsFail = new List<string>();

            var count = 0;
            foreach (var item in listLink)
            {
                if (!string.IsNullOrEmpty(item.DownloadUrl) && count < 5)
                {
                  
                    var checkSave = await SaveRecordingToServerAsync(item.DownloadUrl, $"{item.FileId}.mp4");

                    if (!checkSave.IsSussess)
                    {
                        listDownloadUrlsFail.Add(item.DownloadUrl);
                    }
                    else
                    {
                        var recordingFile = await _context.RecordingFiles.FirstOrDefaultAsync(m => m.DownloadUrl == item.DownloadUrl);
                        if (recordingFile != null)
                        {
                            recordingFile.DownloadedAt = DateTime.Now;
                            await _context.SaveChangesAsync();
                        }
                        count++;
                    }
                }
                if (count > 5) break;
            }

            return new ResponseModel<string>
            {
                IsSussess = true,
                Message = $"All recordings processed successfully. Total processed: {count}.",
                DataList = listDownloadUrlsFail
            };
        }

        public async Task<ResponseModel<ZoomMeetingReportResponses>> GetReportingFilesToWeekAsync()
        {
            var isrefreshToken = await RefeshAccessTokenZoom();
            if (!isrefreshToken.IsSussess)
            {
                return new ResponseModel<ZoomMeetingReportResponses>
                {
                    IsSussess = false,
                    Message = isrefreshToken.Message
                };
            }

            var accessToken = _cache.Get<string>("zoom_access_token");
            var to = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var from = DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-dd");

            List<string> userIdList = new List<string>();
            ZoomMeetingReportResponses result = new ZoomMeetingReportResponses
            {
                ReportResponses = new List<ZoomMeetingReportResponse>()
            };

            var users = await GetUserAsync();
            if (users.Data?.Users != null)
            {
                userIdList.AddRange(users.Data.Users
                    .Where(u => u.Status == "active")
                    .Select(u => u.Email!));
            }

            foreach (var userEmail in userIdList)
            {
                string? nextPageToken = null;
                do
                {
                    var requestUrl = $"v2/report/users/{userEmail}/meetings?from={from}&to={to}&page_size=100";
                    if (!string.IsNullOrEmpty(nextPageToken))
                    {
                        requestUrl += $"&next_page_token={nextPageToken}";
                    }

                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await _client.GetAsync(requestUrl);
                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        throw new Exception($"Zoom API error for user {userEmail}: {response.StatusCode} - {error}");
                    }

                    var json = await response.Content.ReadAsStringAsync();

                    try
                    {
                        var meetingReports = JsonConvert.DeserializeObject<ZoomMeetingReportResponses>(json);

                        if (meetingReports?.ReportResponses != null)
                        {
                            var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

                            foreach (var report in meetingReports.ReportResponses)
                            {
                                if (report.StartTime.HasValue)
                                {
                                    report.StartTime = TimeZoneInfo.ConvertTimeFromUtc(
                                        report.StartTime.Value,
                                        vnTimeZone
                                    );
                                }

                                if (report.EndTime.HasValue)
                                {
                                    report.EndTime = TimeZoneInfo.ConvertTimeFromUtc(
                                        report.EndTime.Value,
                                        vnTimeZone
                                    );
                                }
                            }
                        }
                        if (meetingReports?.ReportResponses != null)
                        {
                            result.ReportResponses.AddRange(meetingReports.ReportResponses);
                        }

                        nextPageToken = meetingReports?.NextPageToken;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error deserializing meeting reports for user {userEmail}: {ex.Message}\nRaw JSON: {json}");
                    }

                    // Delay nhẹ để tránh bị Zoom rate limit
                    await Task.Delay(200);

                } while (!string.IsNullOrEmpty(nextPageToken));
            }

            if (result.ReportResponses.Count == 0)
            {
                return new ResponseModel<ZoomMeetingReportResponses>
                {
                    IsSussess = false,
                    Message = "No meeting reports found."
                };
            }

            return new ResponseModel<ZoomMeetingReportResponses>
            {
                IsSussess = true,
                Message = $"Retrieved {result.ReportResponses.Count} meeting reports successfully.",
                Data = result
            };
        }

        public async Task<ResponseModel<ZoomUsers>> GetUserAsync()
        {
            var isrefreshToken = await RefeshAccessTokenZoom();
            if (!isrefreshToken.IsSussess)
            {
                return new ResponseModel<ZoomUsers>
                {
                    IsSussess = false,
                    Message = isrefreshToken.Message
                };
            }
            var accessToken = _cache.Get<string>("zoom_access_token");
            var requestUrl = $"v2/users";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _client.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Zoom API error: {response.StatusCode} - {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ZoomUsers>(json);
            if (result == null)
            {
                return new ResponseModel<ZoomUsers>
                {
                    IsSussess = false,
                    Message = "No user found."
                };
            }
            return new ResponseModel<ZoomUsers>
            {
                IsSussess = true,
                Message = "user found.",
                Data = result
            }
            ;
        }
        public async Task DownloadReportFormZoomToDbAsync()
        {
            var reports = await GetReportingFilesToWeekAsync();
            if (!reports.IsSussess || reports.Data?.ReportResponses == null)
            {
                throw new Exception(reports.Message ?? "Failed to retrieve reports.");
            }
            foreach (var report in reports.Data.ReportResponses)
            {
                if (report.Uuid == null)
                {
                    throw new Exception("Report UUID is null.");
                }
                var existingReport = await _context.ZoomMeetingReports
                   .AnyAsync(r => r.Uuid == report.Uuid);
                if (existingReport) continue;

                if (string.IsNullOrEmpty(report.Topic))
                {
                    throw new Exception("Report Topic is null or empty.");
                }
                string result = StringHelper.NormalizeVietnameseName(report.Topic);
                int newId = 0;

                var teacherList = await _context.Teachers
                 .Where(t => t.Department != null)
                 .ToListAsync();

                var existingTeacher = teacherList
                        .FirstOrDefault(t => StringHelper.NormalizeVietnameseName(t.Department ?? string.Empty) == result);

                if (existingTeacher != null)
                {
                    newId = existingTeacher.TeacherId;
                }
                else
                {
                    var teacher = new Teacher
                    {
                        FullName = report.Topic.Split('|')[0].Trim(),
                        Email = report.UserEmail ?? string.Empty,
                        Type = StringHelper.DetectTypeFromTopic(report.Topic),
                        Department = result
                    };

                    await _context.Teachers.AddAsync(teacher);
                    await _context.SaveChangesAsync();

                    newId = teacher.TeacherId;
                }
                int typeTeacher = StringHelper.DetectTypeFromTopic(report.Topic);
                await _context.AddAsync(new ZoomMeetingReport
                {
                    Uuid = report.Uuid,
                    Id = report.Id,
                    Topic = report.Topic,
                    Type = report.Type,
                    UserEmail = report.UserEmail,
                    UserName = report.UserName,
                    StartTime = report.StartTime,
                    EndTime = report.EndTime,
                    Duration = report.Duration,
                    ParticipantsCount = report.ParticipantsCount,
                    TotalMinutes = report.TotalMinutes,
                    HostId = report.HostId,
                    Source = report.Source,
                    TeacherId = newId,
                    TypeTeacher = typeTeacher
                });
                await _context.SaveChangesAsync();
            }
        }

        public static double GetFolderSizeInMB(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                return 0;

            long totalBytes = 0;
            DirectoryInfo dirInfo = new DirectoryInfo(folderPath);

            foreach (FileInfo file in dirInfo.GetFiles("*", SearchOption.AllDirectories))
            {
                totalBytes += file.Length;
            }

            double sizeInMB = totalBytes / (1024.0 * 1024.0);
            return Math.Round(sizeInMB, 2);
        }
    }
}
