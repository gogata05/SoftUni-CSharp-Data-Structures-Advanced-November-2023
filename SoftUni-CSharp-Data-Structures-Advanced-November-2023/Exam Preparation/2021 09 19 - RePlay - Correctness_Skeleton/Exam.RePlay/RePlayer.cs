using System;
using System.Collections.Generic;
using System.Linq;

namespace Exam.RePlay
{
    public class RePlayer : IRePlayer
    {
        private Dictionary<string, Dictionary<string, Track>> albums;
        private Queue<Track> queue;

        public RePlayer()
        {
            this.albums = new Dictionary<string, Dictionary<string, Track>>();
            this.queue = new Queue<Track>();
        }

        public int Count
        {
            get { return this.albums.Sum(a => a.Value.Count); }
        }

        public void AddToQueue(string trackName, string albumName)
        {
            if (this.albums.ContainsKey(albumName) && this.albums[albumName].ContainsKey(trackName))
            {
                this.queue.Enqueue(this.albums[albumName][trackName]);
            }
        }

        public void AddTrack(Track track, string album)
        {
            if (!this.albums.ContainsKey(album))
            {
                this.albums[album] = new Dictionary<string, Track>();
            }

            this.albums[album][track.Title] = track;
        }

        public bool Contains(Track track)
        {
            return this.albums.Values.Any(a => a.Values.Contains(track));
        }

        public IEnumerable<Track> GetAlbum(string albumName)
        {
            if (this.albums.ContainsKey(albumName))
            {
                return this.albums[albumName].Values;
            }

            return Enumerable.Empty<Track>();
        }

        public Dictionary<string, List<Track>> GetDiscography(string artistName)
        {
            return this.albums
                .Where(a => a.Value.Values.Any(t => t.Artist == artistName))
                .ToDictionary(a => a.Key, a => a.Value.Values.ToList());
        }

        public Track GetTrack(string title, string albumName)
        {
            if (this.albums.ContainsKey(albumName) && this.albums[albumName].ContainsKey(title))
            {
                return this.albums[albumName][title];
            }

            return null;
        }

        public IEnumerable<Track> GetTracksInDurationRangeOrderedByDurationThenByPlaysDescending(int lowerBound, int upperBound)
        {
            return this.albums.Values
                .SelectMany(a => a.Values)
                .Where(t => t.DurationInSeconds >= lowerBound && t.DurationInSeconds <= upperBound)
                .OrderBy(t => t.DurationInSeconds)
                .ThenByDescending(t => t.Plays);
        }

        public IEnumerable<Track> GetTracksOrderedByAlbumNameThenByPlaysDescendingThenByDurationDescending()
        {
            return this.albums
                .OrderBy(a => a.Key)
                .SelectMany(a => a.Value.Values.OrderByDescending(t => t.Plays).ThenByDescending(t => t.DurationInSeconds));
        }

        public Track Play()
        {
            return this.queue.Count > 0 ? this.queue.Dequeue() : null;
        }

        public void RemoveTrack(string trackTitle, string albumName)
        {
            if (this.albums.ContainsKey(albumName) && this.albums[albumName].ContainsKey(trackTitle))
            {
                this.albums[albumName].Remove(trackTitle);
            }
        }
    }
}
