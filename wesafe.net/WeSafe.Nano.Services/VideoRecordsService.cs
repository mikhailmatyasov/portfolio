using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using WeSafe.Nano.Services.Abstraction.Abstraction.Services;
using WeSafe.Nano.Services.Abstraction.Models;

namespace WeSafe.Nano.Services
{
    public class VideoRecordsService : IVideoRecordsService
    {
        private readonly string videoFolderPath;
        private const string _datePattern = "yyyy-MM-dd-hh-mm-ss";
        private const string _contentFolderName = "Content";
        private const string _videoFolderName = "Video";
        private const int _maxRecordsNumber = 50;
        private CultureInfo _dateProvider = CultureInfo.InvariantCulture;

        public VideoRecordsService()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var contentFolderPath = Directory.GetDirectories(currentDirectory, _contentFolderName).FirstOrDefault();

            if (contentFolderPath == null)
                throw new InvalidOperationException("Content folder is not found.");

            videoFolderPath = Directory.GetDirectories(contentFolderPath, _videoFolderName).FirstOrDefault();

            if (videoFolderPath == null)
                throw new InvalidOperationException("Video folder is not found.");
        }

        public IEnumerable<string> GetVideoFilesPaths(VideoRecordSearchModel videoRecordSearchModel)
        {
            var cameraFolders = GetCameraFolders(videoRecordSearchModel?.CameraId);
            if (cameraFolders == null)
                return null;

            return cameraFolders.SelectMany(c => GetDateFolders(c, videoRecordSearchModel?.StartDateTime, videoRecordSearchModel?.EndDateTime))
                       .Take(_maxRecordsNumber).ToList();
        }

        public string GetVideoFile(int cameraId, DateTime eventDate)
        {
            var cameraFolderPath = GetCameraFolders(cameraId).FirstOrDefault();
            if (cameraFolderPath == null)
                throw new InvalidOperationException("Video for this event is not found.");

            var eventVideo = GetVideoFileByDate(cameraFolderPath, eventDate);
            var startEventTime = GetVideoStartTime(eventVideo, eventDate);

            return $"{eventVideo}#t={startEventTime}";
        }

        private IEnumerable<string> GetCameraFolders(int? cameraId)
        {
            var cameraFolders = Directory.GetDirectories(videoFolderPath).ToList();

            if (cameraId != null)
                cameraFolders = cameraFolders.Where(f => f.Contains(cameraId.ToString())).ToList();

            return cameraFolders;
        }

        private IEnumerable<string> GetDateFolders(string cameraFolderPath, DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var filesPaths = Directory.GetFiles(cameraFolderPath).Where(f => IsMatchesDateFilter(f, startDate, endDate)).ToList();

            return filesPaths.Select(p => Path.GetRelativePath(Directory.GetCurrentDirectory(), p));
        }

        private bool IsMatchesDateFilter(string videoFilePath, DateTime? startDate = null, DateTime? endDate = null)
        {
            var videoRecordStartDateString = Path.GetFileNameWithoutExtension(videoFilePath).Split("_").First();
            var recordStartDate = DateTime.ParseExact(videoRecordStartDateString, _datePattern, _dateProvider);

            return (startDate == null || recordStartDate > startDate) && (endDate == null || recordStartDate < endDate);
        }

        private string GetVideoFileByDate(string cameraFolderPath, DateTime eventDate)
        {
            var videoFiles = Directory.GetFiles(cameraFolderPath).ToList();

            var eventVideo = videoFiles.FirstOrDefault(v => IsEventInTheVideo(v, eventDate));
            if(eventVideo == null)
                throw new InvalidOperationException("Video for this event is not found.");

            return Path.GetRelativePath(Directory.GetCurrentDirectory(), eventVideo);
        }

        private bool IsEventInTheVideo(string videoFilePath, DateTime eventDate)
        {
            if (eventDate == default)
                throw new InvalidOperationException("Invalid date time");

            var videoRecordDates = GetVideoRecordDates(videoFilePath);
            var recordStartDate = DateTime.ParseExact(videoRecordDates[0], _datePattern, _dateProvider);
            var recordEndDate = DateTime.ParseExact(videoRecordDates[1], _datePattern, _dateProvider);

            return recordStartDate < eventDate && recordEndDate > eventDate;
        }

        private string GetVideoStartTime(string videoFilePath, DateTime eventDate)
        {
            var videoRecordDates = GetVideoRecordDates(videoFilePath);
            var recordStartDate = DateTime.ParseExact(videoRecordDates[0], _datePattern, _dateProvider);

            return eventDate.Subtract(recordStartDate).TotalSeconds.ToString();
        }

        private string[] GetVideoRecordDates(string videoFilePath)
        {
            var videoRecordDates = Path.GetFileNameWithoutExtension(videoFilePath).Split("_");
            if (videoRecordDates.Length != 2)
                throw new InvalidOperationException($"Video file {videoFilePath} has invalid name.");

            return videoRecordDates;
        }
    }
}