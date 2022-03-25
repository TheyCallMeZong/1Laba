using System.Text;

namespace _1Laba.VM.Converter
{
    class SimpleConverter : IIntConverter, IBoolConverter, IStringConverter
    {
        public int IntSize { get; } = sizeof(int);
        public int BoolSize { get; } = sizeof(bool);
        public Encoding Encoding { get; } = Encoding.Unicode;

        public byte[] ToBytes(int value)
        {
            return BitConverter.GetBytes(value);
        }

        public int ToInt(byte[] bytes)
        {
            return BitConverter.ToInt32(bytes);
        }

        public byte[] ToBytes(bool value)
        {
            return BitConverter.GetBytes(value);
        }

        public bool ToBool(byte[] bytes)
        {
            return BitConverter.ToBoolean(bytes);
        }

        public byte[] ToBytes(string value)
        {
            return Encoding.Unicode.GetBytes(value);
        }

        public string ToString(byte[] bytes)
        {
            return Encoding.Unicode.GetString(bytes);
        }
    }
}
