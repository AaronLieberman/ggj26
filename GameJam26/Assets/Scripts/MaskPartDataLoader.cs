using System.Collections.Generic;
using UnityEngine;

public static class MaskPartDataLoader
{
    public static MaskPartData[] Load()
    {
        var asset = Resources.Load<TextAsset>("Mask Maker - Mask Parts");
        if (asset == null)
        {
            Debug.LogError("MaskPartDataLoader: 'Mask Maker - Mask Parts.csv' not found in Resources");
            return new MaskPartData[0];
        }

        return Parse(asset.text);
    }

    static MaskPartData[] Parse(string csv)
    {
        csv = csv.Replace("\r\n", "\n").Replace("\r", "\n");
        var lines = csv.Split('\n');
        if (lines.Length == 0) return new MaskPartData[0];

        var headerFields = SplitCsvLine(lines[0]);
        int partCount = headerFields.Count - 1;
        if (partCount <= 0) return new MaskPartData[0];

        var parts = new MaskPartData[partCount];
        for (int i = 0; i < partCount; i++)
            parts[i] = new MaskPartData();

        bool inTags = false;

        for (int row = 0; row < lines.Length; row++)
        {
            string line = lines[row];
            if (string.IsNullOrWhiteSpace(line)) continue;

            var fields = SplitCsvLine(line);
            if (fields.Count == 0) continue;

            string label = fields[0].Trim();

            if (label == "Tags")
                inTags = true;
            else if (!string.IsNullOrEmpty(label))
                inTags = false;

            if (inTags)
            {
                for (int col = 1; col < fields.Count && col - 1 < partCount; col++)
                {
                    string value = fields[col].Trim();
                    if (!string.IsNullOrEmpty(value))
                        parts[col - 1].tags.Add(value);
                }
            }
            else
            {
                for (int col = 1; col < fields.Count && col - 1 < partCount; col++)
                {
                    string value = fields[col].Trim();
                    ApplyField(parts[col - 1], label, value);
                }
            }
        }

        return parts;
    }

    static void ApplyField(MaskPartData p, string label, string value)
    {
        switch (label)
        {
            case "Part Name": p.partName = value; break;
            case "Art Request Priority": p.artRequestPriority = ParseInt(value); break;
            case "Sprite Name": p.spriteName = value; break;
            case "Slot": p.slot = ParseSlot(value); break;
            case "Scary Stat": p.scaryStat = ParseInt(value); break;
            case "Goofy Stat": p.goofyStat = ParseInt(value); break;
            case "Beauty Stat": p.beautyStat = ParseInt(value); break;
            case "Anonymity Stat": p.anonymityStat = ParseInt(value); break;
            case "Spawns In Pairs": p.spawnsInPairs = value == "Yes"; break;
        }
    }

    static MaskPartSlot ParseSlot(string value)
    {
        return value switch
        {
            "Nose" => MaskPartSlot.Nose,
            "Mouth" => MaskPartSlot.Mouth,
            "Eye" => MaskPartSlot.Eye,
            "Horn" => MaskPartSlot.Horn,
            "Ear" => MaskPartSlot.Ear,
            _ => MaskPartSlot.Nose
        };
    }

    static List<string> SplitCsvLine(string line)
    {
        var fields = new List<string>();
        bool inQuotes = false;
        var current = new System.Text.StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char ch = line[i];

            if (inQuotes)
            {
                if (ch == '"')
                {
                    if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    current.Append(ch);
                }
            }
            else
            {
                if (ch == '"')
                {
                    inQuotes = true;
                }
                else if (ch == ',')
                {
                    fields.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(ch);
                }
            }
        }

        fields.Add(current.ToString());
        return fields;
    }

    static int ParseInt(string value)
    {
        if (string.IsNullOrEmpty(value)) return 0;
        int.TryParse(value, out int result);
        return result;
    }
}
