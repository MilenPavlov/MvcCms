workflow Connect-Azure
{
    Param
    (
        [Parameter(Mandatory=$true)]
        [String]
        $automationAccountName = "automationuser@milenpavlovetech.onmicrosoft.com",
        
        [Parameter(Mandatory=$true)]
        [String]
        $azureSubscriptionName = "Visual Studio Premium with MSDN"
    )
    
     $Credential = Get-AutomationPSCredential –Name $automationAccountName
     
     if($Credential -eq $null){
         throw "Could not retrieve '$AzureConnectionName' powershell credential."
         exit
     }
     Add-AzureAccount –Credential $Credential
     
     Select-AzureSubscription -SubscriptionName $azureSubscriptionName
}
