using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebSearchLink.Models;
using System.IO;

namespace WebSearchLink.Service
{
    public class YouTubeUploadService : IYouTubeService
    {
        private readonly HttpClient _client;
        private readonly DbAba3d6Amsernest1234567Context _context;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        public YouTubeUploadService(IHttpClientFactory factory, DbAba3d6Amsernest1234567Context context, IConfiguration config, IWebHostEnvironment env)
        {
            _client = factory.CreateClient("Zoom");
            _context = context;
            _config = config;
            _env = env;
        }
        public async Task<TokenResponse> GetAccessToken(GoogleToken acc)
        {
            var token = await RefreshToken(acc);
            if (token.IsSussess && token.Data != null)
            {
                return token.Data;
            }
            else
            {
                throw new Exception(token.Message ?? "Failed to refresh token");
            }
        }
        public async Task<ResponseModel<TokenResponse>> RefreshToken(GoogleToken acc)
        {
            var tokenEntry = await _context.GoogleTokens.FirstOrDefaultAsync(x => x.UserId == acc.UserId);
            if (tokenEntry == null || string.IsNullOrEmpty(tokenEntry.RefreshToken))
            {
                return new ResponseModel<TokenResponse>
                {
                    IsSussess = false,
                    Message = "Token entry not found or refresh token is empty",
                    Data = null
                };
            }

            var tokenExpiryTime = tokenEntry.IssuedAt.AddSeconds(tokenEntry.ExpiresIn ?? 0);
            var clientSecrets = new ClientSecrets
            {
                ClientId = acc.ClientId,
                ClientSecret = acc.ClientSecret
            };

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = clientSecrets
            });

            var token = new TokenResponse
            {
                RefreshToken = tokenEntry.RefreshToken
            };

            var newToken = await flow.RefreshTokenAsync(acc.UserId, token.RefreshToken, CancellationToken.None);

            // Update the new access_token  
            tokenEntry.AccessToken = newToken.AccessToken;
            tokenEntry.ExpiresIn = (int?)newToken.ExpiresInSeconds;
            tokenEntry.IssuedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ResponseModel<TokenResponse>
            {
                IsSussess = true,
                Message = "Token refreshed successfully",
                Data = newToken
            };
        }
        //public async Task<UserCredential> GetUserCredentialFromToken()
        //{
        //    var token = await GetAccessToken(); 

        //    var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        //    {
        //        ClientSecrets = new ClientSecrets
        //        {
        //            ClientId = _config["Google:ClientId"],
        //            ClientSecret = _config["Google:ClientSecret"]
        //        }
        //    });

        //    return new UserCredential(flow, "duccdung999@gmail.com", token);
        //}

        public async Task<UserCredential> GetUserCredential(GoogleToken account)
        { 
            var token = await GetAccessToken(account);

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = account.ClientId,
                    ClientSecret = account.ClientSecret
                }
            });

            return new UserCredential(flow, account.UserId, token);
        }
        //public async Task<string> UploadVideoAsync(string filePath, string title, string description)
        //{
        //    try
        //    {
        //        var credential = await GetUserCredentialFromToken(); 

        //        var youtubeService = new YouTubeService(new BaseClientService.Initializer
        //        {
        //            HttpClientInitializer = credential,
        //            ApplicationName = "Server-ToolDow&UpVideo"
        //        });

        //        var video = new Video
        //        {
        //            Snippet = new VideoSnippet
        //            {
        //                Title = title,
        //                Description = description,
        //                Tags = new[] { "asp.net", "upload", "youtube" },
        //                CategoryId = "22" // 22 = People & Blogs
        //            },
        //            Status = new VideoStatus { PrivacyStatus = "unlisted" }
        //        };

        //        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        //        var uploadRequest = youtubeService.Videos.Insert(video, "snippet,status", stream, "video/*");
        //        uploadRequest.ChunkSize = ResumableUpload.MinimumChunkSize;
        //        var progress = await uploadRequest.UploadAsync();

        //        if (progress.Status == Google.Apis.Upload.UploadStatus.Completed)
        //        {
        //            var videoId = uploadRequest.ResponseBody.Id;
        //            return $"https://www.youtube.com/watch?v={videoId}";
        //        }
        //        else
        //        {
        //            Console.WriteLine("Upload thất bại: " + progress.Exception?.Message);
        //            return "";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Lỗi khi upload video: {ex.Message}");
        //        return "";
        //    }
        //}
        public async Task<string> UploadVideoWithFallbackAsync(string filePath, string title, string description)
        {
            var accounts = await _context.GoogleTokens.ToListAsync();

            foreach (var account in accounts)
            {
                try
                {
                    var credential = await GetUserCredential(account); 
                    var youtubeService = new YouTubeService(new BaseClientService.Initializer
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = "MultiAccountUploader"
                    });

                    var video = new Video
                    {
                        Snippet = new VideoSnippet
                        {
                            Title = title,
                            Description = description,
                            Tags = new[] { "upload", "youtube", "asp.net" },
                            CategoryId = "22"
                        },
                        Status = new VideoStatus { PrivacyStatus = "unlisted" }
                    };

                    using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    var uploadRequest = youtubeService.Videos.Insert(video, "snippet,status", stream, "video/*");
                    uploadRequest.ChunkSize = ResumableUpload.MinimumChunkSize;
                    var progress = await uploadRequest.UploadAsync();

                    if (progress.Status == UploadStatus.Completed)
                    {
                        return $"https://www.youtube.com/watch?v={uploadRequest.ResponseBody.Id}";
                    }
                    else
                    {
                        if (IsQuotaError(progress.Exception)) continue;  
                        else throw progress.Exception;  
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Upload failed (Account: {account.UserId}): {ex.Message}");
                    if (IsQuotaError(ex)) continue;
                    else throw;
                }
            }

            return "";  
        }

        public async Task<List<string>> UploadAllVideosInFolderAsync()
        {
            var uploadResults = new List<string>();
            var videoFolder = Path.Combine(_env.WebRootPath, "videos");

            if (!Directory.Exists(videoFolder))
                return new List<string> { "Folder 'videos' no exist in wwwroot." };

            var files = Directory.GetFiles(videoFolder, "*.mp4");

            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                string fileId = Path.GetFileNameWithoutExtension(file);
                var infoUpload = await _context.RecordingFiles.Include(x => x.MeetingUu).FirstOrDefaultAsync(x => x.FileId == fileId);
                try
                {
                    if (infoUpload?.MeetingUu?.Topic != null && !infoUpload.Condition)
                    {
                        var result = await UploadVideoWithFallbackAsync(file, infoUpload.MeetingUu.Topic + " Ngày: " + infoUpload.MeetingUu.StartTime, $"Upload file {fileName}");
                        if (!string.IsNullOrEmpty(result))
                        {
                            infoUpload.Condition = true;
                            infoUpload.Url = result;
                            uploadResults.Add($"{fileName}: {result}");
                            File.Delete(file);
                        }
                        else
                        {
                            uploadResults.Add($"{fileName}: false");
                        }
                    }
                }
                catch (Exception ex)
                {
                    uploadResults.Add($"{fileName}:  Error - {ex.Message}");
                }
            }
            await _context.SaveChangesAsync();
            return uploadResults;
        }
        private bool IsQuotaError(Exception ex)
        {
            if (ex is Google.GoogleApiException apiEx)
            {
                return apiEx.Error?.Errors?.Any(e =>
                    e.Reason == "quotaExceeded" ||
                    e.Reason == "userRateLimitExceeded" ||
                    e.Reason == "dailyLimitExceeded") == true;
            }

            return false;
        }

    }
}
