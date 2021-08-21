#Import-Module D:\temp\scratch\PSCmdlet\bin\Debug\net5.0\publish\PSCmdlet.dll -Force


Import-Module $PSScriptRoot\bin\Debug\net5.0\publish\PSCmdlet.dll -Force

function Get-NBContent {
    param(
        [Parameter(ValueFromPipelineByPropertyName)]
        $FullName
    )

    Process {
        $nb = Invoke-TheNotebook $FullName

        foreach ($cell in $nb.Cells) {
            [PSCustomObject]@{            
                Language = $cell.Language
                Contents = $cell.Contents
                Result   = $cell.outputs.text
                FullName = $FullName
            }    
        }
    }
}

function Invoke-NBCode {
    param(
        $Code,
        $TargetKernelName = 'powershell'
    )

    Invoke-RunCode $Code $TargetKernelName
}