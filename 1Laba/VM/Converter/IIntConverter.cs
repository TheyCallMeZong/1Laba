namespace _1Laba.VM.Converter
{
    interface IIntConverter
    {
        int IntSize { get; }

        byte[] ToBytes(int value);
        int ToInt(byte[] bytes);
    }
}
