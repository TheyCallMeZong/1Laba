using _1Laba.VM.Converter;
using System.Text;

namespace _1Laba.VM
{
    class VirtualMemory
    {
        private readonly string _marker = "VM";
        private readonly int _markerSize;

        private readonly IIntConverter _intConverter;
        private readonly IBoolConverter _boolConverter;
        private readonly IStringConverter _stringConverter;

        private readonly FileStream _memoryFile;
        private readonly BinaryReader _br;
        private readonly BinaryWriter _bw;

        private MemoryPage CurrentPage { get; set; }

        public int PageCapacity { get; }
        public int PageNumber { get; }

        public VirtualMemory(string filePath, int pageCapacity, int pageNumber, bool overwrite = true, IIntConverter intConverter = null, IBoolConverter boolConverter = null, IStringConverter stringConverter = null)
        {
            var simpleConverter = new SimpleConverter();

            _intConverter = intConverter ?? simpleConverter;
            _boolConverter = boolConverter ?? simpleConverter;
            _stringConverter = stringConverter ?? simpleConverter;

            _markerSize = _stringConverter.ToBytes(_marker).Length;

            var exist = !overwrite && File.Exists(filePath);

            _memoryFile = new FileStream(filePath, overwrite ? FileMode.Create : FileMode.OpenOrCreate, FileAccess.ReadWrite);
            _memoryFile.Seek(0, SeekOrigin.Begin);

            _br = new BinaryReader(_memoryFile);
            _bw = new BinaryWriter(_memoryFile);

            PageCapacity = pageCapacity;
            PageNumber = pageNumber;

            if (exist)
            {
                var marker = Encoding.Unicode.GetString(_br.ReadBytes(_markerSize));

                var tempCapacity = _intConverter.ToInt(_br.ReadBytes(_intConverter.IntSize));
                var tempNumber = _intConverter.ToInt(_br.ReadBytes(_intConverter.IntSize));

                if (marker != _marker || tempCapacity != pageCapacity || tempNumber != pageNumber)
                    throw new FileLoadException();

                LoadPage(0);
            }
            else
            {
                CurrentPage = new MemoryPage(0, PageCapacity);

                _bw.Write(_stringConverter.ToBytes(_marker));
                _bw.Write(_intConverter.ToBytes(PageCapacity));
                _bw.Write(_intConverter.ToBytes(PageNumber));

                for (int i = 0; i < PageNumber; i++)
                {
                    WritePage(i);
                }
            }
        }

        private void LoadPage(int number)
        {
            _memoryFile.Seek(_markerSize + _intConverter.IntSize * 2 + (_boolConverter.BoolSize + _intConverter.IntSize) * PageCapacity * number, SeekOrigin.Begin);

            var map = new bool[PageCapacity];
            var val = new int[PageCapacity];

            map = map.Select(x => _boolConverter.ToBool(_br.ReadBytes(_boolConverter.BoolSize))).ToArray();
            val = val.Select(x => _intConverter.ToInt(_br.ReadBytes(_intConverter.IntSize))).ToArray();

            CurrentPage = new MemoryPage(number, map, val);
        }

        private void WritePage(int number)
        {
            _memoryFile.Seek(_markerSize + _intConverter.IntSize * 2 + (_boolConverter.BoolSize + _intConverter.IntSize) * PageCapacity * number, SeekOrigin.Begin);

            foreach (var x in CurrentPage.UsingMap.Select(_boolConverter.ToBytes))
            {
                _bw.Write(x);
            }

            foreach (var x in CurrentPage.Values.Select(_intConverter.ToBytes))
            {
                _bw.Write(x);
            }

            _bw.Flush();
        }

        private void ChangePage(int number)
        {
            if (number < 0 || number >= PageNumber)
            {
                throw new ArgumentException();
            }

            if (CurrentPage.Number != number)
                LoadPage(number);
        }

        public bool IsEmpty(int pageNumber, int elementNumber)
        {
            ChangePage(pageNumber);
            return !CurrentPage.UsingMap[elementNumber];
        }

        public void Delete(int pageNumber, int elementNumber)
        {
            ChangePage(pageNumber);

            CurrentPage.UsingMap[elementNumber] = false;
            CurrentPage.Values[elementNumber] = 0;

            WritePage(pageNumber);
        }

        public void PrintPage(int number, bool printEmptyValues = false)
        {
            ChangePage(number);
            Console.WriteLine($"Page #{CurrentPage.Number}");
            for (int i = 0; i < PageCapacity; i++)
            {
                Console.WriteLine($"[{i}]: { (CurrentPage.UsingMap[i] ? CurrentPage.Values[i].ToString() : printEmptyValues ? $"{CurrentPage.Values[i]} <EMPTY>" : "<EMPTY>")}");
            }
            Console.WriteLine();
        }

        public void PrintAllPages(bool printEmptyValues = false)
        {
            for (int i = 0; i < PageNumber; i++)
            {
                PrintPage(i, printEmptyValues);
            }
        }

        public int this[int i]
        {
            get
            {
                ChangePage(i / PageCapacity);
                return CurrentPage.Values[i % PageCapacity];
            }
            set
            {
                var k = i / PageCapacity;
                var j = i % PageCapacity;
                ChangePage(k);

                CurrentPage.Values[j] = value;
                CurrentPage.UsingMap[j] = true;

                WritePage(k);
            }
        }


        public int this[int i, int j]
        {
            get
            {
                ChangePage(i);
                return CurrentPage.Values[j];
            }
            set
            {
                ChangePage(i);

                CurrentPage.Values[j] = value;
                CurrentPage.UsingMap[j] = true;

                WritePage(i);
            }
        }
    }
}
