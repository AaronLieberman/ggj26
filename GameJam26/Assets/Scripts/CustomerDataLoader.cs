using System.Collections.Generic;
using UnityEngine;

public static class CustomerDataLoader
{
    public static CustomerData[] Load()
    {
        var asset = Resources.Load<TextAsset>("Mask Maker - Customers");
        if (asset == null)
        {
            Debug.LogError("CustomerDataLoader: 'Mask Maker - Customers.csv' not found in Resources");
            return new CustomerData[0];
        }

        return Parse(asset.text);
    }

    static CustomerData[] Parse(string csv)
    {
        csv = csv.Replace("\r\n", "\n").Replace("\r", "\n");
        var lines = csv.Split('\n');
        if (lines.Length == 0) return new CustomerData[0];

        // First row determines customer count (columns minus the label column)
        var headerFields = SplitCsvLine(lines[0]);
        int customerCount = headerFields.Count - 1;
        if (customerCount <= 0) return new CustomerData[0];

        var customers = new CustomerData[customerCount];
        for (int i = 0; i < customerCount; i++)
            customers[i] = new CustomerData();

        for (int row = 0; row < lines.Length; row++)
        {
            string line = lines[row];
            if (string.IsNullOrWhiteSpace(line)) continue;

            var fields = SplitCsvLine(line);
            if (fields.Count == 0) continue;

            string label = fields[0].Trim();

            for (int col = 1; col < fields.Count && col - 1 < customerCount; col++)
            {
                string value = fields[col].Trim();
                var c = customers[col - 1];
                ApplyField(c, label, value);
            }
        }

        // Fill in missing customerImageName from customerName
        for (int i = 0; i < customers.Length; i++)
        {
            if (string.IsNullOrEmpty(customers[i].customerImageName) && !string.IsNullOrEmpty(customers[i].customerName))
                customers[i].customerImageName = customers[i].customerName.Replace(" ", "");
        }

        return customers;
    }

    static void ApplyField(CustomerData c, string label, string value)
    {
        switch (label)
        {
            case "Customer Name": c.customerName = value; break;
            case "Time": c.time = ParseFloat(value); break;
            case "Difficulty Tier": c.difficultyTier = ParseInt(value); break;
            case "Customer Sprite": c.customerImageName = value; break;
            case "Mask Base": c.maskBase = value; break;
            case "Intro Dialog": c.customerDialogue = value; break;
            case "Scary Minimum": c.maskScary.Min = ParseInt(value); break;
            case "Scary Maximum": c.maskScary.Max = ParseInt(value); break;
            case "Scary Points Per": c.maskScary.Points = ParseInt(value); break;
            case "Goofy Minimum": c.maskGoofy.Min = ParseInt(value); break;
            case "Goofy Maximum": c.maskGoofy.Max = ParseInt(value); break;
            case "Goofy Points Per": c.maskGoofy.Points = ParseInt(value); break;
            case "Beauty Minimum": c.maskBeauty.Min = ParseInt(value); break;
            case "Beauty Maximum": c.maskBeauty.Max = ParseInt(value); break;
            case "Beauty Points Per": c.maskBeauty.Points = ParseInt(value); break;
            case "Anonymity Minimum": c.maskAnonymity.Min = ParseInt(value); break;
            case "Anonymity Maximum": c.maskAnonymity.Max = ParseInt(value); break;
            case "Anonymity Points Per": c.maskAnonymity.Points = ParseInt(value); break;
            case "{Minimum Tags}": c.minimumTags = ParseTags(value); break;
            case "{Maximum Tags}": c.maximumTags = ParseTags(value); break;
            case "{Points Per Tag}": c.pointsPerTag = ParseInt(value); break;
            case "Max Score": c.maxScore = ParseInt(value); break;
            case "Grade A Score Threshold": c.gradeAThreshold = ParseInt(value); break;
            case "Grade B Score Threshold": c.gradeBThreshold = ParseInt(value); break;
            case "Grade C Score Threshold": c.gradeCThreshold = ParseInt(value); break;
            case "Grade D Score Threshold": c.gradeDThreshold = ParseInt(value); break;
            case "Grade A Dialog": c.gradeADialog = value; break;
            case "Grade B Dialog": c.gradeBDialog = value; break;
            case "Grade C Dialog": c.gradeCDialog = value; break;
            case "Grade D Dialog": c.gradeDDialog = value; break;
            case "Grade F Dialog": c.gradeFDialog = value; break;
        }
    }

    static List<CustomerTag> ParseTags(string value)
    {
        var tags = new List<CustomerTag>();
        if (string.IsNullOrEmpty(value)) return tags;

        var entries = value.Split(':');
        foreach (var entry in entries)
        {
            var parts = entry.Split('-');
            if (parts.Length < 4) continue;

            tags.Add(new CustomerTag
            {
                TagName = parts[0],
                Min = ParseInt(parts[1]),
                Max = ParseInt(parts[2]),
                Points = ParseInt(parts[3])
            });
        }

        return tags;
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

    static float ParseFloat(string value)
    {
        if (string.IsNullOrEmpty(value)) return 0f;
        float.TryParse(value, out float result);
        return result;
    }
}
