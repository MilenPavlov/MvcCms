workflow azure_db_vertically_scale
{
    Param
    (   
        [Parameter(Mandatory=$true)]
        [String]
        $SourceServerName = 'v87l9el49i',
       
        [Parameter(Mandatory=$true)]
        [String]
        $TargetdatabaseName = 'tasklogger_db',
                        
        # Desired Azure SQL Database edition {Basic, Standard, Premium} 
        [Parameter(Mandatory=$true)]
        [String]
        $targetDatabaseEdition = "Basic",
        
        # Desired performance level {Basic, S0, S1, S2, P1, P2, P3} 
        [Parameter(Mandatory=$true)]
        [String]
        $targetWorkerSize = "Basic",
        
        [Parameter(Mandatory=$true)]
        [Long]
        $maxSizeBytes
    )
	
	 # Get the time stamp when the process starts  
	 Write-Output "Started vertical scale on: $((Get-Date).DateTime)"       
     $finalResult = ""
     #scale up/down database       
     #use InlineScript here due to issue: https://social.msdn.microsoft.com/forums/azure/en-US/1adec597-964c-402e-b1f1-b195d00a20be/exception-calling-azureprovisioningconfig?forum=azureautomation          
     InlineScript{                        
         $serviceObjective = Get-AzureSqlDatabaseServiceObjective -ServerName $Using:SourceServerName -ServiceObjectiveName $Using:targetWorkerSize
             
         if($Using:targetDatabaseEdition -eq "Basic")
         { 
             Set-AzureSqlDatabase -ServerName $Using:SourceServerName -DatabaseName $Using:TargetDatabaseName -ServiceObjective $serviceObjective -MaxSizeGB 2 -Edition $Using:targetDatabaseEdition -Force -ErrorAction Stop
         } 
         else
         {
             $scaleOperation = Set-AzureSqlDatabase -ServerName $Using:SourceServerName -DatabaseName $Using:TargetDatabaseName -ServiceObjective $serviceObjective -MaxSizeBytes $Using:maxSizeBytes -Edition $Using:targetDatabaseEdition -Force -ErrorAction Stop                     
         }
     }
             
     $finalResult = InlineScript{$b = $Using:finalResult  + " Database scaled to $Using:targetDatabaseEdition edition, worker size: $Using:targetWorkerSize. "; $b}
     Write-Output "$finalResult"
}
