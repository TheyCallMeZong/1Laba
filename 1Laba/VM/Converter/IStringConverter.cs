using System.Text;

namespace _1Laba.VM.Converter
{
    interface IStringConverter
    {
        Encoding Encoding { get; }

        byte[] ToBytes(string value);
        string ToString(byte[] bytes);
    }
}
