## Worms

**Задача 5** курса [Введение в C# и платформу .Net](https://sites.google.com/view/fitnsudotnet#h.nt7847ls3u36)

## Build

Use 'Build Solution' button in Rider or something

## Run

```bash
Worms\bin\Release\net5.0\Worms.exe <OutputFile> [--help] [--version]
```

```bash
  --visualize            (Default: false) Visualize simulation in CLI.

  --simulate             Name of the world behaviour to simulate. Cannot be used with "--generate-new".

  --generate-new         Generate new world behaviour with given name. Simulation does not run with this option. Cannot be used with "--simulate".

  --connection-string    Required. Connection string used to connect to the database.

  --help                 Display this help screen.

  --version              Display version information.

  OutputFile (pos. 0)    (Default: sim.out) File to write simulation results to.
```
