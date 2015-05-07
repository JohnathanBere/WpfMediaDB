using System;
using System.IO;



public struct ID3v1Tag
   {
        // publically available structure of strings used to hold ID3v1 Tag data
        public string TagHeader; //0-2
        public string TrackName; //3-32
        public string ArtistsName; //33-62
        public string AlbumName; //63-92
        public string Year; //93-96
        public string Comment; //97-126
        public string Genres; //127
   }


public class ID3v1TagReader
{
    public ID3v1TagReader()
	{
	}

    // enumeration (list) of the ID3v1 genre descriptions
    // enumeration (list) of the ID3v1 genre descriptions
    string[] Genres = new string[] 
        {
            "Alternative Hip-hop",
            "Post-Hardcore",
            "Progressive Metal",
            "Blues",
            "Classic Rock", 
            "Country", 
            "Dance", 
            "Disco", 
            "Funk", 
            "Grunge", 
            "Hip-Hop", 
            "Jazz", 
            "Metal", 
            "New Age", 
            "Oldies", 
            "Other", 
            "Pop", 
            "R&B", 
            "Rap", 
            "Reggae", 
            "Rock", 
            "Techno", 
            "Industrial", 
            "Alternative",
            "Ska", 
            "Death Metal", 
            "Pranks", 
            "Soundtrack", 
            "Euro-Techno", 
            "Ambient", 
            "Trip-Hop", 
            "Vocal", 
            "Jazz+Funk", 
            "Fusion", 
            "Trance", 
            "Classical", 
            "Instrumental", 
            "Acid", 
            "House", 
            "Game", 
            "Sound Clip", 
            "Gospel", 
            "Noise", 
            "Alt. Rock", 
            "Bass", 
            "Soul", 
            "Punk", 
            "Space", 
            "Meditative", 
            "Instrumental Pop", 
            "Instrumental Rock", 
            "Ethnic", 
            "Gothic", 
            "Darkwave", 
            "Techno-Industrial", 
            "Electronic",
            "Pop-Folk", 
            "Eurodance", 
            "Dream", 
            "Southern Rock", 
            "Comedy", 
            "Cult", 
            "Gangsta Rap", 
            "Top 40", 
            "Christian Rap",
            "Pop/Funk", 
            "Jungle", 
            "Native American", 
            "Cabaret", 
            "New Wave", 
            "Psychedelic", 
            "Rave", 
            "Showtunes", 
            "Trailer", 
            "Lo-Fi", 
            "Tribal", 
            "Acid Punk", 
            "Acid Jazz", 
            "Polka", 
            "Retro", 
            "Musical", 
            "Rock & Roll", 
            "Hard Rock", 
            "Folk", 
            "Folk/Rock", 
            "National Folk", 
            "Swing",
            "Fast-Fusion", 
            "Bebop",
            "Latin", 
            "Revival", 
            "Celtic", 
            "Bluegrass", 
            "Avantgarde", 
            "Gothic Rock", 
            "Progressive Rock", 
            "Psychedelic Rock", 
            "Symphonic Rock", 
            "Slow Rock",
            "Big Band", 
            "Chorus", 
            "Easy Listening", 
            "Acoustic", 
            "Humour", 
            "Speech", 
            "Chanson", 
            "Opera", 
            "Chamber Music", 
            "Sonata", 
            "Symphony", 
            "Booty Bass", 
            "Primus", 
            "Porn Groove", 
            "Satire", 
            "Slow Jam", 
            "Club", 
            "Tango", 
            "Samba", 
            "Folklore", 
            "Ballad", 
            "Power Ballad",
            "Rhythmic Soul", 
            "Freestyle", 
            "Duet", 
            "Punk Rock", 
            "Drum Solo", 
            "A Cappella", 
            "Euro-House", 
            "Dance Hall", 
            "Goa", 
            "Drum & Bass", 
            "Club-House", 
            "Hardcore", 
            "Terror", 
            "Indie", 
            "BritPop", 
            "Negerpunk", 
            "Polsk Punk", 
            "Beat", 
            "Christian Gangsta Rap", 
            "Heavy Metal", 
            "Black Metal", 
            "Crossover", 
            "Contemporary Christian", 
            "Christian Rock", 
            "Merengue", 
            "Salsa", 
            "Thrash Metal"};


    private byte[] ReadID3FromFileToByte(string sLocalFile)
    {
        // open the given file
        using (FileStream oFS = new FileStream(sLocalFile, FileMode.Open, FileAccess.Read))
        {
            // create a binary data reader on the given file
            using (BinaryReader oBR = new BinaryReader(oFS))
            {
                // start reading 128 bytes from the end of the mp3 file
                // this is the location in the file where the ID3v1 Tag is located
                oBR.BaseStream.Position = (oFS.Length - 128);
                // read and return the last 128 bytes of the file
                return oBR.ReadBytes((int)oFS.Length);
            }
        }
    }


    public ID3v1Tag ReadID3v1Tag(string fileName)
    {
        // last 128 bytes of the file read and stored an an array of bytes
        byte[] bFileData = ReadID3FromFileToByte(fileName);

        // create an ID3v1Tag struture to fill with tag data and return to the caller
        ID3v1Tag tagInfo = new ID3v1Tag();
       
        // extrat each tag field and store it on the tag structure to be returned.
        tagInfo.TagHeader = System.Text.ASCIIEncoding.ASCII.GetString(bFileData, 0, 3).ToString();
        // should really check that TagHeader = "TAG" before continuing

        if (tagInfo.TagHeader == "TAG")
        {
            tagInfo.TrackName = System.Text.ASCIIEncoding.ASCII.GetString(bFileData, 3, 30).ToString();
            tagInfo.ArtistsName = System.Text.ASCIIEncoding.ASCII.GetString(bFileData, 33, 30).ToString();
            tagInfo.AlbumName = System.Text.ASCIIEncoding.ASCII.GetString(bFileData, 63, 30).ToString();
            tagInfo.Year = System.Text.ASCIIEncoding.ASCII.GetString(bFileData, 93, 4).ToString();
            tagInfo.Comment = System.Text.ASCIIEncoding.ASCII.GetString(bFileData, 97, 30).ToString();
            tagInfo.Genres = Genres[bFileData[127]];
        }
        // return the filled tag structure
        return tagInfo;
    }


}
