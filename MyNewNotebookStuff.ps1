#!/usr/bin/env pwsh
Import-Module $PSScriptRoot\bin\Debug\net5.0\publish\PSCmdlet.dll -Force

function Get-NotebookCell {
    <#
        .Example
        Get-NotebookCell testNB.ipynb
    #>
    param(
        [Parameter(ValueFromPipelineByPropertyName)]
        [Alias("PSPath")]
        $Path
    )

    Process {
        $nb = Get-Notebook $Path
        foreach ($cell in $nb.Cells) {
            $Result = switch ($cell.Outputs) {
                {$_ -is [Microsoft.DotNet.Interactive.Notebook.NotebookCellOutput]} {
                    # it's frustrating that pwsh is not object oriented here
                    $_.Text
                }
                {$_ -is [Microsoft.DotNet.Interactive.Notebook.NotebookCellDisplayOutput]}  {
                    $_.Data.Values
                }
            }

            DoDisplay -Cell $cell -Path $Path -Result $result
        }
    }
}

function DoDisplay {
    param(
        $Cell,
        $Result,
        $MimeType = 'text/html',
        $Path
    )

    [PSCustomObject]@{
        Language = $Cell.Language
        Contents = $Cell.Contents
        Result   = $Result
        Mimetype = $MimeType
        Path = $Path
    }
}

function Invoke-NBCode {
    param(
        $Code,
        $TargetKernelName = 'powershell'
    )

    Invoke-RunCode $Code $TargetKernelName
}

Get-NotebookCell testNB.ipynb -OutVariable Cells
#$Cells | Invoke-Kernel