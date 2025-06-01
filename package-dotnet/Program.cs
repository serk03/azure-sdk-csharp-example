using System;
using System.Threading.Tasks;
using Azure;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Storage;
using Azure.ResourceManager.Storage.Models;

class Program
{
    static async Task Main(string[] args)
    {
        string subscriptionId = "19f47d9a-092b-4e66-a53c-404049f984e6";
        string rgName = "my-sdk-rg";
        string location = "uksouth";
        string storageAccountName = "helloworld12345" + Guid.NewGuid().ToString("n").Substring(0, 8);

        var credential = new DefaultAzureCredential();
        var armClient = new ArmClient(credential, subscriptionId);

        SubscriptionResource subscription = await armClient.GetDefaultSubscriptionAsync();

        // Create Resource Group
        ResourceGroupCollection rgCollection = subscription.GetResourceGroups();
        ArmOperation<ResourceGroupResource> rgLro = await rgCollection.CreateOrUpdateAsync(
            WaitUntil.Completed,
            rgName,
            new ResourceGroupData(location));
        ResourceGroupResource resourceGroup = rgLro.Value;

        // Create Storage Account
        StorageAccountCollection storageAccounts = resourceGroup.GetStorageAccounts();
        var storageParams = new StorageAccountCreateOrUpdateContent(
            new StorageSku(StorageSkuName.StandardLrs),
            StorageKind.StorageV2,
            location);

        ArmOperation<StorageAccountResource> storageLro = await storageAccounts.CreateOrUpdateAsync(
            WaitUntil.Completed,
            storageAccountName,
            storageParams);

        Console.WriteLine($"✅ Storage Account '{storageAccountName}' created in resource group '{rgName}'.");
    }
}
