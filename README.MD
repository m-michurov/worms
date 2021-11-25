## Worms

**Задача 6** курса [Введение в C# и платформу .Net](https://sites.google.com/view/fitnsudotnet#h.nt7847ls3u36)

## Build

Use 'Build Solution' button in Rider or something

You must also provide an `app.config` file with appropriate `DATABASE_CONNECTION_STRING`

```xml
<?xml version="1.0" encoding="UTF-8"?>

<configuration>
    <connectionStrings>
        <add
            name="WormsDb"
            connectionString="DATABASE_CONNECTION_STRING" />
    </connectionStrings>
</configuration>
```

## Docker

```bash
docker build -t worms-server .
```

```bash
docker run -p 80:80 worms-server
```

## Run

```bash
Worms\bin\Release\net5.0\Worms.exe <OutputFile> [OPTIONS]
```

See `--help` for help.
