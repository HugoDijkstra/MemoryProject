using System.IO;

namespace MemoryProjectFull
{

    public partial class MemoryConfig
    {

        private static class FileHandler
        {

            private const long FILE_SIGNATURE = 0x54554E475277;
            private const int  END_OF_FILE    = -1;

            private enum Token : byte { GROUP_NAME = 1, ENTRY_NAME = 2, ENTRY_VALUE = 4 }
            private enum Types : byte { INT32, STRING }

            private static bool IsFileValid(long fileSignature)
            {
                return (fileSignature == FILE_SIGNATURE);
            }

            public static void SerializeConfig(MemoryConfig config, string filePath)
            {
                File.WriteAllText(filePath, string.Empty);

                using (var file = File.OpenWrite(filePath))
                using (var writer = new BinaryWriter(file))
                {
                    writer.Write(FILE_SIGNATURE);

                    foreach (var group in config.Groups)
                    {
                        if (group.IsEmpty) continue;

                        writer.Write((byte)Token.GROUP_NAME);
                        writer.Write(group.GetName());

                        foreach (var entry in group.Entries)
                        {
                            if (entry.IsEmpty) continue;

                            writer.Write((byte)Token.ENTRY_NAME);
                            writer.Write(entry.GetName());
                            writer.Write((byte)Token.ENTRY_VALUE);

                            if (entry.GetValueType() == typeof(int))
                            {
                                writer.Write((byte)Types.INT32);
                                writer.Write(entry.GetValue<int>());
                            }
                            else if (entry.GetValueType() == typeof(string))
                            {
                                writer.Write((byte)Types.STRING);
                                writer.Write(entry.GetValue<string>());
                            }
                            else
                            {
                                throw new IOException("Failed to write config! Unknown Value Type: " + entry.GetValueType());
                            }
                        }
                    }

                    writer.Flush();
                }
            }

            public static void DeserializeConfig(MemoryConfig config, string filePath)
            {
                using (var file = File.OpenRead(filePath))
                using (var reader = new BinaryReader(file))
                {
                    if (file.Length == 0) throw new IOException("'" + filePath + "' is an empty file.");
                    if (!IsFileValid( reader.ReadInt64() )) throw new IOException("'" + filePath + "' is not a valid '.mcfg' file.");

                    DataGroup currentGroup = null;
                    string currentEntryName = null;

                    byte expectedTokens = (byte)Token.GROUP_NAME;

                    while (reader.PeekChar() != END_OF_FILE)
                    {
                        var token = reader.ReadByte();
                        if ((expectedTokens & token) == 0) throw new IOException("Failed to read '" + filePath + "'! Invalid Token!");

                        switch ((Token)token)
                        {
                            case Token.ENTRY_VALUE:
                                byte type = reader.ReadByte();

                                switch (type)
                                {
                                    case (byte)Types.INT32:
                                        currentGroup.AddEntry(currentEntryName, reader.ReadInt32());
                                        break;

                                    case (byte)Types.STRING:
                                        currentGroup.AddEntry(currentEntryName, reader.ReadString());
                                        break;

                                    default:
                                        throw new IOException("Failed to read '" + filePath + "'! Unknown Value Type: " + type);
                                }

                                expectedTokens = (byte)Token.ENTRY_NAME | (byte)Token.GROUP_NAME;
                                break;

                            case Token.ENTRY_NAME:
                                currentEntryName = reader.ReadString();

                                expectedTokens = (byte)Token.ENTRY_VALUE;
                                break;

                            case Token.GROUP_NAME:
                                string groupName = reader.ReadString();

                                config.AddGroup(groupName);
                                currentGroup = config.GetGroup(groupName);

                                expectedTokens = (byte)Token.ENTRY_NAME | (byte)Token.GROUP_NAME;
                                break;
                        }
                    }
                }
            }

        }

    }
}
