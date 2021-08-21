Import-Module $PSScriptRoot\bin\Debug\net5.0\publish\PSCmdlet.dll -Force

function Get-NBContent {
    <#
        .Example
        Get-NBContent testNB.ipynb
    #>
    param(
        [Parameter(ValueFromPipelineByPropertyName)]
        $FullName
    )

    Process {
        $nb = Invoke-TheNotebook $FullName

        foreach ($cell in $nb.Cells) {
            switch -Regex ($cell.Language) {
                'pwsh' { 
                    $Result = $cell.Outputs.Text
                }
                'csharp|fsharp' {
                    $Result = $cell.Outputs.Data.Values
                }
            }

            DoDisplay -Cell $cell -FullName $FullName -Result $result
        }
    }
}

function DoDisplay {
    param(
        $Cell,
        $Result,
        $MimeType = 'text/html',
        $FullName
    )
    
    [PSCustomObject]@{            
        Language = $Cell.Language
        Contents = $Cell.Contents
        Result   = $Result
        Mimetype = $MimeType
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
