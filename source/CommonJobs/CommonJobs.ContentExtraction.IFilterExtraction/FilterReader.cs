using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace CommonJobs.ContentExtraction.IFilterExtraction
{
    /// <summary>
    /// Implements a TextReader that reads from an IFilter. 
    /// Taken from http://indexer.codeplex.com/ under Apache License 2.0 (Apache)
    /// </summary>
    internal class FilterReader : TextReader
    {
        //TODO: revisar porque ya le quité algunas incorrecciones, no estoy seguro de que no tenga bugs o memory leaks.
        IFilter _filter;
        private bool _done;
        private STAT_CHUNK _currentChunk;
        private bool _currentChunkValid;
        private char[] _charsLeftFromLastRead;

        public override void Close()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FilterReader()
        {
            Dispose(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (_filter != null)
                Marshal.ReleaseComObject(_filter);
        }

        public override int Read(char[] array, int offset, int count)
        {
            if (_filter == null) return -1;
            int endOfChunksCount = 0;
            int charsRead = 0;

            while (!_done && charsRead < count)
            {
                if (_charsLeftFromLastRead != null)
                {
                    int charsToCopy = (_charsLeftFromLastRead.Length < count - charsRead) ? _charsLeftFromLastRead.Length : count - charsRead;
                    Array.Copy(_charsLeftFromLastRead, 0, array, offset + charsRead, charsToCopy);
                    charsRead += charsToCopy;
                    if (charsToCopy < _charsLeftFromLastRead.Length)
                    {
                        char[] tmp = new char[_charsLeftFromLastRead.Length - charsToCopy];
                        Array.Copy(_charsLeftFromLastRead, charsToCopy, tmp, 0, tmp.Length);
                        _charsLeftFromLastRead = tmp;
                    }
                    else
                        _charsLeftFromLastRead = null;
                    continue;
                };

                if (!_currentChunkValid)
                {
                    IFilterReturnCode res = _filter.GetChunk(out _currentChunk);
                    _currentChunkValid = (res == IFilterReturnCode.S_OK) && ((_currentChunk.flags & CHUNKSTATE.CHUNK_TEXT) != 0);

                    if (res == IFilterReturnCode.FILTER_E_END_OF_CHUNKS)
                        endOfChunksCount++;

                    if (endOfChunksCount > 1)
                        _done = true; //That's it. no more chuncks available
                }

                if (_currentChunkValid)
                {
                    uint bufLength = (uint)(count - charsRead);
                    if (bufLength < 8192)
                        bufLength = 8192; //Read ahead

                    char[] buffer = new char[bufLength];
                    IFilterReturnCode res = _filter.GetText(ref bufLength, buffer);
                    if (res == IFilterReturnCode.S_OK || res == IFilterReturnCode.FILTER_S_LAST_TEXT)
                    {
                        int cRead = (int)bufLength;
                        if (cRead + charsRead > count)
                        {
                            int charsLeft = (cRead + charsRead - count);
                            _charsLeftFromLastRead = new char[charsLeft];
                            Array.Copy(buffer, cRead - charsLeft, _charsLeftFromLastRead, 0, charsLeft);
                            cRead -= charsLeft;
                        }
                        else
                            _charsLeftFromLastRead = null;

                        Array.Copy(buffer, 0, array, offset + charsRead, cRead);
                        charsRead += cRead;
                    }

                    if (res == IFilterReturnCode.FILTER_S_LAST_TEXT || res == IFilterReturnCode.FILTER_E_NO_MORE_TEXT)
                        _currentChunkValid = false;
                }
            }
            return charsRead;
        }
        public bool Filtered { get { return _filter != null; } }

        public FilterReader(string fullPath, string fileName)
        {
            _filter = FilterLoader.LoadAndInitIFilter(fullPath, Path.GetExtension(fileName));
        }
    }
}