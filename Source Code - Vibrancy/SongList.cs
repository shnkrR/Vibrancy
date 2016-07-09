#region Namspace
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace Vibrancy
{
    class SongList
    {
        MediaLibrary mediaLibrary;

        SongCollection allSongsInLibrary;

        public SongList()
        {
            mediaLibrary = new MediaLibrary();
        }

        public SongCollection FetchSongsFromLibrary()
        {
            allSongsInLibrary = mediaLibrary.Songs;

            return allSongsInLibrary;
        }
    }
}
