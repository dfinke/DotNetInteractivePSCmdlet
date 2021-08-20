Import-Module D:\temp\scratch\PSCmdlet\bin\Debug\net5.0\publish\PSCmdlet.dll -Force

function Get-NBContent {
    param(
        [Parameter(ValueFromPipelineByPropertyName)]
        $FullName
    )

    Process {
        [PSCustomObject]@{
            Content  = (Invoke-TheNotebook $FullName).Cells
            # Content  = Invoke-TheNotebook $FullName
            FullName = $FullName
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