using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers
{
    public class FileRegex
    {
        private readonly Regex _dirRegexp;
        private readonly Regex _nameRegexp;

        public FileRegex(string nameRegexp, string dirREgexp = null)
        {
            _nameRegexp = new Regex($"^{nameRegexp}$", RegexOptions.IgnoreCase);
            _dirRegexp = string.IsNullOrWhiteSpace(dirREgexp) ? null : new Regex($"^{dirREgexp}$", RegexOptions.IgnoreCase);
        }

        public bool IsMatch(FileInfo info)
        {
            return (_dirRegexp == null || _dirRegexp.IsMatch(info.DirectoryName)) && _nameRegexp.IsMatch(info.Name);
        }

        public bool IsMatch(string path)
        {
            return _nameRegexp.IsMatch(path);
        }
    }

    public sealed class FileSystemEnumerator
    {
        private readonly List<FileRegex> _excludedFiles;
        private readonly List<FileRegex> _excludedLocations;
        private readonly List<FileRegex> _fileSpecs;
        private readonly LookAt _lookAt;
        private readonly bool _includeSubDirs;
        private readonly string[] _paths;

        public FileSystemEnumerator(string pathsToSearch, string filesToMatch, bool includeSubDirs, string filesToExclude = null,
            string locationsToExclude = null, LookAt lookAt = LookAt.Files)
        {
            if (null == pathsToSearch)
                throw new ArgumentNullException(nameof(pathsToSearch));
            if (null == filesToMatch)
                throw new ArgumentNullException(nameof(filesToMatch));

            CheckFileTypes(filesToMatch, nameof(filesToMatch));
            CheckFileTypes(filesToExclude, nameof(filesToExclude));

            _includeSubDirs = includeSubDirs;
            _paths = pathsToSearch.Split(';', ',');

            _fileSpecs = SplitFilesIntoRegexes(filesToMatch);
            _excludedFiles = SplitFilesIntoRegexes(filesToExclude);
            _excludedLocations = SplitFilesIntoRegexes(locationsToExclude);
            _lookAt = lookAt;
        }

        public IEnumerable<FileInfo> Matches(CancellationToken? cancelToken = null)
        {
            Queue<string> paths = new Queue<string>();
            foreach (string path in _paths)
                paths.Enqueue(path);

            while (paths.Count > 0)
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested)
                    yield break;

                string path = paths.Dequeue();
                if (_excludedLocations == null || !_excludedLocations.Exists(x => x.IsMatch(path)))
                        { 
                    foreach (string file in FileList(path))
                    {
                        FileInfo info = new FileInfo(file);
                        if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested)
                            yield break;

                        bool isDirectory = (info.Attributes & FileAttributes.Directory) != 0;
                        if (isDirectory && _includeSubDirs)
                            paths.Enqueue(file);
                        if (_lookAt == LookAt.Both || (_lookAt == LookAt.Files && !isDirectory) || (_lookAt == LookAt.Directories && isDirectory))
                        {
                            if (_fileSpecs.Any(fileSpec => fileSpec.IsMatch(info) && (_excludedFiles == null || !_excludedFiles.Exists(x => x.IsMatch(info)))))
                                yield return info;
                        }
                    }
                }
            }
        }

        private static IEnumerable<string> FileList(string path)
        {
            try
            {
                return Directory.EnumerateFileSystemEntries(path);
            }
            catch
            {
                return new List<string>();
            }
        }

        public static List<FileRegex> SplitFilesIntoRegexes(string fileTypes)
        {
            List<FileRegex> fileSpecs = null;
            if (!string.IsNullOrEmpty(fileTypes))
            {
                string[] specs = fileTypes.Split(';', ',');
                fileSpecs = new List<FileRegex>(specs.Length);
                foreach (string spec in specs)
                {
                    string[] bits = spec.Split(new[] { '\\' }, 2);
                    if (bits.Length == 1)
                    {
                        string pattern = spec.Trim().Replace(".", @"\.").Replace("*", @".*").Replace("?", @".?");
                        fileSpecs.Add(new FileRegex(pattern));
                    }
                    else
                    {
                        string namePattern = bits[1].Trim().Replace(".", @"\.").Replace("*", @".*").Replace("?", @".?");
                        string dirPattern = bits[0].Trim().Replace(".", @"\.").Replace("*", @".*").Replace("?", @".?");
                        fileSpecs.Add(new FileRegex(namePattern, dirPattern));
                    }
                }
            }
            return fileSpecs;
        }

        private static void CheckFileTypes(string fileTypes, string name)
        {
            if (fileTypes != null && fileTypes.IndexOfAny(new[] { ':', '<', '>', '/' }) > 0)
                throw new ArgumentException("Invalid characters in wild-card pattern");
        }
    }
}
