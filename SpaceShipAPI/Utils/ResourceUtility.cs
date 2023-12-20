namespace SpaceShipAPI.Utils;

using System.Collections.Generic;

public static class ResourceUtility
{
    public static Dictionary<ResourceType, int> ConvertToDictionary(ICollection<StoredResource> storedResources)
    {
        var dictionary = new Dictionary<ResourceType, int>();

        foreach (var storedResource in storedResources)
        {
            if (dictionary.ContainsKey(storedResource.ResourceType))
            {
                dictionary[storedResource.ResourceType] += storedResource.Amount;
            }
            else
            {
                dictionary[storedResource.ResourceType] = storedResource.Amount;
            }
        }
        return dictionary;
    }

    public static ICollection<StoredResource> MapToStoredResources(IDictionary<ResourceType, int> storedResources)
    {
        var result = new List<StoredResource>();

        foreach (var kvp in storedResources)
        {
            result.Add(new StoredResource
            {
                ResourceType = kvp.Key,
                Amount = kvp.Value
            });
        }

        return result;
    }
}
