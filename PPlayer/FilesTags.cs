using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PPlayer
{
    internal sealed class GetTagsTitle
    {
        string fileName;
        string trackNumber;
        string artistName;
        string albumTitle;
        string trackTitle;
        string trackCount;
        string objectName;
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public GetTagsTitle(string fileName)
        {
            TagLib.File file = TagLib.File.Create(fileName);
            this.fileName = fileName;
            try
            {
                this.trackTitle = file.Tag.Title;
            }
            catch (Exception)
            {
                this.trackTitle = "NoTitle";
            }
            try
            {
                this.artistName = file.Tag.AlbumArtists[0];
            }
            catch (Exception)
            {
                try
                {
                    this.artistName = file.Tag.Performers[0]; //Artists, AvbumArtist, Performers
                }
                catch (Exception)
                {
                    this.artistName = "NoArtist";
                }
            }
            try
            {
                this.albumTitle = file.Tag.Album.ToString();
            }
            catch (Exception)
            {
                this.albumTitle = "NoAlbum";
            }
            try
            {
                this.trackNumber = file.Tag.Track.ToString();
            }
            catch (Exception)
            {
                this.trackNumber = "0";
            }
            try
            {
                this.trackCount = file.Tag.TrackCount.ToString();
            }
            catch (Exception)
            {
                this.trackCount = "0";
            }
            this.objectName = this.trackNumber + @"/" + this.trackCount + " - " + this.trackTitle + " - " + this.artistName + " - " + this.albumTitle;
        }

        public override string ToString()
        {
            return this.objectName;
        }
    }
}
