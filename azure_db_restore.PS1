workflow azure_db_restore_v2
{
	Param
    (   
        [Parameter(Mandatory=$true)]
        [String]
        $SourceServerName = 'v87l9el49i',
     
        [Parameter(Mandatory=$true)]
        [String]
        $SourceDatabaseName = 'tasklogger_db',
     
        [Parameter(Mandatory=$true)]
        [String] 
        $TargetdatabaseName = 'tasklogger_db_test',
                    
        [Parameter(Mandatory=$true)]
        [String]
        $automationAccountName = 'automationuser@milenpavlovetech.onmicrosoft.com',
        
        # Desired Azure SQL Database edition {Basic, Standard, Premium} 
        [Parameter(Mandatory=$true)]
        [String]
        $targetDatabaseEdition = "Basic",
        
        # Desired performance level {Basic, S0, S1, S2, P1, P2, P3 etc.} 
        [Parameter(Mandatory=$true)]
        [String]		
        $targetWorkerSize = "Basic",
        
        [Parameter(Mandatory=$true)]
        [String]
        $azureSubscriptionName = "Visual Studio Premium with MSDN"
    )
	
	 #validate database edition and worker size
	 if(-Not (($targetDatabaseEdition -eq "Basic") -or ($targetDatabaseEdition -eq "Standard") -or ($targetDatabaseEdition -eq "Premium")))
	 {
		 throw "Invalid database edition: Supported editions are Basic, Standard and Premium"
		 exit	 
	 }
	 
	 if($targetDatabaseEdition -eq "Basic")
	 {
		 if($targetWorkerSize -ne "Basic")
		 {
			 throw "Worker size 'Basic' is required for database edition $targetDatabaseEdition"
			 exit
		 }
	}
	 
	 if($targetDatabaseEdition -eq "Standard")
	 {
		 if(-Not (($targetWorkerSize -eq "S0") -or ($targetWorkerSize -eq "S1") -or ($targetWorkerSize -eq "S2") -or ($targetWorkerSize -eq "S3")))
		 {
			 throw 'Worker size allowed for Standard database edition are S0, S1, S2 or S3'
			 exit	 
		 }
	 }
	 
	 if($targetDatabaseEdition -eq "Premium")
	 {
		 if(-Not (($targetWorkerSize -eq "P1") -or ($targetWorkerSize -eq "P2") -or ($targetWorkerSize -eq "P4")))
		 {
			 throw 'Worker size allowed for Premium database editions are P1, P2 or P4'
			 exit	 
		 }
	 }
	 
	 # Get automation credential      
     Connect-Azure -AutomationAccountName $automationAccountName -azureSubscriptionName $azureSubscriptionName
     
	 # Get the time stamp when the process starts  
	 Write-Output "Started on: $((Get-Date).DateTime)"       
     
     $finalResult
        
     $existingBackup = Get-AzureSqlDatabase -ServerName $SourceServerName -DatabaseName $TargetDatabaseName      
     
     #delete old backup if exists and new backup created successfully
     if($existingBackup -ne $null)
     {
         Write-Output "Deleting the old $TargetdatabaseName"
         Remove-AzureSqlDatabase -ServerName $SourceServerName -DatabaseName $TargetdatabaseName -Force            
         Write-Output "Deleted old backup successfully"
         $finalResult += " Deleted old backup successfully "
     }  
	     
	 Write-Output "Creating new $TargetDatabaseName"
	 #Start the database restore to refresh the data 
     $result   
     $tempState = -1
     
     #restore under tempname
     $myTimer = [System.Diagnostics.Stopwatch]::StartNew()
     
     $Operation = Start-AzureSqlDatabaseRecovery -SourceServerName $SourceServerName -SourceDatabaseName $SourceDatabaseName -TargetDatabaseName $TargetdatabaseName -ErrorAction Stop
     do
     {                   
        $result = Get-AzureSqlDatabaseOperation –ServerName $SourceServerName –OperationGuid $Operation.ID
        
        if($tempState -ne $result.StateId) # only show changes in operation state
        { 
            $tempState = $result.StateId
            Write-Output "State: $($result.State)"
        }
     }
     while($result.StateId -lt 2)   
	 
     Checkpoint-Workflow 
	 
     if($result.StateId -eq 2) #StateId == 2 means restore completed successfully
     {
         $ErrorActionPreference = "Stop"
         $finalResult = " New backup created "
         
         #get MaxSizeBytes for db 
         $backupDatabase = Get-AzureSqlDatabase -ServerName $SourceServerName -DatabaseName $TargetdatabaseName
         $maxSizeBytes = $backupDatabase.MaxSizeBytes
         
		 #defaults to 2Gb if size could not be determined
         if($maxSizeBytes -eq $null)
         {
             $maxSizeBytes = 2147483648
         }
		 
		 Checkpoint-Workflow 
		 
         #run Child runbook to scale database
         azure_db_vertically_scale -SourceServerName $SourceServerName -TargetdatabaseName $TargetdatabaseName -targetDatabaseEdition $targetDatabaseEdition -targetWorkerSize $targetWorkerSize -maxSizeBytes $maxSizeBytes -ErrorAction Stop
             
         Write-Output "Elapsed Time: $($myTimer.Elapsed.ToString())"
         Write-Output "Final result: $finalResult"               
     }
     if($result.StateId -eq 3) #StateId 3 means Failed - show error details
     { 
         Write-Output "Error: $($result.Error)"
         Write-Output "Error code: $($result.ErrorCode))"
     }
	 Write-Output "Completed on: $((Get-Date).DateTime)"     
}
