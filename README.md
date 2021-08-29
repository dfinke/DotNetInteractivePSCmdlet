# .NET Interactive PowerShell Cmdlet

## Goal - Use the .NET Interactive Engine

Develop a `PowerShell Cmdlet` that works with .NET Interactive Notebooks Engine.

> .NET Interactive takes the power of .NET and embeds it into your interactive experiences. Share code, explore data, write, and learn across your apps in ways you couldn't before.

My `PowerShellNotebook` module is a module for parameterizing, executing, and analyzing .NET Interactive Notebooks.

- Check out the [PowerShellNotebook](https://github.com/dfinke/PowerShellNotebook/blob/master/InvokeExecuteNotebook.ps1) module for an example of how to use this module.

 
## Status

- [Check out the code](https://github.com/dfinke/DotNetInteractivePSCmdlet/blob/master/Class1.cs#L82), it is blocked on using the `.NET Interactive Engine` asynchronously.
- [Jaykul](https://twitter.com/Jaykul) checked in a [branch](https://github.com/dfinke/DotNetInteractivePSCmdlet/tree/joel/wip) that has take it further, but it is not solved.


## End Game

- [ ] Run .NET Interactive Notebooks `headless`. This enables scenarios for Azure Functions, GitHub Actions, and more.
- [ ] Automate inspection, testing, copying, moving, and more with this `cmdlet`, layering PS over them and treating the contents like a file system, and more.
- [ ] See how best to interact with HTML output cells
- [ ] Enable a `sandbox` using PS to experiment with all types if uses.