using System;
using System.Collections.Generic;

[Serializable]
public class UnlockData
{
    public Dictionary<string, bool> unlockDict;
    public List<string> completedStory;
    public List<string> paidStory;
}