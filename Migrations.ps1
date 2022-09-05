function Get-Startup-Path {
    $Path = Join-Path $PSScriptRoot "WebApi";

    return ("'" + $Path + "'");
}

function Get-Persistence-Path {
    $Path = Join-Path $PSScriptRoot "Infrastructure";

    return ("'" + $Path + "'");
}

function Add-Migration {
    param (
        [string] $Name
    )

    if ([string]::IsNullOrWhiteSpace($Name)) {
        Write-Error "The migration must have a name!"

        return;
    }
    if ($Name -like "* *") {
        Write-Error "You should not have spaces in your migration names";

        return;
    }

    $PersistencePath = Get-Persistence-Path;
    $StartupPath = Get-Startup-Path;

    Invoke-Expression ("dotnet ef migrations add '" + $Name + "' --project " + $PersistencePath + " --startup-project " + $StartupPath + " --output-dir Persistence/Migrations --context AppDbContext");
}

function Remove-Migration {
    $PersistencePath = Get-Persistence-Path;
    $StartupPath = Get-Startup-Path;

    Invoke-Expression ("dotnet ef migrations remove --project " + $PersistencePath + " --startup-project " + $StartupPath)
}

$command = $args[0];
switch ($command) {
    "add" {
        Add-Migration($args[1]);

        break;
    }
    "remove" {
        Remove-Migration;

        break;
    }
    Default {
        Write-Error "Unknown command";
    }
}