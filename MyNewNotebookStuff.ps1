Import-Module $PSScriptRoot\bin\Debug\net5.0\publish\PSCmdlet.dll -Force

function Get-NBContent {
    param(
        [Parameter(ValueFromPipelineByPropertyName)]
        $FullName
    )

    Process {
        $nb = Invoke-TheNotebook $FullName

        foreach ($cell in $nb.Cells) {
            $fn = "$($cell.Language)Display"
            if (! (Get-Command $fn -ErrorAction SilentlyContinue) ) {
                Write-Warning "$fn not found, cannot show data for text"
                continue
            }

            &$fn $cell $FullName
        }
    }
}

function pwshDisplay {
    param(
        $Cell,
        $FullName
    )

    $Result = $Cell.Outputs.Text
    
    DoDisplay $Cell $Result $FullName
}

function csharpDisplay {
    param(
        $Cell,
        $FullName
    )

    $Result = $Cell.Outputs.Data.Values
    
    DoDisplay $Cell $Result $FullName
}

function DoDisplay {
    param(
        $Cell,
        $Result,
        $FullName
    )
    
    [PSCustomObject]@{            
        Language = $Cell.Language
        Contents = $Cell.Contents
        Result   = $Result
        FullName = $FullName
    }
}

function Invoke-NBCode {
    param(
        $Code,
        $TargetKernelName = 'powershell'
    )

    Invoke-RunCode $Code $TargetKernelName
}

Get-NBContent "D:\mygit\DotNetInteractivePSCmdlet\testNB.ipynb"