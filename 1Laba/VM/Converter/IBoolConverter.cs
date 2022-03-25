namespace _1Laba.VM.Converter
{
    interface IBoolConverter
    {
        int BoolSize { get; }

        byte[] ToBytes(bool value);
        bool ToBool(byte[] bytes);
    }
}
